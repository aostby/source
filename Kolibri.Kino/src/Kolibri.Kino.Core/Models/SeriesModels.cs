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

/// <summary>A video file in a series folder and the episode(s) it holds ("S01E01E02" holds two).</summary>
public sealed record EpisodeFile(string Path, int SeasonNumber, IReadOnlyList<int> EpisodeNumbers);
