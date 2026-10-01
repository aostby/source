namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Finds a movie's or series' IMDb id from a title and year (TMDb today). Returns null unless the match is clear.
/// </summary>
public interface IImdbIdResolver
{
    Task<string?> FindImdbIdAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>A TV series with this name (or original name) that started in <paramref name="year"/>.</summary>
    Task<string?> FindSeriesImdbIdAsync(string title, int year, CancellationToken ct = default);
}
