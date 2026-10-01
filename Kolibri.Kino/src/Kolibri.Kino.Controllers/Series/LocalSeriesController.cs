using System.Globalization;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Series;

/// <summary>A series in the library and its folder (SilverScreen links a series to its folder, not a file).</summary>
/// <param name="FolderTooBroad">
/// The folder also holds other series (e.g. it is \\server\Series itself), so its files can't be matched to this
/// series' episodes. Fix with <see cref="LocalSeriesController.SetFolderAsync"/>.
/// </param>
public sealed record LocalSeries(Item Item, string? FolderPath, bool FolderExists, bool FolderTooBroad = false)
{
    public int TotalSeasons => int.TryParse(Item.TotalSeasons, out var n) ? n : 0;
}

/// <summary>An episode and, if it's on disk, its file.</summary>
public sealed record EpisodeView(SeriesEpisode Episode, string? FilePath)
{
    public bool OnDisk => FilePath is not null;
}

public sealed record SeasonView(int SeasonNumber, string? Name, IReadOnlyList<EpisodeView> Episodes)
{
    public int OnDiskCount => Episodes.Count(e => e.OnDisk);
}

/// <param name="Source">Where the details came from: "TMDb", "OMDb", or null when only files are known.</param>
/// <param name="Note">Set when details couldn't be fetched (no key, limit reached, offline); what is known is still shown.</param>
public sealed record SeriesDetails(
    IReadOnlyList<SeasonView> Seasons, string? Source, bool FromCache, IReadOnlyList<string> UnrecognisedFiles, string? Note);

