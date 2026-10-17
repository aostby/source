using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoWishlist": one document per wanted title, _id = IMDb id. Kino's own collection (SilverScreen doesn't know it),
/// apart from WatchListItem, where a title can only be on one list.
/// </summary>
public sealed class LiteDbWishlistRepository(KinoDatabase db) : IWishlistRepository
{
    private readonly ILiteCollection<BsonDocument> _docs = db.Database.GetCollection("KinoWishlist");

    public Task<IReadOnlyList<WishlistItem>> GetAllAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<WishlistItem>)_docs.FindAll().Select(FromDocument).ToList(), ct);

    public Task<WishlistItem?> GetAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.FindById(imdbId) is { } doc ? FromDocument(doc) : null, ct);

    public Task UpsertAsync(WishlistItem item, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(item.ImdbId);
        return Task.Run(() => _docs.Upsert(new BsonDocument
        {
            ["_id"] = item.ImdbId,
            ["Title"] = item.Title,
            ["Year"] = item.Year,
            ["Type"] = item.Type,
            ["Genre"] = item.Genre,
            ["ImdbRating"] = item.ImdbRating,
            ["Poster"] = item.Poster,
            ["Added"] = item.Added.ToUniversalTime(),
        }), ct);
    }

    public Task<bool> RemoveAsync(string imdbId, CancellationToken ct = default) =>
        Task.Run(() => _docs.Delete(imdbId), ct);

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
