using Kolibri.Kino.Core.Abstractions;
using TMDbLib.Client;
using TMDbLib.Objects.Find;

namespace Kolibri.Kino.Data;

/// <summary>
/// Finds a TMDb page by IMDb id with TMDb's "find" request (one request), using the TMDb key from the settings.
/// </summary>
public sealed class TmdbLinks(ISettingsStore settings) : ITmdbLinks, IDisposable
{
    private const string Site = "https://www.themoviedb.org";

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
