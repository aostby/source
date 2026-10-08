using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Finds and downloads subtitles (SubDL, with the SubDL key from Settings).</summary>
public interface ISubtitleProvider
{
    /// <summary>
    /// Subtitles for <paramref name="imdbId"/> in <paramref name="languages"/> (codes, comma separated, e.g. "NO,EN").
    /// Throws <see cref="MovieInfoUnavailableException"/> without a key, or when SubDL refuses it.
    /// </summary>
    Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, CancellationToken ct = default);

    /// <summary>The subtitle's zip file.</summary>
    Task<byte[]> DownloadAsync(SubtitleInfo subtitle, CancellationToken ct = default);
}

/// <summary>Subtitles on disk next to a movie file: a .srt with its name, or a "Subs" folder.</summary>
public interface ISubtitleFolder
{
    SubtitleStatus GetStatus(string? videoPath);

    /// <summary>
    /// Unpacks the subtitle files in <paramref name="zip"/> into the Subs folder next to <paramref name="videoPath"/>
    /// (made if missing; a name already there gets a number). Returns how many files were written.
    /// </summary>
    Task<int> SaveZipAsync(string videoPath, byte[] zip, CancellationToken ct = default);

    /// <summary>
    /// Copies the subtitle in Subs that fits the movie (<see cref="SubtitleMatching"/>) next to it as "&lt;movie&gt;.srt".
    /// Returns the new file, or null when none fits (or that .srt is already there).
    /// </summary>
    string? CopyMatchFromSubs(string videoPath);

    /// <summary>The .srt files in the Subs folder next to <paramref name="videoPath"/>, largest first.</summary>
    IReadOnlyList<SubtitleFile> GetSubsFiles(string videoPath);

    /// <summary>
    /// Copies <paramref name="subtitlePath"/> next to the movie as "&lt;movie&gt;.srt" and returns that file.
    /// Throws <see cref="IOException"/> if that .srt is already there (it is never overwritten).
    /// </summary>
    string CopyNextToMovie(string videoPath, string subtitlePath);
}
