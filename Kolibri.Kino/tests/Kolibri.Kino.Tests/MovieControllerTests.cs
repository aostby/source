using Kolibri.Kino.Controllers;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kolibri.Kino.Tests;

public sealed class MovieControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _repo;
    private readonly LiteDbFileItemRepository _files;
    private readonly FakeOmdb _omdb = new();
    private readonly FakeTmdbLinks _tmdb = new();
    private readonly MovieController _controller;

    public MovieControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _repo = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _controller = new MovieController(_repo, _omdb, _files, _tmdb, new NoPosters(), NullLogger<MovieController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task Opens_the_TMDb_page_or_falls_back_to_a_title_search()
    {
        _tmdb.Pages["tt0133093"] = "https://www.themoviedb.org/movie/603";

        Assert.Equal("https://www.themoviedb.org/movie/603", await _controller.GetTmdbPageAsync("tt0133093", "The Matrix"));
        Assert.Equal("https://www.themoviedb.org/search?query=Final%20Fantasy%20VII%20Remake",
            await _controller.GetTmdbPageAsync("tt10323744", "Final Fantasy VII Remake"));

        _tmdb.Throws = new HttpRequestException("401 Unauthorized");
        Assert.StartsWith("https://www.themoviedb.org/search?query=", await _controller.GetTmdbPageAsync("tt0133093", "The Matrix"));
    }

    [Fact]
    public async Task The_linked_path_is_the_file_link_or_else_SilverScreens_TomatoUrl()
    {
        await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt0000001", Title = "Linked", TomatoUrl = @"X:\old\path.mkv" });
        await _files.UpsertAsync(new Kolibri.Kino.Core.Models.FileItem { ImdbId = "tt0000001", FullName = @"X:\Movies\Linked.mkv" });
        await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt0000002", Title = "Series", TomatoUrl = @"\\server\Series\Show" });
        await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt0000003", Title = "Nothing", TomatoUrl = "N/A" });

        Assert.Equal(@"X:\Movies\Linked.mkv", await _controller.GetLinkedPathAsync("tt0000001"));
        Assert.Equal(@"\\server\Series\Show", await _controller.GetLinkedPathAsync("tt0000002"));
        Assert.Null(await _controller.GetLinkedPathAsync("tt0000003"));
    }

    [Fact]
    public async Task Details_come_from_the_library_or_else_from_OMDb_without_storing()
    {
        await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt0133093", Title = "The Matrix (library)", Response = "True" });
        _omdb.Add(LiteDbMovieRepositoryTests.Movie("tt0133093", "The Matrix (OMDb)"));
        _omdb.Add(LiteDbMovieRepositoryTests.Movie("tt0110912", "Pulp Fiction"));

        Assert.Equal("The Matrix (library)", (await _controller.GetDetailsAsync("tt0133093"))?.Title);
        Assert.Equal("Pulp Fiction", (await _controller.GetDetailsAsync("tt0110912"))?.Title);
        Assert.Null(await _repo.GetByImdbIdAsync("tt0110912")); // looking at details doesn't import
        Assert.Null(await _controller.GetDetailsAsync("tt0000000"));
    }

    [Theory]
    [InlineData("4.9", Kolibri.Kino.Core.RatingBand.Low)]
    [InlineData("5.9", Kolibri.Kino.Core.RatingBand.Low)]
    [InlineData("6.0", Kolibri.Kino.Core.RatingBand.Medium)]
    [InlineData("6.9", Kolibri.Kino.Core.RatingBand.Medium)]
    [InlineData("7.0", Kolibri.Kino.Core.RatingBand.High)]
    [InlineData("10", Kolibri.Kino.Core.RatingBand.High)]    // SilverScreen read only "1" here and showed red
    [InlineData("N/A", Kolibri.Kino.Core.RatingBand.Unknown)]
    [InlineData(null, Kolibri.Kino.Core.RatingBand.Unknown)]
    public void Rating_bands(string? rating, Kolibri.Kino.Core.RatingBand band) =>
        Assert.Equal(band, Kolibri.Kino.Core.RatingBands.Of(rating));

    [Fact]
    public async Task A_poster_by_id_comes_from_the_cache_without_asking_OMDb_else_via_the_details()
    {
        var posters = new CachePosters { Cached = { ["tt0000001"] = [1] }, Downloadable = { ["https://example.test/2.jpg"] = [2] } };
        var controller = new MovieController(_repo, _omdb, _files, _tmdb, posters, NullLogger<MovieController>.Instance);
        _omdb.Add(new OMDbApiNet.Model.Item { ImdbId = "tt0000002", Title = "Online" });
        _omdb.ById["tt0000002"].Poster = "https://example.test/2.jpg";

        Assert.Equal([1], await controller.GetPosterAsync("tt0000001"));
        Assert.Equal(0, _omdb.Calls);

        await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt0000002", Title = "Library", Poster = "https://example.test/2.jpg" });
        Assert.Equal([2], await controller.GetPosterAsync("tt0000002"));
        Assert.Equal(0, _omdb.Calls); // the library entry had the address

        _omdb.UnavailableMessage = "Request limit reached!";
        Assert.Null(await controller.GetPosterAsync("tt0000003"));
    }

    /// <summary>Like CachedPosterProvider: the cache by id, else a download of the item's poster address.</summary>
    private sealed class CachePosters : Kolibri.Kino.Core.Abstractions.IPosterProvider
    {
        public Dictionary<string, byte[]> Cached { get; } = [];
        public Dictionary<string, byte[]> Downloadable { get; } = [];

        public Task<byte[]?> GetPosterAsync(OMDbApiNet.Model.Item item, CancellationToken ct = default) =>
            Task.FromResult(Cached.GetValueOrDefault(item.ImdbId) ?? (item.Poster is { } url ? Downloadable.GetValueOrDefault(url) : null));
    }

    private sealed class NoPosters : Kolibri.Kino.Core.Abstractions.IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(OMDbApiNet.Model.Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }

    private sealed class FakeTmdbLinks : Kolibri.Kino.Core.Abstractions.ITmdbLinks
    {
        public Dictionary<string, string> Pages { get; } = [];
        public Exception? Throws { get; set; }

        public Task<string?> FindPageAsync(string imdbId, CancellationToken ct = default) =>
            Throws is not null ? Task.FromException<string?>(Throws) : Task.FromResult(Pages.GetValueOrDefault(imdbId));
    }

    [Fact]
    public async Task Import_stores_item_from_provider()
    {
        _omdb.Add(LiteDbMovieRepositoryTests.Movie("tt0133093", "The Matrix"));

        var imported = await _controller.ImportAsync("tt0133093");
        var local = await _controller.GetLocalAsync();

        Assert.NotNull(imported);
        Assert.Equal("The Matrix", Assert.Single(local).Title);
    }

    [Fact]
    public async Task Import_of_unknown_id_returns_null_and_stores_nothing()
    {
        Assert.Null(await _controller.ImportAsync("tt0000000"));
        Assert.Empty(await _controller.GetLocalAsync());
    }

    [Fact]
    public async Task Removing_from_the_library_drops_the_entry_and_its_link_but_not_the_file()
    {
        var file = Path.Combine(Path.GetTempPath(), $"kino-remove-{Guid.NewGuid():N}.mkv");
        File.WriteAllText(file, "x");
        try
        {
            await _repo.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt10323744", Title = "Final Fantasy VII Remake", Type = "game", Response = "True" });
            await _files.UpsertAsync(new Kolibri.Kino.Core.Models.FileItem { ImdbId = "tt10323744", FullName = file });

            Assert.True(await _controller.RemoveFromLibraryAsync("tt10323744"));

            Assert.Null(await _repo.GetByImdbIdAsync("tt10323744"));
            Assert.Null(await _files.GetByImdbIdAsync("tt10323744"));
            Assert.True(File.Exists(file));
            Assert.False(await _controller.RemoveFromLibraryAsync("tt10323744"));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public async Task Empty_local_search_returns_whole_library()
    {
        await _repo.UpsertAsync(LiteDbMovieRepositoryTests.Movie("tt0133093", "The Matrix"));
        await _repo.UpsertAsync(LiteDbMovieRepositoryTests.Movie("tt0110912", "Pulp Fiction"));

        Assert.Equal(2, (await _controller.SearchLocalAsync("  ")).Count);
    }
}
