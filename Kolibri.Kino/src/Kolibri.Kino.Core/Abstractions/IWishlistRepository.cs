using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>The wishlist: titles wanted, one entry per IMDb id, apart from the library and the watchlists.</summary>
public interface IWishlistRepository
{
    Task<IReadOnlyList<WishlistItem>> GetAllAsync(CancellationToken ct = default);

    Task<WishlistItem?> GetAsync(string imdbId, CancellationToken ct = default);

    /// <summary>Adds the title, or replaces its entry.</summary>
    Task UpsertAsync(WishlistItem item, CancellationToken ct = default);

    Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default);
}
