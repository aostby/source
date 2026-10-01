using System.Globalization;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet;
using TMDbLib.Client;
using TMDbLib.Objects.Find;

namespace Kolibri.Kino.Data;

/// <summary>
/// Season and episode details. TMDb when a TMDb key is set (find the show by IMDb id, then one request per season,
/// with plots, air dates, ratings and screenshots); otherwise OMDb (one request per season, titles and ratings).
/// SilverScreen requested every episode from OMDb separately; neither path does that.
/// </summary>
public sealed class SeriesInfoProvider(ISettingsStore settings) : ISeriesInfoProvider, IDisposable
{
    private const string StillBaseUrl = "https://image.tmdb.org/t/p/w300";

    private readonly Lock _lock = new();
    private (string Key, TMDbClient Client)? _tmdb;

    public async Task<IReadOnlyList<SeriesSeason>> GetSeasonsAsync(string seriesImdbId, int totalSeasonsHint, CancellationToken ct = default)
    {
        if (TmdbClient() is { } tmdb)
        {
            var seasons = await FromTmdbAsync(tmdb, seriesImdbId, ct).ConfigureAwait(false);
            if (seasons is not null) return seasons;
        }
        return await FromOmdbAsync(seriesImdbId, totalSeasonsHint, ct).ConfigureAwait(false);
    }

    /// <summary>Null when TMDb doesn't know the IMDb id (then OMDb is tried).</summary>
    private static async Task<IReadOnlyList<SeriesSeason>?> FromTmdbAsync(TMDbClient client, string imdbId, CancellationToken ct)
    {
        var found = await client.FindAsync(FindExternalSource.Imdb, imdbId, ct).ConfigureAwait(false);
        if (found?.TvResults?.FirstOrDefault() is not { } tv) return null;

        var show = await client.GetTvShowAsync(tv.Id, cancellationToken: ct).ConfigureAwait(false);
        if (show?.Seasons is null) return null;

        var seasons = new List<SeriesSeason>();
        foreach (var listed in show.Seasons.Where(s => s.EpisodeCount > 0))
        {
            ct.ThrowIfCancellationRequested();
            var season = await client.GetTvSeasonAsync(tv.Id, listed.SeasonNumber, cancellationToken: ct).ConfigureAwait(false);
            if (season?.Episodes is null) continue;

            seasons.Add(new SeriesSeason(
                listed.SeasonNumber,
                season.Name,
                season.Episodes.Select(e => new SeriesEpisode(
                    listed.SeasonNumber,
                    (int)e.EpisodeNumber,
                    string.IsNullOrWhiteSpace(e.Name) ? $"Episode {e.EpisodeNumber}" : e.Name,
                    e.AirDate,
                    string.IsNullOrWhiteSpace(e.Overview) ? null : e.Overview,
                    e.VoteAverage > 0 ? Math.Round(e.VoteAverage, 1) : null,
                    e.Runtime,
                    string.IsNullOrEmpty(e.StillPath) ? null : StillBaseUrl + e.StillPath,
                    null)).ToList(),
                "TMDb"));
        }
        return seasons.OrderBy(s => s.SeasonNumber == 0 ? int.MaxValue : s.SeasonNumber).ToList();
    }

    private async Task<IReadOnlyList<SeriesSeason>> FromOmdbAsync(string imdbId, int totalSeasonsHint, CancellationToken ct)
    {
        var key = settings.Current.OMDBkey;
        if (string.IsNullOrWhiteSpace(key))
            throw new MovieInfoUnavailableException("No TMDb or OMDb key. Set one in Settings to get episode details.");

        var client = new AsyncOmdbClient(key, false);
        var seasons = new List<SeriesSeason>();
        var total = Math.Max(1, totalSeasonsHint);
        for (var number = 1; number <= total; number++)
        {
            OMDbApiNet.Model.Season? season;
            try
            {
                season = await client.GetSeasonBySeriesIdAsync(imdbId, number).WaitAsync(ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex) when (ex.StatusCode is null && ex.InnerException is null)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)) continue;
                throw new MovieInfoUnavailableException($"OMDb: {ex.Message}", ex);
            }
            catch (NullReferenceException ex) when (OmdbErrors.IsLibraryCrash(ex))
            {
                throw new MovieInfoUnavailableException($"OMDb: {await OmdbErrors.ExplainAsync(key, ct).ConfigureAwait(false)}", ex);
            }
            if (season?.Episodes is null) continue;

            // The hint may be missing or stale; OMDb reports the real count with the first season.
            if (number == 1 && int.TryParse(season.TotalSeasons, out var reported)) total = Math.Max(total, reported);

            seasons.Add(new SeriesSeason(number, null, season.Episodes
                .Where(e => int.TryParse(e.Episode, out _))
                .Select(e => new SeriesEpisode(
                    number,
                    int.Parse(e.Episode, CultureInfo.InvariantCulture),
                    e.Title ?? $"Episode {e.Episode}",
                    DateTime.TryParse(e.Released, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var aired) ? aired : null,
                    null,
                    double.TryParse(e.ImdbRating, NumberStyles.Float, CultureInfo.InvariantCulture, out var rating) ? rating : null,
                    null,
                    null,
                    e.ImdbId))
                .OrderBy(e => e.EpisodeNumber)
                .ToList(), "OMDb"));
        }
        return seasons;
    }

    private TMDbClient? TmdbClient()
    {
        var key = settings.Current.TMDBkey;
        if (string.IsNullOrWhiteSpace(key)) return null;

        lock (_lock)
        {
            if (_tmdb is { } c && c.Key == key) return c.Client;
            _tmdb?.Client.Dispose();
            var client = new TMDbClient(key);
            _tmdb = (key, client);
            return client;
        }
    }

    public void Dispose() => _tmdb?.Client.Dispose();
}
