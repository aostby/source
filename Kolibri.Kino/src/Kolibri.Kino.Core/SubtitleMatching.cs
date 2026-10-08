using System.Text.RegularExpressions;

namespace Kolibri.Kino.Core;

/// <summary>
/// Which downloaded subtitle fits a movie file. Subtitles are timed to one release, so only these count:
/// <list type="number">
/// <item>the same release: the subtitle's name starts with the movie file's name (e.g. Movie.1080p-GRP.en.srt
/// for Movie.1080p-GRP.mkv), ignoring case and the separators . _ - and spaces;</item>
/// <item>the same release group: the group after the movie name's last '-' (e.g. GRP) is a word in the subtitle's name.</item>
/// </list>
/// </summary>
public static partial class SubtitleMatching
{
    /// <summary>
    /// The best fit for <paramref name="videoPath"/> among <paramref name="subtitlePaths"/> (.srt only), or null.
    /// A same-release subtitle beats a same-group one; among equals, the first by name.
    /// </summary>
    public static string? BestMatch(string videoPath, IEnumerable<string> subtitlePaths)
    {
        var movie = Normalize(Path.GetFileNameWithoutExtension(videoPath));
        var group = ReleaseGroup(Path.GetFileNameWithoutExtension(videoPath));
        if (movie.Length == 0) return null;

        return subtitlePaths
            .Where(p => Path.GetExtension(p).Equals(".srt", StringComparison.OrdinalIgnoreCase))
            .Select(p => (Path: p, Name: Normalize(Path.GetFileNameWithoutExtension(p))))
            .Select(s => (s.Path, Rank: s.Name.StartsWith(movie, StringComparison.Ordinal) ? 1
                : group is not null && $" {s.Name} ".Contains($" {group} ", StringComparison.Ordinal) ? 2
                : 0))
            .Where(s => s.Rank > 0)
            .OrderBy(s => s.Rank)
            .ThenBy(s => Path.GetFileName(s.Path), StringComparer.OrdinalIgnoreCase)
            .Select(s => s.Path)
            .FirstOrDefault();
    }

    /// <summary>Lower case, with every run of separators (. _ - space brackets) as one space.</summary>
    internal static string Normalize(string name) => Separators().Replace(name.ToLowerInvariant(), " ").Trim();

    /// <summary>
    /// "….1080p.WEBRip.x264-GL0P" → "gl0p": the word after the last '-', if it looks like a group (letters/digits, 2–15 long)
    /// and the name is a release name (has a resolution, source or codec), so "Spider-Man" gives no group "man".
    /// </summary>
    internal static string? ReleaseGroup(string name)
    {
        if (!ReleaseTags().IsMatch(name)) return null;
        var dash = name.LastIndexOf('-');
        if (dash < 0) return null;
        var group = name[(dash + 1)..].Trim();
        return Group().IsMatch(group) ? group.ToLowerInvariant() : null;
    }

    [GeneratedRegex(@"[\s._\-\[\]()]+")]
    private static partial Regex Separators();

    [GeneratedRegex(@"\b(\d{3,4}p|x26[45]|h\.?26[45]|hevc|web(rip|-?dl)?|blu-?ray|brrip|bdrip|dvdrip|hdtv)\b", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseTags();

    [GeneratedRegex(@"^[A-Za-z0-9]{2,15}$")]
    private static partial Regex Group();
}
