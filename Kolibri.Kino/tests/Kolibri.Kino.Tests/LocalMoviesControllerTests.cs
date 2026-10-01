using Kolibri.Kino.Controllers.LocalMovies;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Data;
using LiteDB;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class LocalMoviesControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-movies-").FullName;
    private readonly string _inLibrary, _gone, _unknown, _multipart;

    public LocalMoviesControllerTests()
    {
        _inLibrary = Touch("Matrix (1999)", "The.Matrix.1999.mkv");
        _unknown = Touch("Unknown", "Unknown.Movie.mp4");
        _multipart = Touch("Long Movie", "Long.Movie.CD1.avi");
        Touch("Matrix (1999)", "readme.txt");
        Touch("@__thumb", "thumb.mkv");
        _gone = Path.Combine(_folder, "Gone", "Gone.mkv");

        // Written the way SilverScreen does: _id = ImdbId, fields ImdbId + FullName.
        using var legacy = new LiteDatabase(_temp.Path);
        var files = legacy.GetCollection("FileItem");
        files.Insert("tt0133093", new BsonDocument { ["ImdbId"] = "tt0133093", ["FullName"] = _inLibrary.ToUpperInvariant() });
        files.Insert("tt0110912", new BsonDocument { ["ImdbId"] = "tt0110912", ["FullName"] = _gone });
        files.Insert("tt9999999", new BsonDocument { ["ImdbId"] = "tt9999999", ["FullName"] = @"Z:\Elsewhere\Other.mkv" });
        var items = legacy.GetCollection<Item>("Item");
        items.Insert("tt0133093", LiteDbMovieRepositoryTests.Movie("tt0133093", "The Matrix", rating: "8.7"));
        items.Insert("tt0110912", LiteDbMovieRepositoryTests.Movie("tt0110912", "Pulp Fiction", rating: "8.9"));
    }

    public void Dispose()
    {
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public async Task All_lists_library_files_in_folder_best_rated_first()
    {
        using var db = new KinoDatabase(_temp.Path);

        var result = await Controller(db).LoadAsync(_folder, LocalMoviesFilter.All);

        Assert.Equal(["Pulp Fiction", "The Matrix"], result.Movies.Select(m => m.Title));
        Assert.False(result.Movies[0].FileExists);
        Assert.True(result.Movies[1].FileExists); // path stored in different case still matches
        Assert.Equal(2, result.LibraryFilesInFolder);
    }

    [Fact]
    public async Task MissingFiles_lists_only_library_entries_whose_file_is_gone()
    {
        using var db = new KinoDatabase(_temp.Path);

        var result = await Controller(db).LoadAsync(_folder, LocalMoviesFilter.MissingFiles);

        Assert.Equal(_gone, Assert.Single(result.Movies).FilePath);
    }

    [Fact]
    public async Task NotInLibrary_lists_unknown_video_files_and_flags_multipart()
    {
        using var db = new KinoDatabase(_temp.Path);

        var result = await Controller(db).LoadAsync(_folder, LocalMoviesFilter.NotInLibrary);

        Assert.Equal([(_multipart, true), (_unknown, false)], result.UnmatchedFiles.Select(f => (f.FilePath, f.IsMultipart)));
    }

    [Fact]
    public async Task Movies_folder_is_the_shared_MoviesSourcePath_setting()
    {
        using (var legacy = new LiteDatabase(_temp.Path))
            legacy.GetCollection("UserSettings").Insert(Environment.UserName, new BsonDocument
            {
                ["OMDBkey"] = "abc123",
                ["UserFilePaths"] = new BsonDocument { ["MoviesSourcePath"] = @"D:\Movies", ["SeriesSourcePath"] = @"D:\Series" },
            });

        using var db = new KinoDatabase(_temp.Path);
        var controller = Controller(db);

        Assert.Equal(@"D:\Movies", await controller.GetMoviesFolderAsync());

        await controller.SetMoviesFolderAsync(@"E:\Film");

        Assert.Equal(@"E:\Film", await controller.GetMoviesFolderAsync());
        var stored = db.Database.GetCollection("UserSettings").FindById(Environment.UserName);
        Assert.Equal((@"E:\Film", @"D:\Series", "abc123"),
            (stored["UserFilePaths"]["MoviesSourcePath"].AsString, stored["UserFilePaths"]["SeriesSourcePath"].AsString, stored["OMDBkey"].AsString));
    }

    private static LocalMoviesController Controller(KinoDatabase db) => new(
        new LiteDbFileItemRepository(db),
        new LiteDbMovieRepository(db),
        new FileSystemMediaScanner(),
        new LiteDbSettingsStore(db),
        new NoPosters(),
        NullLogger<LocalMoviesController>.Instance);

    private string Touch(string subfolder, string name)
    {
        var path = Path.Combine(_folder, subfolder, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "x");
        return path;
    }

    private sealed class NoPosters : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
