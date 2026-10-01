using System.Net;
using System.Text;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class PlexLibraryTests
{
    private const string Sections = """
        <MediaContainer>
          <Directory key="1" type="movie" title="Filmer" />
          <Directory key="2" type="show" title="Serier" />
          <Directory key="3" type="movie" title="Barnefilmer" />
        </MediaContainer>
        """;

    private const string Section1 = """
        <MediaContainer>
          <Video ratingKey="10" title="The Matrix" year="1999" audienceRating="8.7" contentRating="R" duration="8160000"
                 summary="A hacker learns the truth." thumb="/library/metadata/10/thumb/1">
            <Genre tag="Action" /><Genre tag="Sci-Fi" /><Director tag="Lana Wachowski" /><Role tag="Keanu Reeves" />
            <Guid id="imdb://tt0133093" /><Guid id="tmdb://603" />
          </Video>
          <Video ratingKey="11" title="Home Video" year="2020"><Guid id="local://11" /></Video>
          <Video ratingKey="12" title="Pulp Fiction" year="1994"><Guid id="imdb://tt0110912" /></Video>
        </MediaContainer>
        """;

    private const string Section3 = """
        <MediaContainer>
          <Video ratingKey="30" title="Arthurs julegaverace" originalTitle="Arthur Christmas" year="2011">
            <Guid id="imdb://tt1430607" />
          </Video>
        </MediaContainer>
        """;

    private readonly PlexHandler _handler = new();
    private readonly InMemorySettingsStore _settings = new(new UserSettings { XPlexServerName = "htpc", XPlexToken = "secret-token" });

    public PlexLibraryTests()
    {
        _handler.Get["/library/sections"] = Sections;
        _handler.Get["/library/sections/1/all"] = Section1;
        _handler.Get["/library/sections/3/all"] = Section3;
        _handler.Get["/identity"] = """<MediaContainer machineIdentifier="abc123" />""";
    }

    private PlexLibrary Library() => new(new HttpClient(_handler), _settings);

    [Fact]
    public async Task Loads_movie_sections_only_and_maps_to_OMDb_style_items()
    {
        var matrix = await Library().FindByImdbIdAsync("tt0133093");

        Assert.NotNull(matrix);
        Assert.Equal(("The Matrix", "1999", "136 min", "Action, Sci-Fi", "Keanu Reeves", "movie"),
            (matrix.Title, matrix.Year, matrix.Runtime, matrix.Genre, matrix.Actors, matrix.Type));
        Assert.Equal("http://htpc:32400/library/metadata/10/thumb/1?X-Plex-Token=secret-token", matrix.Poster);
        Assert.DoesNotContain(_handler.Requests, r => r.Path.StartsWith("/library/sections/2"));
        Assert.All(_handler.Tokens, t => Assert.Equal("secret-token", t));
    }

    [Fact]
    public async Task Title_match_uses_original_title_and_allows_one_year_off()
    {
        var library = Library();

        Assert.Equal("tt1430607", (await library.FindMovieByTitleAsync("Arthur Christmas", 2012))?.ImdbId);
        Assert.Null(await library.FindMovieByTitleAsync("Arthur Christmas", 2014));
        Assert.Null(await library.FindMovieByTitleAsync("Home Video", 2020)); // no IMDb guid
    }

    [Fact]
    public async Task Never_matches_on_first_word_only_like_SilverScreen_did()
    {
        Assert.Null(await Library().FindMovieByTitleAsync("The Godfather", 1999));
    }

    [Fact]
    public async Task Not_configured_without_token_and_reloads_when_the_token_changes()
    {
        var library = Library();
        await library.FindByImdbIdAsync("tt0133093");
        var loaded = _handler.Requests.Count;

        await library.FindByImdbIdAsync("tt0110912");
        Assert.Equal(loaded, _handler.Requests.Count);

        await _settings.SaveAsync(new UserSettings { XPlexServerName = "htpc", XPlexToken = "new-token" });
        await library.FindByImdbIdAsync("tt0133093");
        Assert.Equal(2 * loaded, _handler.Requests.Count);
        Assert.Equal("new-token", _handler.Tokens[^1]);

        await _settings.SaveAsync(new UserSettings { XPlexServerName = "htpc" });
        Assert.False(library.IsConfigured);
        Assert.Null(await library.FindByImdbIdAsync("tt0133093"));
    }

    [Fact]
    public async Task Creates_a_missing_playlist_with_all_known_movies()
    {
        _handler.Get["/playlists"] = "<MediaContainer />";

        var result = await Library().AddToPlaylistAsync("Jul", ["tt0133093", "tt0110912", "tt9999999"]);

        Assert.Equal((2, 0, true), (result.Added, result.AlreadyThere, result.Created));
        Assert.Equal(["tt9999999"], result.NotOnServer);
        var create = Assert.Single(_handler.Requests, r => r.Method == HttpMethod.Post);
        Assert.Equal("/playlists", create.Path);
        Assert.Contains("title=Jul", create.Query);
        Assert.Contains(Uri.EscapeDataString("server://abc123/com.plexapp.plugins.library/library/metadata/10,12"), create.Query);
    }

    [Fact]
    public async Task Adds_only_movies_not_already_in_the_playlist()
    {
        _handler.Get["/playlists"] = """<MediaContainer><Playlist title="Jul" ratingKey="500" playlistType="video" /></MediaContainer>""";
        _handler.Get["/playlists/500/items"] = """<MediaContainer><Video ratingKey="10" playlistItemID="7001" /></MediaContainer>""";

        var result = await Library().AddToPlaylistAsync("jul", ["tt0133093", "tt0110912"]);

        Assert.Equal((1, 1, false), (result.Added, result.AlreadyThere, result.Created));
        var put = Assert.Single(_handler.Requests, r => r.Method == HttpMethod.Put);
        Assert.Equal("/playlists/500/items", put.Path);
        Assert.EndsWith(Uri.EscapeDataString("library/metadata/12"), put.Query);
    }

    [Fact]
    public async Task Removes_the_movie_by_its_playlist_item_id()
    {
        _handler.Get["/playlists"] = """<MediaContainer><Playlist title="Jul" ratingKey="500" playlistType="video" /></MediaContainer>""";
        _handler.Get["/playlists/500/items"] = """<MediaContainer><Video ratingKey="10" playlistItemID="7001" /></MediaContainer>""";
        var library = Library();

        Assert.True(await library.RemoveFromPlaylistAsync("Jul", "tt0133093"));
        Assert.False(await library.RemoveFromPlaylistAsync("Jul", "tt0110912")); // on the server, not in the playlist

        var delete = Assert.Single(_handler.Requests, r => r.Method == HttpMethod.Delete);
        Assert.Equal("/playlists/500/items/7001", delete.Path);
    }

    private sealed class PlexHandler : HttpMessageHandler
    {
        public Dictionary<string, string> Get { get; } = [];
        public List<(HttpMethod Method, string Path, string Query)> Requests { get; } = [];
        public List<string> Tokens { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var uri = request.RequestUri!;
            Requests.Add((request.Method, uri.AbsolutePath, Uri.UnescapeDataString(uri.Query) + "|" + uri.Query));
            Tokens.Add(request.Headers.GetValues("X-Plex-Token").Single());

            if (request.Method != HttpMethod.Get)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("") });

            return Task.FromResult(Get.TryGetValue(uri.AbsolutePath, out var xml)
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(xml, Encoding.UTF8, "application/xml") }
                : new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
