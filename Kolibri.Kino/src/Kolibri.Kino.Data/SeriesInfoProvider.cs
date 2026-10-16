using System.Globalization;
using System.Net;
using System.Text.Json;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet;

namespace Kolibri.Kino.Data;

/// <summary>
/// Season and episode details. TMDb when a TMDb key is set: the show with its seasons in one request (TMDb's
/// append_to_response, up to 20 seasons per request), after finding the show by IMDb id unless its TMDb id is known
/// from before. Plots, air dates, ratings and screenshots. Otherwise OMDb (one request per season, titles and ratings).
/// SilverScreen requested every episode from OMDb separately; neither path does that.
/// </summary>
public sealed class SeriesInfoProvider : ISeriesInfoProvider, IDisposable
{
    private const string StillBaseUrl = "https://image.tmdb.org/t/p/w300";
    private const string TmdbApi = "https://api.themoviedb.org/3/";

    /// <summary>TMDb answers at most this many append_to_response parts per request.</summary>
    private const int SeasonsPerRequest = 20;

    private readonly ISettingsStore _settings;
    private readonly HttpClient _http;

    public SeriesInfoProvider(ISettingsStore settings)
        : this(settings, new HttpClient())
    {
    }

    /// <summary>For tests: TMDb's answers come from <paramref name="http"/>.</summary>
    internal SeriesInfoProvider(ISettingsStore settings, HttpClient http)
    {
        _settings = settings;
        _http = http;
    }

    public async Task<SeriesFetchResult> GetSeasonsAsync(string seriesImdbId, int totalSeasonsHint, int? tmdbId = null, int fromSeason = 0, CancellationToken ct = default)
    {
        if (_settings.Current.TMDBkey is { Length: > 0 } key)
        {
            var id = tmdbId ?? await FindTmdbIdAsync(key, seriesImdbId, ct).ConfigureAwait(false);
            if (id is { } found)
            {
                var seasons = await FromTmdbAsync(key, found, totalSeasonsHint, fromSeason, ct).ConfigureAwait(false);
                if (seasons is not null) return new SeriesFetchResult(seasons, found);
            }
        }
        return new SeriesFetchResult(await FromOmdbAsync(seriesImdbId, totalSeasonsHint, fromSeason, ct).ConfigureAwait(false), null);
    }

    /// <summary>TMDb's id for the IMDb id, or null when TMDb doesn't know it (then OMDb is tried).</summary>
    private async Task<int?> FindTmdbIdAsync(string key, string imdbId, CancellationToken ct)
    {
        using var doc = await GetTmdbAsync(key, $"find/{Uri.EscapeDataString(imdbId)}?external_source=imdb_id", ct).ConfigureAwait(false);
        return doc?.RootElement.TryGetProperty("tv_results", out var results) == true
            && results.ValueKind == JsonValueKind.Array && results.GetArrayLength() > 0
            && results[0].TryGetProperty("id", out var id) && id.TryGetInt32(out var value)
            ? value
            : null;
    }

    /// <summary>
    /// The show and its seasons: the first request asks for the specials and the seasons OMDb says there are (up to 20 in all); seasons the show
    /// lists that weren't in it are asked for 20 at a time. Null when TMDb doesn't have the show.
    /// </summary>
    private async Task<IReadOnlyList<SeriesSeason>?> FromTmdbAsync(string key, int tvId, int totalSeasonsHint, int fromSeason, CancellationToken ct)
    {
        // Specials (season 0) too, as the episode window shows them, unless only later seasons are wanted;
        // a season the show doesn't have is left out by TMDb.
        var first = fromSeason > 1 ? fromSeason : 0;
        var guess = Enumerable.Range(first, Math.Clamp(Math.Max(totalSeasonsHint, first) - first + 1, 1, SeasonsPerRequest)).ToList();
        using var show = await GetTmdbAsync(key, ShowPath(tvId, guess), ct).ConfigureAwait(false)
            ?? await GetTmdbAsync(key, ShowPath(tvId, []), ct).ConfigureAwait(false);
        if (show is null || !show.RootElement.TryGetProperty("seasons", out var listed) || listed.ValueKind != JsonValueKind.Array) return null;

        var numbers = listed.EnumerateArray()
            .Where(s => Int(s, "episode_count") > 0)
            .Select(s => Int(s, "season_number"))
            .OfType<int>()
            .Where(n => n >= first)
            .ToList();

        var seasons = new List<SeriesSeason>();
        AddSeasons(show.RootElement, numbers, seasons);

        var rest = numbers.Where(n => seasons.All(s => s.SeasonNumber != n)).ToList();
        foreach (var chunk in rest.Chunk(SeasonsPerRequest))
        {
            using var more = await GetTmdbAsync(key, ShowPath(tvId, chunk), ct).ConfigureAwait(false);
            if (more is not null) AddSeasons(more.RootElement, chunk, seasons);
        }
        return seasons.OrderBy(s => s.SeasonNumber == 0 ? int.MaxValue : s.SeasonNumber).ToList();
    }

