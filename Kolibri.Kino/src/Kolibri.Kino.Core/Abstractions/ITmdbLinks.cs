namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// TMDb page and poster addresses. TMDb pages use TMDb's own ids, so the IMDb id has to be looked up first.
/// </summary>
public interface ITmdbLinks
{
    /// <summary>The TMDb page for the movie, series or episode with this IMDb id, or null if TMDb doesn't know it.</summary>
    Task<string?> FindPageAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The address of TMDb's poster for the movie or series with this IMDb id, or null if it has none.</summary>
    Task<string?> FindPosterAsync(string imdbId, CancellationToken ct = default);

    /// <summary>
    /// TMDb's title (English) for the movie or series with this IMDb id, or null if TMDb doesn't know it. OMDb sometimes
    /// has another language's title, e.g. Hungarian "Sárkányok Kabul felett" for Operation Kabul (tt34375721).
    /// </summary>
    Task<string?> FindTitleAsync(string imdbId, CancellationToken ct = default);

    /// <summary>
    /// Movies like the one with this IMDb id: TMDb's recommendations (else its "similar" list), up to
    /// <paramref name="max"/> that have an IMDb id, with small poster addresses. Empty when TMDb doesn't know the
    /// movie, has none, or there's no key.
    /// </summary>
    Task<IReadOnlyList<TmdbMovieHit>> FindSimilarAsync(string imdbId, int max = 12, CancellationToken ct = default);

    /// <summary>
    /// TMDb's users' average score (0–10) and how many voted, for the movie or series with this IMDb id; null when
    /// TMDb doesn't know it or nobody has voted.
    /// </summary>
    Task<(double Rating, int Votes)?> FindRatingAsync(string imdbId, CancellationToken ct = default);
}
