using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Data.Storage;
using LiteDB;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// Stores <see cref="Item"/> in the "Item" collection with _id = ImdbId,
/// the same layout Kolibri.net.Common.Dal's LiteDBController uses, so existing databases open as-is.
/// </summary>
/// <remarks>
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// Title matching is case-insensitive, as LiteDB's default collation is.
/// </remarks>
public sealed class LiteDbMovieRepository(KinoDatabase db) : IMovieRepository
{
    private readonly IDocumentCollection _items = db.Collection("Item");

    public async Task<IReadOnlyList<Item>> GetAllAsync(string? type = null, CancellationToken ct = default)
    {
        var items = (await _items.FindAllAsync(ct).ConfigureAwait(false)).Select(ToItem);
        if (!string.IsNullOrWhiteSpace(type))
            items = items.Where(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));
        return items.OrderBy(x => x.Title, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public async Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) =>
        await _items.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is { } doc ? ToItem(doc) : null;

    public async Task<IReadOnlyDictionary<string, Item>> GetByImdbIdsAsync(IEnumerable<string> imdbIds, CancellationToken ct = default)
    {
        var found = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
        foreach (var id in imdbIds.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            ct.ThrowIfCancellationRequested();
            if (await GetByImdbIdAsync(id, ct).ConfigureAwait(false) is { } item) found[id] = item;
        }
        return found;
    }

    public async Task<Item?> FindByTitleAsync(string title, int? year, CancellationToken ct = default)
    {
        var trimmed = title.Trim();
        return (await _items.FindAsync("Title", trimmed, ct).ConfigureAwait(false))
            .Select(ToItem)
            .FirstOrDefault(x => year is null || (x.Year ?? "").StartsWith(year.Value.ToString(), StringComparison.Ordinal));
    }

    public async Task<IReadOnlyList<Item>> SearchByTitleAsync(string text, CancellationToken ct = default) =>
        (await GetAllAsync(null, ct).ConfigureAwait(false))
            .Where(x => x.Title?.Contains(text, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

    public async Task<bool> UpsertAsync(Item item, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.ImdbId))
            throw new ArgumentException("Item must have an ImdbId.", nameof(item));

        var isNew = await _items.FindByIdAsync(item.ImdbId, ct).ConfigureAwait(false) is null;
        var doc = BsonMapper.Global.ToDocument(item);
        doc["_id"] = item.ImdbId;
        await _items.UpsertAsync(doc, ct).ConfigureAwait(false);
        return isNew;
    }

    public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) => _items.DeleteAsync(imdbId, ct);

    private static Item ToItem(BsonDocument doc) => BsonMapper.Global.ToObject<Item>(doc);
}
