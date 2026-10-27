using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Trending;

namespace Kolibri.Kino.Data;

/// <summary>
/// TMDb's trending lists for the week (/trending/movie|tv|person/week), first page (20), using the TMDb key from the
/// settings. The lists have no IMDb ids, so each new movie or series costs one more request for them.
/// </summary>
public sealed class TmdbTrending(ISettingsStore settings) : ITrendingSource, IDisposable
{
    /// <summary>TMDb's image server: w342 is a poster a little larger than a card; w185 for a person's photo.</summary>
    private const string PosterBase = "https://image.tmdb.org/t/p/w342";
    private const string PhotoBase = "https://image.tmdb.org/t/p/w185";

    private readonly Lock _lock = new();
    private (string Key, TMDbClient Client)? _client;

    public async Task<IReadOnlyList<TrendingItem>> GetTrendingAsync(
        TrendingKind kind, IReadOnlyDictionary<int, string> knownImdbIds, CancellationToken ct = default)
    {
        if (Client() is not { } client) return [];

        switch (kind)
        {
            case TrendingKind.Movies:
            {
                var list = ((await client.GetTrendingMoviesAsync(TimeWindow.Week, 0, null, ct).ConfigureAwait(false))?.Results ?? []).Where(m => !m.Adult);
                return await Task.WhenAll(list.Select(async m => new TrendingItem(
                    m.Id, m.Title ?? m.OriginalTitle ?? $"TMDb {m.Id}", YearOf(m.ReleaseDate),
                    knownImdbIds.GetValueOrDefault(m.Id) ?? Clean((await client.GetMovieExternalIdsAsync(m.Id, ct).ConfigureAwait(false))?.ImdbId),
                    Image(PosterBase, m.PosterPath), Score(m.VoteAverage, m.VoteCount), Clean(m.Overview), []))).ConfigureAwait(false);
            }
            case TrendingKind.Series:
            {
                var list = (await client.GetTrendingTvAsync(TimeWindow.Week, 0, null, ct).ConfigureAwait(false))?.Results ?? [];
                return await Task.WhenAll(list.Select(async s => new TrendingItem(
                    s.Id, s.Name ?? s.OriginalName ?? $"TMDb {s.Id}", YearOf(s.FirstAirDate),
                    knownImdbIds.GetValueOrDefault(s.Id) ?? Clean((await client.GetTvShowExternalIdsAsync(s.Id, ct).ConfigureAwait(false))?.ImdbId),
                    Image(PosterBase, s.PosterPath), Score(s.VoteAverage, s.VoteCount), Clean(s.Overview), []))).ConfigureAwait(false);
            }
            default:
            {
                // Adult film performers are on TMDb's list too; left out, as the movies' adult titles are.
                var list = ((await client.GetTrendingPeopleAsync(TimeWindow.Week, 0, null, ct).ConfigureAwait(false))?.Results ?? []).Where(p => !p.Adult);
                return list.Select(p => new TrendingItem(
                    p.Id, p.Name ?? $"TMDb {p.Id}", null, null, Image(PhotoBase, p.ProfilePath), null, null,
                    (p.KnownFor ?? []).Select(KnownForTitle).OfType<string>().ToList())).ToList();
            }
        }
    }

    private static string? KnownForTitle(KnownForBase k) => k switch
    {
        KnownForMovie m => Clean(m.Title ?? m.OriginalTitle),
        KnownForTv t => Clean(t.Name ?? t.OriginalName),
        _ => null,
    };

    private static string? YearOf(DateTime? date) => date?.Year.ToString(CultureInfo.InvariantCulture);

    private static double? Score(double average, int votes) => votes > 0 ? Math.Round(average, 1, MidpointRounding.AwayFromZero) : null;

    private static string? Image(string baseUrl, string? path) => string.IsNullOrEmpty(path) ? null : $"{baseUrl}{path}";

    private static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

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
