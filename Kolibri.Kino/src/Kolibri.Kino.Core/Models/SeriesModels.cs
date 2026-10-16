namespace Kolibri.Kino.Core.Models;

/// <summary>One season of a series with its episodes, from TMDb or OMDb.</summary>
/// <param name="Source">"TMDb" or "OMDb", for display.</param>
public sealed record SeriesSeason(int SeasonNumber, string? Name, IReadOnlyList<SeriesEpisode> Episodes, string Source);

/// <summary>An episode's details. Fields a source doesn't have are null.</summary>
/// <param name="StillUrl">A screenshot of the episode (TMDb), or null.</param>
/// <param name="ImdbId">The episode's own IMDb id (OMDb has it; TMDb's season list doesn't).</param>
public sealed record SeriesEpisode(
    int SeasonNumber,
    int EpisodeNumber,
    string Title,
    DateTime? AirDate,
    string? Plot,
    double? Rating,
    int? RuntimeMinutes,
    string? StillUrl,
    string? ImdbId);

/// <summary>An online fetch of a series' seasons: the seasons (empty when nothing was found) and its TMDb id, if known.</summary>
public sealed record SeriesFetchResult(IReadOnlyList<SeriesSeason> Seasons, int? TmdbId);

/// <summary>
/// When a series' seasons were last fetched online, whether anything was found, and its TMDb id (so the next fetch
/// skips TMDb's IMDb-id lookup). Kept so a series nobody knows isn't asked about again on every run.
/// </summary>
public sealed record SeriesFetchInfo(DateTime FetchedUtc, bool Found, int? TmdbId);

/// <summary>A video file in a series folder and the episode(s) it holds ("S01E01E02" holds two).</summary>
public sealed record EpisodeFile(string Path, int SeasonNumber, IReadOnlyList<int> EpisodeNumbers);