/// <summary>
/// The local series window (port of SilverScreen's ShowLocalSeriesForm + DetailsFormSeries).
/// </summary>
/// <remarks>
/// Episode details are fetched when a series is opened (TMDb: one request per season; see
/// <see cref="ISeriesInfoProvider"/>) and cached, so the next time is instant. SilverScreen's stored OMDb seasons are
/// used when nothing can be fetched. Files in the series folder are matched to episodes by name (S01E02 …).
/// </remarks>
public sealed class LocalSeriesController(
    IMovieRepository movies,
    IFileItemRepository files,
    IMediaFileScanner scanner,
    ISeriesRepository seasons,
    ISeriesInfoProvider online,
    IEpisodeFileFinder episodeFiles,
    IPosterProvider posters,
    ISettingsStore settings,
    MovieLinker linker,
    ILogger<LocalSeriesController> logger)
{
    /// <summary>UserFilePaths.SeriesSourcePath, where folder pickers start.</summary>
    public string? SeriesFolder => settings.Current.UserFilePaths.SeriesSourcePath;

    /// <summary>All series in the library with their folders, best rated first (as SilverScreen sorts them).</summary>
    public async Task<IReadOnlyList<LocalSeries>> GetSeriesAsync(CancellationToken ct = default)
    {
        var series = await movies.GetAllAsync("series", ct).ConfigureAwait(false);
        var links = (await files.GetAllAsync(ct).ConfigureAwait(false))
            .GroupBy(f => f.ImdbId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().FullName, StringComparer.OrdinalIgnoreCase);

        // SilverScreen stores the folder both as the FileItem and in TomatoUrl; the FileItem wins.
        string? FolderOf(Item i) => links.GetValueOrDefault(i.ImdbId) ?? (string.IsNullOrWhiteSpace(i.TomatoUrl) || i.TomatoUrl == "N/A" ? null : i.TomatoUrl);
        var folders = series.Select(FolderOf).OfType<string>().ToList();
        var existing = await scanner.FindExistingFoldersAsync(folders, ct).ConfigureAwait(false);

        return series
            .Select(i => FolderOf(i) is { } f
                ? new LocalSeries(i, f, existing.Contains(f), IsTooBroad(f, series.Where(o => o != i).Select(FolderOf).OfType<string>()))
                : new LocalSeries(i, null, false))
            .OrderByDescending(s => Rating(s.Item.ImdbRating))
            .ThenBy(s => s.Item.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// True when <paramref name="folder"/> is the series root (UserFilePaths.SeriesSourcePath) or is, or contains,
    /// another series' folder. Scanning it would match every series' files to this one's episodes.
    /// </summary>
    private bool IsTooBroad(string folder, IEnumerable<string> otherFolders)
    {
        var mine = Normalize(folder);
        if (SeriesFolder is { } root && string.Equals(mine, Normalize(root), StringComparison.OrdinalIgnoreCase)) return true;
        return otherFolders.Select(Normalize).Any(other =>
            string.Equals(other, mine, StringComparison.OrdinalIgnoreCase)
            || other.StartsWith(mine + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));

        static string Normalize(string path) => path.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Seasons and episodes, with the episodes on disk marked. Uses Kino's cache unless <paramref name="refresh"/>;
    /// otherwise fetches online and caches; falls back to SilverScreen's stored OMDb data.
    /// </summary>
    public async Task<SeriesDetails> GetDetailsAsync(LocalSeries series, bool refresh, CancellationToken ct = default)
    {
        var id = series.Item.ImdbId;
        string? note = null;
        var fromCache = false;

        var known = refresh ? [] : await seasons.GetCachedSeasonsAsync(id, ct).ConfigureAwait(false);
        if (known.Count > 0)
        {
            fromCache = true;
        }
        else
        {
            try
            {
                known = await online.GetSeasonsAsync(id, series.TotalSeasons, ct).ConfigureAwait(false);
                if (known.Count > 0) await seasons.SaveSeasonsAsync(id, known, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Episode details for {ImdbId} could not be fetched", id);
                note = ex.Message;
            }

            if (known.Count == 0)
            {
                known = await seasons.GetCachedSeasonsAsync(id, ct).ConfigureAwait(false);
                fromCache = known.Count > 0;
            }
            if (known.Count == 0)
                known = await seasons.GetSilverScreenSeasonsAsync(id, ct).ConfigureAwait(false);
        }

        IReadOnlyList<EpisodeFile> found = [];
        IReadOnlyList<string> unrecognised = [];
        if (series.FolderTooBroad)
        {
            var warning = $"The folder {series.FolderPath} holds other series too, so no files are matched; use Set folder…";
            note = note is null ? warning : $"{note}; {warning}";
        }
        else if (series.FolderExists && series.FolderPath is { } folder)
        {
            (found, unrecognised) = await episodeFiles.FindAsync(folder, ct).ConfigureAwait(false);
        }

        return new SeriesDetails(Merge(known, found), known.FirstOrDefault()?.Source, fromCache, unrecognised, note);
    }

    /// <summary>
    /// Puts each file on its episode(s). Files for episodes the list doesn't have (or seasons it lacks) get their own
    /// rows, named after the file, so everything on disk is visible.
    /// </summary>
    internal static IReadOnlyList<SeasonView> Merge(IReadOnlyList<SeriesSeason> known, IReadOnlyList<EpisodeFile> found)
    {
        var byEpisode = new Dictionary<(int, int), string>();
        foreach (var file in found)
            foreach (var number in file.EpisodeNumbers)
                byEpisode.TryAdd((file.SeasonNumber, number), file.Path);

        var seasonNumbers = known.Select(s => s.SeasonNumber).Concat(found.Select(f => f.SeasonNumber)).Distinct()
            .OrderBy(n => n == 0 ? int.MaxValue : n);

        var result = new List<SeasonView>();
        foreach (var number in seasonNumbers)
        {
            var season = known.FirstOrDefault(s => s.SeasonNumber == number);
            var listed = season?.Episodes ?? [];
            var views = listed.Select(e => new EpisodeView(e, byEpisode.GetValueOrDefault((number, e.EpisodeNumber)))).ToList();

            var listedNumbers = listed.Select(e => e.EpisodeNumber).ToHashSet();
            views.AddRange(byEpisode
                .Where(kv => kv.Key.Item1 == number && !listedNumbers.Contains(kv.Key.Item2))
                .Select(kv => new EpisodeView(
                    new SeriesEpisode(number, kv.Key.Item2, Path.GetFileNameWithoutExtension(kv.Value), null, null, null, null, null, null),
                    kv.Value)));

            result.Add(new SeasonView(number, season?.Name, views.OrderBy(v => v.Episode.EpisodeNumber).ToList()));
        }
        return result;
    }

    /// <summary>
    /// The series with this IMDb id, from the library, Plex or OMDb, and the folder it is already linked to, if any.
    /// Null when the id is unknown.
    /// </summary>
    public async Task<(Item Item, string? LinkedFolder)?> LookUpAsync(string imdbId, CancellationToken ct = default)
    {
        var id = imdbId.Trim().ToLowerInvariant();
        if (await linker.FindByImdbIdAsync(id, usePlex: true, ct).ConfigureAwait(false) is not { } item) return null;
        var link = await files.GetByImdbIdAsync(item.ImdbId, ct).ConfigureAwait(false);
        return (item, link?.FullName);
    }

    /// <summary>
    /// Re-links the series' folder to another IMDb id (when it was matched to the wrong show, e.g. the 2015 or the
    /// 2024 "Dark Matter"). The wrongly matched entry stays in the library, without this folder.
    /// </summary>
    /// <exception cref="InvalidOperationException">The series has no folder, or <paramref name="correct"/> isn't a series.</exception>
    public async Task<LocalSeries> ChangeImdbIdAsync(LocalSeries series, Item correct, CancellationToken ct = default)
    {
        if (series.FolderPath is not { } folder)
            throw new InvalidOperationException($"{series.Item.Title} has no folder to move; use Set folder… on the right series instead.");
        if (!string.Equals(correct.Type, "series", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"{correct.ImdbId} is \"{correct.Title}\", a {correct.Type ?? "title"}, not a series.");

        var wrong = series.Item;
        if (await files.GetByImdbIdAsync(wrong.ImdbId, ct).ConfigureAwait(false) is { } oldLink
            && string.Equals(oldLink.FullName, folder, StringComparison.OrdinalIgnoreCase))
            await files.DeleteAsync(wrong.ImdbId, ct).ConfigureAwait(false);

        // The list falls back to TomatoUrl when there is no FileItem, so clear it too, or the folder stays on the wrong entry.
        if (string.Equals(wrong.TomatoUrl, folder, StringComparison.OrdinalIgnoreCase))
        {
            wrong.TomatoUrl = null;
            await movies.UpsertAsync(wrong, ct).ConfigureAwait(false);
        }

        await linker.LinkAsync(correct, folder, ct).ConfigureAwait(false);
        logger.LogInformation("Folder {Folder} moved from {Wrong} ({WrongTitle}) to {Right} ({RightTitle})",
            folder, wrong.ImdbId, wrong.Title, correct.ImdbId, correct.Title);

        var others = (await GetSeriesAsync(ct).ConfigureAwait(false))
            .Where(s => s.Item.ImdbId != correct.ImdbId).Select(s => s.FolderPath).OfType<string>();
        return new LocalSeries(correct, folder, series.FolderExists, IsTooBroad(folder, others));
    }

    /// <summary>Links the series to <paramref name="folder"/> (SilverScreen's "Finn" / find-by-id).</summary>
    public async Task<LocalSeries> SetFolderAsync(LocalSeries series, string folder, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        await files.UpsertAsync(new FileItem { ImdbId = series.Item.ImdbId, FullName = folder }, ct).ConfigureAwait(false);
        series.Item.TomatoUrl = folder;
        await movies.UpsertAsync(series.Item, ct).ConfigureAwait(false);
        var exists = (await scanner.FindExistingFoldersAsync([folder], ct).ConfigureAwait(false)).Count > 0;
        var others = (await GetSeriesAsync(ct).ConfigureAwait(false))
            .Where(s => s.Item.ImdbId != series.Item.ImdbId).Select(s => s.FolderPath).OfType<string>();
        return series with { FolderPath = folder, FolderExists = exists, FolderTooBroad = IsTooBroad(folder, others) };
    }

    public async Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            return await posters.GetPosterAsync(item, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Poster download failed for {ImdbId}", item.ImdbId);
            return null;
        }
    }

    /// <summary>The episode's screenshot (TMDb), cached like posters under "{seriesId}_S01E02".</summary>
    public Task<byte[]?> GetStillAsync(string seriesImdbId, SeriesEpisode episode, CancellationToken ct = default) =>
        episode.StillUrl is null
            ? Task.FromResult<byte[]?>(null)
            : GetPosterAsync(new Item { ImdbId = $"{seriesImdbId}_S{episode.SeasonNumber:00}E{episode.EpisodeNumber:00}", Poster = episode.StillUrl }, ct);

    private static double Rating(string? rating) =>
        double.TryParse(rating, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : -1;
}
