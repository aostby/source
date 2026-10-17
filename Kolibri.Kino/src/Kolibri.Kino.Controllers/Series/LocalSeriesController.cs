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
/// <param name="Listed">The episode list has it; false for a file whose episode the list doesn't know (named after the file).</param>
public sealed record EpisodeView(SeriesEpisode Episode, string? FilePath, bool Listed = true)
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

/// <summary>A series in the missing episodes report.</summary>
/// <param name="Aired">Episodes aired so far (season 1 and up), by the episode list.</param>
/// <param name="OnDisk">Of those, the ones with a file.</param>
/// <param name="Source">Where the episode list came from ("TMDb", "OMDb"), or null without one.</param>
/// <param name="Note">Why the list couldn't be fetched, or "No episode list".</param>
/// <param name="FetchedOnline">The list was asked for online in this run (else it came from the database).</param>
public sealed record MissingEpisodesRow(
    LocalSeries Series, bool Ongoing, int StartYear, int Aired, int OnDisk, IReadOnlyList<SeriesEpisode> Missing, string? Source, string? Note,
    bool FetchedOnline = false);

/// <summary>How complete a series on disk is; see <see cref="SeriesCompleteness.State"/>.</summary>
public enum CompletenessState
{
    /// <summary>No episode list (TMDb and OMDb don't know it, or can't be asked), so it can't be told.</summary>
    Unknown,
    /// <summary>Ended, and every episode is on disk.</summary>
    Complete,
    /// <summary>Still running, and every episode aired so far is on disk; more are coming.</summary>
    UpToDate,
    /// <summary>Still running, and episodes aired so far are missing.</summary>
    Behind,
    /// <summary>Ended, and episodes are missing.</summary>
    Incomplete,
}

