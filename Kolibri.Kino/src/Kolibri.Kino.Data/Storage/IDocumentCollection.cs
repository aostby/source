using LiteDB;

namespace Kolibri.Kino.Data.Storage;

/// <summary>
/// One collection of library documents, as the repositories use it: the same few operations whether the documents
/// are in the LiteDB file (<see cref="LiteDocumentCollection"/>) or in MariaDB through Kolibri.Kino.KMDb
/// (<see cref="KmdbDocumentCollection"/>). <see cref="KinoDatabase.Collection"/> hands out one that picks between them.
/// </summary>
/// <remarks>Field comparisons are case-insensitive, as LiteDB's default collation and MariaDB's are.</remarks>
public interface IDocumentCollection
{
    string Name { get; }

    Task<BsonDocument?> FindByIdAsync(BsonValue id, CancellationToken ct = default);

    Task<IReadOnlyList<BsonDocument>> FindAllAsync(CancellationToken ct = default);

    /// <summary>The documents where <paramref name="field"/> equals <paramref name="value"/>.</summary>
    Task<IReadOnlyList<BsonDocument>> FindAsync(string field, BsonValue value, CancellationToken ct = default);

    /// <summary>Stores the document as a whole, by its _id (a document without one gets a new ObjectId).</summary>
    Task UpsertAsync(BsonDocument doc, CancellationToken ct = default);

    Task UpsertManyAsync(IReadOnlyCollection<BsonDocument> docs, CancellationToken ct = default);

    Task<bool> DeleteAsync(BsonValue id, CancellationToken ct = default);

    /// <summary>Removes the documents where <paramref name="field"/> equals <paramref name="value"/>; returns how many.</summary>
    Task<int> DeleteManyAsync(string field, BsonValue value, CancellationToken ct = default);
}

/// <summary>The collection in the LiteDB file. LiteDB is synchronous, so calls run on the thread pool.</summary>
public sealed class LiteDocumentCollection(ILiteCollection<BsonDocument> collection) : IDocumentCollection
{
    public string Name => collection.Name;

    public Task<BsonDocument?> FindByIdAsync(BsonValue id, CancellationToken ct = default) =>
        Task.Run(() => (BsonDocument?)collection.FindById(id), ct);

    // ToList: in Shared mode LiteDB closes the file after each operation, so no cursor may stay open.
    public Task<IReadOnlyList<BsonDocument>> FindAllAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<BsonDocument>)collection.FindAll().ToList(), ct);

    public Task<IReadOnlyList<BsonDocument>> FindAsync(string field, BsonValue value, CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<BsonDocument>)collection.Find(Query.EQ(field, value)).ToList(), ct);

    public Task UpsertAsync(BsonDocument doc, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
            collection.Upsert(doc);
        }, ct);

    public Task UpsertManyAsync(IReadOnlyCollection<BsonDocument> docs, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            foreach (var doc in docs)
                if (!doc.ContainsKey("_id") || doc["_id"].IsNull) doc["_id"] = ObjectId.NewObjectId();
            if (docs.Count > 0) collection.Upsert(docs);
        }, ct);

    public Task<bool> DeleteAsync(BsonValue id, CancellationToken ct = default) =>
        Task.Run(() => collection.Delete(id), ct);

    public Task<int> DeleteManyAsync(string field, BsonValue value, CancellationToken ct = default) =>
        Task.Run(() => collection.DeleteMany(Query.EQ(field, value)), ct);
}
