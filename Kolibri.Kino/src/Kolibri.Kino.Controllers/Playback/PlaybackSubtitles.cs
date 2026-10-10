using System.Text;
using System.Text.RegularExpressions;

namespace Kolibri.Kino.Controllers.Playback;

/// <summary>
/// A subtitle next to the video, for the browser's player. <see cref="FileName"/> is what the Play page asks for;
/// <see cref="Language"/> is a two-letter code when the name says ("Movie.no.srt"), and <see cref="IsDefault"/>
/// marks the one shown from the start.
/// </summary>
public sealed record PlayableSubtitle(string FileName, string Label, string? Language, bool IsDefault);

/// <summary>
/// Subtitles beside a video, as SilverScreen and Kino's SubDL button leave them: "Movie.srt" (the standard one,
/// the copy that fits the release) and "Movie.no.srt", "Movie.en.HI.srt" and so on. Browsers only read WebVTT,
/// so <see cref="ToWebVtt"/> converts SRT.
/// </summary>
public static partial class PlaybackSubtitles
{
    private static readonly string[] Extensions = [".srt", ".vtt"];

    /// <summary>Language words in subtitle names: code, name, and the words for it.</summary>
    private static readonly (string Code, string Name, string[] Words)[] Languages =
    [
        ("no", "Norsk", ["no", "nor", "nb", "nob", "nn", "norsk", "norwegian"]),
        ("en", "English", ["en", "eng", "english"]),
        ("sv", "Svenska", ["sv", "swe", "swedish", "svenska"]),
        ("da", "Dansk", ["da", "dan", "danish", "dansk"]),
        ("fi", "Suomi", ["fi", "fin", "finnish"]),
        ("de", "Deutsch", ["de", "ger", "deu", "german"]),
    ];

    /// <summary>
    /// The subtitles for <paramref name="videoPath"/> among <paramref name="files"/> (the folder's file names), the
    /// standard "Movie.srt" first and shown by default; without one, the first in <paramref name="preferred"/>
    /// languages ("NO,EN") is. Others follow in that language order.
    /// </summary>
    public static IReadOnlyList<PlayableSubtitle> Find(string videoPath, IEnumerable<string> files, string? preferred)
    {
        var video = Path.GetFileNameWithoutExtension(videoPath);
        var order = (preferred ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(p => LanguageOf([p.ToLowerInvariant()])?.Code).OfType<string>().ToList();

        var found = files
            .Select(Path.GetFileName).OfType<string>()
            .Where(f => Extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
                        && Path.GetFileNameWithoutExtension(f).StartsWith(video, StringComparison.OrdinalIgnoreCase))
            .Select(f => Describe(f, Path.GetFileNameWithoutExtension(f)[video.Length..]))
            .OrderBy(s => s.Standard ? 0 : 1)
            .ThenBy(s => s.Language is { } code && order.IndexOf(code) is >= 0 and var i ? i : order.Count)
            .ThenBy(s => s.Label, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return found.Select((s, i) => new PlayableSubtitle(s.FileName, s.Label, s.Language, i == 0)).ToList();
    }

    private static (string FileName, string Label, string? Language, bool Standard) Describe(string fileName, string suffix)
    {
        var words = WordSplit().Split(suffix.ToLowerInvariant()).Where(w => w.Length > 0).ToList();
        if (words.Count == 0) return (fileName, "Standard", null, true);
        // "Movie (2).srt": another release's copy of the standard one.
        if (words is [var n] && int.TryParse(n, out _)) return (fileName, $"Standard ({n})", null, false);

        var language = LanguageOf(words);
        var rest = words.Where(w => language is null || !language.Value.Words.Contains(w)).Select(w => w is "hi" or "sdh" ? "HI" : w).ToList();
        var label = language is { } l ? l.Name : string.Join(" ", rest);
        if (language is not null && rest.Count > 0) label += $" ({string.Join(" ", rest)})";
        return (fileName, label.Length > 0 ? label : suffix.Trim('.', ' '), language?.Code, false);
    }

    private static (string Code, string Name, string[] Words)? LanguageOf(IEnumerable<string> words)
    {
        foreach (var word in words)
            foreach (var language in Languages)
                if (language.Words.Contains(word)) return language;
        return null;
    }

    /// <summary>
    /// The file's text: UTF-8 when it is valid UTF-8, else Windows' western code page as older Norwegian subtitles
    /// use (Latin-1 has the same æ, ø and å).
    /// </summary>
    public static string ReadText(byte[] bytes)
    {
        try
        {
            return new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes).TrimStart('﻿');
        }
        catch (DecoderFallbackException)
        {
            return Encoding.Latin1.GetString(bytes);
        }
    }

    /// <summary>
    /// SRT as WebVTT: the header, dots instead of commas in the times, and without SSA tags like {\an8}.
    /// A file that is WebVTT already is returned as it is.
    /// </summary>
    public static string ToWebVtt(string srt)
    {
        var text = srt.Replace("\r\n", "\n").Replace('\r', '\n').TrimStart('﻿');
        if (text.StartsWith("WEBVTT", StringComparison.Ordinal)) return text;

        text = SrtTime().Replace(text, "$1.$2");
        text = SsaTag().Replace(text, string.Empty);
        return "WEBVTT\n\n" + text.Trim() + "\n";
    }

    [GeneratedRegex(@"[.\s_\-()\[\]]+")]
    private static partial Regex WordSplit();

    [GeneratedRegex(@"(\d{1,2}:\d{2}:\d{2}),(\d{3})")]
    private static partial Regex SrtTime();

    [GeneratedRegex(@"\{\\[^}]*\}")]
    private static partial Regex SsaTag();
}
