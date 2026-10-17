namespace Kolibri.Kino.Core.Abstractions;

/// <summary>A movie found on TMDb, with its IMDb id, TMDb's (English) title and, where asked for, TMDb's poster address.</summary>
public sealed record TmdbMovieHit(string ImdbId, string Title, string? Year, string? Poster = null);

/// <summary>
/// Finds a movie's or series' IMDb id from a title and year (TMDb today). Returns null unless the match is clear.
/// </summary>
public interface IImdbIdResolver
{
    Task<string?> FindImdbIdAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>
    /// TMDb's movie search for the title (in <paramref name="year"/>, when given): up to <paramref name="max"/> movies
    /// that have an IMDb id, best first. Finds titles by their English name where OMDb only knows another language's
    /// (Operation Kabul, 2025, is "Sárkányok Kabul felett" on OMDb). Empty without a TMDb key.
    /// </summary>
    Task<IReadOnlyList<TmdbMovieHit>> SearchMoviesAsync(string title, int? year, int max = 8, CancellationToken ct = default);

    /// <summary>A TV series with this name (or original name) that started in <paramref name="year"/>.</summary>
    Task<string?> FindSeriesImdbIdAsync(string title, int year, CancellationToken ct = default);
}
