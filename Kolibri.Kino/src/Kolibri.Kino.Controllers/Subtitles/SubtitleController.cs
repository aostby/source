using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Subtitles;

/// <summary>What a subtitle download did: SubDL's hits, the zips saved, the subtitle files unpacked, and failures.</summary>
/// <param name="Copied">The subtitle copied next to the movie as "&lt;movie&gt;.srt" because it fits the release, if any.</param>
public sealed record SubtitleDownload(string Languages, int Found, int Saved, int Files, string? Folder, IReadOnlyList<string> Failed,
    string? Copied = null);

/// <summary>
/// The subtitle button in the details window (port of SilverScreen's DetailsFormItem): shows whether a movie file
/// has subtitles, and downloads them from SubDL into a "Subs" folder next to it.
/// </summary>
public sealed class SubtitleController(ISubtitleProvider provider, ISubtitleFolder folder, ISettingsStore settings)
{
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
    /// next to <paramref name="videoPath"/>, as SilverScreen did. One failed download doesn't stop the rest.
    /// </summary>
    public async Task<SubtitleDownload> DownloadAsync(string imdbId, string videoPath, IProgress<string>? progress = null,
        CancellationToken ct = default)
    {
        var languages = Languages;
        progress?.Report($"Searching SubDL for {languages} subtitles…");
        var found = await provider.SearchAsync(imdbId, languages, ct).ConfigureAwait(false);

        int saved = 0, files = 0;
        var failed = new List<string>();
        foreach (var (subtitle, index) in found.Select((s, i) => (s, i + 1)))
        {
            progress?.Report($"Downloading subtitle {index} of {found.Count}: {subtitle.Language} {subtitle.ReleaseName}");
            try
            {
                var zip = await provider.DownloadAsync(subtitle, ct).ConfigureAwait(false);
                files += await folder.SaveZipAsync(videoPath, zip, ct).ConfigureAwait(false);
                saved++;
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or InvalidDataException or UnauthorizedAccessException)
            {
                failed.Add($"{subtitle.ReleaseName}: {ex.Message}");
            }
        }

        // One that fits the release goes next to the movie at once.
        var copied = files > 0 ? folder.CopyMatchFromSubs(videoPath) : null;
        var subs = Path.Combine(Path.GetDirectoryName(videoPath)!, "Subs");
        return new SubtitleDownload(languages, found.Count, saved, files, files > 0 ? subs : null, failed, copied);
    }
}
