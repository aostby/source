using System.Net;
using System.Text;
using System.Text.Json;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kolibri.Kino.Tests;

public class PlexWatchlistParseTests
{
    // As discover.provider.plex.tv answers: "guid" is Plex's own id, "Guid" (capital G) the external ids.
    private const string Page = """
        {"MediaContainer":{"size":3,"totalSize":3,"Metadata":[
          {"title":"Bram Stoker's Dracula","year":1992,"type":"movie","guid":"plex://movie/5d776830961905001eb92f31",
           "thumb":"https://metadata-static.plex.tv/a/b.jpg","Guid":[{"id":"imdb://tt0103874"},{"id":"tmdb://6114"}]},
          {"title":"A Spy Among Friends","year":2022,"type":"show","guid":"plex://show/1","Guid":[{"id":"tvdb://402296"},{"id":"imdb://tt15565872"}]},
          {"title":"No ids","type":"movie","guid":"plex://movie/2","thumb":"/library/metadata/2/thumb"}
        ]}}
        """;

    [Fact]
    public void Titles_come_with_their_imdb_id_type_and_poster()
    {
        using var json = JsonDocument.Parse(Page);
        var (items, total) = PlexWatchlist.Parse(json);
        Assert.Equal(3, total);
        Assert.Equal(new PlexWatchlistItem("Bram Stoker's Dracula", "1992", "movie", "tt0103874", "https://metadata-static.plex.tv/a/b.jpg"), items[0]);
        Assert.Equal(("series", "tt15565872"), (items[1].Type, items[1].ImdbId));
        Assert.Equal((null, null, (string?)null), (items[2].Year, items[2].ImdbId, items[2].Poster));
    }

    [Fact]
    public async Task The_watchlist_is_read_page_by_page_with_the_token_in_a_header()
    {
        var handler = new Pages();
        var watchlist = new PlexWatchlist(new InMemorySettingsStore(new UserSettings { XPlexToken = "secret-token" }), handler);
        var items = await watchlist.GetWatchlistAsync();

        Assert.Equal(150, items.Count);
        Assert.Equal(2, handler.Requests.Count);
        Assert.All(handler.Requests, r => Assert.DoesNotContain("secret-token", r.RequestUri!.ToString()));
        Assert.All(handler.Requests, r => Assert.Equal("secret-token", r.Headers.GetValues("X-Plex-Token").Single()));
        Assert.Contains("X-Plex-Container-Start=100", handler.Requests[1].RequestUri!.Query);
    }

    [Fact]
    public void Without_a_token_it_is_not_set_up() =>
        Assert.False(new PlexWatchlist(new InMemorySettingsStore(new UserSettings())).IsConfigured);

    /// <summary>A watchlist of 150 titles in pages of 100.</summary>
    private sealed class Pages : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Requests.Add(request);
            var start = request.RequestUri!.Query.Contains("Start=100") ? 100 : 0;
            var count = start == 0 ? 100 : 50;
            var metadata = string.Join(",", Enumerable.Range(start, count).Select(i =>
                $$"""{"title":"T{{i}}","year":2000,"type":"movie","Guid":[{"id":"imdb://tt{{i:0000000}}"}]}"""));
            var body = "{\"MediaContainer\":{\"size\":" + count + ",\"totalSize\":150,\"Metadata\":[" + metadata + "]}}";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") });
        }
    }
}

public sealed class PlexWatchlistImportTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly LiteDbWishlistRepository _wishlistRepo;
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-plex-watchlist-").FullName;

    public PlexWatchlistImportTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _wishlistRepo = new LiteDbWishlistRepository(_db);
    }

    [Fact]
    public async Task Plex_watchlist_titles_go_on_the_wishlist_except_those_already_there_or_on_disk()
    {
        // On disk: in the library with a file that's there.
        var file = Path.Combine(_folder, "Have.mkv");
        File.WriteAllText(file, "");
        await _movies.UpsertAsync(LiteDbMovieRepositoryTests.Movie("tt0000001", "Have"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0000001", FullName = file });
        // In the library without a file: its library details are used.
        var noFile = LiteDbMovieRepositoryTests.Movie("tt0000002", "In library");
        noFile.Genre = "Drama";
        noFile.ImdbRating = "7.5";
        await _movies.UpsertAsync(noFile);
        // On the wishlist already.
        await _wishlistRepo.UpsertAsync(new WishlistItem { ImdbId = "tt0000003", Title = "Wished", Added = DateTime.Now });

        var plex = new FakeWatchlist
        {
            Items =
            [
                new("Have", "2000", "movie", "tt0000001", null),
                new("In library (Plex title)", "2001", "movie", "tt0000002", "https://metadata-static.plex.tv/2.jpg"),
                new("Wished", "2002", "movie", "tt0000003", null),
                new("New", "2026", "series", "tt0000004", "https://metadata-static.plex.tv/4.jpg"),
                new("No id", null, "movie", null, null),
            ],
        };
        var libraryFiles = new Kolibri.Kino.Controllers.Library.LibraryFiles(_files, new FileSystemMediaScanner(),
            Microsoft.Extensions.Options.Options.Create(new Kolibri.Kino.Core.KinoOptions()));
        var wishlist = new WishlistController(_wishlistRepo, _movies, _files, new FakeOmdb(), new WishlistTests.TmdbTitles(), new FakeTmdb(),
            NullLogger<WishlistController>.Instance, libraryFiles, plex);

        Assert.True(wishlist.PlexWatchlistConfigured);
        var result = await wishlist.ImportPlexWatchlistAsync();

        Assert.Equal(new PlexWatchlistImport(Total: 5, Added: 2, AlreadyThere: 1, OnDisk: 1, WithoutImdbId: 1), result);
        var library = await _wishlistRepo.GetAsync("tt0000002");
        Assert.Equal(("In library", "Drama", "7.5"), (library!.Title, library.Genre, library.ImdbRating));
        var fresh = await _wishlistRepo.GetAsync("tt0000004");
        Assert.Equal(("New", "2026", "series", "https://metadata-static.plex.tv/4.jpg"), (fresh!.Title, fresh.Year, fresh.Type, fresh.Poster));
        Assert.Null(await _wishlistRepo.GetAsync("tt0000001"));

        // Again: nothing new.
        Assert.Equal(0, (await wishlist.ImportPlexWatchlistAsync()).Added);
    }

    [Fact]
    public async Task Without_a_Plex_token_the_import_says_so()
    {
        var wishlist = new WishlistController(_wishlistRepo, _movies, _files, new FakeOmdb(), new WishlistTests.TmdbTitles(), new FakeTmdb(),
            NullLogger<WishlistController>.Instance, null, new FakeWatchlist { Configured = false });
        Assert.False(wishlist.PlexWatchlistConfigured);
        await Assert.ThrowsAsync<InvalidOperationException>(() => wishlist.ImportPlexWatchlistAsync());
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    private sealed class FakeWatchlist : IPlexWatchlist
    {
        public bool Configured { get; init; } = true;
        public IReadOnlyList<PlexWatchlistItem> Items { get; init; } = [];
        public bool IsConfigured => Configured;
        public Task<IReadOnlyList<PlexWatchlistItem>> GetWatchlistAsync(CancellationToken ct = default) => Task.FromResult(Items);
    }
}
