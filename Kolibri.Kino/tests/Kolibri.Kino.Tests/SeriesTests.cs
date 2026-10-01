using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using LiteDB;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class EpisodeFileFinderTests
{
    [Theory]
    [InlineData(@"X:\Buffy\Buffy.the.Vampire.Slayer.S01E02.720p.mkv", 1, new[] { 2 })]
    [InlineData(@"X:\Show\show.s03e10.avi", 3, new[] { 10 })]
    [InlineData(@"X:\Show\Show.S01E01E02.mkv", 1, new[] { 1, 2 })]
    [InlineData(@"X:\Show\Show.S02E01-E03.mkv", 2, new[] { 1, 2, 3 })]
    [InlineData(@"X:\Show\Show - 4x07 - Title.avi", 4, new[] { 7 })]
    [InlineData(@"X:\Sauen Shaun\SauenShaun_06\SauenShaun_S06E02.mkv", 6, new[] { 2 })]
    [InlineData(@"X:\Sauen Shaun\Sauen Shaun 01\Shaun.The.Sheep.E06.Still.life (thecodeishere).avi", 1, new[] { 6 })]
    [InlineData(@"X:\Sauen Shaun\Sauen Shaun 02\Shaun.The.Sheep.E26.Washing.Day.avi", 2, new[] { 26 })]
    [InlineData(@"X:\Show\Season 3\Episode 5.mkv", 3, new[] { 5 })]
    [InlineData(@"X:\Knøttene\Knøttene Julefeiring Episode 2 - Norsk Tale.avi", 1, new[] { 2 })]
    public void Reads_season_and_episode_from_names(string path, int season, int[] episodes)
    {
        var file = EpisodeFileFinder.Parse(path);

        Assert.NotNull(file);
        Assert.Equal(season, file.SeasonNumber);
        Assert.Equal(episodes, file.EpisodeNumbers);
    }

    [Theory]
    [InlineData(@"X:\Pixar\Presto.2008.720p.BluRay.x264.mkv")]
    [InlineData(@"X:\Show\title02.mkv")]
    public void Names_without_an_episode_are_not_guessed(string path) => Assert.Null(EpisodeFileFinder.Parse(path));
}

public sealed class LocalSeriesControllerTests : IDisposable
{
    private const string SeriesId = "tt0118276";

    private readonly TempDatabase _temp = new();
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-series-").FullName;
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly LiteDbSeriesRepository _seasons;
    private readonly FakeSeriesInfo _online = new();
    private readonly FakeOmdb _omdb = new();
    private readonly LocalSeriesController _controller;

