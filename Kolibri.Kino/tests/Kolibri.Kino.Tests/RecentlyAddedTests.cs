using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class RecentlyAddedTests
{
    [Theory]
    [InlineData(@"\\GREENLANTERN\Movies\2026\Film (2026)\film.mkv", "/media/Movies/2026/Film (2026)/film.mkv")]
    [InlineData(@"\\greenlantern\movies\a.mkv", "/media/Movies/a.mkv")] // case doesn't matter
    [InlineData(@"\\GREENLANTERN\Movies", "/media/Movies")]
    [InlineData(@"\\GREENLANTERN\Movies2\a.mkv", @"\\GREENLANTERN\Movies2\a.mkv")] // not a folder boundary
    [InlineData(@"\\htpc\Shared\a.mkv", @"\\htpc\Shared\a.mkv")] // no mapping: unchanged
    public void Stored_windows_paths_map_to_the_mounted_share(string stored, string expected)
    {
        var options = new KinoOptions { PathMappings = [new PathMapping { From = "//GREENLANTERN/Movies", To = "/media/Movies/" }] };

        Assert.Equal(expected, options.MapPath(stored));
    }

    [Fact]
    public void A_mapping_to_a_windows_folder_keeps_backslashes()
    {
        var options = new KinoOptions { PathMappings = [new PathMapping { From = @"\\NAS\Movies", To = @"D:\Movies" }] };

        Assert.Equal(@"D:\Movies\2026\a.mkv", options.MapPath(@"\\NAS\Movies\2026\a.mkv"));
    }

    [Fact]
    public async Task Items_come_newest_first_and_unreachable_files_are_counted()
    {
        var now = DateTime.UtcNow;
        var movies = new InMemoryMovies(Movie("tt1", "Old"), Movie("tt2", "New"), Movie("tt3", "Elsewhere"), Movie("tt4", "No link"));
        var files = new InMemoryFiles(File("tt1", "old.mkv"), File("tt2", "new.mkv"), File("tt3", @"\\htpc\x.mkv"), File("tt9", "orphan.mkv"));
        var dates = new FakeDates { ["old.mkv"] = now.AddDays(-40), ["new.mkv"] = now.AddHours(-2), ["orphan.mkv"] = now };

        var report = await new RecentlyAddedController(movies, files, dates).GetAsync();

        Assert.Equal(["New", "Old"], report.Items.Select(r => r.Item.Title));
        Assert.Equal(1, report.WithoutDate); // tt3; tt9 has no library item and tt4 no file, so neither counts
    }

    [Fact]
    public void A_series_folder_counts_from_its_newest_episode()
    {
        var folder = Directory.CreateTempSubdirectory("kino-series-");
        try
        {
            var season = folder.CreateSubdirectory("Season 01");
            var old = Touch(Path.Combine(season.FullName, "S01E01.mkv"), new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            var newest = Touch(Path.Combine(season.FullName, "S01E02.mkv"), new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc));
            Touch(Path.Combine(folder.FullName, "notes.txt"), new DateTime(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc)); // not a video

            Assert.Equal((newest, true), FileSystemFileDates.Read(folder.FullName));
            Assert.Equal((old, false), FileSystemFileDates.Read(Path.Combine(season.FullName, "S01E01.mkv")));
            Assert.Equal((null, false), FileSystemFileDates.Read(Path.Combine(folder.FullName, "missing.mkv")));
        }
        finally
        {
            folder.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Dates_are_stored_so_a_restart_needs_no_disk_walk()
    {
        using var temp = new TempDatabase();
        var folder = Directory.CreateTempSubdirectory("kino-dates-");
        try
        {
            var film = Path.Combine(folder.FullName, "film.mkv");
            var added = Touch(film, new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc));
            var options = Microsoft.Extensions.Options.Options.Create(new KinoOptions());

            using (var db = new KinoDatabase(temp.Path))
            {
                var first = await Dates(db, options).GetAddedAsync([film]);
                Assert.Equal(added, first[film]);
            }

            System.IO.File.Delete(film); // a fresh instance (a restart) must answer from the store, not the disk
            using (var db = new KinoDatabase(temp.Path))
            {
                var again = await Dates(db, options).GetAddedAsync([film, Path.Combine(folder.FullName, "never.mkv")]);
                Assert.Equal(added, Assert.Single(again).Value);
            }
        }
        finally
        {
            folder.Delete(recursive: true);
        }

        static FileSystemFileDates Dates(KinoDatabase db, Microsoft.Extensions.Options.IOptions<KinoOptions> options) =>
            new(options, new LiteDbFileDateStore(db), Microsoft.Extensions.Logging.Abstractions.NullLogger<FileSystemFileDates>.Instance);
    }

    [Fact]
    public void Series_folders_go_stale_sooner_than_files()
    {
        var checkedAt = new DateTime(2026, 10, 10, 9, 0, 0, DateTimeKind.Utc);
        var hourLater = checkedAt.AddHours(1);

        Assert.True(FileSystemFileDates.IsStale(new StoredFileDate("s", checkedAt, IsFolder: true, checkedAt), hourLater));
        Assert.False(FileSystemFileDates.IsStale(new StoredFileDate("f", checkedAt, IsFolder: false, checkedAt), hourLater));
        Assert.False(FileSystemFileDates.IsStale(new StoredFileDate("m", null, IsFolder: false, checkedAt), hourLater));
        Assert.True(FileSystemFileDates.IsStale(new StoredFileDate("m", null, IsFolder: false, checkedAt), checkedAt.AddDays(1)));
    }

    [Theory]
    [InlineData(0, "Today")]
    [InlineData(1, "Yesterday")]
    [InlineData(6, "Last 7 days")]
    [InlineData(29, "Last 30 days")]
    public void Items_are_listed_under_plex_like_periods(int daysAgo, string expected)
    {
        var now = new DateTime(2026, 10, 10, 9, 0, 0);

        Assert.Equal(expected, RecentlyAddedController.PeriodOf(now.AddDays(-daysAgo), now));
    }

    [Fact]
    public void Older_items_are_listed_under_their_month()
    {
        var now = new DateTime(2026, 10, 10, 9, 0, 0);

        Assert.Contains("2026", RecentlyAddedController.PeriodOf(new DateTime(2026, 8, 3), now));
        Assert.NotEqual(RecentlyAddedController.PeriodOf(new DateTime(2026, 8, 3), now), RecentlyAddedController.PeriodOf(new DateTime(2026, 7, 3), now));
    }

    [Theory]
    [InlineData(0.5, "Just now")]
    [InlineData(5, "5 h ago")]
    [InlineData(30, "1 day ago")]
    [InlineData(24 * 3 + 1, "3 days ago")]
    [InlineData(24 * 21, "3 weeks ago")]
    public void Cards_say_how_long_ago(double hoursAgo, string expected)
    {
        var now = new DateTime(2026, 10, 10, 9, 0, 0, DateTimeKind.Utc);

        Assert.Equal(expected, RecentlyAddedController.Ago(now.AddHours(-hoursAgo), now));
    }

    private static DateTime Touch(string path, DateTime utc)
    {
        System.IO.File.WriteAllText(path, "");
        System.IO.File.SetCreationTimeUtc(path, utc);
        System.IO.File.SetLastWriteTimeUtc(path, utc);
        return utc;
    }

    private static Item Movie(string id, string title) => new() { ImdbId = id, Title = title, Type = "movie" };

    private static FileItem File(string id, string path) => new() { ImdbId = id, FullName = path };

    private sealed class FakeDates : Dictionary<string, DateTime>, IFileDates
    {
        public Task<IReadOnlyDictionary<string, DateTime>> GetAddedAsync(IEnumerable<string> paths, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyDictionary<string, DateTime>>(paths.Where(ContainsKey).ToDictionary(p => p, p => this[p]));
    }

    private sealed class InMemoryFiles(params FileItem[] files) : IFileItemRepository
    {
        public Task<IReadOnlyList<FileItem>> GetAllAsync(CancellationToken ct = default) => Task.FromResult<IReadOnlyList<FileItem>>(files);
        public Task<FileItem?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) => Task.FromResult(files.FirstOrDefault(f => f.ImdbId == imdbId));
        public Task<IReadOnlyList<FileItem>> GetInFolderAsync(string folder, CancellationToken ct = default) => throw new NotSupportedException();
        public Task UpsertAsync(FileItem file, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
    }

    private sealed class InMemoryMovies(params Item[] items) : IMovieRepository
    {
        public Task<IReadOnlyDictionary<string, Item>> GetByImdbIdsAsync(IEnumerable<string> imdbIds, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyDictionary<string, Item>>(items.Where(i => imdbIds.Contains(i.ImdbId)).ToDictionary(i => i.ImdbId));
        public Task<IReadOnlyList<Item>> GetAllAsync(string? type = null, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<Item>>(items);
        public Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) => Task.FromResult(items.FirstOrDefault(i => i.ImdbId == imdbId));
        public Task<Item?> FindByTitleAsync(string title, int? year, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<Item>> SearchByTitleAsync(string text, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<bool> UpsertAsync(Item item, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
    }
}
