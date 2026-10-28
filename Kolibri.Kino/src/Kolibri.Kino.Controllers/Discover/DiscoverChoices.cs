using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Discover;

/// <summary>Discover's filter choices, the same in the Windows app and on the web. <c>Key</c> is the web address's value.</summary>
public static class DiscoverChoices
{
    public static readonly IReadOnlyList<(string Key, string Label, int? From, int? To)> Years =
    [
        ("", "Any year", null, null), ("2020s", "2020s", 2020, null), ("2010s", "2010s", 2010, 2019), ("2000s", "2000s", 2000, 2009),
        ("1990s", "1990s", 1990, 1999), ("1980s", "1980s", 1980, 1989), ("older", "Before 1980", null, 1979),
    ];

    public static readonly IReadOnlyList<(string Key, string Label, double? Min)> Ratings =
        [("", "Any rating", null), ("6", "TMDb 6+", 6), ("7", "TMDb 7+", 7), ("8", "TMDb 8+", 8)];

    public static readonly IReadOnlyList<(string Key, string Label, DiscoverSort Sort)> Sorts =
        [("", "Most popular", DiscoverSort.Popular), ("rated", "Best rated", DiscoverSort.BestRated), ("newest", "Newest", DiscoverSort.Newest)];

    /// <summary>The seasons' keys in the web address.</summary>
    public static string Key(DiscoverSeason season) => season switch
    {
        DiscoverSeason.Winter => "winter",
        DiscoverSeason.SpringAndEaster => "spring",
        DiscoverSeason.Summer => "summer",
        DiscoverSeason.AutumnAndThanksgiving => "autumn",
        DiscoverSeason.Halloween => "halloween",
        _ => "christmas",
    };

    public static DiscoverSeason? SeasonOf(string? key) =>
        Enum.GetValues<DiscoverSeason>().Cast<DiscoverSeason?>().FirstOrDefault(s => Key(s!.Value) == key);
}
