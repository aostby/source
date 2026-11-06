using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Data.Storage;

/// <summary>Backdrops and other art from KMDb (/silverscreen/art/{id}/{type}) while it is in use; none from the LiteDB file.</summary>
public sealed class KmdbArtProvider(KmdbClient kmdb, KmdbConnection connection) : IArtProvider
{
    public async Task<byte[]?> GetArtAsync(string imdbId, string type, CancellationToken ct = default)
    {
        if (!connection.UseKmdb || string.IsNullOrWhiteSpace(imdbId)) return null;
        try
        {
            return await kmdb.GetArtAsync(imdbId, type, ct).ConfigureAwait(false);
        }
        catch (KmdbUnavailableException ex)
        {
            connection.Failed(ex);
            return null;
        }
    }
}
