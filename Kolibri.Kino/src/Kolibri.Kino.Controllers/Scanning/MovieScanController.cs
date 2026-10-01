using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Scanning;

/// <summary>
/// Port of SilverScreen's "Oppdater" (MoviesSearchController.SearchForMovies): match the video files in a folder
/// to movies and link them in the library.
/// </summary>
/// <remarks>
/// Matching order, cheapest first: IMDb id in the name, the local library, Plex, TMDb, OMDb.
/// Differences from SilverScreen, on purpose:
/// <list type="bullet">
/// <item>An existing link to a file that still exists is never overwritten; the second file is reported as a duplicate.</item>
/// <item>Removing library entries for missing files is a separate, explicit call (<see cref="RemoveMissingAsync"/>),
/// and it keeps the movie details.</item>
/// <item>Leftover-file cleanup is a separate step (CleanupController) run on <see cref="ScanReport.Folders"/>.</item>
/// </list>
/// </remarks>
public sealed class MovieScanController(
    IMediaFileScanner scanner,
    IFileItemRepository files,
    IMovieRepository movies,
    IMovieFileNameParser parser,
    IMediaServerLibrary plex,
    IMovieInfoProvider omdb,
    IImdbIdResolver tmdb,
    MovieLinker linker,
    ILogger<MovieScanController> logger)
{
    public async Task<ScanReport> ScanAsync(string folder, ScanMode mode, IProgress<ScanProgress>? progress = null, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        var videoFiles = await scanner.FindVideoFilesAsync(folder, ct).ConfigureAwait(false);
        var links = await files.GetAllAsync(ct).ConfigureAwait(false);
        var run = new Run(
            links.GroupBy(f => f.FullName, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase),
            links.GroupBy(f => f.ImdbId, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase));

        if (!plex.IsConfigured)
            run.PlexOff = "not configured";
        else
            await TryOptionalAsync(run, isPlex: true, () => plex.RefreshAsync(ct)).ConfigureAwait(false);

        // Main files before CD2/Part2/Extra, so the movie links to its main file.
        var ordered = videoFiles
            .OrderBy(MovieFileRules.IsMultipart)
            .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Titles for files that are already linked, in one local read, so the report can name them.
        var linkedIds = ordered.Select(p => run.ByPath.GetValueOrDefault(p)?.ImdbId).OfType<string>();
        run.LinkedTitles = await movies.GetByImdbIdsAsync(linkedIds, ct).ConfigureAwait(false);

        var entries = new List<ScanEntry>(ordered.Count);
        string? stopped = null;

        for (var i = 0; i < ordered.Count; i++)
        {
            var path = ordered[i];
            progress?.Report(new ScanProgress(i, ordered.Count, path));
            try
            {
                entries.Add(await ScanFileAsync(path, mode, run, ct).ConfigureAwait(false));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                stopped = "Cancelled.";
                break;
            }
            catch (MovieInfoUnavailableException ex)
            {
                stopped = ex.Message;
                entries.Add(new ScanEntry(path, ScanOutcome.Failed, Detail: ex.Message));
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Scan failed for {Path}", path);
                entries.Add(new ScanEntry(path, ScanOutcome.Failed, Detail: ex.Message));
            }
        }

        progress?.Report(new ScanProgress(entries.Count, ordered.Count, string.Empty));

        var notes = new List<string>();
        if (run.PlexOff is { } plexOff && plexOff != "not configured") notes.Add($"Plex was skipped: {plexOff}");
        if (run.TmdbOff is { } tmdbOff) notes.Add($"TMDb was skipped: {tmdbOff}");

        // SilverScreen cleaned the folder of every scanned file, linked or not.
        var folders = videoFiles.Select(Path.GetDirectoryName).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        logger.LogInformation("Scanned {Folder} ({Mode}): {Linked} linked, {NotFound} not found{Stopped}",
            folder, mode, entries.Count(e => e.Outcome == ScanOutcome.Linked), entries.Count(e => e.Outcome == ScanOutcome.NotFound),
            stopped is null ? "" : $", stopped: {stopped}");
        return new ScanReport(folder, mode, entries, stopped, notes, folders);
    }

    /// <summary>
    /// Removes the file links in <paramref name="folder"/> whose file no longer exists. Movie details are kept,
    /// so a later scan can link the movie again. Returns the number of links removed.
    /// </summary>
    public async Task<int> RemoveMissingAsync(string folder, CancellationToken ct = default)
    {
        var inFolder = await files.GetInFolderAsync(folder, ct).ConfigureAwait(false);
        var existing = await scanner.FindExistingAsync(inFolder.Select(f => f.FullName), ct).ConfigureAwait(false);

        var removed = 0;
        foreach (var gone in inFolder.Where(f => !existing.Contains(f.FullName)))
        {
            ct.ThrowIfCancellationRequested();
            if (await files.DeleteAsync(gone.ImdbId, ct).ConfigureAwait(false)) removed++;
        }

        logger.LogInformation("Removed {Count} missing file link(s) in {Folder}", removed, folder);
        return removed;
    }

    private async Task<ScanEntry> ScanFileAsync(string path, ScanMode mode, Run run, CancellationToken ct)
    {
        if (run.ByPath.TryGetValue(path, out var link))
        {
            if (mode == ScanMode.NewFiles)
                return new ScanEntry(path, ScanOutcome.AlreadyLinked, link.ImdbId, run.LinkedTitles.GetValueOrDefault(link.ImdbId)?.Title,
                    Detail: run.LinkedTitles.ContainsKey(link.ImdbId) ? null : "Linked to an IMDb id with no details in the library.");

            // "Refresh details" means the full OMDb record, so Plex is not used here.
            var fresh = await omdb.GetByImdbIdAsync(link.ImdbId, ct).ConfigureAwait(false);
            if (fresh is null)
                return new ScanEntry(path, ScanOutcome.Failed, link.ImdbId, Detail: "OMDb has no details for this id.");

            await linker.LinkAsync(fresh, path, ct).ConfigureAwait(false);
            return new ScanEntry(path, ScanOutcome.Refreshed, fresh.ImdbId, fresh.Title, "OMDb");
        }

        var parsed = parser.Parse(path);
        if (parsed.LooksLikeSeries && parsed.ImdbId is null)
            return new ScanEntry(path, ScanOutcome.Skipped, Detail: "Looks like a series episode.");

        var (item, source) = await ResolveAsync(parsed, run, ct).ConfigureAwait(false);
        if (item is null)
        {
            var tried = parsed.Titles.Count == 0 ? "no usable title" : string.Join(" | ", parsed.Titles);
            return new ScanEntry(path, ScanOutcome.NotFound, Detail: $"Tried: {tried}{(parsed.Year is { } y ? $" ({y})" : "")}");
        }

        if (run.ByImdbId.TryGetValue(item.ImdbId, out var other)
            && !string.Equals(other.FullName, path, StringComparison.OrdinalIgnoreCase)
            && (await scanner.FindExistingAsync([other.FullName], ct).ConfigureAwait(false)).Count > 0)
        {
            // CD1/CD2 of one movie in one folder: one link is expected, the rest are just its other parts.
            var sameFolder = string.Equals(Path.GetDirectoryName(other.FullName), Path.GetDirectoryName(path), StringComparison.OrdinalIgnoreCase);
            if (sameFolder && (MovieFileRules.IsMultipart(path) || MovieFileRules.IsMultipart(other.FullName)))
                return new ScanEntry(path, ScanOutcome.OtherPart, item.ImdbId, item.Title, source, $"Other part of {Path.GetFileName(other.FullName)}", other.FullName);

            return new ScanEntry(path, ScanOutcome.Duplicate, item.ImdbId, item.Title, source, $"Already linked to {other.FullName}", other.FullName);
        }

        var linked = await linker.LinkAsync(item, path, ct).ConfigureAwait(false);
        run.ByImdbId[item.ImdbId] = linked;
        run.ByPath[path] = linked;
        return new ScanEntry(path, ScanOutcome.Linked, item.ImdbId, item.Title, source);
    }

    /// <summary>Cheapest source first: IMDb id in the name, the local library, Plex, TMDb, then OMDb by title.</summary>
    private async Task<(Item? Item, string? Source)> ResolveAsync(ParsedMovieFile parsed, Run run, CancellationToken ct)
    {
        if (parsed.ImdbId is { } taggedId && await ByImdbIdAsync(taggedId, run, ct).ConfigureAwait(false) is { } tagged)
            return (tagged, "IMDb id in name");

        foreach (var title in parsed.Titles)
            if (await movies.FindByTitleAsync(title, parsed.Year, ct).ConfigureAwait(false) is { } local)
                return (local, "library");

        foreach (var title in parsed.Titles)
        {
            if (run.PlexOff is not null) break;
            Item? fromPlex = null;
            await TryOptionalAsync(run, isPlex: true, async () => fromPlex = await plex.FindMovieByTitleAsync(title, parsed.Year, ct).ConfigureAwait(false)).ConfigureAwait(false);
            if (fromPlex is not null) return (fromPlex, "Plex");
        }

        foreach (var title in parsed.Titles)
        {
            if (run.TmdbOff is not null) break;
            string? id = null;
            await TryOptionalAsync(run, isPlex: false, async () => id = await tmdb.FindImdbIdAsync(title, parsed.Year, ct).ConfigureAwait(false)).ConfigureAwait(false);
            if (id is not null && await ByImdbIdAsync(id, run, ct).ConfigureAwait(false) is { } found)
                return (found, "TMDb");
        }

        foreach (var title in parsed.Titles)
            if (await omdb.GetMovieByTitleAsync(title, parsed.Year, ct).ConfigureAwait(false) is { } online)
                return (online, "OMDb");

        return (null, null);
    }

    private Task<Item?> ByImdbIdAsync(string imdbId, Run run, CancellationToken ct) =>
        linker.FindByImdbIdAsync(imdbId, usePlex: run.PlexOff is null, ct);

    /// <summary>
    /// Plex and TMDb are optional: if one fails (server offline, bad key), stop using it for this scan and carry on.
    /// </summary>
    private async Task TryOptionalAsync(Run run, bool isPlex, Func<Task> call)
    {
        try
        {
            await call().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "{Source} failed; skipping it for the rest of the scan", isPlex ? "Plex" : "TMDb");
            if (isPlex) run.PlexOff = ex.Message;
            else run.TmdbOff = ex.Message;
        }
    }

    private sealed class Run(Dictionary<string, FileItem> byPath, Dictionary<string, FileItem> byImdbId)
    {
        public Dictionary<string, FileItem> ByPath { get; } = byPath;
        public Dictionary<string, FileItem> ByImdbId { get; } = byImdbId;
        public string? PlexOff { get; set; }
        public string? TmdbOff { get; set; }
        public IReadOnlyDictionary<string, Item> LinkedTitles { get; set; } = new Dictionary<string, Item>();
    }
}
