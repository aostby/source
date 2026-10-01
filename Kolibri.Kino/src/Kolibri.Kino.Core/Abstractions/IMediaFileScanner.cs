namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Finds video files on disk.
/// </summary>
public interface IMediaFileScanner
{
    /// <summary>All files under <paramref name="folder"/> that pass <see cref="MovieFileRules.IsVideoFile"/>.</summary>
    Task<IReadOnlyList<string>> FindVideoFilesAsync(string folder, CancellationToken ct = default);

    /// <summary>The subset of <paramref name="paths"/> that exist on disk (case-insensitive set).</summary>
    Task<IReadOnlySet<string>> FindExistingAsync(IEnumerable<string> paths, CancellationToken ct = default);

    /// <summary>The subset of <paramref name="folders"/> that exist (case-insensitive set).</summary>
    Task<IReadOnlySet<string>> FindExistingFoldersAsync(IEnumerable<string> folders, CancellationToken ct = default);

    /// <summary>The folders directly inside <paramref name="folder"/>; empty if it doesn't exist.</summary>
    Task<IReadOnlyList<string>> GetSubfoldersAsync(string folder, CancellationToken ct = default);
}
