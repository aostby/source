using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>What marking a movie watched on the media server did.</summary>
public enum MarkWatchedOutcome
{
    /// <summary>The server doesn't have the movie.</summary>
    NotOnServer,

    /// <summary>It was watched already, so nothing was sent (a second "played" would count another viewing).</summary>
    AlreadyWatched,

    Marked,
}

/// <summary>
/// The movies a media server (Plex) already knows, with IMDb ids. Free to query, so it's tried before TMDb/OMDb.
/// </summary>
public interface IMediaServerLibrary
{
    /// <summary>False when no server/token is configured; lookups then return nothing.</summary>
    bool IsConfigured { get; }

    /// <summary>Reloads the movie list from the server (done at the start of each scan).</summary>
    Task RefreshAsync(CancellationToken ct = default);

    Task<Item?> FindByImdbIdAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The address of the server's poster for the movie, sized for a poster (not the full original); null when it has none.</summary>
    Task<string?> FindPosterAsync(string imdbId, CancellationToken ct = default);

    /// <summary>A movie with exactly this title (or original title), released in <paramref name="year"/> ±1 when given.</summary>
    Task<Item?> FindMovieByTitleAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>The IMDb ids of the movies watched on the server, for its user.</summary>
    Task<IReadOnlyList<string>> GetWatchedImdbIdsAsync(CancellationToken ct = default);

    /// <summary>Marks the movie watched on the server, for its user, unless it is already.</summary>
    Task<MarkWatchedOutcome> MarkWatchedAsync(string imdbId, CancellationToken ct = default);

    /// <summary>
    /// Sets the user's own rating of the movie or series on the server (1–10, half a star each; null clears it). It's
    /// the token's user's private rating. False when the server doesn't have the title.
    /// </summary>
    Task<bool> RateAsync(string imdbId, int? rating, CancellationToken ct = default);

    /// <summary>Movies whose title contains <paramref name="text"/>, for manual lookup.</summary>
    Task<IReadOnlyList<Item>> SearchAsync(string text, CancellationToken ct = default);

    /// <summary>
    /// The movie's or series' page in the server's own web player (Plex Web), where it plays in any format; null
    /// when the server doesn't have it.
    /// </summary>
    Task<string?> GetWebPageAsync(string imdbId, CancellationToken ct = default);

    /// <summary>
    /// Each episode's page in Plex Web for the series, keyed by (season, episode); empty when the server doesn't
    /// have the series.
    /// </summary>
    Task<IReadOnlyDictionary<(int Season, int Episode), string>> GetEpisodeWebPagesAsync(string seriesImdbId, CancellationToken ct = default);
}
