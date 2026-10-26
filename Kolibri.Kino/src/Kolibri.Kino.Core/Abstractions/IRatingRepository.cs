using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>Your own ratings, one per IMDb id.</summary>
public interface IRatingRepository
{
    Task<MovieRating?> GetAsync(string imdbId, CancellationToken ct = default);

    Task<IReadOnlyList<MovieRating>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Adds the rating, or replaces the one for its IMDb id.</summary>
    Task SaveAsync(MovieRating rating, CancellationToken ct = default);

    /// <summary>Removes the rating; false when there was none.</summary>
    Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default);
}
