using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Season and episode details from an online source: TMDb (one request per season, with plots and stills)
/// or, without a TMDb key, OMDb (one request per season, titles and ratings only).
/// </summary>
public interface ISeriesInfoProvider
{
    /// <summary>All seasons of the series. <paramref name="totalSeasonsHint"/> is OMDb's TotalSeasons, used when TMDb can't be used.</summary>
    Task<IReadOnlyList<SeriesSeason>> GetSeasonsAsync(string seriesImdbId, int totalSeasonsHint, CancellationToken ct = default);
}
