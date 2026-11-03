using Kolibri.Kino.Core.Abstractions;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoSimilar": a movie's similar movies, _id = its IMDb id, Fetched (UTC) and Hits (ImdbId, Title, Year, Poster,
/// ImdbRating). Kino's own collection, so SilverScreen ignores it.
/// </summary>
public sealed class LiteDbSimilarCache(KinoDatabase db) : ISimilarCache
{
    private readonly ILiteCollection<BsonDocument> _docs = db.Database.GetCollection("KinoSimilar");

    public Task<CachedSimilar?> GetAsync(string sourceImdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.FindById(sourceImdbId) is { } doc ? FromDocument(doc) : null, ct);

    public Task<IReadOnlyList<CachedSimilar>> GetManyAsync(IEnumerable<string> sourceImdbIds, CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<CachedSimilar>)sourceImdbIds
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(id => _docs.FindById(id))
            .OfType<BsonDocument>()
            .Select(FromDocument)
            .ToList(), ct);

    public Task SaveAsync(CachedSimilar similar, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(similar.SourceImdbId);
        return Task.Run(() => _docs.Upsert(new BsonDocument
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
        }), ct);
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
