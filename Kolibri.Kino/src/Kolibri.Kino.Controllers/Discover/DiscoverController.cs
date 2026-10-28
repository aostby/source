using System.Globalization;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Trending;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Discover;

/// <summary>What Discover shows: a season, movies or TV shows, and the usual filters. Null filters mean any.</summary>
public sealed record DiscoverFilter(
    DiscoverSeason Season, bool Series = false, int? GenreId = null, int? YearFrom = null, int? YearTo = null,
    double? MinRating = null, DiscoverSort Sort = DiscoverSort.Popular)
{
    /// <summary>The cache's key for a page of it.</summary>
    public string KeyFor(int page) => string.Join("|", Season, Series ? "tv" : "movie", GenreId, YearFrom, YearTo,
        MinRating?.ToString(CultureInfo.InvariantCulture), Sort, page);
}

/// <param name="Entries">The page's titles, with what's in the library (as on Trending).</param>
/// <param name="Note">Said when the page is an older one because TMDb couldn't be asked.</param>
public sealed record DiscoverResult(
    DiscoverFilter Filter, IReadOnlyList<TrendingEntry> Entries, int Page, int TotalPages, int TotalResults, DateTime? FetchedUtc, string? Note)
{
    public bool HasMore => Page < TotalPages;
}

/// <summary>
/// "What's Up → Discover": TMDb titles for a season (any of its keywords), 20 at a time, with genre, year, rating
/// and sort. Each page is kept in the database for <see cref="MaxAge"/>; images go in the image cache like posters.
/// </summary>
public sealed class DiscoverController(
    IDiscoverSource tmdb,
    IDiscoverCache cache,
    IMovieRepository movies,
    LibraryFiles libraryFiles,
    IPosterProvider images,
    ILogger<DiscoverController> logger)
{
    public static readonly TimeSpan MaxAge = TimeSpan.FromHours(6);

    /// <summary>The titles on the pages given out lately, by (TV, TMDb id): where the web's poster address finds the image.</summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<(bool Series, int TmdbId), TrendingItem> Recent = new();

    /// <summary>The season Discover opens on: the one we're in.</summary>
    public static DiscoverSeason CurrentSeason(DateTime? today = null) => DiscoverSeasons.At(today ?? DateTime.Today);

    public Task<IReadOnlyList<DiscoverGenre>> GetGenresAsync(bool series, CancellationToken ct = default) => tmdb.GetGenresAsync(series, ct);

    /// <summary>A page (1 for a new search, then 2, 3 … for Show more), from the database when it's fresh.</summary>
    public async Task<DiscoverResult> GetAsync(DiscoverFilter filter, int page = 1, bool refresh = false, CancellationToken ct = default)
    {
        var key = filter.KeyFor(page);
        var cached = await cache.LoadAsync(key, ct).ConfigureAwait(false);
        DiscoverPage found;
        DateTime? fetched;
        string? note = null;

        if (cached is { } fresh && !refresh && DateTime.UtcNow - fresh.FetchedUtc < MaxAge)
        {
            (found, fetched) = (fresh.Page, fresh.FetchedUtc);
        }
        else
        {
            try
            {
                var season = DiscoverSeasons.Of(filter.Season);
                found = await tmdb.DiscoverAsync(new DiscoverRequest(season.KeywordIds, filter.Series, filter.GenreId, filter.YearFrom,
                    filter.YearTo, filter.MinRating, filter.Sort, page), ct).ConfigureAwait(false);
                fetched = DateTime.UtcNow;
                await cache.SaveAsync(key, found, fetched.Value, ct).ConfigureAwait(false);
                logger.LogInformation("Discover {Key} from TMDb: {Count} of {Total}", key, found.Items.Count, found.TotalResults);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
            {
                if (cached is not { } old) throw new InvalidOperationException($"TMDb's Discover couldn't be asked: {ex.Message}", ex);
                logger.LogWarning(ex, "Discover {Key} from TMDb failed; showing the page from {Fetched}", key, old.FetchedUtc);
                (found, fetched, note) = (old.Page, old.FetchedUtc, $"TMDb couldn't be reached ({ex.Message}); this is the list from {old.FetchedUtc.ToLocalTime():g}.");
            }
        }

        if (Recent.Count > 5000) Recent.Clear(); // a few hundred pages; they're fetched again from the database when asked
        foreach (var item in found.Items) Recent[(filter.Series, item.TmdbId)] = item;

        var local = await movies.GetByImdbIdsAsync(found.Items.Select(i => i.ImdbId).OfType<string>(), ct).ConfigureAwait(false);
        var withFile = await libraryFiles.WithFileAsync(local.Keys, ct).ConfigureAwait(false);
        var entries = found.Items.Select(i => i.ImdbId is { } id && local.TryGetValue(id, out var item)
                ? new TrendingEntry(i, true, Clean(item.ImdbRating), 0, withFile.Contains(id))
                : new TrendingEntry(i, false, null, 0))
            .ToList();
        return new DiscoverResult(filter, entries, found.Page, found.TotalPages, found.TotalResults, fetched, note);
    }

    /// <summary>The poster, from the image cache (by IMDb id, as other posters), downloaded from TMDb the first time.</summary>
    public async Task<byte[]?> GetImageAsync(bool series, TrendingItem item, CancellationToken ct = default)
    {
        try
        {
            var key = TrendingController.ImageKey(series ? TrendingKind.Series : TrendingKind.Movies, item);
            return await images.GetPosterAsync(new Item { ImdbId = key, Poster = item.ImageUrl }, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Poster for {Title} couldn't be fetched", item.Title);
            return null;
        }
    }

    /// <summary>The poster of a title on a page given out lately, by its TMDb id; null when it isn't one of them.</summary>
    public Task<byte[]?> GetImageAsync(bool series, int tmdbId, CancellationToken ct = default) =>
        Recent.TryGetValue((series, tmdbId), out var item) ? GetImageAsync(series, item, ct) : Task.FromResult<byte[]?>(null);

    private static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) || text == "N/A" ? null : text.Trim();
}
