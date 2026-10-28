using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Discover;

/// <param name="Label">The button's text.</param>
/// <param name="Months">When it's the season (1–12), for the season Discover opens on.</param>
/// <param name="KeywordIds">TMDb keywords, any of which puts a title in the season.</param>
public sealed record DiscoverSeasonInfo(DiscoverSeason Season, string Label, IReadOnlyList<int> Months, IReadOnlyList<int> KeywordIds);

/// <summary>
/// The seasons and the TMDb keywords behind them, in calendar order. Keywords that give wrong hits are left out:
/// "holiday" (both vacation and holidays), "nude beach", "easter island", "witch hunt", "witch burning",
/// "killer santa claus" and "elf"; and, as they put well-known films with one such scene at the top of Most popular,
/// "new year's eve" and "new year" (Forrest Gump), "witch" (Spirited Away), "road trip" and "beach".
/// </summary>
public static class DiscoverSeasons
{
    public static readonly IReadOnlyList<DiscoverSeasonInfo> All =
    [
        new(DiscoverSeason.Winter, "Winter", [1, 2],
            [1442 /* winter */, 10794 /* snow */, 159558 /* snow skiing */, 220840 /* winter sport */, 160404 /* valentine's day */]),
        new(DiscoverSeason.SpringAndEaster, "Spring and Easter", [3, 4],
            [9921 /* easter */, 9923 /* easter bunny */, 3473 /* carnival */, 5497 /* mardi gras */, 158054 /* spring break */]),
        new(DiscoverSeason.Summer, "Summer", [5, 6, 7, 8],
            [13088 /* summer */, 14714 /* summer vacation */, 5767 /* summer camp */, 200129 /* summer romance */, 177923 /* midsummer */,
             159743 /* fourth of july */]),
        new(DiscoverSeason.AutumnAndThanksgiving, "Autumn and Thanksgiving", [9],
            [178883 /* autumn */, 190061 /* harvest */, 4543 /* thanksgiving */]),
        new(DiscoverSeason.Halloween, "Halloween", [10],
            [3335 /* halloween */, 9694 /* halloween party */, 232795 /* halloween night */, 182794 /* halloween costume */,
             180193 /* trick or treating */, 209789 /* day of the dead */, 3358 /* haunted house */]),
        new(DiscoverSeason.ChristmasAndNewYear, "Christmas and New Year", [11, 12],
            [207317 /* christmas */, 260365 /* christmas eve */, 193048 /* christmas spirit */, 5570 /* christmas tree */,
             1441 /* christmas party */, 207089 /* nativity */, 355701 /* xmas */, 7328 /* hanukkah */, 1991 /* santa claus */,
             10385 /* reindeer */, 34154 /* north pole */]),
    ];

    public static DiscoverSeasonInfo Of(DiscoverSeason season) => All.First(s => s.Season == season);

    /// <summary>The season of <paramref name="date"/>: what Discover opens on.</summary>
    public static DiscoverSeason At(DateTime date) => All.First(s => s.Months.Contains(date.Month)).Season;
}
