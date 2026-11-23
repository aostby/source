using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kolibri.Kino.Controllers.Subtitles;

/// <summary>What a subtitle download did: SubDL's hits, the zips saved, the subtitle files unpacked, and failures.</summary>
/// <param name="Copied">The subtitle copied next to the movie as "&lt;movie&gt;.srt" because it fits the release, if any.</param>
/// <param name="Stopped">
/// Why the downloads stopped before the end (SubDL refused: too many requests, its daily limit, the key), or null. The
/// files saved until then are kept.
/// </param>
public sealed record SubtitleDownload(string Languages, int Found, int Saved, int Files, string? Folder, IReadOnlyList<string> Failed,
    string? Copied = null, string? Stopped = null);

/// <summary>One episode in a season download: its number and file.</summary>
public sealed record EpisodeVideo(int Episode, string VideoPath);

/// <summary>
/// A season download: each episode's result, in episode order, the episodes skipped because they had a .srt already,
/// and why it stopped early (SubDL refused more), if it did.
/// </summary>
public sealed record SeasonSubtitleDownload(IReadOnlyList<(EpisodeVideo Episode, SubtitleDownload Result)> Episodes,
    IReadOnlyList<EpisodeVideo> HadSubtitles, string? Stopped = null);

/// <summary>
/// The subtitle button in the details window (port of SilverScreen's DetailsFormItem): shows whether a movie file
/// has subtitles, and downloads them from SubDL into a "Subs" folder next to it. Failed downloads are logged as warnings.
/// </summary>
public sealed class SubtitleController(ISubtitleProvider provider, ISubtitleFolder folder, ISettingsStore settings,
    ILogger<SubtitleController>? logger = null)
{
    private readonly ILogger _log = logger ?? NullLogger<SubtitleController>.Instance;

    public bool HasKey => !string.IsNullOrWhiteSpace(settings.Current.SUBDLkey);

    public string Languages => settings.Current.SubtitleLanguages is { Length: > 0 } languages
        ? languages.Trim()
        : UserSettings.DefaultSubtitleLanguages;

    public SubtitleStatus GetStatus(string? videoPath) => folder.GetStatus(videoPath);

    /// <summary>
    /// Copies the subtitle in Subs that fits this release next to the movie as "&lt;movie&gt;.srt" (the button turns green).
    /// Null when none fits.
    /// </summary>
    public string? UseMatchFromSubs(string videoPath) => folder.CopyMatchFromSubs(videoPath);

    /// <summary>
    /// The downloaded .srt files, largest first: when none is for this release, the largest is often the right one
    /// (complete, with the most lines), so the UI offers it.
    /// </summary>
    public IReadOnlyList<SubtitleFile> GetSubsFiles(string videoPath) => folder.GetSubsFiles(videoPath);

    /// <summary>Copies the chosen subtitle next to the movie as "&lt;movie&gt;.srt" (the button turns green).</summary>
    public string UseSubtitle(string videoPath, string subtitlePath) => folder.CopyNextToMovie(videoPath, subtitlePath);

    /// <summary>
    /// Every subtitle SubDL has for <paramref name="imdbId"/> in <see cref="Languages"/>, unpacked into the Subs folder
    /// next to <paramref name="videoPath"/>, as SilverScreen did. One failed download doesn't stop the rest; SubDL refusing
    /// more (too many requests, its limit) does, with <see cref="SubtitleDownload.Stopped"/> saying why.
    /// </summary>
    public Task<SubtitleDownload> DownloadAsync(string imdbId, string videoPath, IProgress<string>? progress = null,
        CancellationToken ct = default) =>
        DownloadAsync((languages, token) => provider.SearchAsync(imdbId, languages, token), videoPath, progress, ct);

    /// <summary>
    /// As <see cref="DownloadAsync(string, string, IProgress{string}?, CancellationToken)"/> for one episode of the series
    /// <paramref name="seriesImdbId"/>: into "Subs\&lt;episode file name&gt;" next to <paramref name="videoPath"/>, keeping only
    /// that episode's files from season packs.
    /// </summary>
    public Task<SubtitleDownload> DownloadEpisodeAsync(string seriesImdbId, int season, int episode, string videoPath,
        IProgress<string>? progress = null, CancellationToken ct = default) =>
        DownloadAsync((languages, token) => provider.SearchEpisodeAsync(seriesImdbId, season, episode, languages, token),
            videoPath, progress, ct);

    /// <summary>
    /// <see cref="DownloadEpisodeAsync"/> for each of <paramref name="episodes"/> (one season) without a .srt next to it; the
    /// ones with one are skipped. A zip found for several episodes (a season pack) is downloaded once. One failed download
    /// doesn't stop the rest; SubDL refusing more does (<see cref="SeasonSubtitleDownload.Stopped"/>), keeping what was saved.
    /// </summary>
    public async Task<SeasonSubtitleDownload> DownloadSeasonAsync(string seriesImdbId, int season, IReadOnlyList<EpisodeVideo> episodes,
        IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var zips = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        var results = new List<(EpisodeVideo, SubtitleDownload)>();
        var had = new List<EpisodeVideo>();
        var todo = episodes.OrderBy(e => e.Episode).ToList();
        foreach (var (episode, index) in todo.Select((e, i) => (e, i + 1)))
        {
            if (folder.GetStatus(episode.VideoPath).State == SubtitleState.SrtFile)
            {
                had.Add(episode);
                continue;
            }
            var code = FormattableString.Invariant($"S{season:00}E{episode.Episode:00}");
            var episodeProgress = progress is null ? null : new Prefixed(progress, $"{code} ({index} of {todo.Count}): ");
            var result = await DownloadAsync((languages, token) => provider.SearchEpisodeAsync(seriesImdbId, season, episode.Episode, languages, token),
                episode.VideoPath, episodeProgress, ct, zips).ConfigureAwait(false);
            results.Add((episode, result));
            if (result.Stopped is { } stopped) return new SeasonSubtitleDownload(results, had, $"{code}: {stopped}");
        }
        return new SeasonSubtitleDownload(results, had);
    }

    /// <summary>Progress with the episode in front ("S01E05 (5 of 8): Downloading …").</summary>
    private sealed class Prefixed(IProgress<string> inner, string prefix) : IProgress<string>
    {
        public void Report(string value) => inner.Report(prefix + value);
    }

    /// <param name="zips">Zips already downloaded in this run, by URL (a season download); null for one title.</param>
    private async Task<SubtitleDownload> DownloadAsync(Func<string, CancellationToken, Task<IReadOnlyList<SubtitleInfo>>> search,
        string videoPath, IProgress<string>? progress, CancellationToken ct, Dictionary<string, byte[]>? zips = null)
    {
        var languages = Languages;
        var name = Path.GetFileName(videoPath);
        progress?.Report($"Searching SubDL for {languages} subtitles…");
        IReadOnlyList<SubtitleInfo> found;
        try
        {
            found = await search(languages, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is MovieInfoUnavailableException or HttpRequestException)
        {
            _log.LogWarning("Subtitles for {File}: SubDL's search failed: {Message}", name, ex.Message);
            return new SubtitleDownload(languages, 0, 0, 0, null, [], Stopped: ex.Message);
        }

        int saved = 0, files = 0;
        string? stoppedBy = null;
        var failed = new List<string>();
        foreach (var (subtitle, index) in found.Select((s, i) => (s, i + 1)))
        {
            progress?.Report($"Downloading subtitle {index} of {found.Count}: {subtitle.Language} {subtitle.ReleaseName}");
            try
            {
                if (zips is null || !zips.TryGetValue(subtitle.DownloadUrl, out var zip))
                {
                    zip = await provider.DownloadAsync(subtitle, ct).ConfigureAwait(false);
                    zips?.Add(subtitle.DownloadUrl, zip);
                }
                files += await folder.SaveZipAsync(videoPath, zip, ct).ConfigureAwait(false);
                saved++;
            }
            catch (MovieInfoUnavailableException ex)
            {
                // SubDL refuses more (429, its limit): the rest would fail the same way.
                _log.LogWarning("Subtitles for {File}: stopped after {Saved} of {Found} download(s): {Message}", name, saved, found.Count, ex.Message);
                stoppedBy = ex.Message;
                break;
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or InvalidDataException or UnauthorizedAccessException)
            {
                _log.LogWarning("Subtitles for {File}: download of {Language} {Release} failed: {Message}",
                    name, subtitle.Language, subtitle.ReleaseName, ex.Message);
                failed.Add($"{subtitle.ReleaseName}: {ex.Message}");
            }
        }

        // One that fits the release goes next to the movie at once.
        var copied = files > 0 ? folder.CopyMatchFromSubs(videoPath) : null;
        return new SubtitleDownload(languages, found.Count, saved, files, files > 0 ? folder.SubsFolder(videoPath) : null, failed, copied, stoppedBy);
    }
}
