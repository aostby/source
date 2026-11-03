namespace Kolibri.Kino.Core.Abstractions;

/// <summary>A movie TMDb suggests as like another one, with its IMDb rating when it was looked up (null if none).</summary>
public sealed record SimilarHit(TmdbMovieHit Movie, string? ImdbRating);

/// <summary>A movie's similar movies, as last fetched from TMDb.</summary>
public sealed record CachedSimilar(string SourceImdbId, IReadOnlyList<SimilarHit> Hits, DateTime FetchedUtc);

/// <summary>
/// Similar movies kept per movie, so the details page doesn't ask TMDb each time and Suggestions for you can be made
/// without asking it at all.
/// </summary>
public interface ISimilarCache
{
    Task<CachedSimilar?> GetAsync(string sourceImdbId, CancellationToken ct = default);

    /// <summary>The kept lists of the given movies; those without one are left out.</summary>
    Task<IReadOnlyList<CachedSimilar>> GetManyAsync(IEnumerable<string> sourceImdbIds, CancellationToken ct = default);

    Task SaveAsync(CachedSimilar similar, CancellationToken ct = default);
}
