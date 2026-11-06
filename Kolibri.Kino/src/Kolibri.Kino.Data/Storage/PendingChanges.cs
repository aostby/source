using LiteDB;

namespace Kolibri.Kino.Data.Storage;

/// <summary>A change made in the LiteDB file while KMDb didn't answer.</summary>
public sealed record PendingChange(string Key, string Collection, BsonValue Id, string Operation, DateTime TimeUtc);

/// <summary>
/// Changes made in the LiteDB file while KMDb didn't answer (collection "KinoPendingSync" in the same file), sent to
/// KMDb when it answers again: the document as it then is in the LiteDB file, or its removal. One entry per document;
/// the last change counts.
/// </summary>
public sealed class PendingChanges(LiteDatabase db)
{
    public const string Upsert = "upsert";
    public const string Delete = "delete";
    public const string CollectionName = "KinoPendingSync";

    private readonly ILiteCollection<BsonDocument> _docs = db.GetCollection(CollectionName);

    public Task AddAsync(string collection, BsonValue id, string operation, CancellationToken ct = default) =>
        Task.Run(() => _docs.Upsert(new BsonDocument
        {
            ["_id"] = Key(collection, id),
            ["Collection"] = collection,
            ["Id"] = id,
            ["Operation"] = operation,
            ["Time"] = DateTime.UtcNow,
        }), ct);

    public Task<IReadOnlyList<PendingChange>> AllAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<PendingChange>)_docs.FindAll().ToList()
            .Select(d => new PendingChange(d["_id"].AsString, d["Collection"].AsString, d["Id"], d["Operation"].AsString, d["Time"].AsDateTime))
            .OrderBy(c => c.TimeUtc)
            .ToList(), ct);

    public Task<int> CountAsync(CancellationToken ct = default) => Task.Run(() => _docs.Count(), ct);

    public Task RemoveAsync(string key, CancellationToken ct = default) => Task.Run(() => _docs.Delete(key), ct);

    internal static string Key(string collection, BsonValue id) => collection + "\u001f" + KmdbClient.Id(id);
}
