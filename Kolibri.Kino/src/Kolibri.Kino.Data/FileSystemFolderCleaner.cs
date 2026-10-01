using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Data;

public sealed class FileSystemFolderCleaner : IFolderCleaner
{
    private static readonly EnumerationOptions Recursive = new() { RecurseSubdirectories = true, IgnoreInaccessible = true };

    public Task<IReadOnlyList<string>> FindAsync(IEnumerable<string> folders, IReadOnlyList<string> patterns, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var found = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var folder in Distinct(folders))
            {
                if (!Directory.Exists(folder)) continue;
                foreach (var file in Directory.EnumerateFiles(folder, "*", Recursive))
                {
                    ct.ThrowIfCancellationRequested();
                    if (CleanupRules.Matches(Path.GetFileName(file), patterns)) found.Add(file);
                }
            }
            return (IReadOnlyList<string>)found.ToList();
        }, ct);

    public Task<CleanupResult> DeleteAsync(IReadOnlyList<string> files, IEnumerable<string> folders, bool deleteEmptyFolders, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var failed = new List<string>();
            var deleted = 0;
            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (!File.Exists(file)) continue;
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                    deleted++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    failed.Add($"{file}: {ex.Message}");
                }
            }

            var foldersDeleted = 0;
            if (deleteEmptyFolders)
                foreach (var folder in Distinct(folders))
                    foldersDeleted += DeleteEmptySubfolders(folder, ct);

            return new CleanupResult(deleted, foldersDeleted, failed);
        }, ct);

    /// <summary>
    /// Removes empty folders below <paramref name="folder"/>, deepest first, and the folder itself if it ends up empty.
    /// Same as SilverScreen's FileUtilities.DeleteEmptyDirs.
    /// </summary>
    private static int DeleteEmptySubfolders(string folder, CancellationToken ct)
    {
        if (!Directory.Exists(folder)) return 0;

        var count = 0;
        try
        {
            foreach (var sub in Directory.EnumerateDirectories(folder))
            {
                ct.ThrowIfCancellationRequested();
                count += DeleteEmptySubfolders(sub, ct);
            }
            if (!Directory.EnumerateFileSystemEntries(folder).Any())
            {
                Directory.Delete(folder);
                count++;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // In use, offline or no permission: leave it.
        }
        return count;
    }

    /// <summary>Drops folders already covered by a parent in the list, so nothing is walked twice.</summary>
    private static IEnumerable<string> Distinct(IEnumerable<string> folders)
    {
        var sorted = folders
            .Select(f => Path.TrimEndingDirectorySeparator(Path.GetFullPath(f)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var kept = new List<string>();
        foreach (var folder in sorted)
            if (!kept.Any(parent => folder.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                kept.Add(folder);
        return kept;
    }
}
