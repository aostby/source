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
    /// The web service on problembar.net that keeps a copy of the wishlist and your ratings. Without a URL and a key,
    /// nothing is sent.
    /// </summary>
    public WebSyncOptions WebSync { get; set; } = new();

    /// <summary>
    /// How File → Prowlarr picks releases (size, quality, seeders). Prowlarr's address and API key are user settings
    /// (Settings, category Prowlarr), as Plex's are.
    /// </summary>
    public ProwlarrOptions Prowlarr { get; set; } = new();

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

    /// <summary>
    /// KMDb's address for this program, over the user's "KMDb URL" setting: in Docker the web app reaches KMDb by its
    /// service name (http://kmdb:8080), not the address the PC uses. Empty: the setting decides.
    /// </summary>
    public string KmdbUrl { get; set; } = string.Empty;

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

/// <summary>
/// <see cref="KinoOptions.WebSync"/>: post.php in uranus/apps/KolibriKino, which stores each file it gets (named as the
/// downloads are, "Wishlist - 2026-11-01.csv") for get.php to hand out.
/// </summary>
public sealed class WebSyncOptions
{
    /// <summary>post.php's address, e.g. https://problembar.net/uranus/apps/KolibriKino/post.php. Empty turns it off.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>The key in kino_config.php on the server, sent as X-Api-Key. Empty turns it off.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>How long to wait after a change before sending, so a run of changes goes as one file.</summary>
    public int DelaySeconds { get; set; } = 10;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(ApiKey);
}

/// <summary>
/// <see cref="KinoOptions.Prowlarr"/>: which of Prowlarr's search results are shown for a wanted title, and when getting
/// one asks first. Prowlarr's indexers here can't search by IMDb id, so the search is "title year" and the results are
/// checked against the title and year. The lists are null when not set (the configuration binder adds to a list that
/// has items already), and the Effective… properties give the defaults then.
/// </summary>
public sealed class ProwlarrOptions
{
    /// <summary>Newznab categories searched for movies. Null means 2000 (Movies, all of them).</summary>
    public List<int>? MovieCategories { get; set; }

    /// <summary>Newznab categories searched for series and episodes. Null means 5000 (TV, all of it).</summary>
    public List<int>? SeriesCategories { get; set; }

    /// <summary>
    /// Qualities wanted, as in release names: 2160p, 1080p, 720p, SD, or Unknown for a name without one.
    /// Null means 1080p and 720p.
    /// </summary>
    public List<string>? Qualities { get; set; }

    /// <summary>Releases with fewer seeders are left out.</summary>
    public int MinSeeders { get; set; } = 5;

    /// <summary>The best this many (most seeders first) are shown for each title.</summary>
    public int TopResults { get; set; } = 8;

    /// <summary>Getting a release bigger than this many GB asks first.</summary>
    public double ConfirmAboveGb { get; set; } = 6;

    /// <summary>
    /// Words in a release name (whole words, case doesn't matter) that leave it out: camera copies and the like.
    /// Null means <see cref="DefaultExcludedWords"/>.
    /// </summary>
    public List<string>? ExcludedWords { get; set; }

    /// <summary>Where a .torrent file is saved before it's opened. Empty means the Downloads folder.</summary>
    public string TorrentFolder { get; set; } = string.Empty;

    public static readonly IReadOnlyList<string> DefaultExcludedWords =
        ["CAM", "HDCAM", "TS", "HDTS", "TELESYNC", "TC", "TELECINE", "SCR", "SCREENER", "DVDSCR", "WORKPRINT"];

    public IReadOnlyList<int> EffectiveMovieCategories => MovieCategories is { Count: > 0 } ? MovieCategories : [2000];
    public IReadOnlyList<int> EffectiveSeriesCategories => SeriesCategories is { Count: > 0 } ? SeriesCategories : [5000];
    public IReadOnlyList<string> EffectiveQualities => Qualities is { Count: > 0 } ? Qualities : ["1080p", "720p"];
    public IReadOnlyList<string> EffectiveExcludedWords => ExcludedWords ?? DefaultExcludedWords;
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
