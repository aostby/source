using System.Globalization;
using System.Text.RegularExpressions;

namespace Kolibri.Kino.Controllers.Series;

/// <summary>What a series folder's name says: an IMDb tag, or a title and maybe the first year.</summary>
public sealed record SeriesFolderGuess(string? ImdbId, string Title, int? Year);

/// <summary>
/// Reads series folder names such as "Hatfields.and.McCoys.2012 {imdb-tt1985443}", "Preacher {tt5016504}",
/// "Monsters 1988 Complete Seasons 1 to 3 TVRip x264 [i_c]" or "The.North.Water.S01.COMPLETE.720p.AMZN.WEBRip".
/// </summary>
public static partial class SeriesFolderName
{
    public static SeriesFolderGuess Parse(string folderName)
    {
        var id = ImdbTag().Match(folderName) is { Success: true } tag ? tag.Groups["id"].Value.ToLowerInvariant() : null;

        var name = BracketYear().Replace(folderName, " ${y} ");  // "Man on Fire [2026]" keeps its year
        name = Brackets().Replace(name, " ");                    // {imdb-tt…}, [i_c], [TGx]
        name = Separators().Replace(name, " ");                  // dots and underscores, but "Mr. Robot" stays
        if (Noise().Match(name) is { Success: true } noise) name = name[..noise.Index];

        int? year = null;
        foreach (var match in Year().Matches(name).Reverse())
        {
            if (name[..match.Index].Trim().Length == 0) continue; // "1923" is a title, not a year
            year = int.Parse(match.Groups["y"].Value, CultureInfo.InvariantCulture);
            name = name[..match.Index];
            break;
        }

        name = Parentheses().Replace(name, " "); // "Sh'at Neila (Valley Of Tears)": search the first title
        var title = Spaces().Replace(name, " ").Trim(' ', '-', '–', ',');
        return new SeriesFolderGuess(id, title.Length > 0 ? title : folderName.Trim(), year);
    }

    /// <summary>The name without its {imdb-tt…} / {tt…} / [imdb-tt…] tag: "Horizon {imdb-tt17505010}" → "Horizon".</summary>
    public static string WithoutImdbTag(string folderName)
    {
        var name = BracketedTag().Replace(folderName, "").Trim();
        return name.Length > 0 ? name : folderName;
    }

    [GeneratedRegex(@"\s*[\{\[]\s*(?:imdb[-_ ]?)?tt\d{7,9}\s*[\}\]]", RegexOptions.IgnoreCase)]
    private static partial Regex BracketedTag();

    [GeneratedRegex(@"(?<![a-z0-9])(?:imdb[-_ ]?)?(?<id>tt\d{7,9})(?!\d)", RegexOptions.IgnoreCase)]
    private static partial Regex ImdbTag();

    [GeneratedRegex(@"\[(?<y>(?:19|20)\d{2})\]")]
    private static partial Regex BracketYear();

    [GeneratedRegex(@"\{[^}]*\}|\[[^\]]*\]")]
    private static partial Regex Brackets();

    [GeneratedRegex(@"_+|\.(?! )")]
    private static partial Regex Separators();

    /// <summary>Where release details start: S01, S01E07, Season 2, Complete, 720p, WEBRip, x264 …</summary>
    [GeneratedRegex(@"\b(?:S\d{1,2}(?:E\d{1,3})?|Seasons?\s*\d+|Complete|Mini\s*Series|Miniseries|\d{3,4}p|WEB[- ]?(?:DL|Rip)|TVRip|DVDRip|BDRip|BRRip|BluRay|HDTV|x26[45]|H\s?26[45]|HEVC|AMZN|NF|ATVP|HMAX|DSNP|REPACK|PROPER|MULTi)\b", RegexOptions.IgnoreCase)]
    private static partial Regex Noise();

    [GeneratedRegex(@"(?<=^|\s)\(?(?<y>(?:19|20)\d{2})\)?(?=\s|$)")]
    private static partial Regex Year();

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex Parentheses();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Spaces();
}
