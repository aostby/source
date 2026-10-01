namespace Kolibri.Kino.Core.Models;

/// <summary>
/// What a video file's name and folder say about the movie.
/// </summary>
/// <param name="Titles">Title candidates, most likely first (from the file name and from the folder name).</param>
/// <param name="Year">Release year, from the file name or else the folder name.</param>
/// <param name="ImdbId">An IMDb id tagged in the file or folder name, e.g. "{imdb-tt0133093}".</param>
/// <param name="LooksLikeSeries">The name has an episode marker (S01E02, E06, ...).</param>
public sealed record ParsedMovieFile(IReadOnlyList<string> Titles, int? Year, string? ImdbId, bool LooksLikeSeries);
