namespace Kolibri.Kino.KMDb;

/// <summary>One title from title_basics with its rating (title_ratings) and crew (title_crew, comma lists of nm ids).</summary>
public sealed record TitleRow(
    string Id,
    string TitleType,
    string PrimaryTitle,
    string? OriginalTitle,
    bool IsAdult,
    int? StartYear,
    int? EndYear,
    int? RuntimeMinutes,
    string? Genres,
    decimal? Rating,
    int? Votes,
    string? Directors = null,
    string? Writers = null);

/// <summary>Where an episode belongs (title_episode).</summary>
public sealed record EpisodeLink(string SeriesId, int? Season, int? Episode);

/// <summary>One episode in a season list.</summary>
public sealed record SeasonEpisodeRow(string Id, int? Episode, string Title, int? Year, decimal? Rating);

/// <summary>One page of search hits, best (most votes) first.</summary>
public sealed record SearchPage(IReadOnlyList<TitleRow> Rows, int Total, bool TooMany);
