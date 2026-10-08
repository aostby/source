using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Calls to the online services per day, kept in the database for the usage report.</summary>
public interface IApiUsageRepository
{
    /// <summary>Adds <paramref name="entries"/> to the stored counts of their days.</summary>
    Task AddAsync(IReadOnlyCollection<ApiUsageEntry> entries, CancellationToken ct = default);

    /// <summary>The calls on <paramref name="date"/>, or null if there were none.</summary>
    Task<ApiUsageDay?> GetDayAsync(DateOnly date, CancellationToken ct = default);

    /// <summary>The days with calls from <paramref name="from"/> to <paramref name="to"/> (inclusive; null = no limit), oldest first.</summary>
    Task<IReadOnlyList<ApiUsageDay>> GetDaysAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default);
}

/// <summary>
/// Counts Kino's calls to OMDb, TMDb, SubDL and Plex as they happen and saves them now and then
/// (and when the program closes) through <see cref="IApiUsageRepository"/>.
/// </summary>
public interface IApiUsageTracker
{
    /// <summary>Counts one call to <paramref name="service"/> today (or on <paramref name="date"/>).</summary>
    void Record(string service, bool failed, DateOnly? date = null);

    /// <summary>Saves the calls counted since the last save, e.g. before a report is read.</summary>
    Task FlushAsync(CancellationToken ct = default);
}