    private static string ShowPath(int tvId, IReadOnlyCollection<int> seasons) =>
        seasons.Count == 0 ? $"tv/{tvId}" : $"tv/{tvId}?append_to_response={string.Join(',', seasons.Select(n => $"season/{n}"))}";

    /// <summary>The appended "season/N" parts of a show answer that are among <paramref name="wanted"/>.</summary>
    private static void AddSeasons(JsonElement show, IEnumerable<int> wanted, List<SeriesSeason> into)
    {
        foreach (var number in wanted)
        {
            if (!show.TryGetProperty($"season/{number}", out var season) || !season.TryGetProperty("episodes", out var episodes)
                || episodes.ValueKind != JsonValueKind.Array)
                continue;

            into.Add(new SeriesSeason(
                number,
                Str(season, "name"),
                episodes.EnumerateArray()
                    .Where(e => Int(e, "episode_number") is not null)
                    .Select(e =>
                    {
                        var episode = Int(e, "episode_number")!.Value;
                        var vote = e.TryGetProperty("vote_average", out var v) && v.TryGetDouble(out var d) ? d : 0;
                        return new SeriesEpisode(
                            number,
                            episode,
                            Str(e, "name") ?? $"Episode {episode}",
                            DateTime.TryParse(Str(e, "air_date"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var aired) ? aired : null,
                            Str(e, "overview"),
                            vote > 0 ? Math.Round(vote, 1) : null,
                            Int(e, "runtime"),
                            Str(e, "still_path") is { } still ? StillBaseUrl + still : null,
                            null);
                    })
                    .OrderBy(e => e.EpisodeNumber)
                    .ToList(),
                "TMDb"));
        }
    }

    /// <summary>A TMDb answer, or null when TMDb doesn't have it (404) or rejects the request (e.g. an unknown season appended).</summary>
    private async Task<JsonDocument?> GetTmdbAsync(string key, string path, CancellationToken ct)
    {
        var url = $"{TmdbApi}{path}{(path.Contains('?') ? '&' : '?')}api_key={Uri.EscapeDataString(key)}";
        using var response = await _http.GetAsync(url, ct).ConfigureAwait(false);
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity) return null;
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new MovieInfoUnavailableException("TMDb rejected the key. Check the TMDb key in Settings.");
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        return await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);
    }

    private static string? Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String && v.GetString() is { Length: > 0 } s ? s : null;

    private static int? Int(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i) ? i : null;

    private async Task<IReadOnlyList<SeriesSeason>> FromOmdbAsync(string imdbId, int totalSeasonsHint, int fromSeason, CancellationToken ct)
    {
        var key = _settings.Current.OMDBkey;
        if (string.IsNullOrWhiteSpace(key))
            throw new MovieInfoUnavailableException("No TMDb or OMDb key. Set one in Settings to get episode details.");

        var client = new AsyncOmdbClient(key, false);
        var seasons = new List<SeriesSeason>();
        var start = Math.Max(1, fromSeason);
        var total = Math.Max(start, totalSeasonsHint);
        for (var number = start; number <= total; number++)
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

            // The hint may be missing or stale; OMDb reports the real count with the first season asked for.
            if (number == start && int.TryParse(season.TotalSeasons, out var reported)) total = Math.Max(total, reported);

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

    public void Dispose() => _http.Dispose();
}
