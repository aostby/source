using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>The series on a media server (Plex) that got new episodes lately.</summary>
public interface IMediaServerSeries
{
    /// <summary>False when no server/token is configured.</summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Series with episodes added since <paramref name="since"/> (local time), newest first, each with those episodes.
    /// </summary>
    Task<IReadOnlyList<RecentSeriesUpdate>> GetRecentlyUpdatedSeriesAsync(DateTime since, CancellationToken ct = default);
}
