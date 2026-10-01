using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// The "WatchListItem" collection (_id = ImdbId), shared with SilverScreen. Works on the stored documents, so
/// fields Kino doesn't use (SilverScreen's Picture bytes) survive status changes.
/// </summary>
/// <remarks>
/// An empty list is a placeholder document with only a WatchListName, as SilverScreen's AddWatchListName makes.
/// </remarks>
public sealed class LiteDbWatchListRepository(KinoDatabase db) : IWatchListRepository
{
    private readonly ILiteCollection<BsonDocument> _docs = db.Database.GetCollection("WatchListItem");

    public Task<IReadOnlyList<string>> GetListNamesAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<string>)_docs.FindAll()
            .Select(d => d["WatchListName"])
            .Where(v => v.IsString && !string.IsNullOrWhiteSpace(v.AsString))
            .Select(v => v.AsString)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.CurrentCultureIgnoreCase)
            .ToList(), ct);

    public Task<IReadOnlyList<WatchListItem>> GetItemsAsync(string listName, CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<WatchListItem>)_docs.FindAll()
            .Where(d => d["WatchListName"].IsString
                     && string.Equals(d["WatchListName"].AsString, listName, StringComparison.OrdinalIgnoreCase)
                     && d["ImdbId"].IsString && d["ImdbId"].AsString.Length > 0)
            .Select(d => BsonMapper.Global.ToObject<WatchListItem>(d))
            .OrderBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList(), ct);

    public Task<WatchListItem?> GetAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.FindById(imdbId) is { } doc ? BsonMapper.Global.ToObject<WatchListItem>(doc) : null, ct);

    public Task UpsertAsync(WatchListItem item, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(item.ImdbId);
        return Task.Run(() =>
        {
            var incoming = BsonMapper.Global.ToDocument(item);
            var stored = _docs.FindById(item.ImdbId) ?? new BsonDocument();
            foreach (var (key, value) in incoming) stored[key] = value;
            stored["_id"] = item.ImdbId;
            _docs.Upsert(stored);
        }, ct);
    }

    public Task<bool> SetWatchedAsync(string imdbId, bool watched, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            if (_docs.FindById(imdbId) is not { } doc) return false;
            doc["Watched"] = watched ? "Y" : "N";
            return _docs.Update(doc);
        }, ct);

    public Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.Delete(imdbId) || _docs.DeleteMany(Query.EQ("ImdbId", imdbId)) > 0, ct);

    public Task CreateListAsync(string listName, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            if (_docs.Exists(Query.EQ("WatchListName", listName))) return;
            _docs.Insert(new BsonDocument { ["WatchListName"] = listName, ["Watched"] = "N" });
        }, ct);
}
