using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

public interface IWatchListRepository
{
    /// <summary>All list names, including empty lists.</summary>
    Task<IReadOnlyList<string>> GetListNamesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<WatchListItem>> GetItemsAsync(string listName, CancellationToken ct = default);

    Task<WatchListItem?> GetAsync(string imdbId, CancellationToken ct = default);

    /// <summary>Adds or replaces the entry for <see cref="OMDbApiNet.Model.Item.ImdbId"/> (moving it if it was on another list).</summary>
    Task UpsertAsync(WatchListItem item, CancellationToken ct = default);

    Task<bool> SetWatchedAsync(string imdbId, bool watched, CancellationToken ct = default);

    Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default);

    /// <summary>Makes an empty list exist (SilverScreen's placeholder entry without a movie).</summary>
    Task CreateListAsync(string listName, CancellationToken ct = default);
}
