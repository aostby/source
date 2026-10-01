using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Data;

public sealed class FileSystemMediaScanner : IMediaFileScanner
{
    private static readonly EnumerationOptions Recursive = new()
    {
        RecurseSubdirectories = true,
        IgnoreInaccessible = true,
    };

    public Task<IReadOnlyList<string>> FindVideoFilesAsync(string folder, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            if (!Directory.Exists(folder)) return (IReadOnlyList<string>)[];

            var files = new List<string>();
            foreach (var file in Directory.EnumerateFiles(folder, "*", Recursive))
            {
                ct.ThrowIfCancellationRequested();
                if (MovieFileRules.IsVideoFile(file)) files.Add(file);
            }
            return (IReadOnlyList<string>)files;
        }, ct);

    public Task<IReadOnlySet<string>> FindExistingAsync(IEnumerable<string> paths, CancellationToken ct = default) =>
        Existing(paths, File.Exists, ct);

    public Task<IReadOnlySet<string>> FindExistingFoldersAsync(IEnumerable<string> folders, CancellationToken ct = default) =>
        Existing(folders, Directory.Exists, ct);

    public Task<IReadOnlyList<string>> GetSubfoldersAsync(string folder, CancellationToken ct = default) =>
        Task.Run(() => Directory.Exists(folder)
            ? (IReadOnlyList<string>)Directory.EnumerateDirectories(folder, "*", new EnumerationOptions { IgnoreInaccessible = true }).ToList()
            : [], ct);

    private static Task<IReadOnlySet<string>> Existing(IEnumerable<string> paths, Func<string, bool> exists, CancellationToken ct) =>
        Task.Run(() =>
        {
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in paths)
            {
                ct.ThrowIfCancellationRequested();
                if (exists(path)) existing.Add(path);
            }
            return (IReadOnlySet<string>)existing;
        }, ct);
}
