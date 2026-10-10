namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// When library files arrived, read from the file system. The database doesn't store it, so this is what
/// "Recently added" sorts by, as Plex sorts by its own addedAt.
/// </summary>
public interface IFileDates
{
    /// <summary>
    /// The added time (UTC) for each path that can be reached, keyed by the path as given. A file counts as added
    /// when it was created or last written, whichever is later (a copy keeps the old write time but gets a new
    /// creation time). A folder (a series) counts as added when its newest video file was. Paths that don't exist
    /// here are left out.
    /// </summary>
    Task<IReadOnlyDictionary<string, DateTime>> GetAddedAsync(IEnumerable<string> paths, CancellationToken ct = default);
}
