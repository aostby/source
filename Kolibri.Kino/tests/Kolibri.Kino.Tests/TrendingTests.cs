using Kolibri.Kino.Controllers.Trending;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using FileItem = Kolibri.Kino.Core.Models.FileItem;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class TrendingTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbTrendingCache _cache;
    private readonly LiteDbFileItemRepository _files;
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-trending-").FullName;
    private readonly FakeTrending _tmdb = new();
    private readonly RecordingImages _images = new();
    private readonly TrendingController _controller;

    public TrendingTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _cache = new LiteDbTrendingCache(_db);
        _files = new LiteDbFileItemRepository(_db);
        var libraryFiles = new Kolibri.Kino.Controllers.Library.LibraryFiles(_files, new FileSystemMediaScanner(),
            Microsoft.Extensions.Options.Options.Create(new Kolibri.Kino.Core.KinoOptions()));
        _controller = new TrendingController(_tmdb, _cache, _movies, libraryFiles, _images, NullLogger<TrendingController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public async Task Only_a_title_linked_to_a_file_that_is_there_has_one()
    {
        var file = Path.Combine(_folder, "Spider-Man.mkv");
        File.WriteAllText(file, "");
        var seriesFolder = Directory.CreateDirectory(Path.Combine(_folder, "Slow Horses")).FullName;
        foreach (var (id, title) in new[] { ("tt1", "With file"), ("tt2", "Imported, no file"), ("tt3", "File gone"), ("tt4", "Series") })
            await _movies.UpsertAsync(Movie(id, title));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt1", FullName = file });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt3", FullName = Path.Combine(_folder, "gone.mkv") });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt4", FullName = seriesFolder });
        _tmdb.Lists[TrendingKind.Movies] = [Title(1, "With file", "tt1"), Title(2, "Imported, no file", "tt2"), Title(3, "File gone", "tt3"),
            Title(4, "Series", "tt4"), Title(5, "Not in library", "tt5")];

        var result = await _controller.GetAsync(TrendingKind.Movies);

        Assert.Equal([(true, true), (true, false), (true, false), (true, true), (false, false)],
            result.Entries.Select(e => (e.InLibrary, e.HasFile)));
    }

    private static TrendingItem Title(int id, string title, string? imdbId) =>
        new(id, title, "2026", imdbId, $"https://image.tmdb.org/t/p/w342/{id}.jpg", 7.4, "A plot.", []);

    [Fact]
    public async Task A_fresh_list_comes_from_the_database_and_Refresh_asks_TMDb_again_reusing_the_IMDb_ids()
    {
        _tmdb.Lists[TrendingKind.Movies] = [Title(1, "Clayface", "tt34890576")];
        await _controller.GetAsync(TrendingKind.Movies);
        await _controller.GetAsync(TrendingKind.Movies);
        Assert.Equal(1, _tmdb.Calls);

        await _controller.GetAsync(TrendingKind.Movies, refresh: true);
        Assert.Equal(2, _tmdb.Calls);
        Assert.Equal("tt34890576", _tmdb.LastKnown[1]); // not asked for again
    }

    [Fact]
    public async Task An_old_list_is_fetched_again()
    {
        await _cache.SaveAsync(TrendingKind.Series, [Title(9, "Old", "tt9")], DateTime.UtcNow - TrendingController.MaxAge - TimeSpan.FromMinutes(1));
        _tmdb.Lists[TrendingKind.Series] = [Title(10, "New", "tt10")];

        var result = await _controller.GetAsync(TrendingKind.Series);

        Assert.Equal(["New"], result.Entries.Select(e => e.Item.Title));
        Assert.Null(result.Note);
    }

    [Fact]
    public async Task When_TMDb_fails_the_last_list_is_shown_with_a_note_and_without_one_it_says_why()
    {
        _tmdb.Throws = new HttpRequestException("offline");
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAsync(TrendingKind.Movies));

        var when = DateTime.UtcNow.AddDays(-1);
        await _cache.SaveAsync(TrendingKind.Movies, [Title(1, "Yesterday's", "tt1")], when);
        var result = await _controller.GetAsync(TrendingKind.Movies);

        Assert.Equal(["Yesterday's"], result.Entries.Select(e => e.Item.Title));
        Assert.StartsWith("TMDb couldn't be reached (offline)", result.Note);
    }

    [Fact]
    public async Task Titles_in_the_library_are_marked_with_its_IMDb_rating_and_people_count_their_library_titles()
    {
        await _movies.UpsertAsync(Movie("tt22084616", "Spider-Man: Brand New Day", rating: "8.2"));
        var withHer = Movie("tt0000002", "A Walk to Remember");
        withHer.Actors = "Mandy Moore, Shane West";
        await _movies.UpsertAsync(withHer);
        _tmdb.Lists[TrendingKind.Movies] = [Title(1, "Spider-Man: Brand New Day", "tt22084616"), Title(2, "Clayface", "tt34890576"), Title(3, "No IMDb id", null)];
        _tmdb.Lists[TrendingKind.People] = [new(5, "Mandy Moore", null, null, null, null, null, ["Tangled"]), new(6, "Nobody Here", null, null, null, null, null, [])];

        var movies = await _controller.GetAsync(TrendingKind.Movies);
        var people = await _controller.GetAsync(TrendingKind.People);

        Assert.Equal([(true, "8.2"), (false, null), (false, null)], movies.Entries.Select(e => (e.InLibrary, e.ImdbRating)));
        Assert.Equal([1, 0], people.Entries.Select(e => e.LibraryTitles));
        Assert.Equal(["A Walk to Remember"], (await _controller.GetLibraryTitlesWithAsync("mandy moore")).Select(i => i.Title));
    }

    [Fact]
    public async Task Images_go_in_the_image_cache_under_the_IMDb_id_else_TMDbs()
    {
        _tmdb.Lists[TrendingKind.Movies] = [Title(1, "Has IMDb id", "tt1"), Title(2, "Hasn't", null)];
        _tmdb.Lists[TrendingKind.People] = [new(5, "Mandy Moore", null, null, "https://image.tmdb.org/t/p/w185/m.jpg", null, null, [])];
        await _controller.GetAsync(TrendingKind.Movies);
        await _controller.GetAsync(TrendingKind.People);

        await _controller.GetImageAsync(TrendingKind.Movies, 1);
        await _controller.GetImageAsync(TrendingKind.Movies, 2);
        await _controller.GetImageAsync(TrendingKind.People, 5);
        Assert.Null(await _controller.GetImageAsync(TrendingKind.People, 999)); // not on the list

        Assert.Equal(
            [("tt1", "https://image.tmdb.org/t/p/w342/1.jpg"), ("tmdb-movie-2", "https://image.tmdb.org/t/p/w342/2.jpg"),
             ("tmdb-person-5", "https://image.tmdb.org/t/p/w185/m.jpg")],
            _images.Asked);
    }

    [Fact]
    public async Task The_database_keeps_every_field()
    {
        var person = new TrendingItem(5, "Mandy Moore", null, null, "https://x/m.jpg", null, null, ["Tangled", "This Is Us"]);
        var title = Title(1, "Clayface", "tt34890576");
        var at = new DateTime(2026, 10, 27, 0, 46, 0, DateTimeKind.Utc);
        await _cache.SaveAsync(TrendingKind.People, [person], at);
        await _cache.SaveAsync(TrendingKind.Movies, [title], at);

        var people = (await _cache.LoadAsync(TrendingKind.People))!.Value;
        var movies = (await _cache.LoadAsync(TrendingKind.Movies))!.Value;

        Assert.Equal(at, people.FetchedUtc);
        Assert.Equal((person.Title, person.ImageUrl, person.TmdbRating), (people.Items[0].Title, people.Items[0].ImageUrl, people.Items[0].TmdbRating));
        Assert.Equal(person.KnownFor, people.Items[0].KnownFor);
        Assert.Equal((title.ImdbId, title.Year, title.TmdbRating, title.Overview), (movies.Items[0].ImdbId, movies.Items[0].Year, movies.Items[0].TmdbRating, movies.Items[0].Overview));
        Assert.Null(await _cache.LoadAsync(TrendingKind.Series));
    }

    private sealed class FakeTrending : ITrendingSource
    {
        public Dictionary<TrendingKind, List<TrendingItem>> Lists { get; } = [];
        public Exception? Throws { get; set; }
        public int Calls { get; private set; }
        public IReadOnlyDictionary<int, string> LastKnown { get; private set; } = new Dictionary<int, string>();

        public Task<IReadOnlyList<TrendingItem>> GetTrendingAsync(TrendingKind kind, IReadOnlyDictionary<int, string> knownImdbIds, CancellationToken ct = default)
        {
            Calls++;
            LastKnown = knownImdbIds;
            return Throws is not null ? Task.FromException<IReadOnlyList<TrendingItem>>(Throws)
                : Task.FromResult<IReadOnlyList<TrendingItem>>(Lists.GetValueOrDefault(kind) ?? []);
        }
    }

    private sealed class RecordingImages : IPosterProvider
    {
        public List<(string Key, string? Url)> Asked { get; } = [];

        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default)
        {
            Asked.Add((item.ImdbId, item.Poster));
            return Task.FromResult<byte[]?>([1]);
        }
    }
}
