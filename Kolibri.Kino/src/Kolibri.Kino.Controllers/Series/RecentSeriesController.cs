using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Series;

/// <summary>
/// What Plex got lately, matched to the series in the library: <see cref="InLibrary"/> by the series' IMDb id
/// (else its title), and the rest in <see cref="NotInLibrary"/>.
/// </summary>
public sealed record RecentSeriesReport(
    int Days,
    IReadOnlyDictionary<string, RecentSeriesUpdate> InLibrary,
    IReadOnlyList<RecentSeriesUpdate> NotInLibrary);

/// <summary>Local series → Recently updated: the series that got new episodes on Plex lately.</summary>
public sealed class RecentSeriesController(IMediaServerSeries server)
{
    public const int DefaultDays = 30;

    /// <summary>Plex's server name and token are set in Settings; else the button is hidden.</summary>
    public bool IsAvailable => server.IsConfigured;

    /// <summary>
    /// Series with episodes added on Plex in the last <paramref name="days"/> days, keyed by the IMDb id of the
    /// matching series in <paramref name="library"/>.
    /// </summary>
    public async Task<RecentSeriesReport> GetAsync(IReadOnlyList<LocalSeries> library, int days = DefaultDays, CancellationToken ct = default)
    {
        var updates = await server.GetRecentlyUpdatedSeriesAsync(DateTime.Now.AddDays(-days), ct).ConfigureAwait(false);

        var byId = library.GroupBy(s => s.Item.ImdbId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        var byTitle = library.GroupBy(s => s.Item.Title.Trim(), StringComparer.CurrentCultureIgnoreCase)
            .Where(g => g.Count() == 1)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.CurrentCultureIgnoreCase);

        var inLibrary = new Dictionary<string, RecentSeriesUpdate>(StringComparer.OrdinalIgnoreCase);
        var notInLibrary = new List<RecentSeriesUpdate>();
        foreach (var update in updates)
        {
            var series = update.ImdbId is { } id && byId.TryGetValue(id, out var s) ? s
                : byTitle.GetValueOrDefault(update.Title.Trim());
            if (series is null) notInLibrary.Add(update);
            else inLibrary.TryAdd(series.Item.ImdbId, update);
        }
        return new RecentSeriesReport(days, inLibrary, notInLibrary);
    }
}
