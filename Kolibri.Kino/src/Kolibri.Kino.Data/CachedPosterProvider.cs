using Kolibri.Kino.Core.Abstractions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// Posters from the image cache (keyed by IMDb id, or by poster URL as SilverScreen does); downloads on a miss
/// and stores the bytes as-is, so there is no re-encoding and no growth on repeat lookups.
/// </summary>
public sealed class CachedPosterProvider(LiteDbImageStore store, HttpClient http) : IPosterProvider
{
    public async Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(item.ImdbId)) return null;

        if (await store.GetAsync(item.ImdbId, ct).ConfigureAwait(false) is { } cached)
            return cached;

        if (!Uri.TryCreate(item.Poster, UriKind.Absolute, out var url) || url.Scheme is not ("http" or "https"))
            return null;

        // SilverScreen mostly caches posters under their URL.
        if (await store.GetAsync(item.Poster!, ct).ConfigureAwait(false) is { } cachedByUrl)
            return cachedByUrl;

        using var response = await http.GetAsync(url, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) return null;

        var bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
        await store.SaveAsync(item.ImdbId, bytes, ct).ConfigureAwait(false);
        return bytes;
    }
}