/// <summary>
/// Whether a series has all its episodes: those aired so far (season 1 and up) by TMDb's or OMDb's list, and which of
/// them have no file.
/// </summary>
/// <param name="Ongoing">The years are open ("2018–"), so more may come.</param>
/// <param name="Next">The next episode to air, when the list has one.</param>
/// <param name="Source">Where the episode list came from ("TMDb", "OMDb").</param>
/// <param name="Note">Why there is no list, or why it couldn't be brought up to date.</param>
public sealed record SeriesCompleteness(
    bool Ongoing, int Aired, int OnDisk, IReadOnlyList<SeriesEpisode> Missing, SeriesEpisode? Next, string? Source, string? Note)
{
    public CompletenessState State =>
        Aired == 0 ? (Next is null ? CompletenessState.Unknown : CompletenessState.UpToDate)
        : Missing.Count == 0 ? (Ongoing ? CompletenessState.UpToDate : CompletenessState.Complete)
        : Ongoing ? CompletenessState.Behind : CompletenessState.Incomplete;

    /// <summary>"Complete", "Up to date, more coming", "Missing 3 of 40 episodes" …</summary>
    public string Label => State switch
    {
        CompletenessState.Complete => "Complete",
        CompletenessState.UpToDate => "Up to date, more coming",
        CompletenessState.Behind or CompletenessState.Incomplete => $"Missing {Missing.Count} of {Aired} episodes",
        _ => "Completeness unknown",
    };

    /// <summary>A sentence on how it was decided, for a tooltip.</summary>
    public string Explanation => State switch
    {
        CompletenessState.Complete => $"Ended; all {Aired} episodes are here ({Source}'s episode list).",
        CompletenessState.UpToDate => $"Still running; all {Aired} episodes aired so far are here ({Source}'s episode list).{NextText}",
        CompletenessState.Behind => $"Still running; {OnDisk} of the {Aired} episodes aired so far are here ({Source}'s episode list).{NextText}",
        CompletenessState.Incomplete => $"Ended; {OnDisk} of its {Aired} episodes are here ({Source}'s episode list).",
        _ => Note ?? "No episode list from TMDb or OMDb.",
    };

    private string NextText => Next is { } n
        ? $" Next: S{n.SeasonNumber:00}E{n.EpisodeNumber:00}{(n.AirDate is { } d ? $" on {d:yyyy-MM-dd}" : "")}."
        : "";
}

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
        var (known, fromCache, note) = await LoadSeasonsAsync(series, refresh, ct).ConfigureAwait(false);

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
    /// The episode list, from the database first: Kino's cache, else what SilverScreen stored. Only when neither has it
    /// (or with <paramref name="refresh"/>) it is fetched online and stored, and a series no source knows is remembered,
    /// so it isn't asked about again for a while. When fetching fails (offline, no key, limit) the stored lists are used
    /// anyway. <c>Note</c> says why fetching failed.
    /// </summary>
    private async Task<(IReadOnlyList<SeriesSeason> Known, bool FromCache, string? Note)> LoadSeasonsAsync(
        LocalSeries series, bool refresh, CancellationToken ct)
    {
        var id = series.Item.ImdbId;
        if (!refresh)
        {
            // Locally first: Kino's list, then what SilverScreen stored. Online only when neither knows the series.
            var cached = await seasons.GetCachedSeasonsAsync(id, ct).ConfigureAwait(false);
            if (cached.Count > 0) return (cached, true, null);
            var stored = await seasons.GetSilverScreenSeasonsAsync(id, ct).ConfigureAwait(false);
            if (stored.Count > 0) return (stored, false, null);
        }

        IReadOnlyList<SeriesSeason> known = [];
        var info = await seasons.GetFetchInfoAsync(id, ct).ConfigureAwait(false);
        string? note;
        if (!refresh && RecentlyNotFound(info))
        {
            note = $"Not found on TMDb or OMDb on {info!.FetchedUtc.ToLocalTime():d}; Refresh episodes asks again";
        }
        else
        {
            (known, note) = await FetchAsync(series, info, ct).ConfigureAwait(false);
            if (known.Count > 0) return (known, false, null);
        }

        known = await seasons.GetCachedSeasonsAsync(id, ct).ConfigureAwait(false);
        if (known.Count > 0) return (known, true, note);
        return (await seasons.GetSilverScreenSeasonsAsync(id, ct).ConfigureAwait(false), false, note);
    }

    /// <summary>A series no source knew is asked about again only after this, so it doesn't cost requests on every run.</summary>
    internal static readonly TimeSpan NotFoundRetryAfter = TimeSpan.FromDays(30);

    /// <summary>An ongoing series' list is fetched again by the report only when it is older than this.</summary>
    internal static readonly TimeSpan OngoingRefreshAfter = TimeSpan.FromDays(3);

    private static bool RecentlyNotFound(SeriesFetchInfo? info) =>
        info is { Found: false } && DateTime.UtcNow - info.FetchedUtc < NotFoundRetryAfter;

    /// <summary>
    /// Fetches the seasons online (with the TMDb id from the last fetch, saving TMDb's lookup), caches them, and records
    /// the fetch, also when nothing was found. A failure (offline, limit) isn't recorded, so the next run tries again.
    /// </summary>
    /// <remarks>
    /// With <paramref name="stored"/> (an ongoing series' list), only its last season and later ones are fetched, as
    /// SilverScreen fetched only the seasons it didn't have; the earlier seasons are kept as stored.
    /// </remarks>
    private async Task<(IReadOnlyList<SeriesSeason> Known, string? Note)> FetchAsync(
        LocalSeries series, SeriesFetchInfo? info, CancellationToken ct, IReadOnlyList<SeriesSeason>? stored = null)
    {
        var id = series.Item.ImdbId;
        var from = stored?.Where(s => s.SeasonNumber > 0).Select(s => s.SeasonNumber).DefaultIfEmpty(0).Max() ?? 0;
        try
        {
            var result = await online.GetSeasonsAsync(id, series.TotalSeasons, info?.TmdbId, from, ct).ConfigureAwait(false);
            var found = result.Seasons.Count > 0;
            var all = found && from > 1
                ? stored!.Where(s => s.SeasonNumber < from).Concat(result.Seasons)
                    .OrderBy(s => s.SeasonNumber == 0 ? int.MaxValue : s.SeasonNumber).ToList()
                : result.Seasons;
            if (found) await seasons.SaveSeasonsAsync(id, all, ct).ConfigureAwait(false);
            await seasons.SaveFetchInfoAsync(id, new SeriesFetchInfo(DateTime.UtcNow, found, result.TmdbId ?? info?.TmdbId), ct).ConfigureAwait(false);
            return (all, found ? null : "Not found on TMDb or OMDb");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Episode details for {ImdbId} could not be fetched", id);
            return ([], ex.Message);
        }
    }

    /// <summary>
    /// The report's episode list, from the database when it can: Kino's cache, else SilverScreen's stored seasons.
    /// Online only for a series with neither (and not found recently), or, with <paramref name="refreshOngoing"/>,
    /// an ongoing series whose list is over <see cref="OngoingRefreshAfter"/> old and has no episode still to air
    /// (a list with one still to come already holds what will air next).
    /// </summary>
    private async Task<(IReadOnlyList<SeriesSeason> Known, bool Fetched, string? Note)> ReportSeasonsAsync(
        LocalSeries series, bool ongoing, bool refreshOngoing, CancellationToken ct)
    {
        var id = series.Item.ImdbId;
        var known = await seasons.GetCachedSeasonsAsync(id, ct).ConfigureAwait(false);
        if (known.Count == 0) known = await seasons.GetSilverScreenSeasonsAsync(id, ct).ConfigureAwait(false);
        var info = await seasons.GetFetchInfoAsync(id, ct).ConfigureAwait(false);

        var fetch = known.Count == 0
            ? !RecentlyNotFound(info)
            : refreshOngoing && ongoing
                && (info is null || DateTime.UtcNow - info.FetchedUtc > OngoingRefreshAfter)
                && !known.SelectMany(s => s.Episodes).Any(e => e.AirDate > DateTime.Today);
        if (!fetch)
            return (known, false, known.Count == 0 && info is { Found: false } ? $"Not found on TMDb or OMDb on {info.FetchedUtc.ToLocalTime():d}" : null);

        var (fetched, note) = await FetchAsync(series, info, ct, stored: known.Count > 0 ? known : null).ConfigureAwait(false);
        return fetched.Count > 0 ? (fetched, true, null) : (known, true, note);
    }

    /// <summary>
    /// Missing episodes for every series with a folder: aired episodes (season 1 and up, air date today or earlier,
    /// or unknown) that have no file. Ongoing series first, then ended ones; newest first within each.
    /// Episode lists come from the database where possible; see <see cref="ReportSeasonsAsync"/> for when it goes online.
    /// </summary>
    public async Task<IReadOnlyList<MissingEpisodesRow>> GetMissingEpisodesAsync(
        bool refreshOngoing, IProgress<(int Done, int Total, string Title)>? progress = null, CancellationToken ct = default)
    {
        var all = await GetSeriesAsync(ct).ConfigureAwait(false);
        var withFolder = all.Where(s => s.FolderExists && !s.FolderTooBroad).ToList();
        var today = DateTime.Today;
        var rows = new List<MissingEpisodesRow>();

        for (var i = 0; i < withFolder.Count; i++)
        {
            var series = withFolder[i];
            progress?.Report((i, withFolder.Count, series.Item.Title));
            var ongoing = IsOngoing(series.Item.Year);

            var (known, fetched, note) = await ReportSeasonsAsync(series, ongoing, refreshOngoing, ct).ConfigureAwait(false);
            var (found, _) = await episodeFiles.FindAsync(series.FolderPath!, ct).ConfigureAwait(false);
            var onDisk = found.SelectMany(f => f.EpisodeNumbers.Select(n => (f.SeasonNumber, n))).ToHashSet();
            var (aired, missing) = Compare(known, onDisk, today);

            rows.Add(new MissingEpisodesRow(series, ongoing, StartYear(series.Item.Year), aired.Count, aired.Count - missing.Count,
                missing, known.FirstOrDefault()?.Source, known.Count == 0 ? note ?? "No episode list" : note, fetched));
        }
        progress?.Report((withFolder.Count, withFolder.Count, ""));

        return rows
            .OrderByDescending(r => r.Ongoing)
            .ThenByDescending(r => r.StartYear)
            .ThenBy(r => r.Series.Item.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Whether the series has all its episodes, given the (season, episode) numbers on disk (the caller has read the
    /// folder, e.g. through its own path mapping). The episode list comes from the database, fetched online when there
    /// is none, or when a running series' list is old and has nothing still to air (see <see cref="ReportSeasonsAsync"/>).
    /// Null when the id isn't a series in the library.
    /// </summary>
    public async Task<SeriesCompleteness?> GetCompletenessAsync(
        string imdbId, IEnumerable<(int Season, int Episode)> onDisk, CancellationToken ct = default)
    {
        if (await movies.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is not { } item
            || !string.Equals(item.Type, "series", StringComparison.OrdinalIgnoreCase))
            return null;

        var ongoing = IsOngoing(item.Year);
        var (known, _, note) = await ReportSeasonsAsync(new LocalSeries(item, null, false), ongoing, refreshOngoing: true, ct)
            .ConfigureAwait(false);
        var today = DateTime.Today;
        var (aired, missing) = Compare(known, onDisk.ToHashSet(), today);
        var next = known.Where(s => s.SeasonNumber > 0).SelectMany(s => s.Episodes)
            .Where(e => e.AirDate?.Date > today)
            .OrderBy(e => e.AirDate).ThenBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber)
            .FirstOrDefault();
        return new SeriesCompleteness(ongoing, aired.Count, aired.Count - missing.Count, missing, next,
            known.FirstOrDefault()?.Source, known.Count == 0 ? note ?? "No episode list from TMDb or OMDb" : note);
    }

    /// <summary>
    /// The completeness of seasons already loaded (<see cref="GetDetailsAsync"/>), without asking anything again:
    /// the whole series (season 1 and up), or with <paramref name="season"/> that one season. A season that still has
    /// episodes to come counts as running, even when the series' years say it has ended.
    /// </summary>
    public static SeriesCompleteness CompletenessOf(LocalSeries series, SeriesDetails details, SeasonView? season = null)
    {
        var today = DateTime.Today;
        var listed = (season is null ? details.Seasons.Where(s => s.SeasonNumber > 0) : [season])
            .SelectMany(s => s.Episodes)
            .Where(v => v.Listed)
            .ToList();
        var aired = listed.Where(v => v.Episode.AirDate is null || v.Episode.AirDate.Value.Date <= today).ToList();
        var missing = aired.Where(v => !v.OnDisk).Select(v => v.Episode).ToList();
        var next = listed.Select(v => v.Episode).Where(e => e.AirDate?.Date > today)
            .OrderBy(e => e.AirDate).ThenBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber)
            .FirstOrDefault();
        var ongoing = IsOngoing(series.Item.Year) || (season is not null && next is not null);
        return new SeriesCompleteness(ongoing, aired.Count, aired.Count - missing.Count, missing, next, details.Source,
            details.Seasons.Count == 0 ? details.Note ?? "No episode list from TMDb or OMDb" : details.Note);
    }

    /// <summary>Episodes aired by <paramref name="today"/> (season 1 and up; no air date counts as aired), and those without a file.</summary>
    private static (List<SeriesEpisode> Aired, List<SeriesEpisode> Missing) Compare(
        IReadOnlyList<SeriesSeason> known, IReadOnlySet<(int, int)> onDisk, DateTime today)
    {
        var aired = known.Where(s => s.SeasonNumber > 0)
            .SelectMany(s => s.Episodes)
            .Where(e => e.AirDate is null || e.AirDate.Value.Date <= today)
            .OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber)
            .ToList();
        return (aired, aired.Where(e => !onDisk.Contains((e.SeasonNumber, e.EpisodeNumber))).ToList());
    }

    /// <summary>OMDb writes an ongoing series' years as "2010–" (open end); "2010–2015" or "2010" has ended.</summary>
    public static bool IsOngoing(string? year) => year?.Trim() is { Length: > 0 } y && (y.EndsWith('–') || y.EndsWith('-'));

    /// <summary>The first year in "2010–2015", or 0.</summary>
    public static int StartYear(string? year) =>
        year is { Length: >= 4 } && int.TryParse(year[..4], NumberStyles.None, CultureInfo.InvariantCulture, out var y) ? y : 0;

    /// <summary>"S01E03, S02E01–E05, S03E02": runs of following episodes in one season are shortened.</summary>
    public static string Describe(IEnumerable<SeriesEpisode> episodes)
    {
        var parts = new List<string>();
        foreach (var season in episodes.GroupBy(e => e.SeasonNumber).OrderBy(g => g.Key))
        {
            var numbers = season.Select(e => e.EpisodeNumber).Distinct().Order().ToList();
            for (var i = 0; i < numbers.Count;)
            {
                var j = i;
                while (j + 1 < numbers.Count && numbers[j + 1] == numbers[j] + 1) j++;
                parts.Add(j == i ? $"S{season.Key:00}E{numbers[i]:00}" : $"S{season.Key:00}E{numbers[i]:00}–E{numbers[j]:00}");
                i = j + 1;
            }
        }
        return string.Join(", ", parts);
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
                    kv.Value, Listed: false)));

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
