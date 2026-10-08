using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kolibri.Kino.WinForms;

/// <summary>
/// The log for the windows' status messages (forms are made with ActivatorUtilities, so they get it from here).
/// Set at startup; until then nothing is logged.
/// </summary>
internal static partial class KinoLog
{
    public static ILoggerFactory Factory { get; set; } = NullLoggerFactory.Instance;

    /// <summary>
    /// How a status message is logged: "Error…" as an error; work in progress ("Loading library…",
    /// "Scanning 3 of 120: …", "Downloading subtitle 2 of 9") as Debug, so only shown with Logging:LogLevel:Default
    /// set to Debug; everything else (results such as "Saved …", "Removed …", "4,106 item(s) in library.") as Information.
    /// </summary>
    public static LogLevel LevelOf(string status) =>
        status.StartsWith("Error", StringComparison.OrdinalIgnoreCase) ? LogLevel.Error
        : status.EndsWith('…') || InProgress().IsMatch(status) ? LogLevel.Debug
        : LogLevel.Information;

    /// <summary>Starts with a verb in -ing ("Scanning", "Searching"), but not "Nothing".</summary>
    [GeneratedRegex(@"^(?!Nothing\b)[A-Z][a-z]+ing\b")]
    private static partial Regex InProgress();
}
