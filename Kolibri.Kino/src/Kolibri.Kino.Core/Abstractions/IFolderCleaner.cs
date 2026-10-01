namespace Kolibri.Kino.Core.Abstractions;

public sealed record CleanupResult(int FilesDeleted, int FoldersDeleted, IReadOnlyList<string> Failed);

/// <summary>
/// Finds and deletes leftover files in movie folders (see <see cref="CleanupRules"/>).
/// </summary>
public interface IFolderCleaner
{
    /// <summary>Files under <paramref name="folders"/> (recursive) that match <paramref name="patterns"/>.</summary>
    Task<IReadOnlyList<string>> FindAsync(IEnumerable<string> folders, IReadOnlyList<string> patterns, CancellationToken ct = default);

    /// <summary>
    /// Permanently deletes <paramref name="files"/> (clearing read-only first), then, if asked, empty folders
    /// under <paramref name="folders"/>. Files that can't be deleted are reported, not thrown.
    /// </summary>
    Task<CleanupResult> DeleteAsync(IReadOnlyList<string> files, IEnumerable<string> folders, bool deleteEmptyFolders, CancellationToken ct = default);
}
