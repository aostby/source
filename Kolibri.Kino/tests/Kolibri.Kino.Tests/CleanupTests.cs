using Kolibri.Kino.Controllers.Cleanup;
using Kolibri.Kino.Core;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Tests;

public sealed class CleanupTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-clean-").FullName;

    public void Dispose()
    {
        foreach (var file in Directory.EnumerateFiles(_folder, "*", SearchOption.AllDirectories))
            File.SetAttributes(file, FileAttributes.Normal);
        Directory.Delete(_folder, recursive: true);
    }

    [Theory]
    [InlineData("movie.nfo", true)]
    [InlineData("RARBG.txt", true)]
    [InlineData("Movie.rus.srt", true)]
    [InlineData("Movie_rus.srt", true)]
    [InlineData("Movie.rus.HI.srt", true)]
    [InlineData("Movie.Russian.srt", true)]
    [InlineData("Alice.srt", false)]          // "ice.srt" is Icelandic, not the end of "Alice"
    [InlineData("Animal.srt", false)]         // "mal.srt"
    [InlineData("Grand Budapest Hotel.srt", false)] // "tel.srt"
    [InlineData("Movie.eng.srt", false)]
    [InlineData("Movie.nor.srt", false)]
    [InlineData("Movie.mkv", false)]
    public void Default_patterns(string fileName, bool delete) =>
        Assert.Equal(delete, CleanupRules.Matches(fileName, CleanupRules.DefaultPatterns));

    [Fact]
    public void Video_files_are_never_matched_even_if_a_pattern_says_so() =>
        Assert.False(CleanupRules.Matches("Movie.mkv", [".mkv"]));

    [Fact]
    public async Task Finds_and_deletes_leftovers_including_read_only_and_removes_empty_folders()
    {
        var nfo = Touch(@"Movie (2001)\movie.nfo");
        var sample = Touch(@"Movie (2001)\Subs\Movie.rus.srt");
        var keep = Touch(@"Movie (2001)\Movie.2001.mkv");
        var keepSubs = Touch(@"Movie (2001)\Movie.eng.srt");
        File.SetAttributes(nfo, FileAttributes.ReadOnly);

        var controller = new CleanupController(new FileSystemFolderCleaner(), Options.Create(new KinoOptions()),
            NullLogger<CleanupController>.Instance);

        var plan = await controller.PlanAsync([Path.GetDirectoryName(keep)!, _folder]);
        Assert.Equal([nfo, sample], plan.Files.Order(StringComparer.OrdinalIgnoreCase));

        var result = await controller.ExecuteAsync(plan);

        Assert.Equal((2, 1), (result.FilesDeleted, result.FoldersDeleted)); // the emptied Subs folder
        Assert.Empty(result.Failed);
        Assert.True(File.Exists(keep) && File.Exists(keepSubs));
        Assert.False(Directory.Exists(Path.GetDirectoryName(sample)));
    }

    [Fact]
    public async Task Patterns_from_appsettings_replace_the_defaults()
    {
        Touch(@"Movie\movie.nfo");
        var custom = Touch(@"Movie\notes.log");
        var options = new KinoOptions { Cleanup = { FilePatterns = [".log"] } };
        var controller = new CleanupController(new FileSystemFolderCleaner(), Options.Create(options), NullLogger<CleanupController>.Instance);

        var plan = await controller.PlanAsync([_folder]);

        Assert.Equal(custom, Assert.Single(plan.Files));
    }

    private string Touch(string relative)
    {
        var path = Path.Combine(_folder, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "x");
        return path;
    }

    [Fact]
    public async Task Tools_delete_empty_folders_and_listed_files_but_keep_the_chosen_folder()
    {
        Directory.CreateDirectory(Path.Combine(_folder, @"Empty\Also empty"));
        Directory.CreateDirectory(Path.Combine(_folder, "Movie (2007)"));
        File.WriteAllText(Path.Combine(_folder, @"Movie (2007)\movie.mkv"), "");
        Directory.CreateDirectory(Path.Combine(_folder, "Only junk"));
        var junk = Path.Combine(_folder, @"Only junk\RARBG.txt");
        File.WriteAllText(junk, "");
        var controller = new CleanupController(new FileSystemFolderCleaner(), Options.Create(new KinoOptions()), NullLogger<CleanupController>.Instance);

        // Empty folders only: the folder with a .txt stays, as does the .txt.
        var emptyOnly = await controller.DeleteEmptyFoldersAsync(_folder);
        Assert.Equal((0, 2), (emptyOnly.FilesDeleted, emptyOnly.FoldersDeleted));
        Assert.False(Directory.Exists(Path.Combine(_folder, "Empty")));
        Assert.True(File.Exists(junk));

        // With the leftover files (as listed): the file, then its folder, left empty.
        var plan = await controller.PlanAsync([_folder]);
        Assert.Equal([junk], plan.Files);
        var withFiles = await controller.CleanFolderAsync(_folder, plan.Files);
        Assert.Equal((1, 1), (withFiles.FilesDeleted, withFiles.FoldersDeleted));
        Assert.False(Directory.Exists(Path.Combine(_folder, "Only junk")));
        Assert.True(File.Exists(Path.Combine(_folder, @"Movie (2007)\movie.mkv")));
        Assert.True(Directory.Exists(_folder)); // the chosen folder is kept, even when it ends up empty
        Assert.Equal(0, (await controller.DeleteEmptyFoldersAsync(_folder)).FoldersDeleted);
    }
}
