namespace Kolibri.Kino.Core;

/// <summary>
/// Settings bound from the "Kino" section of appsettings.json. API keys and the Plex token are not here:
/// they live in the database's user settings (see <see cref="Models.UserSettings"/>), edited in the Settings window.
/// </summary>
public sealed class KinoOptions
{
    public const string SectionName = "Kino";

    /// <summary>Full path to the LiteDB file. Compatible with the SilverScreen database.</summary>
    public string LiteDbPath { get; set; } = string.Empty;

    /// <summary>Image cache file. Empty means next to <see cref="LiteDbPath"/> with extension .imgdb, like SilverScreen.</summary>
    public string ImageDbPath { get; set; } = string.Empty;

    /// <summary>The log (what Kino's windows and controllers reported). Empty means next to <see cref="LiteDbPath"/> with extension .logdb.</summary>
    public string LogDbPath { get; set; } = string.Empty;

    /// <summary>Log entries older than this many days are deleted.</summary>
    public int LogRetentionDays { get; set; } = 30;

    /// <summary>
    /// Whose UserSettings document to use (its _id). Empty means the Windows user running the app, as SilverScreen does.
    /// Set it where the user name differs, e.g. in a container, to share the settings saved on Windows.
    /// </summary>
    public string SettingsUser { get; set; } = string.Empty;

    public CleanupOptions Cleanup { get; set; } = new();

    public string ResolveSettingsUser() => string.IsNullOrWhiteSpace(SettingsUser) ? Environment.UserName : SettingsUser.Trim();

    public string ResolveLogDbPath() =>
        string.IsNullOrWhiteSpace(LogDbPath) && !string.IsNullOrWhiteSpace(LiteDbPath)
            ? Path.ChangeExtension(LiteDbPath, ".logdb")
            : LogDbPath;

    public string ResolveImageDbPath() =>
        string.IsNullOrWhiteSpace(ImageDbPath) && !string.IsNullOrWhiteSpace(LiteDbPath)
            ? Path.ChangeExtension(LiteDbPath, ".imgdb")
            : ImageDbPath;
}

public enum CleanupAfterScan
{
    /// <summary>List the files and ask before deleting.</summary>
    Ask,

    /// <summary>Delete without asking, as SilverScreen did.</summary>
    Always,

    Never,
}

/// <summary>
/// Removing leftover files (.nfo, samples' .txt, foreign subtitles …) from movie folders after a scan.
/// </summary>
public sealed class CleanupOptions
{
    public CleanupAfterScan AfterScan { get; set; } = CleanupAfterScan.Ask;

    public bool DeleteEmptyFolders { get; set; } = true;

    /// <summary>
    /// File name endings to delete. ".nfo" matches the extension; "rus.srt" matches "Movie.rus.srt",
    /// "Movie_rus.srt" and the hearing-impaired "Movie.rus.HI.srt", but not "Walrus.srt".
    /// Null (not set) means <see cref="CleanupRules.DefaultPatterns"/>.
    /// </summary>
    public List<string>? FilePatterns { get; set; }

    public IReadOnlyList<string> EffectivePatterns => FilePatterns is { Count: > 0 } ? FilePatterns : CleanupRules.DefaultPatterns;
}
