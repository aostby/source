using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "My rating" per title in Kino's own "KinoRating" collection (_id = IMDb id).
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// </summary>
public sealed class LiteDbRatingRepository(KinoDatabase db) : IRatingRepository
{
    private readonly IDocumentCollection _docs = db.Collection("KinoRating");

    public async Task<MovieRating?> GetAsync(string imdbId, CancellationToken ct = default) =>
        await _docs.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is { } doc ? FromDocument(doc) : null;

    public async Task<IReadOnlyList<MovieRating>> GetAllAsync(CancellationToken ct = default) =>
        (await _docs.FindAllAsync(ct).ConfigureAwait(false)).Select(FromDocument).ToList();

    public Task SaveAsync(MovieRating rating, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rating.ImdbId);
        return _docs.UpsertAsync(new BsonDocument
        {
            ["_id"] = rating.ImdbId,
            ["Rating"] = rating.Rating,
            ["Review"] = rating.Review,
            ["Title"] = rating.Title,
            ["Updated"] = rating.UpdatedUtc.ToUniversalTime(),
        }, ct);
    }

    public Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) => _docs.DeleteAsync(imdbId, ct);

    private static MovieRating FromDocument(BsonDocument d) => new(
        d["_id"].AsString,
        d["Rating"].IsNumber ? d["Rating"].AsInt32 : 0,
        d["Review"].IsString ? d["Review"].AsString : null,
        d["Title"].IsString ? d["Title"].AsString : null,
        d["Updated"].IsDateTime ? DateTime.SpecifyKind(d["Updated"].AsDateTime.ToUniversalTime(), DateTimeKind.Utc) : DateTime.MinValue);
}
