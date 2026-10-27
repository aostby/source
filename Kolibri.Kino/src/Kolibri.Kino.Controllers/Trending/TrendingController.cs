using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Trending;

/// <param name="Item">The title or person, as TMDb lists it.</param>
/// <param name="InLibrary">A movie or series that's in SilverScreen (the library), with or without a file.</param>
/// <param name="HasFile">In the library and linked to a file (a series: its folder) that's there. Only then is it shown
/// as In SilverScreen in green; in the library without one (e.g. imported from OMDb), it's salmon.</param>
/// <param name="ImdbRating">Its IMDb rating from the library, when it's there.</param>
/// <param name="LibraryTitles">A person: how many titles in the library they're in (as actor or director).</param>
public sealed record TrendingEntry(TrendingItem Item, bool InLibrary, string? ImdbRating, int LibraryTitles, bool HasFile = false);

/// <param name="Note">Said when the list is an older one because TMDb couldn't be asked (or there's no TMDb key).</param>
public sealed record TrendingResult(TrendingKind Kind, IReadOnlyList<TrendingEntry> Entries, DateTime? FetchedUtc, string? Note);

/// <summary>
/// "What's Up → Trending": TMDb's trending movies, TV shows and people this week, with what's in the library. A list is
/// kept in the database and fetched again after <see cref="MaxAge"/>; images go in the image cache like posters.
/// </summary>
public sealed class TrendingController(
    ITrendingSource tmdb,
    ITrendingCache cache,
    IMovieRepository movies,
    LibraryFiles libraryFiles,
    IPosterProvider images,
    ILogger<TrendingController> logger)
{
    /// <summary>TMDb updates the lists through the day; a few hours old is new enough.</summary>
    public static readonly TimeSpan MaxAge = TimeSpan.FromHours(6);

    public async Task<TrendingResult> GetAsync(TrendingKind kind, bool refresh = false, CancellationToken ct = default)
    {
        var cached = await cache.LoadAsync(kind, ct).ConfigureAwait(false);
        IReadOnlyList<TrendingItem> items;
        DateTime? fetched;
        string? note = null;

        if (cached is { } fresh && !refresh && DateTime.UtcNow - fresh.FetchedUtc < MaxAge)
        {
            (items, fetched) = (fresh.Items, fresh.FetchedUtc);
        }
        else
        {
            try
            {
                var known = (cached?.Items ?? []).Where(i => i.ImdbId is not null)
                    .GroupBy(i => i.TmdbId).ToDictionary(g => g.Key, g => g.First().ImdbId!);
                items = await tmdb.GetTrendingAsync(kind, known, ct).ConfigureAwait(false);
                if (items.Count == 0 && cached is not null)
                {
                    (items, fetched, note) = (cached.Value.Items, cached.Value.FetchedUtc, "TMDb gave no list (is there a TMDb key in Settings?); this is the last one.");
                }
                else
                {
                    fetched = DateTime.UtcNow;
                    await cache.SaveAsync(kind, items, fetched.Value, ct).ConfigureAwait(false);
                    logger.LogInformation("Trending {Kind} from TMDb: {Count}", kind, items.Count);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                if (cached is not { } old) throw new InvalidOperationException($"TMDb's trending list couldn't be fetched: {ex.Message}", ex);
                logger.LogWarning(ex, "Trending {Kind} from TMDb failed; showing the list from {Fetched}", kind, old.FetchedUtc);
                (items, fetched, note) = (old.Items, old.FetchedUtc, $"TMDb couldn't be reached ({ex.Message}); this is the list from {old.FetchedUtc.ToLocalTime():g}.");
            }
        }

        return new TrendingResult(kind, await WithLibraryAsync(kind, items, ct).ConfigureAwait(false), fetched, note);
    }

    /// <summary>Movies and series: whether the library has them. People: how many library titles they're in.</summary>
    private async Task<IReadOnlyList<TrendingEntry>> WithLibraryAsync(TrendingKind kind, IReadOnlyList<TrendingItem> items, CancellationToken ct)
    {
        if (kind == TrendingKind.People)
        {
            var all = await movies.GetAllAsync(ct: ct).ConfigureAwait(false);
            return items.Select(p => new TrendingEntry(p, false, null, all.Count(new LibraryFilter(Person: p.Title).Matches))).ToList();
        }

        var local = await movies.GetByImdbIdsAsync(items.Select(i => i.ImdbId).OfType<string>(), ct).ConfigureAwait(false);
        var withFile = await libraryFiles.WithFileAsync(local.Keys, ct).ConfigureAwait(false);
        return items.Select(i => i.ImdbId is { } id && local.TryGetValue(id, out var item)
                ? new TrendingEntry(i, true, Clean(item.ImdbRating), 0, withFile.Contains(id))
                : new TrendingEntry(i, false, null, 0))
            .ToList();
    }

    /// <summary>Library titles with the person as actor or director, best rated first (a person's card opens these).</summary>
    public async Task<IReadOnlyList<Item>> GetLibraryTitlesWithAsync(string name, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var filter = new LibraryFilter(Person: name);
        return (await movies.GetAllAsync(ct: ct).ConfigureAwait(false))
            .Where(filter.Matches)
            .OrderByDescending(i => Watchlists.WatchlistController.Rating(i.ImdbRating))
            .ThenBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// The poster or photo, from the image cache (by IMDb id, as other posters; a person by "tmdb-person-{id}"),
    /// downloaded from TMDb the first time. Null when there's none or it can't be fetched now.
    /// </summary>
    public async Task<byte[]?> GetImageAsync(TrendingKind kind, TrendingItem item, CancellationToken ct = default)
    {
        try
        {
            return await images.GetPosterAsync(new Item { ImdbId = ImageKey(kind, item), Poster = item.ImageUrl }, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Image for {Title} couldn't be fetched", item.Title);
            return null;
        }
    }

    /// <summary>The image of an entry on the last fetched list, by its TMDb id; null when it isn't on it.</summary>
    public async Task<byte[]?> GetImageAsync(TrendingKind kind, int tmdbId, CancellationToken ct = default) =>
        (await cache.LoadAsync(kind, ct).ConfigureAwait(false))?.Items.FirstOrDefault(i => i.TmdbId == tmdbId) is { } item
            ? await GetImageAsync(kind, item, ct).ConfigureAwait(false)
            : null;

    /// <summary>The image cache's key: a title's IMDb id (shared with the library's posters), else its TMDb id.</summary>
    public static string ImageKey(TrendingKind kind, TrendingItem item) => kind switch
    {
        TrendingKind.People => $"tmdb-person-{item.TmdbId}",
        _ => item.ImdbId ?? $"tmdb-{(kind == TrendingKind.Movies ? "movie" : "tv")}-{item.TmdbId}",
    };

    /// <summary>TMDb's page for the entry, for titles without an IMDb id and for people.</summary>
    public static string TmdbPage(TrendingKind kind, int tmdbId) =>
        $"https://www.themoviedb.org/{kind switch { TrendingKind.Movies => "movie", TrendingKind.Series => "tv", _ => "person" }}/{tmdbId}";

    public static string Label(TrendingKind kind) => kind switch
    {
        TrendingKind.Movies => "Trending movies",
        TrendingKind.Series => "Trending TV shows",
        _ => "Trending people",
    };

    private static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) || text == "N/A" ? null : text.Trim();
}
