using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// The "WatchListItem" collection as SilverScreen stores it: one document per title (_id = ImdbId) with its list name
/// and Watched "Y"/"N"; an empty list is a document without an ImdbId. Read as documents, since old entries can lack fields.
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// </summary>
public sealed class LiteDbWatchListRepository(KinoDatabase db) : IWatchListRepository
{
    private readonly IDocumentCollection _docs = db.Collection("WatchListItem");

    public async Task<IReadOnlyList<string>> GetListNamesAsync(CancellationToken ct = default) =>
        (await _docs.FindAllAsync(ct).ConfigureAwait(false))
            .Select(d => d["WatchListName"])
            .Where(v => v.IsString && !string.IsNullOrWhiteSpace(v.AsString))
            .Select(v => v.AsString)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    public async Task<IReadOnlyList<WatchListItem>> GetItemsAsync(string listName, CancellationToken ct = default) =>
        (await _docs.FindAllAsync(ct).ConfigureAwait(false))
            .Where(d => d["WatchListName"].IsString
                     && string.Equals(d["WatchListName"].AsString, listName, StringComparison.OrdinalIgnoreCase)
                     && d["ImdbId"].IsString && d["ImdbId"].AsString.Length > 0)
            .Select(d => BsonMapper.Global.ToObject<WatchListItem>(d))
            .OrderBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    public async Task<WatchListItem?> GetAsync(string imdbId, CancellationToken ct = default) =>
        await _docs.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is { } doc ? BsonMapper.Global.ToObject<WatchListItem>(doc) : null;

    public async Task<IReadOnlySet<string>> GetWatchedImdbIdsAsync(CancellationToken ct = default) =>
        (await _docs.FindAsync("Watched", "Y", ct).ConfigureAwait(false))
            .Select(d => d["ImdbId"])
            .Where(v => v.IsString && v.AsString.Length > 0)
            .Select(v => v.AsString)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    public async Task UpsertAsync(WatchListItem item, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(item.ImdbId);
        // Merged into the stored document, so fields this program doesn't know (SilverScreen's) are kept.
        var incoming = BsonMapper.Global.ToDocument(item);
        var stored = await _docs.FindByIdAsync(item.ImdbId, ct).ConfigureAwait(false) ?? new BsonDocument();
        foreach (var (key, value) in incoming) stored[key] = value;
        stored["_id"] = item.ImdbId;
        await _docs.UpsertAsync(stored, ct).ConfigureAwait(false);
    }

    public async Task<bool> SetWatchedAsync(string imdbId, bool watched, CancellationToken ct = default)
    {
        if (await _docs.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is not { } doc) return false;
        doc["Watched"] = watched ? "Y" : "N";
        await _docs.UpsertAsync(doc, ct).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) =>
        await _docs.DeleteAsync(imdbId, ct).ConfigureAwait(false) || await _docs.DeleteManyAsync("ImdbId", imdbId, ct).ConfigureAwait(false) > 0;

    public async Task CreateListAsync(string listName, CancellationToken ct = default)
    {
        if ((await _docs.FindAsync("WatchListName", listName, ct).ConfigureAwait(false)).Count > 0) return;
        await _docs.UpsertAsync(new BsonDocument { ["WatchListName"] = listName, ["Watched"] = "N" }, ct).ConfigureAwait(false);
    }
}
