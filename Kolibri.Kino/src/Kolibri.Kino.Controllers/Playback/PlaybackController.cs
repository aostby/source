using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Controllers.Playback;

/// <summary>
/// A library file the web page can send to the browser's player. <see cref="LocalPath"/> is where it is on this
/// machine (after <see cref="KinoOptions.PathMappings"/>). <see cref="LikelyPlays"/> is a guess from the
/// extension: browsers play MP4/WebM, sometimes MKV (H.264 inside), never AVI.
/// </summary>
public sealed record PlayableFile(string LocalPath, string FileName, string ContentType, bool LikelyPlays);

/// <summary>
/// An episode in a series folder. <see cref="Key"/> is its path inside the folder ("Season 1/Show S01E02.mkv"),
/// what the Play page puts in the address; <see cref="Season"/> is 0 for files whose name doesn't say.
/// </summary>
public sealed record PlayableEpisode(string Key, int Season, IReadOnlyList<int> Episodes, string FileName)
{
    /// <summary>"S01E02", "S01E01–E03", or the file name when the numbers are unknown.</summary>
    public string Label => Episodes.Count == 0 ? FileName
        : Episodes.Count == 1 ? $"S{Season:00}E{Episodes[0]:00}"
        : $"S{Season:00}E{Episodes[0]:00}–E{Episodes[^1]:00}";
}

