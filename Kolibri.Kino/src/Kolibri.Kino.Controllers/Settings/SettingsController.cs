using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Controllers.Settings;

/// <summary>
/// Behind the Settings window (port of SilverScreen's "Innstillinger" property grid).
/// </summary>
public sealed class SettingsController(ISettingsStore settings, IConnectionTester tester, ILogger<SettingsController> logger)
{
    public const string DefaultKeysMessage =
        "You're using the shared default OMDb/TMDb keys. They work, but everyone using them shares OMDb's daily limit " +
        "(1,000 requests), so lookups can stop early. Free keys of your own: omdbapi.com/apikey.aspx and themoviedb.org/settings/api.";

    /// <summary>True while the OMDb or TMDb key is one of the shared defaults; the UI then asks for real keys.</summary>
    public bool UsesDefaultKeys => settings.Current.UsesDefaultKeys();

    /// <summary>A copy to edit; nothing changes until <see cref="SaveAsync"/>.</summary>
    public UserSettings GetEditableCopy() => settings.Current.Clone();

    /// <summary>Tries the OMDb key, TMDb key and Plex token in <paramref name="edited"/> without saving them.</summary>
    public Task<IReadOnlyList<ConnectionCheck>> TestAsync(UserSettings edited, CancellationToken ct = default) =>
        tester.TestAsync(edited, ct);

    /// <summary>
    /// The database file a "LiteDB file" entry means: a folder (existing, or a path without extension) gets the file
    /// name of <paramref name="current"/> inside it, e.g. E:\RELEASE\SilverScreenDB → E:\RELEASE\SilverScreenDB\SilverScreen.db.
    /// Null when the entry is empty or the same file as <paramref name="current"/>.
    /// </summary>
    public static DatabaseChoice? CheckDatabasePath(string? entry, string? current)
    {
        if (string.IsNullOrWhiteSpace(entry)) return null;
        var path = Path.GetFullPath(entry.Trim().Trim('"'));
        if (Directory.Exists(path) || !Path.HasExtension(path))
            path = Path.Combine(path, Path.GetFileName(current) is { Length: > 0 } name ? name : DefaultDatabaseFileName);
        if (!string.IsNullOrWhiteSpace(current) && string.Equals(path, Path.GetFullPath(current), StringComparison.OrdinalIgnoreCase))
            return null;
        return new DatabaseChoice(path, Directory.Exists(Path.GetDirectoryName(path)), File.Exists(path));
    }

    public const string DefaultDatabaseFileName = "SilverScreen.db";

    /// <summary>Saves to the database; OMDb, TMDb and Plex use the new values from the next call on.</summary>
    public async Task SaveAsync(UserSettings edited, CancellationToken ct = default)
    {
        await settings.SaveAsync(edited, ct).ConfigureAwait(false);
        logger.LogInformation("Settings saved for {User}", edited.UserName);
    }
}

/// <summary>A new database file chosen in Settings: whether its folder exists (required) and the file (else a new, empty one).</summary>
public sealed record DatabaseChoice(string Path, bool FolderExists, bool FileExists);
