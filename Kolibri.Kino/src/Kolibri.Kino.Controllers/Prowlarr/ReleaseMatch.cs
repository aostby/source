using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Prowlarr;

/// <summary>What is wanted from Prowlarr: a movie (title + year), a whole series, or one episode.</summary>
public enum WantedKind
{
    Movie,
    Series,
    Episode,
}

/// <summary>
/// Whether a release is the wanted title. A search on "Grindhouse 2007" also finds "Grindhouse Death Proof 2007" and
/// "Grindhouse Planet Terror 2007", so the release name must start with the title and go on with the year (a movie),
/// with S01, S01E02, Season, Complete or a year (a series), or with the episode's S06E03. Case, dots, brackets, accents
/// and a leading "The" don't matter. Then IMDb (when both have an id), unwanted words, quality and seeders.
/// </summary>
public static partial class ReleaseMatch
{
    /// <summary>The kind of title, from its type ("series", "episode"; anything else is a movie).</summary>
    public static WantedKind KindOf(WishlistItem wanted) =>
        wanted.IsEpisode || string.Equals(wanted.Type, "episode", StringComparison.OrdinalIgnoreCase) ? WantedKind.Episode
        : string.Equals(wanted.Type, "series", StringComparison.OrdinalIgnoreCase) ? WantedKind.Series
        : WantedKind.Movie;

    /// <summary>
    /// What Prowlarr is asked: "Grindhouse 2007" for a movie, the title for a series (release names seldom have its year),
    /// "The Bear S03E04" for an episode (Kino's wishlist names them "The Bear S03E04: Napkins").
    /// </summary>
    public static string Query(WishlistItem wanted) => KindOf(wanted) switch
    {
        WantedKind.Episode => EpisodeOf(wanted) is { } e ? $"{e.Series} S{e.Season:00}E{e.Episode:00}" : wanted.Title,
        WantedKind.Series => wanted.Title,
        _ => string.IsNullOrWhiteSpace(wanted.Year) ? wanted.Title : $"{wanted.Title} {wanted.Year}",
    };

    /// <summary>
    /// A title typed in by hand, as a wanted title: "Bosch.Legacy.S01E10" or "Bosch Legacy S01E10" is an episode,
    /// "The Bear S03" or "The Bear Season 3" a series, "Grindhouse (2007)", "Grindhouse, 2007" or "Grindhouse.2007" a
    /// movie of that year; anything else a movie by title only (no year to check). A release's dotted form
    /// (no spaces) is read with the dots and underscores as spaces.
    /// </summary>
    public static WishlistItem FromText(string text)
    {
        var plain = text.Trim();
        var dotted = !plain.Contains(' ');
        if (dotted) plain = plain.Replace('.', ' ').Replace('_', ' ').Trim();

        if (EpisodeTitle().Match(plain) is { Success: true } episode)
        {
            var season = int.Parse(episode.Groups["season"].Value, CultureInfo.InvariantCulture);
            var number = int.Parse(episode.Groups["episode"].Value, CultureInfo.InvariantCulture);
            return Manual($"{episode.Groups["series"].Value.Trim()} S{season:00}E{number:00}", null, "episode");
        }
        if (SeasonTitle().Match(plain) is { Success: true } series) return Manual(series.Groups["series"].Value.Trim(), null, "series");

        var (title, year) = Wishlist.WishlistController.SplitYear(plain);
        // "Grindhouse.2007" (a release's form) ends in its year; "Blade Runner 2049" typed with spaces is all title.
        if (year is null && dotted && TrailingYear().Match(plain) is { Success: true } m)
            (title, year) = (m.Groups["title"].Value.Trim(), int.Parse(m.Groups["year"].Value, CultureInfo.InvariantCulture));
        return Manual(title, year?.ToString(CultureInfo.InvariantCulture), year is null ? null : "movie");

        static WishlistItem Manual(string title, string? year, string? type) =>
            new() { ImdbId = "", Title = title, Year = year, Type = type, Added = DateTime.Now };
    }

    [GeneratedRegex(@"^(?<series>.+?)\s+(?:S(?<season>\d{1,2})|Season\s+(?<season>\d{1,2}))$", RegexOptions.IgnoreCase)]
    private static partial Regex SeasonTitle();

    [GeneratedRegex(@"^(?<title>.+?)\s+(?<year>(?:19|20)\d\d)$")]
    private static partial Regex TrailingYear();

    /// <summary>"The Bear S03E04: Napkins" → ("The Bear", 3, 4); null when the title has no SxxEyy.</summary>
    public static (string Series, int Season, int Episode)? EpisodeOf(WishlistItem wanted) =>
        EpisodeTitle().Match(wanted.Title) is { Success: true } m
            ? (m.Groups["series"].Value.Trim(), int.Parse(m.Groups["season"].Value, CultureInfo.InvariantCulture), int.Parse(m.Groups["episode"].Value, CultureInfo.InvariantCulture))
            : null;

