using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Controllers.Cleanup;

/// <param name="Folders">Where to look (recursively) and, afterwards, where empty folders are removed.</param>
/// <param name="Files">What would be deleted.</param>
public sealed record CleanupPlan(IReadOnlyList<string> Folders, IReadOnlyList<string> Files);

/// <summary>
/// Port of SilverScreen's StartUnwantedFilesCleanup: delete leftover files (.nfo, .txt, .jpg, .exe, foreign
/// subtitles …) from movie folders. The patterns come from Kino:Cleanup:FilePatterns in appsettings.json.
/// </summary>
/// <remarks>
/// Split into <see cref="PlanAsync"/> and <see cref="ExecuteAsync"/> so the UI can show the list first
/// (Kino:Cleanup:AfterScan = Ask). Video files are never deleted, whatever the patterns say.
/// </remarks>
public sealed class CleanupController(IFolderCleaner cleaner, IOptions<KinoOptions> options, ILogger<CleanupController> logger)
{
    private CleanupOptions Settings => options.Value.Cleanup;

    public CleanupAfterScan AfterScan => Settings.AfterScan;

    public bool DeleteEmptyFolders => Settings.DeleteEmptyFolders;

    public async Task<CleanupPlan> PlanAsync(IEnumerable<string> folders, CancellationToken ct = default)
    {
        var list = folders.ToList();
        var found = await cleaner.FindAsync(list, Settings.EffectivePatterns, ct).ConfigureAwait(false);
        return new CleanupPlan(list, found);
    }

    /// <summary>
    /// File → Tools → Delete empty folders: removes the empty folders anywhere below <paramref name="root"/>, deepest
    /// first (a folder holding only empty folders goes too). The chosen folder itself is kept.
    /// </summary>
    public Task<CleanupResult> DeleteEmptyFoldersAsync(string root, CancellationToken ct = default) => CleanFolderAsync(root, [], ct);

    /// <summary>
    /// File → Tools → Delete empty folders and leftover files: deletes <paramref name="files"/> (a <see cref="PlanAsync"/>
    /// the user has seen), then the folders below <paramref name="root"/> left empty; the chosen folder itself is kept.
    /// </summary>
    public async Task<CleanupResult> CleanFolderAsync(string root, IReadOnlyList<string> files, CancellationToken ct = default)
    {
        if (!Directory.Exists(root)) throw new DirectoryNotFoundException($"{root} doesn't exist.");
        var below = Directory.GetDirectories(root);
        var result = await cleaner.DeleteAsync(files, below, deleteEmptyFolders: true, ct).ConfigureAwait(false);
        logger.LogInformation("Cleanup of {Root} deleted {Files} file(s) and {Folders} empty folder(s); {Failed} failed",
            root, result.FilesDeleted, result.FoldersDeleted, result.Failed.Count);
        return result;
    }

    public async Task<CleanupResult> ExecuteAsync(CleanupPlan plan, CancellationToken ct = default)
    {
        var result = await cleaner.DeleteAsync(plan.Files, plan.Folders, Settings.DeleteEmptyFolders, ct).ConfigureAwait(false);
        logger.LogInformation("Cleanup deleted {Files} file(s) and {Folders} empty folder(s); {Failed} failed",
            result.FilesDeleted, result.FoldersDeleted, result.Failed.Count);
        return result;
    }
}
