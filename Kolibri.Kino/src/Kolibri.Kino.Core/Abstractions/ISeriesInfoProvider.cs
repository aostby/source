using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Season and episode details from an online source: TMDb (the show with up to 20 seasons per request, with plots
/// and stills) or, without a TMDb key, OMDb (one request per season, titles and ratings only).
/// </summary>
public interface ISeriesInfoProvider
{
    /// <summary>
    /// All seasons of the series (empty when no source knows it). <paramref name="totalSeasonsHint"/> is OMDb's
    /// TotalSeasons. <paramref name="tmdbId"/>, when known from an earlier fetch, saves TMDb's IMDb-id lookup.
    /// <paramref name="fromSeason"/> above 1 fetches only that season and later ones (and no specials), as SilverScreen
    /// fetched only the seasons it didn't have: the earlier ones are already stored and don't change.
    /// </summary>
    Task<SeriesFetchResult> GetSeasonsAsync(string seriesImdbId, int totalSeasonsHint, int? tmdbId = null, int fromSeason = 0, CancellationToken ct = default);
}
