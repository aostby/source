using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoDiscover": one document per request and page (_id = the request's key), Fetched (UTC), Page, TotalPages,
/// TotalResults and Items (as in KinoTrending). Kino's own collection, so SilverScreen ignores it.
/// </summary>
public sealed class LiteDbDiscoverCache(KinoDatabase db) : IDiscoverCache
{
    private readonly IDocumentCollection _docs = db.Collection("KinoDiscover");

    public async Task<(DiscoverPage Page, DateTime FetchedUtc)?> LoadAsync(string key, CancellationToken ct = default)
    {
        if (await _docs.FindByIdAsync(key, ct).ConfigureAwait(false) is not { } doc || !doc["Fetched"].IsDateTime) return null;
        var page = new DiscoverPage(LiteDbTrendingCache.FromArray(doc["Items"]), Int(doc, "Page"), Int(doc, "TotalPages"), Int(doc, "TotalResults"));
        return (page, DateTime.SpecifyKind(doc["Fetched"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc));
    }

    public Task SaveAsync(string key, DiscoverPage page, DateTime fetchedUtc, CancellationToken ct = default) =>
        _docs.UpsertAsync(new BsonDocument
        {
            ["_id"] = key,
            ["Fetched"] = fetchedUtc.ToUniversalTime(),
            ["Page"] = page.Page,
            ["TotalPages"] = page.TotalPages,
            ["TotalResults"] = page.TotalResults,
            ["Items"] = LiteDbTrendingCache.ToArray(page.Items),
        }, ct);

    private static int Int(BsonDocument d, string field) => d[field].IsNumber ? d[field].AsInt32 : 0;
}
