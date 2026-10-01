using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Online source of movie information (OMDb today; could be TMDb later).
/// Lookups return null for "not found" and throw <see cref="MovieInfoUnavailableException"/>
/// when the service can't be used at all.
/// </summary>
public interface IMovieInfoProvider
{
    Task<IReadOnlyList<SearchItem>> SearchAsync(string query, int page = 1, CancellationToken ct = default);

    Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default);

    /// <summary>A movie (not series) with exactly this title, in <paramref name="year"/> when given.</summary>
    Task<Item?> GetMovieByTitleAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>A series with this title that started in <paramref name="year"/> when given.</summary>
    Task<Item?> GetSeriesByTitleAsync(string title, int? year, CancellationToken ct = default);
}