    public LocalSeriesControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _seasons = new LiteDbSeriesRepository(_db);
        var scanner = new FileSystemMediaScanner();
        var linker = new Kolibri.Kino.Controllers.Library.MovieLinker(_files, _movies, new FakePlex { IsConfigured = false }, _omdb);
        _controller = new LocalSeriesController(_movies, _files, scanner, _seasons, _online, new EpisodeFileFinder(scanner),
            new NoPosters(), new InMemorySettingsStore(), linker, NullLogger<LocalSeriesController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public async Task Lists_series_with_their_folders_best_rated_first()
    {
        await _movies.UpsertAsync(Series(SeriesId, "Buffy", "8.3"));
        await _movies.UpsertAsync(Series("tt0000002", "Gone Show", "9.0", tomatoUrl: @"Z:\nowhere"));
        await _movies.UpsertAsync(Series("tt0000003", "No Folder", "7.0"));
        await _movies.UpsertAsync(new Item { ImdbId = "tt0133093", Title = "The Matrix", Type = "movie" });
        await _files.UpsertAsync(new FileItem { ImdbId = SeriesId, FullName = _folder });

        var series = await _controller.GetSeriesAsync();

        Assert.Equal([("Gone Show", false, @"Z:\nowhere"), ("Buffy", true, _folder), ("No Folder", false, null)],
            series.Select(s => (s.Item.Title, s.FolderExists, s.FolderPath)));
    }

    [Fact]
    public async Task Fetches_online_once_marks_files_on_disk_and_then_uses_the_cache()
    {
        Touch("Season 1", "Buffy.S01E01.mkv");
        Touch("Season 1", "Buffy.S01E03.mkv");   // not in the fetched list
        Touch("Season 2", "Buffy.S02E01.mkv");   // season not in the fetched list
        Touch("Extras", "Making of.mkv");
        _online.Seasons = [Season(1, 2, "TMDb")];

        var first = await _controller.GetDetailsAsync(Local(), refresh: false);
        var second = await _controller.GetDetailsAsync(Local(), refresh: false);

        Assert.Equal(1, _online.Calls);
        Assert.Equal(("TMDb", false, true), (first.Source, first.FromCache, second.FromCache));
        Assert.Equal([1, 2], first.Seasons.Select(s => s.SeasonNumber));
        Assert.Equal([(1, true), (2, false), (3, true)], first.Seasons[0].Episodes.Select(e => (e.Episode.EpisodeNumber, e.OnDisk)));
        Assert.Equal("Buffy.S01E03", first.Seasons[0].Episodes[2].Episode.Title); // file-only row named after the file
        Assert.Single(first.Seasons[1].Episodes);
        Assert.Single(first.UnrecognisedFiles);
    }

    [Fact]
    public async Task Refresh_fetches_again_even_with_a_cache()
    {
        _online.Seasons = [Season(1, 2, "TMDb")];
        await _controller.GetDetailsAsync(Local(), refresh: false);
        _online.Seasons = [Season(1, 3, "TMDb")];

        var refreshed = await _controller.GetDetailsAsync(Local(), refresh: true);

        Assert.Equal(2, _online.Calls);
        Assert.Equal(3, refreshed.Seasons[0].Episodes.Count);
        Assert.Equal(3, (await _seasons.GetCachedSeasonsAsync(SeriesId))[0].Episodes.Count);
    }

    [Fact]
    public async Task Falls_back_to_what_SilverScreen_stored_when_nothing_can_be_fetched()
    {
        SilverScreenStoredSeason1();
        _online.Throws = new MovieInfoUnavailableException("OMDb: Request limit reached!");

        var details = await _controller.GetDetailsAsync(Local(), refresh: false);

        Assert.Equal(("OMDb", "OMDb: Request limit reached!"), (details.Source, details.Note));
        var episode = Assert.Single(details.Seasons.Single().Episodes).Episode;
        Assert.Equal(("Welcome to the Hellmouth", "tt0452716", 8.1, 43, new DateTime(1997, 3, 10)),
            (episode.Title, episode.ImdbId, episode.Rating, episode.RuntimeMinutes, episode.AirDate));
        Assert.StartsWith("When teen vampire slayer", episode.Plot);
        Assert.Empty(await _seasons.GetCachedSeasonsAsync(SeriesId)); // SilverScreen's data isn't copied into Kino's cache
    }

    [Fact]
    public async Task A_folder_holding_other_series_is_too_broad_and_its_files_are_not_matched()
    {
        // Misfits and Firefly were linked to \\GREENLANTERN\Series itself; 'Allo 'Allo! files then showed as Misfits episodes.
        var allo = Path.Combine(_folder, "'Allo 'Allo! {imdb-tt0086659}");
        Touch("'Allo 'Allo! {imdb-tt0086659}", "Allo.Allo.S01E01.avi");
        await _movies.UpsertAsync(Series("tt0086659", "'Allo 'Allo!", "8.3"));
        await _movies.UpsertAsync(Series("tt1548850", "Misfits", "8.1"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0086659", FullName = allo });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt1548850", FullName = _folder + Path.DirectorySeparatorChar });
        _online.Seasons = [Season(1, 2, "TMDb")];

        var series = await _controller.GetSeriesAsync();
        var misfits = series.Single(s => s.Item.Title == "Misfits");
        var details = await _controller.GetDetailsAsync(misfits, refresh: false);

        Assert.Equal((true, false), (misfits.FolderTooBroad, series.Single(s => s.Item.Title == "'Allo 'Allo!").FolderTooBroad));
        Assert.All(details.Seasons.SelectMany(s => s.Episodes), e => Assert.False(e.OnDisk));
        Assert.Contains("Set folder", details.Note);

        var fixedUp = await _controller.SetFolderAsync(misfits, Path.Combine(_folder, "Misfits"));
        Assert.False(fixedUp.FolderTooBroad);
    }

    [Fact]
    public async Task Changing_the_imdb_id_moves_the_folder_to_the_right_series()
    {
        // "Dark Matter": the folder holds the 2024 show but was matched to the 2015 one.
        var folder = Path.Combine(_folder, "Dark Matter");
        Directory.CreateDirectory(folder);
        var wrong = Series("tt4159076", "Dark Matter", "7.4", tomatoUrl: folder);
        await _movies.UpsertAsync(wrong);
        await _files.UpsertAsync(new FileItem { ImdbId = "tt4159076", FullName = folder });
        _omdb.Add(Series("tt19231492", "Dark Matter", "7.8"));

        var found = await _controller.LookUpAsync(" TT19231492 ");
        Assert.NotNull(found);
        Assert.Null(found.Value.LinkedFolder);

        var series = (await _controller.GetSeriesAsync()).Single();
        var updated = await _controller.ChangeImdbIdAsync(series, found.Value.Item);

        Assert.Equal(("tt19231492", folder), (updated.Item.ImdbId, updated.FolderPath));
        var all = await _controller.GetSeriesAsync();
        Assert.Equal(folder, all.Single(s => s.Item.ImdbId == "tt19231492").FolderPath);
        Assert.Null(all.Single(s => s.Item.ImdbId == "tt4159076").FolderPath); // stays in the library, without the folder
        Assert.Null(await _files.GetByImdbIdAsync("tt4159076"));
    }

    [Fact]
    public async Task A_movie_id_is_refused()
    {
        await _files.UpsertAsync(new FileItem { ImdbId = SeriesId, FullName = _folder });
        var movie = new Item { ImdbId = "tt0133093", Title = "The Matrix", Type = "movie" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.ChangeImdbIdAsync(Local(), movie));
        Assert.Equal(_folder, (await _files.GetByImdbIdAsync(SeriesId))?.FullName);
    }

    [Fact]
    public async Task Setting_a_folder_links_the_series_to_it()
    {
        var updated = await _controller.SetFolderAsync(Local() with { FolderPath = null, FolderExists = false }, _folder);

        Assert.True(updated.FolderExists);
        Assert.Equal(_folder, (await _files.GetByImdbIdAsync(SeriesId))?.FullName);
    }

    private LocalSeries Local() => new(Series(SeriesId, "Buffy", "8.3"), _folder, FolderExists: true);

    private static Item Series(string id, string title, string rating, string? tomatoUrl = null) =>
        new() { ImdbId = id, Title = title, Type = "series", ImdbRating = rating, TotalSeasons = "7", TomatoUrl = tomatoUrl, Response = "True" };

    private static SeriesSeason Season(int number, int episodes, string source) => new(number, $"Season {number}",
        Enumerable.Range(1, episodes).Select(e => new SeriesEpisode(number, e, $"Episode {e}", null, "Plot", 8.0, 42, null, null)).ToList(), source);

    /// <summary>As SilverScreen's GetShowByIdAsync stores it: the OMDb season list and each episode's OMDb record.</summary>
    private void SilverScreenStoredSeason1()
    {
        _db.Database.GetCollection("KolibriSeason").Insert(new BsonDocument
        {
            ["_id"] = $"{SeriesId}_1", ["SeriesId"] = SeriesId, ["Title"] = "Buffy", ["SeasonNumber"] = "1", ["TotalSeasons"] = "7",
            ["Episodes"] = new BsonArray(new BsonDocument
            {
                ["Title"] = "Welcome to the Hellmouth", ["Released"] = "1997-03-10", ["Episode"] = "1", ["ImdbRating"] = "8.1", ["ImdbId"] = "tt0452716",
            }),
        });
        _db.Database.GetCollection("Episode").Insert(new BsonDocument
        {
            ["_id"] = "tt0452716", ["Title"] = "Welcome to the Hellmouth", ["Released"] = "10 Mar 1997", ["Runtime"] = "43 min",
            ["Plot"] = "When teen vampire slayer Buffy tries to start a new life at Sunnydale High…", ["ImdbRating"] = "8.1",
        });
    }

    private void Touch(string subfolder, string name)
    {
        var path = Path.Combine(_folder, subfolder, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "x");
    }

    private sealed class FakeSeriesInfo : ISeriesInfoProvider
    {
        public IReadOnlyList<SeriesSeason> Seasons { get; set; } = [];
        public Exception? Throws { get; set; }
        public int Calls { get; private set; }

        public Task<IReadOnlyList<SeriesSeason>> GetSeasonsAsync(string seriesImdbId, int totalSeasonsHint, CancellationToken ct = default)
        {
            Calls++;
            return Throws is not null ? Task.FromException<IReadOnlyList<SeriesSeason>>(Throws) : Task.FromResult(Seasons);
        }
    }

    private sealed class NoPosters : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
