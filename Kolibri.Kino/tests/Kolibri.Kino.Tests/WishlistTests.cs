using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class WishlistTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly FakeOmdb _omdb = new();
    private readonly TmdbTitles _tmdb = new();
    private readonly FakeTmdb _tmdbSearch = new();
    private readonly WishlistController _wishlist;

    public WishlistTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _wishlist = new WishlistController(new LiteDbWishlistRepository(_db), _movies, _files, _omdb, _tmdb, _tmdbSearch, NullLogger<WishlistController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task A_title_in_the_library_without_a_file_is_still_to_get()
    {
        var folder = Directory.CreateTempSubdirectory("kino-wishlist-").FullName;
        try
        {
            var file = Path.Combine(folder, "Have.mkv");
            File.WriteAllText(file, "");
            await _movies.UpsertAsync(LiteDbMovieRepositoryTests.Movie("tt0000001", "Have"));
            await _movies.UpsertAsync(LiteDbMovieRepositoryTests.Movie("tt0000002", "Imported, no file"));
            await _files.UpsertAsync(new Kolibri.Kino.Core.Models.FileItem { ImdbId = "tt0000001", FullName = file });
            _omdb.Add(LiteDbMovieRepositoryTests.Movie("tt0000003", "To get"));
            var libraryFiles = new Kolibri.Kino.Controllers.Library.LibraryFiles(_files, new FileSystemMediaScanner(),
                Microsoft.Extensions.Options.Options.Create(new Kolibri.Kino.Core.KinoOptions()));
            var wishlist = new WishlistController(new LiteDbWishlistRepository(_db), _movies, _files, _omdb, _tmdb, _tmdbSearch,
                NullLogger<WishlistController>.Instance, libraryFiles);
            foreach (var id in new[] { "tt0000001", "tt0000002", "tt0000003" }) await wishlist.AddAsync(id);

            var rows = await wishlist.GetAsync();

            // To get first: the one not in the library and the one without a file, then the one you have.
            Assert.Equal([("Imported, no file", false, true), ("To get", false, false), ("Have", true, true)],
                rows.Select(r => (r.Item.Title, r.InSilverScreen, r.InLibrary)));
            Assert.Contains(";false;", WishlistController.ToCsv(rows).Split('\n').Single(l => l.StartsWith("Imported")));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Theory]
    [InlineData("Operation Kabul, 2025", "Operation Kabul", 2025)]
    [InlineData("Operation Kabul (2025)", "Operation Kabul", 2025)]
    [InlineData("  heat ,1995 ", "heat", 1995)]
    [InlineData("Blade Runner 2049", "Blade Runner 2049", null)]
    [InlineData("1917", "1917", null)]
    [InlineData("Ocean's Eleven, 3000", "Ocean's Eleven, 3000", null)]
    public void A_year_after_a_comma_or_in_brackets_is_the_year(string text, string title, int? year) =>
        Assert.Equal((title, year), WishlistController.SplitYear(text));

    [Fact]
    public async Task Title_comma_year_keeps_to_that_year_and_uses_TMDbs_English_title()
    {
        await _movies.UpsertAsync(Movie("tt0001", "Operation Kabul", year: "2003")); // in the library, the other year
        _omdb.Searchable.AddRange([Movie("tt0001", "Operation Kabul", year: "2003"), Movie("tt34375721", "Sárkányok Kabul felett", year: "2025")]);
        _omdb.Searchable[1].Title = "Kabul"; // the fake finds by title; OMDb's own title for it is Hungarian
        _omdb.Add(Movie("tt34375721", "Sárkányok Kabul felett", year: "2025"));
        _tmdb.Titles["tt34375721"] = "Operation Kabul";

        var hit = Assert.Single((await _wishlist.SearchAsync("Kabul, 2025")).Hits);
        Assert.Equal(("tt34375721", "Operation Kabul", false), (hit.ImdbId, hit.Title, hit.InSilverScreen));

        Assert.Equal("Operation Kabul", (await _wishlist.AddAsync("tt34375721")).Title);
    }

    [Fact]
    public async Task TMDb_finds_the_English_title_OMDb_doesnt_know()
    {
        // OMDb's search misses it (its title there is Hungarian); TMDb's finds it.
        _tmdbSearch.Movies.AddRange([new("tt34375721", "Operation Kabul", "2025"), new("tt0001", "Operation Kabul", "2003")]);

        var hit = Assert.Single((await _wishlist.SearchAsync("Operation Kabul, 2025")).Hits);

        Assert.Equal(("tt34375721", "Operation Kabul", "2025", false), (hit.ImdbId, hit.Title, hit.Year, hit.InSilverScreen));
    }

    [Fact]
    public async Task Series_are_found_and_added_but_not_games()
    {
        _omdb.Searchable.AddRange([
            new OMDbApiNet.Model.Item { ImdbId = "tt0903747", Title = "Breaking Bad", Year = "2008–2013", Type = "series" },
            new OMDbApiNet.Model.Item { ImdbId = "tt0000042", Title = "Breaking Bad: The Game", Year = "2010", Type = "game" }]);
        _omdb.Add(new OMDbApiNet.Model.Item { ImdbId = "tt0903747", Title = "Breaking Bad", Year = "2008–2013", Type = "series" });

        var hit = Assert.Single((await _wishlist.SearchAsync("Breaking Bad")).Hits);
        var added = await _wishlist.AddAsync(hit.ImdbId);

        Assert.Equal(("tt0903747", "series", false), (hit.ImdbId, hit.Type, hit.InSilverScreen));
        Assert.Equal(("series", "2008–2013"), (added.Type, added.Year));
    }

    [Fact]
    public async Task Missing_episodes_go_on_the_wishlist_and_come_off_when_they_are_here()
    {
        await _movies.UpsertAsync(new OMDbApiNet.Model.Item { ImdbId = "tt5875444", Title = "The Show", Type = "series", Genre = "Drama" });
        var episodes = new[]
        {
            new SeriesEpisode(6, 3, "Three", new DateTime(2026, 10, 1), null, null, null, null, null),
            new SeriesEpisode(6, 4, "Four", null, null, null, null, null, null),
        };

        Assert.Equal(2, await _wishlist.AddEpisodesAsync("tt5875444", episodes));
        Assert.Equal(0, await _wishlist.AddEpisodesAsync("tt5875444", episodes));   // already there
        var item = Assert.Single(await _wishlist.GetAsync(), r => r.Item.ImdbId == "tt5875444_S06E03").Item;
        Assert.Equal(("The Show S06E03: Three", "episode", "2026", true, "tt5875444"), (item.Title, item.Type, item.Year, item.IsEpisode, item.TitleImdbId));

        Assert.Equal(["S06E03"], await _wishlist.RemoveEpisodesOnDiskAsync("tt5875444", [(6, 3), (6, 5)]));
        Assert.Equal([(6, 4)], await _wishlist.GetEpisodesAsync("tt5875444"));
    }

    [Fact]
    public async Task Import_adds_new_entries_and_skips_IMDb_ids_already_there()
    {
        _omdb.Add(new OMDbApiNet.Model.Item { ImdbId = "tt34375721", Title = "Operation Kabul", Year = "2025", Type = "movie" });
        await _wishlist.AddAsync("tt34375721");
        const string csv = "\uFEFFTitle;Year;Type;IMDb rating;Genre;IMDb id;In SilverScreen;File;Added\r\n" +
                           "Operation Kabul;2025;movie;;Action;tt34375721;false;;2026-10-17\r\n" +
                           "Ted Lasso S04E09: Mae Rides the Bus;2026;episode;;\"Comedy; Drama\";tt10986410_S04E09;false;;2026-10-18\r\n" +
                           "Ted Lasso S04E09 again;2026;episode;;;TT10986410_s4e9;false;;\r\n" +
                           "No id;2026;movie;;;;false;;\r\n";

        var result = await _wishlist.ImportCsvAsync(csv);

        Assert.Equal(new WishlistImport(1, 2, 1), result);
        var episode = Assert.Single(await _wishlist.GetAsync(), r => r.Item.IsEpisode).Item;
        Assert.Equal(("tt10986410_S04E09", "Ted Lasso S04E09: Mae Rides the Bus", "Comedy; Drama", new DateTime(2026, 10, 18)),
            (episode.ImdbId, episode.Title, episode.Genre, episode.Added));
        await Assert.ThrowsAsync<FormatException>(() => _wishlist.ImportCsvAsync("Title;Year\r\nX;2020\r\n"));
    }

    [Fact]
    public void Csv_reads_what_it_writes()
    {
        var line = Kolibri.Kino.Controllers.Csv.Line("a;b", "say \"hi\"", "two\nlines", "", "plain");

        Assert.Equal([["a;b", "say \"hi\"", "two\nlines", "", "plain"]], Kolibri.Kino.Controllers.Csv.Parse(line + "\r\n\r\n"));
    }

    private sealed class TmdbTitles : Kolibri.Kino.Core.Abstractions.ITmdbLinks
    {
        public Dictionary<string, string> Titles { get; } = [];
        public Task<string?> FindTitleAsync(string imdbId, CancellationToken ct = default) => Task.FromResult(Titles.GetValueOrDefault(imdbId));
        public Task<string?> FindPageAsync(string imdbId, CancellationToken ct = default) => Task.FromResult<string?>(null);
        public Task<string?> FindPosterAsync(string imdbId, CancellationToken ct = default) => Task.FromResult<string?>(null);
        public Task<(double Rating, int Votes)?> FindRatingAsync(string imdbId, CancellationToken ct = default) => Task.FromResult<(double, int)?>(null);
        public Task<IReadOnlyList<Kolibri.Kino.Core.Abstractions.TmdbMovieHit>> FindSimilarAsync(string imdbId, int max = 12, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Kolibri.Kino.Core.Abstractions.TmdbMovieHit>>([]);
    }

    [Fact]
    public async Task Search_finds_the_library_first_then_OMDb_saying_which_is_which()
    {
        await _movies.UpsertAsync(Movie("tt1", "Heat", year: "1995"));
        await _movies.UpsertAsync(Movie("tt3", "Heat Wave", year: "2009")); // OMDb finds it too, but not by this title here
        _omdb.Searchable.AddRange([Movie("tt1", "Heat"), Movie("tt2", "The Heat", year: "2013"), Movie("tt3", "Heat Wave")]);

        var result = await _wishlist.SearchAsync("heat");

        Assert.Null(result.OnlineError);
        Assert.Equal([("tt1", true), ("tt3", true), ("tt2", false)], result.Hits.Select(h => (h.ImdbId, h.InSilverScreen)));
    }

    [Fact]
    public async Task Without_OMDb_the_library_is_still_searched()
    {
        await _movies.UpsertAsync(Movie("tt1", "Heat"));
        _omdb.UnavailableMessage = "OMDb: Request limit reached!";

        var result = await _wishlist.SearchAsync("Heat");

        Assert.Equal("OMDb: Request limit reached!", result.OnlineError);
        Assert.Equal("tt1", Assert.Single(result.Hits).ImdbId);
    }

    [Fact]
    public async Task A_wanted_title_from_OMDb_is_not_put_in_the_library_and_shows_to_get_until_it_is()
    {
        _omdb.Add(Movie("tt2", "The Heat", year: "2013"));
        _omdb.Searchable.Add(Movie("tt2", "The Heat", year: "2013"));

        var added = await _wishlist.AddAsync("tt2");
        Assert.Equal((added.ImdbId, added.Title), ((await _wishlist.AddAsync("tt2")).ImdbId, (await _wishlist.AddAsync("tt2")).Title)); // once
        Assert.Null(await _movies.GetByImdbIdAsync("tt2"));
        Assert.False(Assert.Single(await _wishlist.GetAsync()).InSilverScreen);
        Assert.True((await _wishlist.SearchAsync("The Heat")).Hits.Single().OnWishlist);

        // Got it later: the list says so, with the file.
        await _movies.UpsertAsync(Movie("tt2", "The Heat", year: "2013"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt2", FullName = @"X:\Movies\The Heat (2013)\heat.mkv" });
        var row = Assert.Single(await _wishlist.GetAsync());
        Assert.Equal((true, @"X:\Movies\The Heat (2013)\heat.mkv"), (row.InSilverScreen, row.FilePath));
    }

    [Fact]
    public async Task The_file_has_In_SilverScreen_as_true_or_false_and_lists_those_to_get_first()
    {
        await _movies.UpsertAsync(Movie("tt1", "Heat"));
        _omdb.Add(Movie("tt2", "The Heat", year: "2013"));
        await _wishlist.AddAsync("tt1");
        await _wishlist.AddAsync("tt2");

        var lines = WishlistController.ToCsv(await _wishlist.GetAsync()).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("Title;Year;Type;IMDb rating;Genre;IMDb id;In SilverScreen;File;Added", lines[0]);
        Assert.StartsWith("The Heat;2013;movie;7.0;;tt2;false;;", lines[1]);
        Assert.StartsWith("Heat;1999;movie;7.0;;tt1;true;;", lines[2]);
        Assert.True(await _wishlist.RemoveAsync("tt1"));
        Assert.Single(await _wishlist.GetAsync());
    }
}
