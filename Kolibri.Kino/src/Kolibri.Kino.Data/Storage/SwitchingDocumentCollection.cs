using LiteDB;

namespace Kolibri.Kino.Data.Storage;

/// <summary>
/// The collection the repositories use (<see cref="KinoDatabase.Collection"/>). Without KMDb set up it is the LiteDB
/// file. With KMDb in use (<see cref="KmdbConnection"/>) it reads from KMDb and writes to KMDb and to the LiteDB file,
/// which so stays a current fallback. While KMDb is set up but doesn't answer, or when a call to it fails, it uses the
/// LiteDB file and notes each change (<see cref="PendingChanges"/>), sent to KMDb when it answers again.
/// </summary>
/// <remarks>The connection is looked up on each call: repositories are made before it is.</remarks>
public sealed class SwitchingDocumentCollection(KinoDatabase db, string name) : IDocumentCollection
{
    private readonly LiteDocumentCollection _lite = new(db.Database.GetCollection(name));
    private KmdbDocumentCollection? _kmdb;

    public string Name => name;

    private KmdbConnection? Connection => db.Kmdb;

    private KmdbDocumentCollection Kmdb => _kmdb ??= new KmdbDocumentCollection(db.KmdbClient!, name);

    public Task<BsonDocument?> FindByIdAsync(BsonValue id, CancellationToken ct = default) =>
        ReadAsync(c => c.FindByIdAsync(id, ct));

    public Task<IReadOnlyList<BsonDocument>> FindAllAsync(CancellationToken ct = default) =>
        ReadAsync(c => c.FindAllAsync(ct));

    public Task<IReadOnlyList<BsonDocument>> FindAsync(string field, BsonValue value, CancellationToken ct = default) =>
        ReadAsync(c => c.FindAsync(field, value, ct));

    public async Task UpsertAsync(BsonDocument doc, CancellationToken ct = default)
    {
        if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
        await _lite.UpsertAsync(doc, ct).ConfigureAwait(false);
        await ToKmdbAsync(doc["_id"], PendingChanges.Upsert, () => Kmdb.UpsertAsync(doc, ct), ct).ConfigureAwait(false);
    }

    public async Task UpsertManyAsync(IReadOnlyCollection<BsonDocument> docs, CancellationToken ct = default)
    {
        foreach (var doc in docs)
            if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
        await _lite.UpsertManyAsync(docs, ct).ConfigureAwait(false);
        if (Connection is not { } connection) return;
        if (connection.UseKmdb)
        {
            try
            {
                await Kmdb.UpsertManyAsync(docs, ct).ConfigureAwait(false);
                return;
            }
            catch (KmdbUnavailableException ex)
            {
                connection.Failed(ex);
            }
        }
        if (connection.IsConfigured)
            foreach (var doc in docs) await db.Pending.AddAsync(name, doc["_id"], PendingChanges.Upsert, ct).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(BsonValue id, CancellationToken ct = default)
    {
        var deleted = await _lite.DeleteAsync(id, ct).ConfigureAwait(false);
        var inKmdb = false;
        await ToKmdbAsync(id, PendingChanges.Delete, async () => inKmdb = await Kmdb.DeleteAsync(id, ct).ConfigureAwait(false), ct).ConfigureAwait(false);
        return deleted || inKmdb;
    }

    public async Task<int> DeleteManyAsync(string field, BsonValue value, CancellationToken ct = default)
    {
        // By id, so each removal can be noted for KMDb when it doesn't answer.
        var ids = (await ReadAsync(c => c.FindAsync(field, value, ct)).ConfigureAwait(false))
            .Concat(await _lite.FindAsync(field, value, ct).ConfigureAwait(false))
            .Select(d => d["_id"])
            .Distinct()
            .ToList();
        var count = 0;
        foreach (var id in ids)
            if (await DeleteAsync(id, ct).ConfigureAwait(false)) count++;
        return count;
    }

    private async Task<T> ReadAsync<T>(Func<IDocumentCollection, Task<T>> read)
    {
        if (Connection is { UseKmdb: true } connection)
        {
            try
            {
                return await read(Kmdb).ConfigureAwait(false);
            }
            catch (KmdbUnavailableException ex)
            {
                connection.Failed(ex);
            }
        }
        return await read(_lite).ConfigureAwait(false);
    }

    /// <summary>Sends a change to KMDb when it is in use; otherwise (or when that fails) notes it for later.</summary>
    private async Task ToKmdbAsync(BsonValue id, string operation, Func<Task> write, CancellationToken ct)
    {
        if (Connection is not { } connection) return;
        if (connection.UseKmdb)
        {
            try
            {
                await write().ConfigureAwait(false);
                return;
            }
            catch (KmdbUnavailableException ex)
            {
                connection.Failed(ex);
            }
        }
        if (connection.IsConfigured) await db.Pending.AddAsync(name, id, operation, ct).ConfigureAwait(false);
    }
}
