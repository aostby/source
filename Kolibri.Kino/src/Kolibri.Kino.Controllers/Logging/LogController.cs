using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Controllers.Logging;

/// <summary>Help → Log: what Kino's windows and controllers have reported, kept for <see cref="RetentionDays"/> days.</summary>
public sealed class LogController(ILogStore store, IOptions<KinoOptions> options)
{
    /// <summary>The most entries shown at once; narrow the filter to see older ones.</summary>
    public const int MaxEntries = 5000;

    public int RetentionDays => Math.Max(1, options.Value.LogRetentionDays);

    public string LogFile => options.Value.ResolveLogDbPath();

    /// <summary>
    /// Entries from the last <paramref name="days"/> days (null = all kept), at <paramref name="minLevel"/> or above,
    /// containing <paramref name="text"/>; newest first.
    /// </summary>
    public Task<IReadOnlyList<LogEntry>> GetAsync(int? days, LogLevel minLevel, string? text, CancellationToken ct = default) =>
        store.GetAsync(days is { } d ? DateTime.Today.AddDays(1 - d) : null, minLevel, text, MaxEntries, ct);
}
