namespace Kolibri.Kino.Core;

/// <summary>
/// Which leftover files in a movie folder may be deleted. Defaults are SilverScreen's list
/// (MoviesSearchController.StartUnwantedFilesCleanup): extras plus subtitles in languages other than
/// English and the Scandinavian ones.
/// </summary>
public static class CleanupRules
{
    public static readonly IReadOnlyList<string> DefaultPatterns =
    [
        ".nfo", ".txt", ".jpg", ".exe",
        "Bulgarian.srt", "Dutch.srt", "Estonian.srt", "Japanese.srt", "Korean.srt", "Latvian.srt", "Lithuanian.srt",
        "Russian.srt", "Slovenian.srt", "Thai.srt", "Turkish.srt", "Vietnamese.srt", "arabic.srt", "french.srt",
        "german.srt", "greek.srt", "spanish.srt",
        "kir.srt", "kaz.srt", "mac.srt", "srp.srt", "arm.srt", "geo.srt", "kan.srt", "mal.srt", "khm.srt", "aze.srt",
        "ice.srt", "alb.srt", "baq.srt", "cat.srt", "fil.srt", "glg.srt", "ara.srt", "bul.srt", "ces.srt", "chi.srt",
        "cze.srt", "deu.srt", "dut.srt", "ell.srt", "esp.srt", "est.srt", "fin.srt", "fra.srt", "fre.srt", "ger.srt",
        "gre.srt", "heb.srt", "hin.srt", "hrv.srt", "hun.srt", "ind.srt", "ita.srt", "jpn.srt", "kor.srt", "lav.srt",
        "lit.srt", "may.srt", "nl.srt", "nld.srt", "pol.srt", "por.srt", "ron.srt", "rum.srt", "rus.srt", "slo.srt",
        "slv.srt", "spa.srt", "tam.srt", "tel.srt", "tha.srt", "tur.srt", "ukr.srt", "vie.srt", "zho.srt",
    ];

    private static readonly char[] Separators = ['.', '_', '-', ' '];

    /// <summary>
    /// True when <paramref name="fileName"/> ends with one of <paramref name="patterns"/>. Video files never match.
    /// </summary>
    /// <remarks>
    /// A pattern that doesn't start with "." must follow a separator, so "ice.srt" matches "Movie.ice.srt"
    /// but not "Alice.srt". SilverScreen used a plain EndsWith and so deleted such English subtitles.
    /// "xxx.srt" also matches the hearing-impaired "xxx.HI.srt".
    /// </remarks>
    public static bool Matches(string fileName, IReadOnlyList<string> patterns)
    {
        if (MovieFileRules.IsVideoFile(fileName)) return false;

        foreach (var pattern in patterns)
        {
            if (string.IsNullOrWhiteSpace(pattern)) continue;
            if (EndsWithPattern(fileName, pattern)) return true;

            var dot = pattern.LastIndexOf('.');
            if (dot > 0 && EndsWithPattern(fileName, $"{pattern[..dot]}.HI{pattern[dot..]}")) return true;
        }
        return false;
    }

    private static bool EndsWithPattern(string fileName, string pattern)
    {
        if (!fileName.EndsWith(pattern, StringComparison.OrdinalIgnoreCase)) return false;
        if (pattern.StartsWith('.')) return true;

        var before = fileName.Length - pattern.Length - 1;
        return before < 0 || Separators.Contains(fileName[before]);
    }
}
