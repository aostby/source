using LiteDB;

namespace Kolibri.Kino.Data.Storage;

/// <summary>
/// A collection in MariaDB through KMDb. Whole-collection reads are kept for 30 seconds (the library list is read
/// often and is a few thousand documents); this program's own writes clear that.
/// </summary>
public sealed class KmdbDocumentCollection(KmdbClient kmdb, string name) : IDocumentCollection
{
    private static readonly TimeSpan CacheFor = TimeSpan.FromSeconds(30);
    private (IReadOnlyList<BsonDocument> Docs, DateTime Until)? _all;

    public string Name => name;

    public Task<BsonDocument?> FindByIdAsync(BsonValue id, CancellationToken ct = default) => kmdb.GetAsync(name, id, ct);

    public async Task<IReadOnlyList<BsonDocument>> FindAllAsync(CancellationToken ct = default)
    {
        if (_all is { } cached && cached.Until > DateTime.UtcNow) return cached.Docs;
        var docs = await kmdb.ListAsync(name, null, null, ct).ConfigureAwait(false);
        _all = (docs, DateTime.UtcNow + CacheFor);
        return docs;
    }

    public Task<IReadOnlyList<BsonDocument>> FindAsync(string field, BsonValue value, CancellationToken ct = default) =>
        kmdb.ListAsync(name, field, value, ct);

    public async Task UpsertAsync(BsonDocument doc, CancellationToken ct = default)
    {
        if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
        _all = null;
        await kmdb.PutAsync(name, doc, ct).ConfigureAwait(false);
    }

    public async Task UpsertManyAsync(IReadOnlyCollection<BsonDocument> docs, CancellationToken ct = default)
    {
        foreach (var doc in docs)
            if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
        _all = null;
        // In batches: file dates can be thousands at once.
        foreach (var batch in docs.Chunk(200))
            await kmdb.PutManyAsync(name, batch, ct).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(BsonValue id, CancellationToken ct = default)
    {
        _all = null;
        return await kmdb.DeleteAsync(name, id, ct).ConfigureAwait(false);
    }

    public async Task<int> DeleteManyAsync(string field, BsonValue value, CancellationToken ct = default)
    {
        var count = 0;
        foreach (var doc in await FindAsync(field, value, ct).ConfigureAwait(false))
            if (await DeleteAsync(doc["_id"], ct).ConfigureAwait(false)) count++;
        return count;
    }
}
