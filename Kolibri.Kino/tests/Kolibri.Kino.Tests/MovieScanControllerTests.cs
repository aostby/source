using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Scanning;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class MovieScanControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-scan-").FullName;
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly FakeOmdb _omdb = new();
    private readonly FakeTmdb _tmdb = new();
    private readonly FakePlex _plex = new();
    private readonly MovieScanController _scan;

    public MovieScanControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        var linker = new MovieLinker(_files, _movies, _plex, _omdb);
        _scan = new MovieScanController(new FileSystemMediaScanner(), _files, _movies, new MovieFileNameParser(),
            _plex, _omdb, _tmdb, linker, NullLogger<MovieScanController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public async Task Links_new_file_found_on_OMDb_and_keeps_path_in_TomatoUrl()
    {
        var path = Touch(@"The Matrix (1999)\The.Matrix.1999.mkv");
        _omdb.Add(Movie("tt0133093", "The Matrix"));

        var entry = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.NewFiles)).Entries);

        Assert.Equal((ScanOutcome.Linked, "tt0133093", "OMDb"), (entry.Outcome, entry.ImdbId, entry.Source));
        Assert.Equal(path, Assert.Single(await _files.GetAllAsync()).FullName);
        Assert.Equal(path, (await _movies.GetByImdbIdAsync("tt0133093"))?.TomatoUrl);
    }

    [Fact]
    public async Task Uses_imdb_tag_then_library_then_TMDb_before_OMDb_title_search()
    {
        Touch(@"Pulp {imdb-tt0110912}\pulp.mkv");
        Touch(@"Local Movie (2000)\Local.Movie.2000.mkv");
        Touch(@"Via Tmdb (2005)\Via.Tmdb.2005.mkv");
        _omdb.Add(Movie("tt0110912", "Pulp Fiction", rating: "8.9")).Add(Movie("tt0000555", "Found Elsewhere"));
        await _movies.UpsertAsync(new() { ImdbId = "tt0000200", Title = "Local Movie", Year = "2000", Response = "True" });
        _tmdb.Ids["Via Tmdb|2005"] = "tt0000555";

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal(
            ["tt0000200 library", "tt0110912 IMDb id in name", "tt0000555 TMDb"],
            report.Entries.Select(e => $"{e.ImdbId} {e.Source}"));
    }

    [Fact]
    public async Task Second_file_for_a_linked_movie_is_a_duplicate_and_not_relinked()
    {
        Touch(@"The Matrix (1999)\The.Matrix.1999.mkv");
        Touch(@"The Matrix (1999) copy\The.Matrix.1999.mkv");
        _omdb.Add(Movie("tt0133093", "The Matrix"));

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal([ScanOutcome.Linked, ScanOutcome.Duplicate], report.Entries.Select(e => e.Outcome));
        Assert.Equal(report.Entries[0].FilePath, Assert.Single(await _files.GetAllAsync()).FullName);
        Assert.Contains(report.Entries[0].FilePath, report.Entries[1].Detail);
        Assert.Equal(report.Entries[0].FilePath, report.Entries[1].LinkedFile);
        Assert.Null(report.Entries[0].LinkedFile);
    }

    [Fact]
    public async Task Other_parts_in_the_same_folder_are_not_duplicates_but_a_copy_elsewhere_is()
    {
        // The Kurosawa box set: CD1 linked, CD2 next to it; Dersu Uzala also linked to a copy in another folder.
        var cd1 = Touch(@"Seven.Samurai.1954\Seven.Samurai.1954.CD1.avi");
        Touch(@"Seven.Samurai.1954\Seven.Samurai.1954.CD2.avi");
        var elsewhere = Touch(@"Other\Dersu Uzala (1975)\Dersu Uzala (1975).mkv");
        Touch(@"Boxset\Dersu.Uzala.1975.DVDRip\Dersu.Uzala.1975.DVDRip.avi");
        await _movies.UpsertAsync(Movie("tt0047478", "Seven Samurai", year: "1954"));
        await _movies.UpsertAsync(Movie("tt0071411", "Dersu Uzala", year: "1975"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0047478", FullName = cd1 });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0071411", FullName = elsewhere });

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal(ScanOutcome.OtherPart, Assert.Single(report.Entries, e => e.FilePath.EndsWith("CD2.avi")).Outcome);
        var dersu = Assert.Single(report.Entries, e => e.FilePath.EndsWith("DVDRip.avi"));
        Assert.Equal((ScanOutcome.Duplicate, elsewhere), (dersu.Outcome, dersu.LinkedFile));
    }

    [Fact]
    public async Task Moved_file_is_relinked()
    {
        var moved = Touch(@"New Place\The Matrix (1999)\The.Matrix.1999.mkv");
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0133093", FullName = Path.Combine(_folder, "Old Place", "matrix.mkv") });
        await _movies.UpsertAsync(Movie("tt0133093", "The Matrix"));

        var entry = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.NewFiles)).Entries);

        Assert.Equal((ScanOutcome.Linked, "library"), (entry.Outcome, entry.Source));
        Assert.Equal(moved, Assert.Single(await _files.GetAllAsync()).FullName);
    }

    [Fact]
    public async Task NewFiles_skips_linked_files_and_All_refreshes_them()
    {
        var path = Touch(@"The Matrix (1999)\The.Matrix.1999.mkv");
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0133093", FullName = path });
        _omdb.Add(Movie("tt0133093", "The Matrix", rating: "8.7"));

        await _movies.UpsertAsync(Movie("tt0133093", "The Matrix"));

        var newOnly = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.NewFiles)).Entries);
        Assert.Equal((ScanOutcome.AlreadyLinked, "The Matrix", null), (newOnly.Outcome, newOnly.Title, newOnly.Detail));
        Assert.Equal(0, _omdb.Calls);

        var all = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.All)).Entries);
        Assert.Equal(ScanOutcome.Refreshed, all.Outcome);
        Assert.Equal("8.7", (await _movies.GetByImdbIdAsync("tt0133093"))?.ImdbRating);
    }

    [Fact]
    public async Task Series_episodes_are_skipped_and_unknown_titles_reported()
    {
        Touch(@"Shaun\Shaun.The.Sheep.S01E02.mkv");
        Touch(@"Nothing Known (2001)\Nothing.Known.2001.mkv");

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal([ScanOutcome.NotFound, ScanOutcome.Skipped], report.Entries.Select(e => e.Outcome));
        Assert.Contains("Nothing Known", report.Entries[0].Detail);
        Assert.Empty(await _files.GetAllAsync());
    }

    [Fact]
    public async Task Stops_when_OMDb_is_unavailable_instead_of_failing_every_file()
    {
        Touch(@"A (2001)\A.Movie.2001.mkv");
        Touch(@"B (2002)\B.Movie.2002.mkv");
        _omdb.UnavailableMessage = "OMDb: Request limit reached!";

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal("OMDb: Request limit reached!", report.StoppedReason);
        Assert.Equal(ScanOutcome.Failed, Assert.Single(report.Entries).Outcome);
    }

    [Fact]
    public async Task TMDb_failure_is_reported_and_OMDb_still_used()
    {
        Touch(@"The Matrix (1999)\The.Matrix.1999.mkv");
        _omdb.Add(Movie("tt0133093", "The Matrix"));
        _tmdb.Throws = new HttpRequestException("401 Unauthorized");

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal(ScanOutcome.Linked, Assert.Single(report.Entries).Outcome);
        Assert.Null(report.StoppedReason);
        Assert.Contains(report.Notes, n => n.StartsWith("TMDb was skipped"));
    }

    [Fact]
    public async Task Plex_is_tried_before_TMDb_and_OMDb()
    {
        Touch(@"Spies In Disguise (2019)\Spies.In.Disguise.2019.mkv");
        _plex.Add(Movie("tt5814534", "Spies In Disguise", year: "2019"));
        _tmdb.Ids["Spies In Disguise|2019"] = "tt5814534";

        var entry = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.NewFiles)).Entries);

        Assert.Equal((ScanOutcome.Linked, "Plex"), (entry.Outcome, entry.Source));
        Assert.Equal((0, 0), (_tmdb.Calls, _omdb.Calls));
    }

    [Fact]
    public async Task Imdb_tag_is_resolved_through_Plex_before_OMDb()
    {
        Touch(@"Pulp {imdb-tt0110912}\pulp.mkv");
        _plex.Add(Movie("tt0110912", "Pulp Fiction"));

        var entry = Assert.Single((await _scan.ScanAsync(_folder, ScanMode.NewFiles)).Entries);

        Assert.Equal((ScanOutcome.Linked, "tt0110912"), (entry.Outcome, entry.ImdbId));
        Assert.Equal(0, _omdb.Calls);
    }

    [Fact]
    public async Task Plex_offline_is_noted_and_the_scan_continues()
    {
        Touch(@"The Matrix (1999)\The.Matrix.1999.mkv");
        _omdb.Add(Movie("tt0133093", "The Matrix"));
        _plex.Throws = new HttpRequestException("No such host is known.");

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal(ScanOutcome.Linked, Assert.Single(report.Entries).Outcome);
        Assert.Contains(report.Notes, n => n.StartsWith("Plex was skipped"));
    }

    [Fact]
    public async Task Report_lists_every_folder_holding_a_video_file()
    {
        var a = Touch(@"A (2001)\A.Movie.2001.mkv");
        var b = Touch(@"B (2002)\B.Movie.2002.mkv");

        var report = await _scan.ScanAsync(_folder, ScanMode.NewFiles);

        Assert.Equal([Path.GetDirectoryName(a), Path.GetDirectoryName(b)], report.Folders.Order());
    }

    [Fact]
    public async Task RemoveMissing_removes_only_links_to_missing_files_and_keeps_details()
    {
        var present = Touch(@"Here\here.mkv");
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0000001", FullName = present });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0000002", FullName = Path.Combine(_folder, "Gone", "gone.mkv") });
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0000003", FullName = @"Z:\Other\elsewhere.mkv" });
        await _movies.UpsertAsync(Movie("tt0000002", "Gone Movie"));

        Assert.Equal(1, await _scan.RemoveMissingAsync(_folder));

        Assert.Equal(["tt0000001", "tt0000003"], (await _files.GetAllAsync()).Select(f => f.ImdbId).Order());
        Assert.NotNull(await _movies.GetByImdbIdAsync("tt0000002"));
    }

    private string Touch(string relative)
    {
        var path = Path.Combine(_folder, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "x");
        return path;
    }
}
