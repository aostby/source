using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoRating": your own ratings, one document per title, _id = IMDb id, Rating = 1–10, Review, Title, Updated (UTC).
/// Kino's own collection, so SilverScreen ignores it.
/// </summary>
public sealed class LiteDbRatingRepository(KinoDatabase db) : IRatingRepository
{
    private readonly ILiteCollection<BsonDocument> _docs = db.Database.GetCollection("KinoRating");

    public Task<MovieRating?> GetAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.FindById(imdbId) is { } doc ? FromDocument(doc) : null, ct);

    public Task<IReadOnlyList<MovieRating>> GetAllAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<MovieRating>)_docs.FindAll().Select(FromDocument).ToList(), ct);

    public Task SaveAsync(MovieRating rating, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rating.ImdbId);
        return Task.Run(() => _docs.Upsert(new BsonDocument
        {
            ["_id"] = rating.ImdbId,
            ["Rating"] = rating.Rating,
            ["Review"] = rating.Review,
            ["Title"] = rating.Title,
            ["Updated"] = rating.UpdatedUtc.ToUniversalTime(),
        }), ct);
    }

    public Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.Delete(imdbId), ct);

    private static MovieRating FromDocument(BsonDocument d) => new(
        d["_id"].AsString,
        d["Rating"].IsNumber ? d["Rating"].AsInt32 : 0,
        d["Review"].IsString ? d["Review"].AsString : null,
        d["Title"].IsString ? d["Title"].AsString : null,
        d["Updated"].IsDateTime ? DateTime.SpecifyKind(d["Updated"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc) : DateTime.MinValue);
}
