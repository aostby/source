using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.LocalMovies;

public enum LocalMoviesFilter
{
    /// <summary>Every library file in the folder ("Alle").</summary>
    All,

    /// <summary>Library entries whose file is gone ("IkkeEksisterende").</summary>
    MissingFiles,

    /// <summary>Video files on disk that the library doesn't know ("Uten treff").</summary>
    NotInLibrary,
}

/// <summary>A library file. <see cref="Item"/> is null when the file is linked to an id with no stored details.</summary>
public sealed record LocalMovie(string ImdbId, string FilePath, bool FileExists, Item? Item)
{
    public string Title => Item?.Title ?? Path.GetFileNameWithoutExtension(FilePath);
}

public sealed record UnmatchedFile(string FilePath, bool IsMultipart);

public sealed record LocalMoviesResult(
    string Folder,
    LocalMoviesFilter Filter,
    IReadOnlyList<LocalMovie> Movies,
    IReadOnlyList<UnmatchedFile> UnmatchedFiles,
    int LibraryFilesInFolder);
