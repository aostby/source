using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Core.Models;

/// <summary>
/// One line of Kino's log: a status message from a window, or a message from a controller.
/// <paramref name="Source"/> is the window or class (e.g. LocalMoviesForm); <paramref name="Error"/> the exception, if any.
/// </summary>
public sealed record LogEntry(DateTime Time, LogLevel Level, string Source, string Message, string? Error = null);
