using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoTrending": one document per list, _id = Movies / Series / People, Fetched (UTC) and Items. Kino's own
/// collection, so SilverScreen ignores it.
/// </summary>
public sealed class LiteDbTrendingCache(KinoDatabase db) : ITrendingCache
{
    private readonly ILiteCollection<BsonDocument> _docs = db.Database.GetCollection("KinoTrending");

    public Task<(IReadOnlyList<TrendingItem> Items, DateTime FetchedUtc)?> LoadAsync(TrendingKind kind, CancellationToken ct = default) =>
        Task.Run<(IReadOnlyList<TrendingItem>, DateTime)?>(() =>
        {
            if (_docs.FindById(kind.ToString()) is not { } doc || !doc["Fetched"].IsDateTime) return null;
            var items = doc["Items"].IsArray ? doc["Items"].AsArray.Where(v => v.IsDocument).Select(v => FromDocument(v.AsDocument)).ToList() : [];
            return (items, DateTime.SpecifyKind(doc["Fetched"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc));
        }, ct);

    public Task SaveAsync(TrendingKind kind, IReadOnlyList<TrendingItem> items, DateTime fetchedUtc, CancellationToken ct = default) =>
        Task.Run(() => _docs.Upsert(new BsonDocument
        {
            ["_id"] = kind.ToString(),
            ["Fetched"] = fetchedUtc.ToUniversalTime(),
            ["Items"] = new BsonArray(items.Select(i => (BsonValue)new BsonDocument
            {
                ["TmdbId"] = i.TmdbId,
                ["Title"] = i.Title,
                ["Year"] = i.Year,
                ["ImdbId"] = i.ImdbId,
                ["Image"] = i.ImageUrl,
                ["TmdbRating"] = i.TmdbRating is { } r ? new BsonValue(r) : BsonValue.Null,
                ["Overview"] = i.Overview,
                ["KnownFor"] = new BsonArray(i.KnownFor.Select(k => new BsonValue(k))),
            })),
        }), ct);

    private static TrendingItem FromDocument(BsonDocument d) => new(
        d["TmdbId"].IsNumber ? d["TmdbId"].AsInt32 : 0,
        Text(d, "Title") ?? "",
        Text(d, "Year"),
        Text(d, "ImdbId"),
        Text(d, "Image"),
        d["TmdbRating"].IsNumber ? d["TmdbRating"].AsDouble : null,
        Text(d, "Overview"),
        d["KnownFor"].IsArray ? d["KnownFor"].AsArray.Where(v => v.IsString).Select(v => v.AsString).ToList() : []);

    private static string? Text(BsonDocument d, string field) => d[field].IsString ? d[field].AsString : null;
}
