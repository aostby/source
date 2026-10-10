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

    /// <summary>
    /// Where the library's file paths are on this machine. The database stores Windows paths
    /// (\\GREENLANTERN\Movies\…); in the container the same share is mounted elsewhere (/media/Movies).
    /// Empty means paths are used as stored.
    /// </summary>
    public List<PathMapping> PathMappings { get; set; } = [];

    /// <summary>
    /// <paramref name="path"/> with the first matching <see cref="PathMappings"/> prefix replaced. Slashes and case
    /// don't matter for the match, and the prefix must end at a folder boundary (\\NAS\Movies doesn't match \\NAS\Movies2).
    /// </summary>
    public string MapPath(string path)
    {
        foreach (var mapping in PathMappings)
        {
            var from = PathMapping.Normalize(mapping.From).TrimEnd('/');
            if (from.Length == 0) continue;

            var normalized = PathMapping.Normalize(path);
            if (!normalized.StartsWith(from, StringComparison.OrdinalIgnoreCase)) continue;
            if (normalized.Length > from.Length && normalized[from.Length] != '/') continue;

            var rest = normalized[from.Length..].TrimStart('/');
            var to = mapping.To.TrimEnd('/', '\\');
            var separator = to.Contains('\\') ? '\\' : '/';
            return rest.Length == 0 ? to : to + separator + rest.Replace('/', separator);
        }
        return path;
    }

    /// <summary>
    /// Host names the browser must use instead of the ones Kino uses itself. Plex Web links point at the Plex
    /// server by its Windows name (http://HTPC:32400/…), which phones can't look up; HTPC → htpc.local works there.
    /// Empty means links are sent as they are.
    /// </summary>
    public List<HostMapping> BrowserHosts { get; set; } = [];

    /// <summary>
    /// <paramref name="url"/> with its host replaced by the first matching <see cref="BrowserHosts"/> entry
    /// (case doesn't matter); port, path and fragment are kept. Anything else is returned unchanged.
    /// </summary>
    public string ForBrowser(string url)
    {
        if (BrowserHosts.Count == 0 || !Uri.TryCreate(url, UriKind.Absolute, out var uri)) return url;

        foreach (var mapping in BrowserHosts)
        {
            if (string.IsNullOrWhiteSpace(mapping.To) || !string.Equals(uri.Host, mapping.From.Trim(), StringComparison.OrdinalIgnoreCase)) continue;

            // Replace only the host in the original text: UriBuilder would re-escape Plex Web's #!/server/… fragment.
            var start = url.IndexOf("://", StringComparison.Ordinal) + 3;
            var hostAt = url.IndexOf(uri.Host, start, StringComparison.OrdinalIgnoreCase);
            return hostAt < 0 ? url : url[..hostAt] + mapping.To.Trim() + url[(hostAt + uri.Host.Length)..];
        }
        return url;
    }

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

/// <summary>One <see cref="KinoOptions.PathMappings"/> entry: a stored path prefix and where it is here.</summary>
public sealed class PathMapping
{
    /// <summary>As stored in the database, e.g. \\GREENLANTERN\Movies (or //GREENLANTERN/Movies, easier in YAML).</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>The same folder on this machine, e.g. /media/Movies.</summary>
    public string To { get; set; } = string.Empty;

    internal static string Normalize(string path) => path.Trim().Replace('\\', '/');
}

/// <summary>One <see cref="KinoOptions.BrowserHosts"/> entry: the host name Kino uses and the one the browser should.</summary>
public sealed class HostMapping
{
    /// <summary>As in the links Kino makes, e.g. HTPC.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>What browsers on the network can look up, e.g. htpc.local or 192.168.5.16.</summary>
    public string To { get; set; } = string.Empty;
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
