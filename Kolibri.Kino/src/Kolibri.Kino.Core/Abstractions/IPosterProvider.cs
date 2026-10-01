using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// Poster images, cached locally after the first download.
/// </summary>
public interface IPosterProvider
{
    /// <summary>The encoded image (usually JPEG), or null if the item has no poster.</summary>
    Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default);
}
