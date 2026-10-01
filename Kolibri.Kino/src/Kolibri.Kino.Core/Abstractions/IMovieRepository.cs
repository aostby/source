using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Local storage of OMDb items (movies, series, episodes), keyed by IMDb id.
/// </summary>
public interface IMovieRepository
{
    /// <summary>All stored items, optionally filtered by OMDb type ("movie", "series", "episode").</summary>
    Task<IReadOnlyList<Item>> GetAllAsync(string? type = null, CancellationToken ct = default);

    Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The stored items for the given ids, keyed by ImdbId. Ids with no item are left out.</summary>
    Task<IReadOnlyDictionary<string, Item>> GetByImdbIdsAsync(IEnumerable<string> imdbIds, CancellationToken ct = default);

    /// <summary>An item with exactly this title (case-insensitive), released in <paramref name="year"/> when given.</summary>
    Task<Item?> FindByTitleAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>Items whose title contains <paramref name="text"/> (case-insensitive).</summary>
    Task<IReadOnlyList<Item>> SearchByTitleAsync(string text, CancellationToken ct = default);

    /// <summary>Inserts or replaces the item. Returns true if it was inserted, false if it replaced an existing one.</summary>
    Task<bool> UpsertAsync(Item item, CancellationToken ct = default);

    Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default);
}
