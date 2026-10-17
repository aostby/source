using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Wishlist;

/// <summary>A search hit (a movie or a series): in the library (SilverScreen.db) already, or found online and to be got.</summary>
public sealed record WishlistHit(string ImdbId, string Title, string? Year, string? Type, bool InSilverScreen, bool OnWishlist);

/// <param name="Hits">The library's hits first, then OMDb's that aren't in the library.</param>
/// <param name="OnlineError">Why OMDb couldn't be searched (no key, daily limit …); the library's hits are still there.</param>
public sealed record WishlistSearch(IReadOnlyList<WishlistHit> Hits, string? OnlineError);

/// <summary>What an import did.</summary>
/// <param name="Added">Rows put on the wishlist.</param>
/// <param name="AlreadyThere">Rows whose IMDb id was on the wishlist already (or twice in the file); left as they were.</param>
/// <param name="Invalid">Rows without a usable IMDb id.</param>
public sealed record WishlistImport(int Added, int AlreadyThere, int Invalid);

/// <summary>A wishlist entry, with whether SilverScreen.db has the title now and the file it is linked to, if any.</summary>
public sealed record WishlistRow(WishlistItem Item, bool InSilverScreen, string? FilePath);

/// <summary>
/// The wishlist: titles wanted (movies and series), perhaps not owned. A search looks in the library (SilverScreen.db) first, then OMDb,
/// and says which is which; titles from OMDb aren't put in the library, so they have no file. The list says, each time
/// it is read, whether the library has each title now, and is written to a file with that as a true/false column.
/// </summary>
public sealed partial class WishlistController(
    IWishlistRepository wishlist,
    IMovieRepository library,
    IFileItemRepository files,
    IMovieInfoProvider omdb,
    ITmdbLinks tmdb,
    IImdbIdResolver tmdbSearch,
    ILogger<WishlistController> logger)
{
    /// <summary>
    /// "Operation Kabul, 2025" or "Operation Kabul (2025)": the title and the year. Without a comma or brackets the
    /// text is all title, so "Blade Runner 2049" stays a title.
    /// </summary>
    public static (string Title, int? Year) SplitYear(string text)
    {
        var match = TitleAndYear().Match(text.Trim());
        return match.Success ? (match.Groups["title"].Value.Trim(), int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture)) : (text.Trim(), null);
    }

    [GeneratedRegex(@"^(?<title>.+?)\s*(?:,\s*|\(\s*)(?<year>(?:19|20)\d\d)\s*\)?$")]
    private static partial Regex TitleAndYear();

    /// <summary>
    /// The library's titles containing the text (or the one with that IMDb id), then OMDb's search (movies and series)
    /// and TMDb's (movies); "title, year" (or "title (year)") keeps to that year (a series' first year). Titles found
    /// online are TMDb's (English) ones where TMDb knows the title. OMDb's games and single episodes are left out.
    /// </summary>
    public async Task<WishlistSearch> SearchAsync(string text, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var imdbId = MovieController.ImdbIdIn(text);
        var (title, year) = imdbId is null ? SplitYear(text) : (text.Trim(), null);

        IReadOnlyList<Item> local = imdbId is not null
            ? await library.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } one ? [one] : []
            : (await library.SearchByTitleAsync(title, ct).ConfigureAwait(false))
                .Where(i => year is null || (i.Year ?? "").StartsWith(year.Value.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                .ToList();

        IReadOnlyList<SearchItem> online = [];
        string? onlineError = null;
        try
        {
            online = imdbId is not null
                ? await omdb.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } item
                    ? [new SearchItem { ImdbId = item.ImdbId, Title = item.Title, Year = item.Year, Type = item.Type, Poster = item.Poster }]
                    : []
                : await omdb.SearchAsync(title, year: year, ct: ct).ConfigureAwait(false);
        }
        catch (MovieInfoUnavailableException ex)
        {
            onlineError = ex.Message;
        }

        // TMDb finds titles by their English name, where OMDb may only know another language's
        // ("Operation Kabul, 2025" is "Sárkányok Kabul felett" on OMDb, so OMDb's search misses it).
        if (imdbId is null)
        {
            try
            {
                var fromTmdb = await tmdbSearch.SearchMoviesAsync(title, year, ct: ct).ConfigureAwait(false);
                var known = online.Select(o => o.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
                online = online
                    .Concat(fromTmdb.Where(t => known.Add(t.ImdbId))
                        .Select(t => new SearchItem { ImdbId = t.ImdbId, Title = t.Title, Year = t.Year, Type = "movie" }))
                    .ToList();
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                logger.LogWarning(ex, "TMDb search for {Title} failed", title);
            }
        }

        var wanted = (await wishlist.GetAllAsync(ct).ConfigureAwait(false)).Select(w => w.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var localIds = local.Select(i => i.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var onlineOnly = online
            .Where(s => !string.IsNullOrEmpty(s.ImdbId) && !localIds.Contains(s.ImdbId) && WantedType(s.Type))
            .ToList();
        // An OMDb hit may still be in the library (the title search just didn't match it there).
        var inLibrary = await library.GetByImdbIdsAsync(onlineOnly.Select(s => s.ImdbId), ct).ConfigureAwait(false);

        var titles = await EnglishTitlesAsync(onlineOnly.Select(s => s.ImdbId), ct).ConfigureAwait(false);

        var hits = local
            .Select(i => new WishlistHit(i.ImdbId, i.Title, Clean(i.Year), Clean(i.Type), true, wanted.Contains(i.ImdbId)))
            .Concat(onlineOnly.Select(s => new WishlistHit(s.ImdbId, titles.GetValueOrDefault(s.ImdbId) ?? s.Title, Clean(s.Year), Clean(s.Type),
                inLibrary.ContainsKey(s.ImdbId), wanted.Contains(s.ImdbId))))
            .ToList();
        return new WishlistSearch(hits, onlineError);
    }

    /// <summary>
    /// Puts the title on the wishlist, with details from the library, else OMDb (one call). Not added to the library.
    /// Returns the entry (the one already there, if it was).
    /// </summary>
    public async Task<WishlistItem> AddAsync(string imdbId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        if (await wishlist.GetAsync(imdbId, ct).ConfigureAwait(false) is { } already) return already;

        var fromLibrary = await library.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);
        var item = fromLibrary
                   ?? await omdb.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false)
                   ?? throw new InvalidOperationException($"No details found for {imdbId}.");
        // The library's own title; for one from OMDb, TMDb's English title where it knows it.
        var title = fromLibrary is null ? (await EnglishTitlesAsync([imdbId], ct).ConfigureAwait(false)).GetValueOrDefault(imdbId) : null;
        var entry = new WishlistItem
        {
            ImdbId = item.ImdbId,
            Title = title ?? item.Title,
            Year = Clean(item.Year),
            Type = Clean(item.Type),
            Genre = Clean(item.Genre),
            ImdbRating = Clean(item.ImdbRating),
            Poster = Clean(item.Poster),
            Added = DateTime.Now,
        };
        await wishlist.UpsertAsync(entry, ct).ConfigureAwait(false);
        logger.LogInformation("Added {ImdbId} {Title} to the wishlist", entry.ImdbId, entry.Title);
        return entry;
    }

    /// <summary>
    /// Puts a series' episodes (missing ones, from the series page) on the wishlist, each as "Series S06E03: Title" with
    /// the series' genre and poster. Episodes already on it are left as they are. Returns how many were added.
    /// </summary>
    public async Task<int> AddEpisodesAsync(string seriesImdbId, IEnumerable<SeriesEpisode> episodes, CancellationToken ct = default)
    {
        var series = await library.GetByImdbIdAsync(seriesImdbId, ct).ConfigureAwait(false)
                     ?? throw new InvalidOperationException($"{seriesImdbId} isn't in the library.");
        var already = await GetEpisodesAsync(seriesImdbId, ct).ConfigureAwait(false);
        var added = 0;
        foreach (var e in episodes.Where(e => !already.Contains((e.SeasonNumber, e.EpisodeNumber))))
        {
            await wishlist.UpsertAsync(new WishlistItem
            {
                ImdbId = WishlistItem.EpisodeId(series.ImdbId, e.SeasonNumber, e.EpisodeNumber),
                Title = $"{series.Title} S{e.SeasonNumber:00}E{e.EpisodeNumber:00}" + (string.IsNullOrWhiteSpace(e.Title) ? "" : $": {e.Title}"),
                Year = e.AirDate?.Year.ToString(CultureInfo.InvariantCulture),
                Type = "episode",
                Genre = Clean(series.Genre),
                Poster = Clean(series.Poster),
                Added = DateTime.Now,
            }, ct).ConfigureAwait(false);
            added++;
        }
        if (added > 0) logger.LogInformation("Added {Count} episode(s) of {ImdbId} {Title} to the wishlist", added, series.ImdbId, series.Title);
        return added;
    }

    /// <summary>The (season, episode) numbers of the series' episodes on the wishlist.</summary>
    public async Task<IReadOnlySet<(int Season, int Episode)>> GetEpisodesAsync(string seriesImdbId, CancellationToken ct = default)
    {
        var prefix = seriesImdbId + "_";
        return (await wishlist.GetAllAsync(ct).ConfigureAwait(false))
            .Where(w => w.ImdbId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(w => EpisodeNumbers().Match(w.ImdbId))
            .Where(m => m.Success)
            .Select(m => (int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture)))
            .ToHashSet();
    }

    /// <summary>
    /// Takes the series' episodes that are on disk now (the numbers given) off the wishlist; returns their labels
    /// ("S06E03"), so the page can say so.
    /// </summary>
    public async Task<IReadOnlyList<string>> RemoveEpisodesOnDiskAsync(
        string seriesImdbId, IEnumerable<(int Season, int Episode)> onDisk, CancellationToken ct = default)
    {
        var wanted = await GetEpisodesAsync(seriesImdbId, ct).ConfigureAwait(false);
        var got = onDisk.Where(wanted.Contains).Distinct().OrderBy(e => e.Season).ThenBy(e => e.Episode).ToList();
        foreach (var (season, episode) in got)
            await wishlist.RemoveAsync(WishlistItem.EpisodeId(seriesImdbId, season, episode), ct).ConfigureAwait(false);
        if (got.Count > 0) logger.LogInformation("Took {Count} episode(s) of {ImdbId} off the wishlist: on disk now", got.Count, seriesImdbId);
        return got.Select(e => $"S{e.Season:00}E{e.Episode:00}").ToList();
    }

    [GeneratedRegex(@"_S(\d+)E(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodeNumbers();

    public async Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default)
    {
        var removed = await wishlist.RemoveAsync(imdbId, ct).ConfigureAwait(false);
        if (removed) logger.LogInformation("Removed {ImdbId} from the wishlist", imdbId);
        return removed;
    }

    /// <summary>The wishlist, those to get first, then by title; whether the library has each title is looked up now.</summary>
    public async Task<IReadOnlyList<WishlistRow>> GetAsync(CancellationToken ct = default)
    {
        var items = await wishlist.GetAllAsync(ct).ConfigureAwait(false);
        var inLibrary = await library.GetByImdbIdsAsync(items.Select(i => i.ImdbId), ct).ConfigureAwait(false);
        var rows = new List<WishlistRow>();
        foreach (var item in items)
        {
            var have = inLibrary.ContainsKey(item.ImdbId);
            var file = have ? (await files.GetByImdbIdAsync(item.ImdbId, ct).ConfigureAwait(false))?.FullName : null;
            rows.Add(new WishlistRow(item, have, file));
        }
        return rows.OrderBy(r => r.InSilverScreen).ThenBy(r => r.Item.Title, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    /// <summary>
    /// The list as CSV for Excel (see <see cref="Csv"/>), with "In SilverScreen" as true/false to sort or filter on.
    /// </summary>
    public static string ToCsv(IEnumerable<WishlistRow> rows)
    {
        var csv = new StringBuilder();
        csv.AppendLine(Csv.Line("Title", "Year", "Type", "IMDb rating", "Genre", "IMDb id", "In SilverScreen", "File", "Added"));
        foreach (var r in rows)
            csv.AppendLine(Csv.Line(r.Item.Title, r.Item.Year, r.Item.Type, r.Item.ImdbRating, r.Item.Genre, r.Item.ImdbId,
                r.InSilverScreen ? "true" : "false", r.FilePath, r.Item.Added.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        return csv.ToString();
    }

    /// <summary>
    /// Reads a wishlist written by <see cref="ToCsv"/> (e.g. the web app's Download CSV) and puts its entries on this
    /// wishlist as they are: no lookups online. Entries whose IMDb id is on the wishlist already are left alone.
    /// Columns are found by their headings, so only "IMDb id" is required; the poster comes from the library when it has
    /// the title (or, for an episode, the series).
    /// </summary>
    /// <exception cref="FormatException">The file has no "IMDb id" column.</exception>
    public async Task<WishlistImport> ImportCsvAsync(string csv, CancellationToken ct = default)
    {
        var rows = Csv.Parse(csv);
        if (rows.Count == 0) throw new FormatException("The file is empty.");
        var header = rows[0].Select(h => h.Trim()).ToList();
        int Column(string name) => header.FindIndex(h => h.Equals(name, StringComparison.OrdinalIgnoreCase));
        var idColumn = Column("IMDb id");
        if (idColumn < 0) throw new FormatException("The file has no \"IMDb id\" column; is it a wishlist written by Kino?");
        int title = Column("Title"), year = Column("Year"), type = Column("Type"), rating = Column("IMDb rating"),
            genre = Column("Genre"), added = Column("Added");
        string? Value(string[] row, int column) => column >= 0 && column < row.Length ? Clean(row[column]) : null;

        var known = (await wishlist.GetAllAsync(ct).ConfigureAwait(false)).Select(w => w.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var entries = new List<WishlistItem>();
        int already = 0, invalid = 0;
        foreach (var row in rows.Skip(1))
        {
            if (WishlistId().Match(Value(row, idColumn) ?? "") is not { Success: true } match)
            {
                invalid++;
                continue;
            }
            var id = match.Groups["season"].Success
                ? WishlistItem.EpisodeId(match.Groups["title"].Value.ToLowerInvariant(),
                    int.Parse(match.Groups["season"].Value, CultureInfo.InvariantCulture), int.Parse(match.Groups["episode"].Value, CultureInfo.InvariantCulture))
                : match.Groups["title"].Value.ToLowerInvariant();
            if (!known.Add(id))
            {
                already++;
                continue;
            }
            entries.Add(new WishlistItem
            {
                ImdbId = id,
                Title = Value(row, title) ?? id,
                Year = Value(row, year),
                Type = Value(row, type),
                Genre = Value(row, genre),
                ImdbRating = Value(row, rating),
                Added = DateTime.TryParseExact(Value(row, added), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                    ? date : DateTime.Now,
            });
        }

        var inLibrary = await library.GetByImdbIdsAsync(entries.Select(e => e.TitleImdbId), ct).ConfigureAwait(false);
        foreach (var entry in entries)
        {
            entry.Poster = inLibrary.GetValueOrDefault(entry.TitleImdbId) is { } item ? Clean(item.Poster) : null;
            await wishlist.UpsertAsync(entry, ct).ConfigureAwait(false);
        }
        logger.LogInformation("Imported {Added} wishlist entries ({Already} already there, {Invalid} without an IMDb id)",
            entries.Count, already, invalid);
        return new WishlistImport(entries.Count, already, invalid);
    }

    /// <summary>"tt0133093", or an episode's "tt10986410_S04E09".</summary>
    [GeneratedRegex(@"^(?<title>tt\d{7,9})(?:_S(?<season>\d+)E(?<episode>\d+))?$", RegexOptions.IgnoreCase)]
    private static partial Regex WishlistId();

    /// <summary>
    /// TMDb's (English) titles for these IMDb ids, asked at the same time; an id TMDb doesn't know, or a failure (no key,
    /// no answer), is left out, so OMDb's title stays.
    /// </summary>
    private async Task<Dictionary<string, string>> EnglishTitlesAsync(IEnumerable<string> imdbIds, CancellationToken ct)
    {
        var found = await Task.WhenAll(imdbIds.Distinct(StringComparer.OrdinalIgnoreCase).Select(async id =>
        {
            try
            {
                return (Id: id, Title: await tmdb.FindTitleAsync(id, ct).ConfigureAwait(false));
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                logger.LogDebug(ex, "TMDb title for {ImdbId} not found", id);
                return (Id: id, Title: (string?)null);
            }
        })).ConfigureAwait(false);
        return found.Where(f => f.Title is not null).ToDictionary(f => f.Id, f => f.Title!, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Movies and series (and an unknown type); not OMDb's games or single episodes.</summary>
    private static bool WantedType(string? type) =>
        Clean(type) is not { } t || t.Equals("movie", StringComparison.OrdinalIgnoreCase) || t.Equals("series", StringComparison.OrdinalIgnoreCase);

    /// <summary>OMDb writes "N/A" for missing values.</summary>
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) || value == "N/A" ? null : value.Trim();
}