    [GeneratedRegex(@"^(?<series>.+?)\s+S(?<season>\d{1,2})E(?<episode>\d{1,3})\b", RegexOptions.IgnoreCase)]
    private static partial Regex EpisodeTitle();

    /// <summary>The quality in a release name: 2160p, 1080p, 720p or SD; null when it says none.</summary>
    public static string? QualityOf(string releaseTitle)
    {
        var words = Words(releaseTitle);
        if (words.Any(w => w is "2160p" or "4k" or "uhd")) return "2160p";
        if (words.Any(w => w is "1080p" or "1080i")) return "1080p";
        if (words.Contains("720p")) return "720p";
        if (words.Any(w => w is "576p" or "480p" or "360p" or "sd" or "dvdrip" or "xvid")) return "SD";
        return null;
    }

    /// <summary>Why the release isn't wanted, or null when it is.</summary>
    public static string? WhyNot(WishlistItem wanted, ProwlarrRelease release, string? quality, ProwlarrOptions options)
    {
        if (TitleMismatch(wanted, release.Title) is { } why) return why;
        if (!string.IsNullOrEmpty(release.ImdbId) && !string.IsNullOrEmpty(wanted.ImdbId) && KindOf(wanted) == WantedKind.Movie
            && !string.Equals(release.ImdbId, wanted.TitleImdbId, StringComparison.OrdinalIgnoreCase))
            return $"Another title on IMDb ({release.ImdbId})";

        var words = Words(release.Title);
        if (options.EffectiveExcludedWords.FirstOrDefault(x => words.Contains(x.Trim().ToLowerInvariant())) is { } excluded)
            return $"Says {excluded}";

        var qualities = options.EffectiveQualities;
        if (!qualities.Any(q => string.Equals(q.Trim(), quality ?? "Unknown", StringComparison.OrdinalIgnoreCase)))
            return quality is null ? "No quality in the name" : $"Quality {quality}";

        if (release.Seeders < options.MinSeeders) return $"Only {release.Seeders} seeder(s)";
        return null;
    }

    /// <summary>Null when the release name is the title (and year, or episode), else why not.</summary>
    private static string? TitleMismatch(WishlistItem wanted, string releaseTitle)
    {
        var kind = KindOf(wanted);
        var episode = kind == WantedKind.Episode ? EpisodeOf(wanted) : null;
        var title = WithoutArticle(Words(episode?.Series ?? wanted.Title));
        var release = WithoutArticle(Words(releaseTitle));
        if (title.Count == 0 || release.Count <= title.Count || !release.Take(title.Count).SequenceEqual(title))
            return "Another title";

        var next = release[title.Count];
        switch (kind)
        {
            case WantedKind.Movie:
                var year = wanted.Year?.Trim() is { Length: >= 4 } y ? y[..4] : null;
                if (year is null || next == year) return null;
                return YearWord().IsMatch(next) ? $"Year {next}" : $"Not \"{wanted.Title} {year}\"";
            case WantedKind.Series:
                return SeriesWord().IsMatch(next) || YearWord().IsMatch(next) ? null : "Another title";
            default:
                if (episode is not { } e) return null; // no SxxEyy to compare
                var at = YearWord().IsMatch(next) && release.Count > title.Count + 1 ? release[title.Count + 1] : next;
                return EpisodeWord().Match(at) is { Success: true } m
                       && int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) == e.Season
                       && int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) == e.Episode
                    ? null : SeriesWord().IsMatch(at) ? "Another episode" : "Another title";
        }
    }

    [GeneratedRegex(@"^(18|19|20)\d\d$")]
    private static partial Regex YearWord();

    [GeneratedRegex(@"^(s\d{1,2}(e\d{1,3})?|season|seasons|complete|series)$")]
    private static partial Regex SeriesWord();

    [GeneratedRegex(@"^s(\d{1,2})e(\d{1,3})$")]
    private static partial Regex EpisodeWord();

    private static List<string> WithoutArticle(List<string> words) =>
        words.Count > 1 && words[0] is "the" or "a" or "an" ? words.Skip(1).ToList() : words;

    /// <summary>
    /// "Grindhouse.(2007).[720p]" → grindhouse, 2007, 720p: lower case, accents gone, "&amp;" as "and", apostrophes
    /// dropped (Don't → dont), everything else that isn't a letter or digit splits words.
    /// </summary>
    public static List<string> Words(string text)
    {
        var plain = new StringBuilder(text.Length);
        foreach (var c in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || c is '\'' or '’') continue;
            plain.Append(c == '&' ? " and " : char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : ' ');
        }
        return plain.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
