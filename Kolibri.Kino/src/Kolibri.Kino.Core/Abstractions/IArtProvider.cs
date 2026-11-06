namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// A title's other images besides the poster: backdrop (fanart), landscape, clearlogo … from KMDb's image cache
/// (TMDb, Kodi). Null when there is none, or while the library is in the LiteDB file (it has posters only).
/// </summary>
public interface IArtProvider
{
    /// <param name="imdbId">The title.</param>
    /// <param name="type">backdrop, landscape, clearlogo, clearart, banner, discart, keyart, thumb.</param>
    /// <param name="ct"></param>
    Task<byte[]?> GetArtAsync(string imdbId, string type, CancellationToken ct = default);
}
