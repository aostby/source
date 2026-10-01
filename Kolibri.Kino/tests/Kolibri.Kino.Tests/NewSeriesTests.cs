using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class SeriesFolderNameTests
{
    // Folder names from \\GREENLANTERN\Series.
    [Theory]
    [InlineData("Hatfields.and.McCoys.2012 {imdb-tt1985443}", "tt1985443", "Hatfields and McCoys", 2012)]
    [InlineData("Preacher {tt5016504}", "tt5016504", "Preacher", null)]
    [InlineData("Sh'at Neila (Valley Of Tears)  {imdb-tt8190688}", "tt8190688", "Sh'at Neila", null)]
    [InlineData("Monsters 1988 Complete Seasons 1 to 3 TVRip x264 [i_c]", null, "Monsters", 1988)]
    [InlineData("Secret Army (1977) - Complete - DVDRip 576p - Plus Unbroadcast Episode", null, "Secret Army", 1977)]
    [InlineData("The Hitchhiker's Guide to the Galaxy (1981)", null, "The Hitchhiker's Guide to the Galaxy", 1981)]
    [InlineData("Man on Fire [2026] MUlti YG", null, "Man on Fire", 2026)]
    [InlineData("The.Road.2025.S01E07.1080p.HEVC.x265-MeGusta[EZTVx.to]", null, "The Road", 2025)]
    [InlineData("The.Lowdown.2025.S01.720p.WEB-DL-[Feranki1980]", null, "The Lowdown", 2025)]
    [InlineData("War.Machine.World.War.II (2025)", null, "War Machine World War II", 2025)]
    [InlineData("The Andromeda Strain 2008 Mini Series 720p x264 [i_c]", null, "The Andromeda Strain", 2008)]
    [InlineData("Tom.Clancys.Jack.Ryan.S03.COMPLETE.REPACK.720p.AMZN.WEBRip.x264-GalaxyTV[TGx]", null, "Tom Clancys Jack Ryan", null)]
    [InlineData("The.North.Water.S01.COMPLETE.720p.AMZN.WEBRip.x264-GalaxyTV[TGx]", null, "The North Water", null)]
    [InlineData("The.Abandons.S01.1080p.NF.WEB-DL.H.264-EniaHD", null, "The Abandons", null)]
    [InlineData("Lessons.in.Chemistry.S01.COMPLETE.720p.ATVP.WEBRip.x264[EZTVx.to]", null, "Lessons in Chemistry", null)]
    [InlineData("The.Last.Days.Of.Ptolemy.Grey", null, "The Last Days Of Ptolemy Grey", null)]
    [InlineData("BOSCH", null, "BOSCH", null)]
    [InlineData("1923", null, "1923", null)]
    public void Reads_imdb_tag_title_and_year(string name, string? imdbId, string title, int? year) =>
        Assert.Equal(new SeriesFolderGuess(imdbId, title, year), SeriesFolderName.Parse(name));
}

