using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// The library's record of which file belongs to which IMDb id. One file per IMDb id.
/// </summary>
public interface IFileItemRepository
{
    Task<IReadOnlyList<FileItem>> GetAllAsync(CancellationToken ct = default);

    Task<FileItem?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default);

    /// <summary>File items whose path is inside <paramref name="folder"/> (recursive, case-insensitive).</summary>
    Task<IReadOnlyList<FileItem>> GetInFolderAsync(string folder, CancellationToken ct = default);

    /// <summary>Links <see cref="FileItem.ImdbId"/> to <see cref="FileItem.FullName"/>, replacing any earlier link for that id.</summary>
    Task UpsertAsync(FileItem file, CancellationToken ct = default);

    /// <summary>Removes the file link only; the movie's details stay in the library.</summary>
    Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default);
}
