using Kolibri.Kino.Core;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data;

/// <summary>
/// The image cache file (SilverScreen.imgdb), shared with SilverScreen's ImageCacheDB.
/// </summary>
/// <remarks>
/// Layout: collection "Image", _id = key string (IMDb id, URL or name), Data = encoded JPEG/PNG bytes.
/// This replaces the old "ImageBase" collection, keyed by key.GetHashCode(). That hash is randomized
/// per process on .NET Core, so every run missed the cache and stored the image again as base64 BMP.
/// </remarks>
public sealed class LiteDbImageStore : IDisposable
{
    public const string CollectionName = "Image";

    private readonly LiteDatabase _db;
    private readonly ILiteCollection<BsonDocument> _images;

    public LiteDbImageStore(IOptions<KinoOptions> options)
        : this(options.Value.ResolveImageDbPath())
    {
    }

    public LiteDbImageStore(string imageDbPath)
    {
        _db = LiteDbFile.OpenShared(imageDbPath, "Kino:ImageDbPath");
        _images = _db.GetCollection(CollectionName);
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken ct = default) =>
        Task.Run(() => _images.FindById(key)?["Data"] is { IsBinary: true } data ? data.AsBinary : null, ct);

    public Task SaveAsync(string key, byte[] data, CancellationToken ct = default) =>
        Task.Run(() => _images.Upsert(new BsonDocument
        {
            ["_id"] = key,
            ["Data"] = data,
            ["Added"] = DateTime.UtcNow,
        }), ct);

    public void Dispose() => _db.Dispose();
}
