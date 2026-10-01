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

    public async Task<CleanupResult> ExecuteAsync(CleanupPlan plan, CancellationToken ct = default)
    {
        var result = await cleaner.DeleteAsync(plan.Files, plan.Folders, Settings.DeleteEmptyFolders, ct).ConfigureAwait(false);
        logger.LogInformation("Cleanup deleted {Files} file(s) and {Folders} empty folder(s); {Failed} failed",
            result.FilesDeleted, result.FoldersDeleted, result.Failed.Count);
        return result;
    }
}
