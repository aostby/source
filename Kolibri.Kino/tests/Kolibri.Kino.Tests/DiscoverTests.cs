using Kolibri.Kino.Controllers.Discover;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;
using FileItem = Kolibri.Kino.Core.Models.FileItem;

namespace Kolibri.Kino.Tests;

public sealed class DiscoverTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly LiteDbDiscoverCache _cache;
    private readonly FakeDiscover _tmdb = new();
    private readonly DiscoverController _controller;
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-discover-").FullName;

    public DiscoverTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _cache = new LiteDbDiscoverCache(_db);
        var libraryFiles = new LibraryFiles(_files, new FileSystemMediaScanner(), Microsoft.Extensions.Options.Options.Create(new KinoOptions()));
        _controller = new DiscoverController(_tmdb, _cache, _movies, libraryFiles, new NoImages(), NullLogger<DiscoverController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    private static TrendingItem Title(int id, string title, string? imdbId) => new(id, title, "1993", imdbId, null, 7.0, "A plot.", []);

    [Theory]
    [InlineData(1, DiscoverSeason.Winter)]
    [InlineData(2, DiscoverSeason.Winter)]
    [InlineData(4, DiscoverSeason.SpringAndEaster)]
    [InlineData(7, DiscoverSeason.Summer)]
    [InlineData(9, DiscoverSeason.AutumnAndThanksgiving)]
    [InlineData(10, DiscoverSeason.Halloween)]
    [InlineData(11, DiscoverSeason.ChristmasAndNewYear)]
    [InlineData(12, DiscoverSeason.ChristmasAndNewYear)]
    public void Discover_opens_on_the_season_we_are_in(int month, DiscoverSeason expected) =>
        Assert.Equal(expected, DiscoverController.CurrentSeason(new DateTime(2026, month, 15)));

    [Fact]
    public void Every_month_has_one_season_and_the_keywords_that_give_wrong_hits_are_left_out()
    {
        Assert.Equal(Enumerable.Range(1, 12), DiscoverSeasons.All.SelectMany(s => s.Months).Order());
        Assert.Equal(Enum.GetValues<DiscoverSeason>().Length, DiscoverSeasons.All.Count);
        var keywords = DiscoverSeasons.All.SelectMany(s => s.KeywordIds).ToList();
        Assert.Equal(keywords.Count, keywords.Distinct().Count());
        // holiday, nude beach, easter island, witch hunt, witch burning, killer santa claus, elf,
        // new year's eve, new year, witch, road trip, beach
        Assert.DoesNotContain(keywords, k => k is 65 or 193724 or 175992 or 177894 or 2871 or 175388 or 342557
            or 613 or 252123 or 616 or 7312 or 966);
    }

    [Fact]
    public async Task A_page_asks_TMDb_with_the_seasons_keywords_and_filters_then_comes_from_the_database()
    {
        _tmdb.Page = new DiscoverPage([Title(1, "Home Alone", "tt0099785")], 2, 5, 90);
        var filter = new DiscoverFilter(DiscoverSeason.ChristmasAndNewYear, Series: false, GenreId: 35, YearFrom: 1990, YearTo: 1999,
            MinRating: 7, Sort: DiscoverSort.BestRated);

        var result = await _controller.GetAsync(filter, page: 2);
        await _controller.GetAsync(filter, page: 2);

        var asked = Assert.Single(_tmdb.Requests);
        Assert.Equal(DiscoverSeasons.Of(DiscoverSeason.ChristmasAndNewYear).KeywordIds, asked.KeywordIds);
        Assert.Equal((false, 35, 1990, 1999, 7.0, DiscoverSort.BestRated, 2),
            (asked.Series, asked.GenreId, asked.YearFrom, asked.YearTo, asked.MinRating, asked.Sort, asked.Page));
        Assert.Equal((2, 5, 90, true), (result.Page, result.TotalPages, result.TotalResults, result.HasMore));

        await _controller.GetAsync(filter, page: 2, refresh: true);
        Assert.Equal(2, _tmdb.Requests.Count);
        await _controller.GetAsync(filter with { GenreId = 18 }, page: 2);
        Assert.Equal(3, _tmdb.Requests.Count); // another filter is another page
    }

    [Fact]
    public async Task When_TMDb_fails_the_kept_page_is_shown_with_a_note_and_without_one_it_says_why()
    {
        var filter = new DiscoverFilter(DiscoverSeason.Halloween);
        _tmdb.Throws = new HttpRequestException("offline");
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAsync(filter));

        await _cache.SaveAsync(filter.KeyFor(1), new DiscoverPage([Title(1, "Hocus Pocus", "tt0107120")], 1, 1, 1), DateTime.UtcNow.AddDays(-1));
        var result = await _controller.GetAsync(filter);

        Assert.Equal(["Hocus Pocus"], result.Entries.Select(e => e.Item.Title));
        Assert.StartsWith("TMDb couldn't be reached (offline)", result.Note);
    }

    [Fact]
    public async Task Titles_say_whether_you_have_them_with_a_file()
    {
        var file = Path.Combine(_folder, "Home Alone.mkv");
        File.WriteAllText(file, "");
        await _movies.UpsertAsync(Movie("tt0099785", "Home Alone", rating: "7.7"));
        await _movies.UpsertAsync(Movie("tt0104431", "Home Alone 2")); // imported, no file
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0099785", FullName = file });
        _tmdb.Page = new DiscoverPage([Title(1, "Home Alone", "tt0099785"), Title(2, "Home Alone 2", "tt0104431"), Title(3, "Elf", "tt0319343")], 1, 1, 3);

        var result = await _controller.GetAsync(new DiscoverFilter(DiscoverSeason.ChristmasAndNewYear));

        Assert.Equal([(true, true, "7.7"), (true, false, "7.0"), (false, false, null)],
            result.Entries.Select(e => (e.InLibrary, e.HasFile, e.ImdbRating)));
    }

    [Fact]
    public async Task The_database_keeps_a_page()
    {
        var page = new DiscoverPage([Title(1, "Hocus Pocus", "tt0107120"), Title(2, "No IMDb id", null)], 3, 12, 230);
        await _cache.SaveAsync("key", page, DateTime.UtcNow);

        var kept = (await _cache.LoadAsync("key"))!.Value.Page;

        Assert.Equal((3, 12, 230), (kept.Page, kept.TotalPages, kept.TotalResults));
        Assert.Equal(["tt0107120", null], kept.Items.Select(i => i.ImdbId));
        Assert.Null(await _cache.LoadAsync("other"));
    }

    [Fact]
    public void TMDbs_address_has_any_of_the_keywords_and_the_filters()
    {
        var today = new DateTime(2026, 10, 28);
        var popular = TmdbDiscover.Address(new DiscoverRequest([3335, 616], false, 27, 1990, 1999, 6, DiscoverSort.Popular, 2), "key", today);
        Assert.StartsWith("https://api.themoviedb.org/3/discover/movie?", popular);
        foreach (var part in new[] { "with_keywords=3335|616", "sort_by=popularity.desc", "page=2", "with_genres=27", "include_adult=false",
                     "primary_release_date.gte=1990-01-01", "primary_release_date.lte=1999-12-31", "vote_average.gte=6", "vote_count.gte=20" })
            Assert.Contains(part, popular);

        var bestRated = TmdbDiscover.Address(new DiscoverRequest([1], false, null, null, null, null, DiscoverSort.BestRated, 1), "key", today);
        Assert.Contains("sort_by=vote_average.desc", bestRated);
        Assert.Contains("vote_count.gte=200", bestRated);
        Assert.DoesNotContain("with_genres", bestRated);

        var newestTv = TmdbDiscover.Address(new DiscoverRequest([1], true, null, 2020, null, null, DiscoverSort.Newest, 1), "key", today);
        Assert.StartsWith("https://api.themoviedb.org/3/discover/tv?", newestTv);
        Assert.Contains("sort_by=first_air_date.desc", newestTv);
        Assert.Contains("first_air_date.gte=2020-01-01", newestTv);
        Assert.Contains("first_air_date.lte=2026-10-28", newestTv); // nothing that hasn't aired yet
    }

    private sealed class FakeDiscover : IDiscoverSource
    {
        public DiscoverPage Page { get; set; } = new([], 1, 0, 0);
        public Exception? Throws { get; set; }
        public List<DiscoverRequest> Requests { get; } = [];

        public Task<DiscoverPage> DiscoverAsync(DiscoverRequest request, CancellationToken ct = default)
        {
            Requests.Add(request);
            return Throws is not null ? Task.FromException<DiscoverPage>(Throws) : Task.FromResult(Page);
        }

        public Task<IReadOnlyList<DiscoverGenre>> GetGenresAsync(bool series, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<DiscoverGenre>>([new(35, "Comedy")]);
    }

    private sealed class NoImages : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
