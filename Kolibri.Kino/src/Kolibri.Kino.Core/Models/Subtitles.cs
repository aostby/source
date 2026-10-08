namespace Kolibri.Kino.Core.Models;

/// <summary>A subtitle SubDL offers for a title: a zip file at <see cref="DownloadUrl"/>.</summary>
public sealed record SubtitleInfo(string ReleaseName, string Language, string DownloadUrl, bool HearingImpaired, string? Author);

/// <summary>What subtitles a movie file has on disk (as SilverScreen's subtitle button showed it).</summary>
public enum SubtitleState
{
    /// <summary>No file linked, or the file is gone.</summary>
    NoFile,

    /// <summary>A .srt next to the movie with its name, e.g. Movie.srt or Movie.en.srt.</summary>
    SrtFile,

    /// <summary>A Subs folder next to the movie with subtitle files in it.</summary>
    SubsFolder,

    /// <summary>No subtitles found, but an .mkv often has them built in.</summary>
    NoneMaybeBuiltIn,

    /// <summary>No subtitles found.</summary>
    None,
}

/// <summary>A downloaded subtitle file in the Subs folder.</summary>
public sealed record SubtitleFile(string Path, long Bytes);

/// <param name="Path">The .srt file or the Subs folder (for <see cref="SubtitleState.SrtFile"/> and <see cref="SubtitleState.SubsFolder"/>).</param>
/// <param name="Files">Subtitle files in the Subs folder.</param>
public sealed record SubtitleStatus(SubtitleState State, string? Path = null, int Files = 0);
