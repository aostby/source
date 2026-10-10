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
}
