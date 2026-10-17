using Kolibri.Kino.Core.Abstractions;
using TMDbLib.Client;
using TMDbLib.Objects.Find;

namespace Kolibri.Kino.Data;

/// <summary>
/// Finds a TMDb page or poster by IMDb id with TMDb's "find" request (one request), using the TMDb key from the settings.
/// </summary>
public sealed class TmdbLinks(ISettingsStore settings) : ITmdbLinks, IDisposable
{
    private const string Site = "https://www.themoviedb.org";

    /// <summary>TMDb's image server; w500 is a poster about as large as OMDb's.</summary>
    private const string PosterBase = "https://image.tmdb.org/t/p/w500";

    private readonly Lock _lock = new();
    private (string Key, TMDbClient Client)? _client;

    public async Task<string?> FindPageAsync(string imdbId, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        if (found?.MovieResults?.FirstOrDefault() is { } movie) return $"{Site}/movie/{movie.Id}";
        if (found?.TvResults?.FirstOrDefault() is { } tv) return $"{Site}/tv/{tv.Id}";
        if (found?.TvEpisode?.FirstOrDefault() is { } episode)
            return $"{Site}/tv/{episode.ShowId}/season/{episode.SeasonNumber}/episode/{episode.EpisodeNumber}";
        return null;
    }

    public async Task<string?> FindPosterAsync(string imdbId, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        var path = found?.MovieResults?.FirstOrDefault()?.PosterPath ?? found?.TvResults?.FirstOrDefault()?.PosterPath;
        return string.IsNullOrEmpty(path) ? null : $"{PosterBase}{path}";
    }

    /// <summary>A small poster (TMDb's w185) for lists of suggestions.</summary>
    private const string SmallPosterBase = "https://image.tmdb.org/t/p/w185";

    public async Task<IReadOnlyList<TmdbMovieHit>> FindSimilarAsync(string imdbId, int max = 12, CancellationToken ct = default)
    {
        if (Client() is not { } client) return [];

        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        if (found?.MovieResults?.FirstOrDefault() is not { } movie) return [];

        var suggested = (await client.GetMovieRecommendationsAsync(movie.Id, page: 0, ct).ConfigureAwait(false))?.Results;
        if (suggested is not { Count: > 0 })
            suggested = (await client.GetMovieSimilarAsync(movie.Id, page: 0, ct).ConfigureAwait(false))?.Results;

        // The lists have no IMDb ids: one request per movie for it, at the same time.
        var hits = await Task.WhenAll((suggested ?? []).Take(max).Select(async m =>
        {
            var ids = await client.GetMovieExternalIdsAsync(m.Id, ct).ConfigureAwait(false);
            return string.IsNullOrWhiteSpace(ids?.ImdbId) ? null
                : new TmdbMovieHit(ids.ImdbId, m.Title ?? m.OriginalTitle ?? ids.ImdbId,
                    m.ReleaseDate?.Year.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    string.IsNullOrEmpty(m.PosterPath) ? null : $"{SmallPosterBase}{m.PosterPath}");
        })).ConfigureAwait(false);
        return hits.OfType<TmdbMovieHit>().ToList();
    }

    public async Task<string?> FindTitleAsync(string imdbId, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        var title = found?.MovieResults?.FirstOrDefault()?.Title ?? found?.TvResults?.FirstOrDefault()?.Name;
        return string.IsNullOrWhiteSpace(title) ? null : title.Trim();
    }

    public async Task<(double Rating, int Votes)?> FindRatingAsync(string imdbId, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        (double Rating, int Votes)? score =
            found?.MovieResults?.FirstOrDefault() is { } movie ? (movie.VoteAverage, movie.VoteCount)
            : found?.TvResults?.FirstOrDefault() is { } tv ? (tv.VoteAverage, tv.VoteCount)
            : null;
        return score is { Votes: > 0 } ? score : null;
    }

    private TMDbClient? Client()
    {
        var key = settings.Current.TMDBkey;
        if (string.IsNullOrWhiteSpace(key)) return null;

        lock (_lock)
        {
            if (_client is { } c && c.Key == key) return c.Client;
            _client?.Client.Dispose();
            var client = new TMDbClient(key);
            _client = (key, client);
            return client;
        }
    }

    public void Dispose() => _client?.Client.Dispose();
}
