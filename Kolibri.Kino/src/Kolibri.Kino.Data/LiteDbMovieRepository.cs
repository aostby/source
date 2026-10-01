using Kolibri.Kino.Core.Abstractions;
using LiteDB;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// Stores <see cref="Item"/> in the "Item" collection with _id = ImdbId,
/// the same layout Kolibri.net.Common.Dal's LiteDBController uses, so existing databases open as-is.
/// </summary>
/// <remarks>
/// LiteDB is synchronous; calls are pushed to the thread pool so the UI thread never blocks on disk I/O.
/// </remarks>
public sealed class LiteDbMovieRepository(KinoDatabase db) : IMovieRepository
{
    private readonly ILiteCollection<Item> _items = db.Database.GetCollection<Item>("Item");

    public Task<IReadOnlyList<Item>> GetAllAsync(string? type = null, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var query = string.IsNullOrWhiteSpace(type)
                ? _items.FindAll()
                : _items.Find(x => x.Type == type.ToLowerInvariant());
            return (IReadOnlyList<Item>)query.OrderBy(x => x.Title).ToList();
        }, ct);

    public Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => (Item?)_items.FindById(imdbId), ct);

    public Task<IReadOnlyDictionary<string, Item>> GetByImdbIdsAsync(IEnumerable<string> imdbIds, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            var found = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
            foreach (var id in imdbIds.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                ct.ThrowIfCancellationRequested();
                if (_items.FindById(id) is { } item) found[id] = item;
            }
            return (IReadOnlyDictionary<string, Item>)found;
        }, ct);

    public Task<Item?> FindByTitleAsync(string title, int? year, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            // LiteDB's default collation compares strings case-insensitively.
            var trimmed = title.Trim();
            return _items.Find(x => x.Title == trimmed)
                .FirstOrDefault(x => year is null || (x.Year ?? "").StartsWith(year.Value.ToString(), StringComparison.Ordinal));
        }, ct);

    public Task<IReadOnlyList<Item>> SearchByTitleAsync(string text, CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<Item>)_items.Find(x => x.Title.Contains(text)).OrderBy(x => x.Title).ToList(), ct);

    public Task<bool> UpsertAsync(Item item, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.ImdbId))
            throw new ArgumentException("Item must have an ImdbId.", nameof(item));

        return Task.Run(() => _items.Upsert(new BsonValue(item.ImdbId), item), ct);
    }

    public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _items.Delete(imdbId), ct);
}
