using System.Net;
using System.Text;
using Kolibri.Kino.Controllers.Prowlarr;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Tests;

public sealed class ProwlarrTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly FakeOmdb _omdb = new();
    private readonly FakeTmdb _tmdb = new();
    private readonly FakeProwlarr _prowlarr = new();

    public ProwlarrTests() => _db = new KinoDatabase(_temp.Path);

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    private ProwlarrController Controller(string? key = "secret", ProwlarrOptions? options = null, string? url = UserSettings.DefaultProwlarrUrl) => new(
        new InMemorySettingsStore(new UserSettings { ProwlarrApiKey = key, ProwlarrUrl = url }),
        Options.Create(new KinoOptions { Prowlarr = options ?? new ProwlarrOptions() }),
        new WishlistController(new LiteDbWishlistRepository(_db), new LiteDbMovieRepository(_db), new LiteDbFileItemRepository(_db),
            _omdb, new WishlistTests.TmdbTitles(), _tmdb, NullLogger<WishlistController>.Instance),
        NullLogger<ProwlarrController>.Instance,
        handler: _prowlarr);

    private static WishlistItem Movie(string title, string year, string imdbId = "") => new() { Title = title, Year = year, ImdbId = imdbId, Type = "movie" };

    private static ProwlarrRelease Release(string title, int seeders = 100, double gb = 2, string? imdbId = null) =>
        new(title, "TorrentDownload", (long)(gb * 1e9), seeders, 1, null, "http://prowlarr/magnet", null, null, imdbId);

    [Fact]
    public void Grindhouse_keeps_the_double_feature_and_leaves_out_its_two_halves()
    {
        // The top of a real search for "Grindhouse 2007" (Prowlarr, 190 results from 6 indexers).
        var releases = new[]
        {
            Release("Grindhouse (2007) [720p] [BluRay] [YTS MX]", 373, 1.8),
            Release("Grindhouse (2007) [720p] [BluRay] [YTS] [YIFY]", 222, 1.8),
            Release("Grindhouse 2007 1080p BluRay x265  88", 200, 3.2, "tt0462322"),
            Release("Grindhouse Death Proof 2007 1080p BluRay HEVC x265 5 1 BONE", 192, 1.5, "tt1028528"),
            Release("Grindhouse Planet Terror 2007 1080p BluRay HEVC x265 51 BONE", 186, 1.4),
            Release("Grindhouse (2007) (1080p BluRay x265 HEVC 10bit AC3 5 1 r00t)", 127, 9.8),
            Release("Grindhouse.2007.1080p.BluRay.x264-WRONGID", 90, 4, "tt1028528"),
            Release("Grindhouse 2007 2160p UHD BluRay", 300, 30),
            Release("Grindhouse 2007 HDCAM 720p", 400, 1),
            Release("Grindhouse 2007 720p", 2, 1),
            Release("Grindhouse 2008 720p BluRay", 50, 1),
        };

        var result = ProwlarrController.Check(Movie("Grindhouse", "2007", "tt0462322"), "Grindhouse 2007", releases, new ProwlarrOptions());

        Assert.Equal([373, 222, 200, 127], result.Best.Select(c => c.Release.Seeders));
        Assert.Equal(releases.Length, result.All.Count);
        string? Why(string start) => result.All.Single(c => c.Release.Title.StartsWith(start, StringComparison.Ordinal)).LeftOut;
        Assert.Equal("Not \"Grindhouse 2007\"", Why("Grindhouse Death Proof"));
        Assert.Equal("Another title on IMDb (tt1028528)", Why("Grindhouse.2007.1080p.BluRay.x264-WRONGID"));
        Assert.Equal("Quality 2160p", Why("Grindhouse 2007 2160p"));
        Assert.Equal("Says HDCAM", Why("Grindhouse 2007 HDCAM"));
        Assert.Equal("Only 2 seeder(s)", Why("Grindhouse 2007 720p"));
        Assert.Equal("Year 2008", Why("Grindhouse 2008"));
    }

    [Theory]
    [InlineData("The Matrix", "The.Matrix.1999.1080p.BluRay", null)]
    [InlineData("The Matrix", "Matrix (1999) 720p", null)]
    [InlineData("Amélie", "Amelie.2001.1080p.BluRay", null)]
    [InlineData("Don't Look Up", "Dont.Look.Up.2021.1080p.WEB", null)]
    [InlineData("Fast & Furious", "Fast.and.Furious.2009.1080p", null)]
    [InlineData("The Matrix", "The Matrix Reloaded 2003 1080p", "Not \"The Matrix 1999\"")]
    public void Names_match_without_case_dots_accents_or_the(string title, string release, string? why)
    {
        var year = release.Contains("2001") ? "2001" : release.Contains("2021") ? "2021" : release.Contains("2009") ? "2009" : "1999";
        var check = ProwlarrController.Check(Movie(title, year), title, [Release(release)], new ProwlarrOptions()).All.Single();
        Assert.Equal(why, check.LeftOut);
    }

    [Fact]
    public void Series_and_episodes_go_by_season_and_episode()
    {
        var series = new WishlistItem { Title = "The Bear", Year = "2022–", Type = "series" };
        var episode = new WishlistItem { ImdbId = "tt14452776_S03E04", Title = "The Bear S03E04: Napkins", Year = "2024", Type = "episode" };
        var releases = new[]
        {
            Release("The.Bear.S03.1080p.WEB"), Release("The Bear S03E04 1080p WEB"), Release("The.Bear.2022.S03E04.720p"),
            Release("The Bear S03E05 1080p WEB"), Release("The Bear Necessities 2020 1080p"),
        };

        Assert.Equal("The Bear", ReleaseMatch.Query(series));
        Assert.Equal("The Bear S03E04", ReleaseMatch.Query(episode));
        Assert.Equal([null, null, null, null, "Another title"],
            ProwlarrController.Check(series, "", releases, new ProwlarrOptions()).All.OrderBy(c => Array.IndexOf(releases, c.Release)).Select(c => c.LeftOut));
        Assert.Equal(["Another episode", null, null, "Another episode", "Another title"],
            ProwlarrController.Check(episode, "", releases, new ProwlarrOptions()).All.OrderBy(c => Array.IndexOf(releases, c.Release)).Select(c => c.LeftOut));
    }

    [Theory]
    [InlineData("Bosch.Legacy.S01E10", "Bosch Legacy S01E10", null, "episode", "Bosch Legacy S01E10")]
    [InlineData("bosch legacy s1e10", "bosch legacy S01E10", null, "episode", "bosch legacy S01E10")]
    [InlineData("The Bear Season 3", "The Bear", null, "series", "The Bear")]
    [InlineData("The.Bear.S03", "The Bear", null, "series", "The Bear")]
    [InlineData("Grindhouse (2007)", "Grindhouse", "2007", "movie", "Grindhouse 2007")]
    [InlineData("Grindhouse.2007", "Grindhouse", "2007", "movie", "Grindhouse 2007")]
    [InlineData("Blade Runner 2049", "Blade Runner 2049", null, null, "Blade Runner 2049")]
    public void A_search_typed_in_becomes_a_wanted_title(string text, string title, string? year, string? type, string query)
    {
        var wanted = ReleaseMatch.FromText(text);
        Assert.Equal((title, year, type, query), (wanted.Title, wanted.Year, wanted.Type, ReleaseMatch.Query(wanted)));
    }

    [Fact]
    public void A_typed_in_episode_keeps_only_that_episode()
    {
        var releases = new[] { Release("Bosch.Legacy.S01E10.1080p.WEB.H264"), Release("Bosch Legacy S01E09 1080p"), Release("Bosch.S01E10.1080p") };
        var result = ProwlarrController.Check(ReleaseMatch.FromText("Bosch.Legacy.S01E10"), "", releases, new ProwlarrOptions());
        Assert.Equal(["Bosch.Legacy.S01E10.1080p.WEB.H264"], result.Best.Select(c => c.Release.Title));
    }

    [Theory]
    [InlineData("Movie 2007 2160p", "2160p")]
    [InlineData("Movie.2007.4K.HDR", "2160p")]
    [InlineData("Movie 2007 1080p", "1080p")]
    [InlineData("Movie (2007) [720p]", "720p")]
    [InlineData("Movie 2007 DVDRip XviD", "SD")]
    [InlineData("Movie 2007 BluRay", null)]
    public void The_quality_is_read_from_the_name(string title, string? quality) => Assert.Equal(quality, ReleaseMatch.QualityOf(title));

    [Fact]
    public async Task A_search_asks_for_title_and_year_with_the_key_and_reads_both_kinds_of_link()
    {
        _prowlarr.Respond = _ => Json("""
            [{"title":"Grindhouse (2007) [720p] [BluRay] [YTS MX]","indexer":"TorrentDownload","size":1800000000,"seeders":373,"leechers":40,
              "imdbId":462322,"publishDate":"2026-10-30T17:38:18Z","magnetUrl":"http://prowlarr/2/download?link=m"},
             {"title":"Grindhouse 2007 1080p BluRay x265","indexer":"Knaben","size":3200000000,"seeders":200,"imdbId":0,
              "downloadUrl":"http://prowlarr/5/download?link=t"}]
            """);

        var result = await Controller().SearchAsync(Movie("Grindhouse", "2007"));

        Assert.Equal("http://greenlantern:9696/api/v1/search?query=Grindhouse%202007&type=search&categories=2000", _prowlarr.Urls.Single());
        Assert.Equal("secret", _prowlarr.Key);
        Assert.Equal([("magnet", "tt0462322"), ("torrent", null)], result.Best.Select(c => (c.Release.Links, c.Release.ImdbId)));
    }

    [Fact]
    public async Task Series_are_searched_in_the_TV_categories()
    {
        _prowlarr.Respond = _ => Json("[]");
        await Controller(options: new ProwlarrOptions { SeriesCategories = [5000, 5040] }).SearchAsync(new WishlistItem { Title = "The Bear", Year = "2022", Type = "series" });
        Assert.EndsWith("query=The%20Bear&type=search&categories=5000&categories=5040", _prowlarr.Urls.Single());
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "didn't accept the API key")]
    [InlineData(HttpStatusCode.InternalServerError, "Prowlarr answered 500")]
    public async Task A_refusal_is_said_in_words(HttpStatusCode status, string expected)
    {
        _prowlarr.Respond = _ => new HttpResponseMessage(status);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller().SearchAsync(Movie("Grindhouse", "2007")));
        Assert.Contains(expected, error.Message);
    }

    [Fact]
    public async Task Without_the_key_nothing_is_asked()
    {
        var controller = Controller(key: null);
        Assert.False(controller.IsConfigured);
        Assert.Equal("no Prowlarr API key", controller.ConfigProblem);
        Assert.Null(Controller().ConfigProblem);
        Assert.Equal("no Prowlarr URL and no Prowlarr API key", Controller(key: " ", url: "").ConfigProblem);
        Assert.Equal("the Prowlarr URL \"nas:9696\" isn't an http(s) address", Controller(url: "nas:9696").ConfigProblem);
        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.SearchAsync(Movie("Grindhouse", "2007")));
        Assert.Empty(_prowlarr.Urls);
    }

    [Fact]
    public async Task Prowlarrs_magnet_link_is_followed_to_the_magnet()
    {
        _prowlarr.Respond = _ => new HttpResponseMessage(HttpStatusCode.Redirect) { Headers = { Location = new Uri("magnet:?xt=urn:btih:ABC&dn=Grindhouse") } };
        var link = await Controller().GetLinkAsync(Release("Grindhouse (2007) [720p]"));
        Assert.Equal(("magnet:?xt=urn:btih:ABC&dn=Grindhouse", (byte[]?)null), (link.Magnet, link.Torrent));
    }

    [Theory]
    [InlineData("magnet:?xt=urn:btih:047d66c2", "magnet:?xt=urn:btih:047d66c2&dn=Bosch%20Legacy%20S01E10%201080p")]
    [InlineData("magnet:?xt=urn:btih:047d66c2&tr=udp%3A%2F%2Ft.example", "magnet:?xt=urn:btih:047d66c2&tr=udp%3A%2F%2Ft.example&dn=Bosch%20Legacy%20S01E10%201080p")]
    [InlineData("magnet:?xt=urn:btih:047d66c2&dn=Own.Name", "magnet:?xt=urn:btih:047d66c2&dn=Own.Name")]
    public void A_magnet_link_without_a_name_gets_the_release_name(string magnet, string expected) =>
        Assert.Equal(expected, ProwlarrController.Named(magnet, "Bosch Legacy S01E10 1080p"));

    [Fact]
    public async Task A_followed_magnet_link_without_a_name_gets_the_release_name()
    {
        _prowlarr.Respond = _ => new HttpResponseMessage(HttpStatusCode.Redirect) { Headers = { Location = new Uri("magnet:?xt=urn:btih:047d66c2") } };
        var link = await Controller().GetLinkAsync(Release("Bosch.Legacy.S01E10.1080p"));
        Assert.Equal("magnet:?xt=urn:btih:047d66c2&dn=Bosch.Legacy.S01E10.1080p", link.Magnet);
    }

    [Fact]
    public async Task The_download_link_gives_the_torrent_file()
    {
        var torrent = Encoding.ASCII.GetBytes("d8:announce30:http://tracker.example/announcee");
        _prowlarr.Respond = _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(torrent) };
        var release = Release("Grindhouse: 2007 1080p") with { MagnetUrl = null, DownloadUrl = "http://prowlarr/5/download?link=t" };

        var link = await Controller().GetLinkAsync(release);

        Assert.Equal(torrent, link.Torrent);
        Assert.Equal("Grindhouse  2007 1080p.torrent", link.FileName);
    }

    [Fact]
    public async Task A_page_that_is_neither_is_refused()
    {
        _prowlarr.Respond = _ => Json("<html>login</html>");
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller().GetLinkAsync(Release("Grindhouse 2007")));
        Assert.Contains("neither a magnet link nor a .torrent file", error.Message);
    }

    [Fact]
    public async Task A_title_without_an_id_is_found_by_title_and_year()
    {
        _omdb.Searchable.Add(new OMDbApiNet.Model.Item { ImdbId = "tt0462322", Title = "Grindhouse", Year = "2007", Type = "movie" });
        _omdb.Searchable.Add(new OMDbApiNet.Model.Item { ImdbId = "tt1028528", Title = "Death Proof", Year = "2007", Type = "movie" });

        var found = await Controller().LookUpAsync(new WishlistItem { Title = "grindhouse", Year = "2007" });

        Assert.Equal(("tt0462322", "movie"), (found?.ImdbId, found?.Type));
        Assert.Null(await Controller().LookUpAsync(new WishlistItem { Title = "Grindhouse", Year = "1999" }));
    }

    [Fact]
    public void The_Prowlarr_address_ends_in_a_slash_and_defaults_to_the_NAS()
    {
        Assert.Equal("http://greenlantern:9696/", new UserSettings().GetProwlarrBaseUrl());
        Assert.Equal("http://nas:9696/prowlarr/", new UserSettings { ProwlarrUrl = " http://nas:9696/prowlarr " }.GetProwlarrBaseUrl());
        Assert.Null(new UserSettings { ProwlarrUrl = "nas:9696" }.GetProwlarrBaseUrl());
    }

    private static HttpResponseMessage Json(string body) => new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class FakeProwlarr : HttpMessageHandler
    {
        public Func<HttpRequestMessage, HttpResponseMessage> Respond = _ => new HttpResponseMessage(HttpStatusCode.OK);
        public List<string> Urls { get; } = [];
        public string? Key;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Urls.Add(request.RequestUri!.AbsoluteUri);
            Key = request.Headers.TryGetValues("X-Api-Key", out var values) ? values.Single() : null;
            return Task.FromResult(Respond(request));
        }
    }
}
