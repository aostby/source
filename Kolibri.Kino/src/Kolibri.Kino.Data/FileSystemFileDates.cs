using System.Collections.Concurrent;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// File dates through <see cref="KinoOptions.PathMappings"/>, so the Windows paths in the database work in the
/// container too. Walking a library of cold disks takes minutes, so dates are stored (<see cref="LiteDbFileDateStore"/>)
/// and only paths never seen before are read while the caller waits. Stale ones are answered from the store and
/// read again in the background: series folders after <see cref="FolderFreshFor"/> (new episodes), files after
/// <see cref="FileFreshFor"/>, unreachable paths after <see cref="MissingFreshFor"/>.
/// </summary>
public sealed class FileSystemFileDates(IOptions<KinoOptions> options, LiteDbFileDateStore store, ILogger<FileSystemFileDates> logger) : IFileDates
{
    internal static readonly TimeSpan FolderFreshFor = TimeSpan.FromMinutes(30);
    internal static readonly TimeSpan FileFreshFor = TimeSpan.FromDays(7);
    internal static readonly TimeSpan MissingFreshFor = TimeSpan.FromHours(6);

    private static readonly EnumerationOptions Recursive = new() { RecurseSubdirectories = true, IgnoreInaccessible = true };

    // One disk walk at a time: the Library shelf and the Recently added page ask at once, and the second
    // should get the first one's answers, not walk the disks again.
    private readonly SemaphoreSlim _reading = new(1, 1);
    private ConcurrentDictionary<string, StoredFileDate>? _known;
    private int _refreshing;

    /// <summary>For tests: the clock.</summary>
    internal Func<DateTime> UtcNow { get; init; } = () => DateTime.UtcNow;

    public async Task<IReadOnlyDictionary<string, DateTime>> GetAddedAsync(IEnumerable<string> paths, CancellationToken ct = default)
    {
        var list = paths.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var known = await KnownAsync(ct).ConfigureAwait(false);

        if (list.Any(p => !known.ContainsKey(p)))
        {
            await _reading.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // Again inside the lock: another caller may have read them meanwhile.
                await ReadAndStoreAsync(list.Where(p => !known.ContainsKey(p)).ToList(), ct).ConfigureAwait(false);
            }
            finally
            {
                _reading.Release();
            }
        }

        var now = UtcNow();
        var stale = list.Where(p => known.TryGetValue(p, out var d) && IsStale(d, now)).ToList();
        if (stale.Count > 0) RefreshInBackground(stale);

        var found = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in list)
            if (known.TryGetValue(path, out var d) && d.AddedUtc is { } added) found[path] = added;
        return found;
    }

    internal static bool IsStale(StoredFileDate date, DateTime nowUtc) =>
        nowUtc - date.CheckedUtc > (date.AddedUtc is null ? MissingFreshFor : date.IsFolder ? FolderFreshFor : FileFreshFor);

    private async Task<ConcurrentDictionary<string, StoredFileDate>> KnownAsync(CancellationToken ct)
    {
        if (_known is { } known) return known;

        await _reading.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            return _known ??= new ConcurrentDictionary<string, StoredFileDate>(
                (await Task.Run(store.LoadAll, ct).ConfigureAwait(false)).Select(d => KeyValuePair.Create(d.Path, d)),
                StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            _reading.Release();
        }
    }

    /// <summary>Reads the paths (several at a time) and stores the answers. Call with <see cref="_reading"/> held.</summary>
    private async Task ReadAndStoreAsync(IReadOnlyList<string> paths, CancellationToken ct)
    {
        if (paths.Count == 0) return;

        var mappings = options.Value;
        var read = new ConcurrentBag<StoredFileDate>();
        await Task.Run(() =>
            // Several at a time: each stat waits on a disk (or, over SMB on Windows, on the network).
            Parallel.ForEach(paths, new ParallelOptions { MaxDegreeOfParallelism = 8, CancellationToken = ct }, path =>
            {
                var (added, isFolder) = Read(mappings.MapPath(path));
                read.Add(new StoredFileDate(path, added, isFolder, UtcNow()));
            }), ct).ConfigureAwait(false);

        var known = _known!;
        foreach (var date in read) known[date.Path] = date;
        await Task.Run(() => store.Save(read), CancellationToken.None).ConfigureAwait(false);
    }

    private void RefreshInBackground(IReadOnlyList<string> paths)
    {
        if (Interlocked.CompareExchange(ref _refreshing, 1, 0) != 0) return;

        _ = Task.Run(async () =>
        {
            try
            {
                await _reading.WaitAsync().ConfigureAwait(false);
                try
                {
                    await ReadAndStoreAsync(paths, CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    _reading.Release();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Refreshing {Count} file date(s) failed", paths.Count);
            }
            finally
            {
                Volatile.Write(ref _refreshing, 0);
            }
        });
    }

    /// <summary>
    /// The added time of a file, or of a folder's newest video file (else the folder itself); null if the path
    /// isn't here.
    /// </summary>
    internal static (DateTime? AddedUtc, bool IsFolder) Read(string localPath)
    {
        try
        {
            if (File.Exists(localPath)) return (Added(new FileInfo(localPath)), false);
            if (!Directory.Exists(localPath)) return (null, false);

            DateTime? newest = null;
            foreach (var file in new DirectoryInfo(localPath).EnumerateFiles("*", Recursive))
            {
                if (!MovieFileRules.IsVideoFile(file.FullName)) continue;
                var added = Added(file);
                if (newest is null || added > newest) newest = added;
            }
            return (newest ?? Added(new DirectoryInfo(localPath)), true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return (null, false);
        }
    }

    // A copy keeps the old write time but gets a new creation time, so the later of the two is when it arrived.
    private static DateTime Added(FileSystemInfo info) =>
        info.CreationTimeUtc > info.LastWriteTimeUtc ? info.CreationTimeUtc : info.LastWriteTimeUtc;
}
