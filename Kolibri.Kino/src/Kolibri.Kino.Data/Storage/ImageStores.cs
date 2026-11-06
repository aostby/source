using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Data.Storage;

/// <summary>The image cache: posters, stills and photos by key (an IMDb id, tt…_S01E02, tmdb-person-123 or an address).</summary>
public interface IImageStore
{
    Task<byte[]?> GetAsync(string key, CancellationToken ct = default);

    Task SaveAsync(string key, byte[] data, CancellationToken ct = default);
}

/// <summary>
/// The image cache in KMDb while that is in use (an IMDb id there finds the best poster, from any source, and KMDb
/// can fetch one it lacks), else the image file (SilverScreen.imgdb). Images got from KMDb are kept in the file too,
/// and new ones go to both, so the file stays a usable fallback.
/// </summary>
public sealed class SwitchingImageStore(LiteDbImageStore file, KmdbClient kmdb, KmdbConnection connection, ILogger<SwitchingImageStore> log) : IImageStore
{
    public async Task<byte[]?> GetAsync(string key, CancellationToken ct = default)
    {
        if (connection.UseKmdb)
        {
            try
            {
                if (await kmdb.GetImageAsync(key, ct).ConfigureAwait(false) is { } data)
                {
                    if (await file.GetAsync(key, ct).ConfigureAwait(false) is null) await file.SaveAsync(key, data, ct).ConfigureAwait(false);
                    return data;
                }
            }
            catch (KmdbUnavailableException ex)
            {
                connection.Failed(ex);
            }
        }
        return await file.GetAsync(key, ct).ConfigureAwait(false);
    }

    public async Task SaveAsync(string key, byte[] data, CancellationToken ct = default)
    {
        await file.SaveAsync(key, data, ct).ConfigureAwait(false);
        if (!connection.UseKmdb) return;
        try
        {
            await kmdb.PutImageAsync(key, data, ct).ConfigureAwait(false);
        }
        catch (KmdbUnavailableException ex)
        {
            // An image KMDb misses is no loss: it fetches posters itself, and the next save sends it again.
            log.LogInformation("Image {Key} not sent to KMDb: {Message}", key, ex.Message);
        }
    }
}
