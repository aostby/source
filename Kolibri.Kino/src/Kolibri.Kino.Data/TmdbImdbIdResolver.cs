using Kolibri.Kino.Core.Abstractions;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace Kolibri.Kino.Data;

/// <summary>
/// Finds IMDb ids through TMDb's movie and TV search, using the TMDb key from the user settings (no key: returns null).
/// Only accepts a clear match, since a wrong match links the file to the wrong movie.
/// </summary>
public sealed class TmdbImdbIdResolver(ISettingsStore settings) : IImdbIdResolver, IDisposable
{
    private readonly Lock _lock = new();
    private (string Key, TMDbClient Client)? _client;

    public async Task<string?> FindImdbIdAsync(string title, int? year, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var search = await client.SearchMovieAsync(title, page: 0, includeAdult: false, year: year ?? 0, cancellationToken: ct)
            .ConfigureAwait(false);
        if (Pick(search?.Results ?? [], title, year) is not { } match) return null;

        var movie = await client.GetMovieAsync(match.Id, MovieMethods.Undefined, ct).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(movie?.ImdbId) ? null : movie.ImdbId;
    }

    public async Task<string?> FindSeriesImdbIdAsync(string title, int year, CancellationToken ct = default)
    {
        if (Client() is not { } client) return null;

        var search = await client.SearchTvShowAsync(title, page: 0, includeAdult: false, firstAirDateYear: year, cancellationToken: ct)
            .ConfigureAwait(false);
        if (PickSeries(search?.Results ?? [], title, year) is not { } match) return null;

        var ids = await client.GetTvShowExternalIdsAsync(match.Id, ct).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(ids?.ImdbId) ? null : ids.ImdbId;
    }

    /// <summary>Same name (or original name) that first aired in the same year, or one year off.</summary>
    internal static SearchTv? PickSeries(IReadOnlyList<SearchTv> results, string title, int year)
    {
        bool SameTitle(SearchTv s) =>
            string.Equals(s.Name, title, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.OriginalName, title, StringComparison.OrdinalIgnoreCase);

        bool NearYear(SearchTv s, int slack) => s.FirstAirDate?.Year is { } y && Math.Abs(y - year) <= slack;

        return results.FirstOrDefault(s => SameTitle(s) && NearYear(s, 0))
            ?? results.FirstOrDefault(s => SameTitle(s) && NearYear(s, 1));
    }

    /// <summary>
    /// Same title (or original title) in the same year; then one year off (release dates vary by country);
    /// then a single search result close to the year. Unlike SilverScreen, never "most popular that year".
    /// </summary>
    internal static SearchMovie? Pick(IReadOnlyList<SearchMovie> results, string title, int? year)
    {
        bool SameTitle(SearchMovie m) =>
            string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.OriginalTitle, title, StringComparison.OrdinalIgnoreCase);

        bool NearYear(SearchMovie m, int slack) =>
            year is null || (m.ReleaseDate?.Year is { } y && Math.Abs(y - year.Value) <= slack);

        return results.FirstOrDefault(m => SameTitle(m) && NearYear(m, 0))
            ?? results.FirstOrDefault(m => SameTitle(m) && NearYear(m, 1))
            ?? (results.Count == 1 && year is not null && NearYear(results[0], 1) ? results[0] : null);
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
