using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// Similar movies per title in Kino's own "KinoSimilar" collection (_id = the title's IMDb id), kept 30 days.
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// </summary>
public sealed class LiteDbSimilarCache(KinoDatabase db) : ISimilarCache
{
    private readonly IDocumentCollection _docs = db.Collection("KinoSimilar");

    public async Task<CachedSimilar?> GetAsync(string sourceImdbId, CancellationToken ct = default) =>
        await _docs.FindByIdAsync(sourceImdbId, ct).ConfigureAwait(false) is { } doc ? FromDocument(doc) : null;

    public async Task<IReadOnlyList<CachedSimilar>> GetManyAsync(IEnumerable<string> sourceImdbIds, CancellationToken ct = default)
    {
        var found = new List<CachedSimilar>();
        foreach (var id in sourceImdbIds.Distinct(StringComparer.OrdinalIgnoreCase))
            if (await _docs.FindByIdAsync(id, ct).ConfigureAwait(false) is { } doc) found.Add(FromDocument(doc));
        return found;
    }

    public Task SaveAsync(CachedSimilar similar, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(similar.SourceImdbId);
        return _docs.UpsertAsync(new BsonDocument
        {
            ["_id"] = similar.SourceImdbId,
            ["Fetched"] = similar.FetchedUtc.ToUniversalTime(),
            ["Hits"] = new BsonArray(similar.Hits.Select(h => (BsonValue)new BsonDocument
            {
                ["ImdbId"] = h.Movie.ImdbId,
                ["Title"] = h.Movie.Title,
                ["Year"] = h.Movie.Year,
                ["Poster"] = h.Movie.Poster,
                ["ImdbRating"] = h.ImdbRating,
            })),
        }, ct);
    }

    private static CachedSimilar FromDocument(BsonDocument d) => new(
        d["_id"].AsString,
        d["Hits"].IsArray
            ? d["Hits"].AsArray.Where(v => v.IsDocument).Select(v => v.AsDocument)
                .Where(h => h["ImdbId"].IsString)
                .Select(h => new SimilarHit(new TmdbMovieHit(h["ImdbId"].AsString, Text(h, "Title") ?? h["ImdbId"].AsString, Text(h, "Year"), Text(h, "Poster")), Text(h, "ImdbRating")))
                .ToList()
            : [],
        d["Fetched"].IsDateTime ? DateTime.SpecifyKind(d["Fetched"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc) : DateTime.MinValue);

    private static string? Text(BsonDocument d, string field) => d[field].IsString ? d[field].AsString : null;
}