/// <summary>
/// Play: the library's own file in the browser (for a series, one of the episodes in its folder), or the title
/// on Plex, which converts any format. Only files the library links to, or inside a linked series folder, can
/// be sent, never an arbitrary path.
/// </summary>
public sealed class PlaybackController(
    IFileItemRepository files,
    IEpisodeFileFinder episodes,
    IMediaServerLibrary plex,
    ISettingsStore settings,
    IOptions<KinoOptions> options,
    ILogger<PlaybackController> logger)
{
    /// <summary>
    /// The linked video file, or for a series the episode <paramref name="episodeKey"/> (from
    /// <see cref="GetEpisodesAsync"/>). Null when there's nothing here to play.
    /// </summary>
    public async Task<PlayableFile?> GetFileAsync(string imdbId, string? episodeKey = null, CancellationToken ct = default)
    {
        if (await LocalLinkAsync(imdbId, ct).ConfigureAwait(false) is not { } local) return null;

        var path = local;
        if (!string.IsNullOrEmpty(episodeKey))
        {
            if (Inside(local, episodeKey) is not { } episodePath) return null;
            path = episodePath;
        }
        if (!MovieFileRules.IsVideoFile(path) || !await Task.Run(() => File.Exists(path), ct).ConfigureAwait(false)) return null;

        var (type, likely) = TypeOf(path);
        return new PlayableFile(path, Path.GetFileName(path), type, likely);
    }

    /// <summary>
    /// The subtitles next to the file <see cref="GetFileAsync"/> gives, the standard one ("Movie.srt") first and
    /// shown by default. Empty when there's no file or no subtitle.
    /// </summary>
    public async Task<IReadOnlyList<PlayableSubtitle>> GetSubtitlesAsync(string imdbId, string? episodeKey = null, CancellationToken ct = default)
    {
        if (await GetFileAsync(imdbId, episodeKey, ct).ConfigureAwait(false) is not { } file) return [];
        var folder = Path.GetDirectoryName(file.LocalPath)!;
        var names = await Task.Run(() => Directory.EnumerateFiles(folder).ToList(), ct).ConfigureAwait(false);
        return PlaybackSubtitles.Find(file.LocalPath, names, settings.Current.SubtitleLanguages);
    }

    /// <summary>
    /// The subtitle <paramref name="fileName"/> as WebVTT, or null unless it is one of <see cref="GetSubtitlesAsync"/>'s,
    /// so the address can't reach other files.
    /// </summary>
    public async Task<string?> GetSubtitleVttAsync(string imdbId, string? episodeKey, string fileName, CancellationToken ct = default)
    {
        if (await GetFileAsync(imdbId, episodeKey, ct).ConfigureAwait(false) is not { } file) return null;
        var subtitles = await GetSubtitlesAsync(imdbId, episodeKey, ct).ConfigureAwait(false);
        if (subtitles.FirstOrDefault(s => s.FileName == fileName) is not { } subtitle) return null;

        var bytes = await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(file.LocalPath)!, subtitle.FileName), ct).ConfigureAwait(false);
        return PlaybackSubtitles.ToWebVtt(PlaybackSubtitles.ReadText(bytes));
    }

    /// <summary>A series' episodes, by season and episode, files without numbers last. Empty for a movie.</summary>
    public async Task<IReadOnlyList<PlayableEpisode>> GetEpisodesAsync(string imdbId, CancellationToken ct = default)
    {
        if (await LocalLinkAsync(imdbId, ct).ConfigureAwait(false) is not { } folder
            || !await Task.Run(() => Directory.Exists(folder), ct).ConfigureAwait(false))
            return [];

        var (found, unrecognised) = await episodes.FindAsync(folder, ct).ConfigureAwait(false);
        return found
            .Select(e => new PlayableEpisode(KeyOf(folder, e.Path), e.SeasonNumber, e.EpisodeNumbers, Path.GetFileName(e.Path)))
            .OrderBy(e => e.Season).ThenBy(e => e.Episodes.FirstOrDefault()).ThenBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
            .Concat(unrecognised
                .Select(p => new PlayableEpisode(KeyOf(folder, p), 0, [], Path.GetFileName(p)))
                .OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>Whether Play has anything of its own: the movie's file, or a series folder with episodes.</summary>
    public async Task<bool> HasLocalVideoAsync(string imdbId, CancellationToken ct = default) =>
        await GetFileAsync(imdbId, ct: ct).ConfigureAwait(false) is not null
        || (await GetEpisodesAsync(imdbId, ct).ConfigureAwait(false)).Count > 0;

    /// <summary>The title in Plex Web, or null when Plex isn't set up, doesn't have it or can't be reached.</summary>
    public async Task<string?> GetPlexPageAsync(string imdbId, CancellationToken ct = default)
    {
        if (!plex.IsConfigured) return null;
        try
        {
            return await plex.GetWebPageAsync(imdbId, ct).ConfigureAwait(false) is { } page ? options.Value.ForBrowser(page) : null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException { CancellationToken.IsCancellationRequested: false } or InvalidOperationException)
        {
            logger.LogWarning(ex, "No Plex page for {ImdbId}", imdbId);
            return null;
        }
    }

    /// <summary>
    /// Each episode's page in Plex Web, keyed by (season, episode); empty when Plex isn't set up, doesn't have the
    /// series or can't be reached. <see cref="PlexPageOf"/> finds an episode's.
    /// </summary>
    public async Task<IReadOnlyDictionary<(int Season, int Episode), string>> GetPlexEpisodePagesAsync(string seriesImdbId, CancellationToken ct = default)
    {
        if (!plex.IsConfigured) return new Dictionary<(int, int), string>();
        try
        {
            var pages = await plex.GetEpisodeWebPagesAsync(seriesImdbId, ct).ConfigureAwait(false);
            return pages.ToDictionary(p => p.Key, p => options.Value.ForBrowser(p.Value));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException { CancellationToken.IsCancellationRequested: false } or InvalidOperationException)
        {
            logger.LogWarning(ex, "No Plex episodes for {ImdbId}", seriesImdbId);
            return new Dictionary<(int, int), string>();
        }
    }

    /// <summary>The Plex page for the episode's first number (a double episode is one file), or null.</summary>
    public static string? PlexPageOf(PlayableEpisode episode, IReadOnlyDictionary<(int Season, int Episode), string> pages) =>
        episode.Episodes.Count > 0 && pages.TryGetValue((episode.Season, episode.Episodes[0]), out var page) ? page : null;

    internal static (string ContentType, bool LikelyPlays) TypeOf(string path) =>
        Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".mp4" or ".m4v" => ("video/mp4", true),
            ".webm" => ("video/webm", true),
            ".mkv" => ("video/x-matroska", false),
            ".avi" => ("video/x-msvideo", false),
            ".mov" => ("video/quicktime", false),
            _ => ("application/octet-stream", false),
        };

    /// <summary>
    /// <paramref name="key"/> resolved inside <paramref name="folder"/>, or null if it points outside it
    /// ("../", an absolute path): the address must not reach other files on the server.
    /// </summary>
    internal static string? Inside(string folder, string key)
    {
        var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder)) + Path.DirectorySeparatorChar;
        var relative = key.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        if (Path.IsPathRooted(relative)) return null;

        var full = Path.GetFullPath(Path.Combine(root, relative));
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return full.StartsWith(root, comparison) ? full : null;
    }

    private static string KeyOf(string folder, string path) => Path.GetRelativePath(folder, path).Replace('\\', '/');

    private async Task<string?> LocalLinkAsync(string imdbId, CancellationToken ct) =>
        await files.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { FullName.Length: > 0 } link
            ? options.Value.MapPath(link.FullName)
            : null;
}
