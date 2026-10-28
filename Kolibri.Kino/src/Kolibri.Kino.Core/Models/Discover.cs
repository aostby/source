namespace Kolibri.Kino.Core.Models;

/// <summary>Discover's seasons: what TMDb's keywords for each are combined into (see DiscoverSeasons).</summary>
public enum DiscoverSeason
{
    Winter,
    SpringAndEaster,
    Summer,
    AutumnAndThanksgiving,
    Halloween,
    ChristmasAndNewYear,
}

public enum DiscoverSort
{
    /// <summary>TMDb's popularity, most popular first.</summary>
    Popular,

    /// <summary>TMDb's users' average, best first (only titles with enough votes to mean something).</summary>
    BestRated,

    /// <summary>Released (a series: first aired) most recently first, up to today.</summary>
    Newest,
}

/// <summary>One request to TMDb's /discover: any of the keywords, with the filters, one page (20 titles).</summary>
/// <param name="Series">TV shows instead of movies.</param>
/// <param name="MinRating">TMDb's users' average at least this (0–10); null for any.</param>
public sealed record DiscoverRequest(
    IReadOnlyList<int> KeywordIds, bool Series, int? GenreId, int? YearFrom, int? YearTo, double? MinRating, DiscoverSort Sort, int Page);

/// <summary>One page of Discover: the titles (IMDb ids looked up), which page it is and how many there are in all.</summary>
public sealed record DiscoverPage(IReadOnlyList<TrendingItem> Items, int Page, int TotalPages, int TotalResults);

/// <summary>A TMDb genre (movies and TV shows have their own lists).</summary>
public sealed record DiscoverGenre(int Id, string Name);
