using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// The wishlist in Kino's own "KinoWishlist" collection (_id = IMDb id, or "{series}_S01E02" for an episode).
/// Through <see cref="KinoDatabase.Collection"/>: the LiteDB file, or MariaDB through KMDb when that is in use.
/// </summary>
public sealed class LiteDbWishlistRepository(KinoDatabase db) : IWishlistRepository
{
    private readonly IDocumentCollection _docs = db.Collection("KinoWishlist");

    public async Task<IReadOnlyList<WishlistItem>> GetAllAsync(CancellationToken ct = default) =>
        (await _docs.FindAllAsync(ct).ConfigureAwait(false)).Select(FromDocument).ToList();

    public async Task<WishlistItem?> GetAsync(string imdbId, CancellationToken ct = default) =>
        await _docs.FindByIdAsync(imdbId, ct).ConfigureAwait(false) is { } doc ? FromDocument(doc) : null;

    public Task UpsertAsync(WishlistItem item, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(item.ImdbId);
        return _docs.UpsertAsync(new BsonDocument
        {
            ["_id"] = item.ImdbId,
            ["Title"] = item.Title,
            ["Year"] = item.Year,
            ["Type"] = item.Type,
            ["Genre"] = item.Genre,
            ["ImdbRating"] = item.ImdbRating,
            ["Poster"] = item.Poster,
            ["Added"] = item.Added.ToUniversalTime(),
        }, ct);
    }

    public Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) => _docs.DeleteAsync(imdbId, ct);

    private static WishlistItem FromDocument(BsonDocument d) => new()
    {
        ImdbId = d["_id"].AsString,
        Title = Text(d, "Title") ?? d["_id"].AsString,
        Year = Text(d, "Year"),
        Type = Text(d, "Type"),
        Genre = Text(d, "Genre"),
        ImdbRating = Text(d, "ImdbRating"),
        Poster = Text(d, "Poster"),
        Added = d.TryGetValue("Added", out var added) && added.IsDateTime ? added.AsDateTime.ToLocalTime() : DateTime.MinValue,
    };

    private static string? Text(BsonDocument d, string name) => d.TryGetValue(name, out var v) && v.IsString ? v.AsString : null;
}
