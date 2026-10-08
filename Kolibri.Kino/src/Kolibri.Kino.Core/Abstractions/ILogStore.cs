using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Kino's log, kept in its own database for <see cref="KinoOptions.LogRetentionDays"/> days.</summary>
public interface ILogStore
{
    /// <summary>
    /// Entries from <paramref name="from"/> on (local time; null = all), at <paramref name="minLevel"/> or above,
    /// whose message, source or error contains <paramref name="text"/>; newest first, at most <paramref name="max"/>.
    /// </summary>
    Task<IReadOnlyList<LogEntry>> GetAsync(DateTime? from, LogLevel minLevel, string? text, int max, CancellationToken ct = default);

    /// <summary>Saves the entries written since the last save (they are saved every few seconds anyway).</summary>
    Task FlushAsync(CancellationToken ct = default);
}