public sealed class NewSeriesControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly string _root = Directory.CreateTempSubdirectory("kino-newseries-").FullName;
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly FakeOmdb _omdb = new();
    private readonly FakeTmdb _tmdb = new();
    private readonly NewSeriesController _controller;

    public NewSeriesControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        var plex = new FakePlex { IsConfigured = false };
        var scanner = new FileSystemMediaScanner();
        var linker = new MovieLinker(_files, _movies, plex, _omdb);
        var settings = new InMemorySettingsStore();
        settings.Current.UserFilePaths.SeriesSourcePath = _root;
        var lookup = new ManualLookupController(new MovieFileNameParser(), _movies, plex, _omdb, _files, scanner, new NoPosters(), linker,
            NullLogger<ManualLookupController>.Instance);
        _controller = new NewSeriesController(_movies, _files, scanner, new FileSystemFolderRenamer(), new LiteDbSeriesRepository(_db), _tmdb, _omdb, settings, linker,
            lookup, NullLogger<NewSeriesController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public async Task Finds_folders_nothing_is_linked_to()
    {
        var linkedSeries = Folder("Buffy {imdb-tt0118276}");
        var withLinkedMovie = Folder("The.Many.Saints.of.Newark.2021.720p");
        Folder("#recycle");
        Folder("Hatfields.and.McCoys.2012 {imdb-tt1985443}");
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0118276", FullName = linkedSeries + @"\" });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt8110232", FullName = Path.Combine(withLinkedMovie, "movie.mkv") });

        var scan = await _controller.FindNewFoldersAsync(includeIgnored: false);

        var folder = Assert.Single(scan.Folders);
        Assert.Equal(("Hatfields.and.McCoys.2012 {imdb-tt1985443}", "tt1985443"), (folder.Name, folder.Guess.ImdbId));
    }

    [Fact]
    public async Task Links_a_folder_by_its_imdb_tag()
    {
        var path = Folder("Hatfields.and.McCoys.2012 {imdb-tt1985443}");
        _omdb.Add(Series("tt1985443", "Hatfields & McCoys", "2012"));

        var result = await AddAll();

        Assert.Equal((NewSeriesStatus.Linked, "tt1985443"), (result.Status, result.Item?.ImdbId));
        Assert.Equal(path, (await _files.GetByImdbIdAsync("tt1985443"))?.FullName);
        Assert.Equal(path, (await _movies.GetByImdbIdAsync("tt1985443"))?.TomatoUrl);
        Assert.Empty((await _controller.FindNewFoldersAsync(includeIgnored: false)).Folders);
    }

    [Fact]
    public async Task An_imdb_tag_for_a_movie_is_not_linked_but_asked_about()
    {
        Folder("Horizon {imdb-tt17505010}");
        _omdb.Add(Horizon());

        var result = await AddAll();

        Assert.Equal(NewSeriesStatus.NeedsInput, result.Status);
        Assert.Contains("not a series", result.Note);
        Assert.Null(await _files.GetByImdbIdAsync("tt17505010"));
        Assert.Null(await _movies.GetByImdbIdAsync("tt17505010"));
    }

    [Fact]
    public async Task Title_and_year_are_matched_through_TMDb()
    {
        var path = Folder("Monsters 1988 Complete Seasons 1 to 3 TVRip x264 [i_c]");
        _tmdb.SeriesIds["Monsters|1988"] = "tt0094517";
        _omdb.Add(Series("tt0094517", "Monsters", "1988–1991"));

        var result = await AddAll();

        Assert.Equal((NewSeriesStatus.Linked, "by TMDb"), (result.Status, result.Note));
        Assert.Equal(path, (await _files.GetByImdbIdAsync("tt0094517"))?.FullName);
    }

    [Fact]
    public async Task A_series_already_in_the_library_is_matched_by_title_ignoring_punctuation()
    {
        var path = Folder("Tom.Clancys.Jack.Ryan.S03.COMPLETE.720p");
        await _movies.UpsertAsync(Series("tt5057054", "Tom Clancy's Jack Ryan", "2018–2023"));

        var result = await AddAll();

        Assert.Equal((NewSeriesStatus.Linked, "tt5057054"), (result.Status, result.Item?.ImdbId));
        Assert.Equal(path, (await _files.GetByImdbIdAsync("tt5057054"))?.FullName);
    }

    [Fact]
    public async Task A_title_without_a_year_is_not_looked_up_online()
    {
        Folder("Daredevil");

        var result = await AddAll();

        Assert.Equal(NewSeriesStatus.NeedsInput, result.Status);
        Assert.Equal((0, 0), (_tmdb.Calls, _omdb.Calls));
    }

    [Fact]
    public async Task A_series_linked_to_another_existing_folder_is_not_moved()
    {
        var other = Folder("Breaking Bad");
        Folder("Breaking.Bad {imdb-tt0903747}");
        await _movies.UpsertAsync(Series("tt0903747", "Breaking Bad", "2008–2013"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0903747", FullName = other });

        // "Breaking Bad" is linked, so only the tagged folder is new.
        var result = await AddAll();

        Assert.Equal((NewSeriesStatus.LinkedElsewhere, other), (result.Status, result.OtherFolder));
        Assert.Equal(other, (await _files.GetByImdbIdAsync("tt0903747"))?.FullName);
    }

    [Fact]
    public async Task A_series_linked_to_the_series_root_is_moved_to_its_own_folder()
    {
        // SilverScreen linked Breaking Bad to \\GREENLANTERN\Series itself.
        var path = Folder("Breaking.Bad {imdb-tt0903747}");
        await _movies.UpsertAsync(Series("tt0903747", "Breaking Bad", "2008–2013"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0903747", FullName = _root });

        var scan = await _controller.FindNewFoldersAsync(includeIgnored: false);
        var result = Assert.Single(await _controller.AddAutomaticallyAsync(scan.Folders));

        Assert.Equal(NewSeriesStatus.Linked, result.Status);
        Assert.Equal(path, (await _files.GetByImdbIdAsync("tt0903747"))?.FullName);
    }

    [Fact]
    public async Task Ignored_folders_are_left_out_unless_asked_for()
    {
        Folder("Some.Movie.2021.720p");
        var scan = await _controller.FindNewFoldersAsync(includeIgnored: false);
        await _controller.IgnoreAsync(scan.Folders.Single());

        var again = await _controller.FindNewFoldersAsync(includeIgnored: false);
        var all = await _controller.FindNewFoldersAsync(includeIgnored: true);

        Assert.Equal((0, 1), (again.Folders.Count, again.IgnoredCount));
        Assert.Single(all.Folders);
    }

    [Fact]
    public async Task Movies_are_never_linked_in_the_series_folder()
    {
        var folder = (await Scan("Horizon.An.American.Saga.Chapter.1.2024.720p")).Single();

        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.LinkAsync(folder, Horizon(), replaceExisting: false));
        Assert.Null(await _files.GetByImdbIdAsync("tt17505010"));
    }

    [Fact]
    public async Task A_wrong_imdb_tag_is_removed_from_the_folder_name()
    {
        var folder = (await Scan("Horizon.An.American.Saga.Chapter.1.2024 {imdb-tt0000001}")).Single();

        var renamed = await _controller.RemoveImdbTagAsync(folder);

        Assert.Equal(Path.Combine(_root, "Horizon.An.American.Saga.Chapter.1.2024"), renamed.Path);
        Assert.True(Directory.Exists(renamed.Path));
        Assert.False(Directory.Exists(folder.Path));
        Assert.Equal(new SeriesFolderGuess(null, "Horizon An American Saga Chapter 1", 2024), renamed.Guess);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.RemoveImdbTagAsync(renamed));
    }

    [Theory]
    [InlineData("Hatfields.and.McCoys.2012 {imdb-tt1985443}", "Hatfields.and.McCoys.2012")]
    [InlineData("Preacher {tt5016504}", "Preacher")]
    [InlineData("Show [imdb-tt1234567] 2001", "Show 2001")]
    [InlineData("Daredevil", "Daredevil")]
    public void Imdb_tags_come_out_of_names(string name, string without) => Assert.Equal(without, SeriesFolderName.WithoutImdbTag(name));

    private static Item Horizon() =>
        new() { ImdbId = "tt17505010", Title = "Horizon: An American Saga - Chapter 1", Year = "2024", Type = "movie", Response = "True" };

    [Fact]
    public async Task OMDb_limit_stops_online_lookups_for_the_rest()
    {
        Folder("A {imdb-tt0000001}");
        Folder("B {imdb-tt0000002}");
        _omdb.UnavailableMessage = "OMDb: Request limit reached!";

        var results = await _controller.AddAutomaticallyAsync((await _controller.FindNewFoldersAsync(false)).Folders);

        Assert.All(results, r => Assert.Equal(NewSeriesStatus.NeedsInput, r.Status));
        Assert.Equal(1, _omdb.Calls);
    }

    private async Task<NewSeriesResult> AddAll() =>
        Assert.Single(await _controller.AddAutomaticallyAsync((await _controller.FindNewFoldersAsync(includeIgnored: false)).Folders));

    private async Task<IReadOnlyList<NewSeriesFolder>> Scan(string name)
    {
        Folder(name);
        return (await _controller.FindNewFoldersAsync(includeIgnored: false)).Folders;
    }

    private string Folder(string name) => Directory.CreateDirectory(Path.Combine(_root, name)).FullName;

    private static Item Series(string id, string title, string year) =>
        new() { ImdbId = id, Title = title, Year = year, Type = "series", Response = "True" };

    private sealed class NoPosters : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
