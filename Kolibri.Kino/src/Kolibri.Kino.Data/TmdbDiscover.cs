using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// TMDb's /discover/movie and /discover/tv, asked directly: "|" between keywords means any of them, which TMDbLib
/// doesn't offer for TV. The lists have no IMDb ids; each title's is asked for once and kept for good in
/// "KinoTmdbImdb" (_id = "movie:{tmdb id}" or "tv:{tmdb id}", ImdbId, empty when TMDb has none).
/// </summary>
public sealed class TmdbDiscover(ISettingsStore settings, HttpClient http, KinoDatabase db) : IDiscoverSource
{
    private const string Api = "https://api.themoviedb.org/3/";
    private const string PosterBase = "https://image.tmdb.org/t/p/w342";

    /// <summary>Fewest votes a title needs: few for the popular and newest lists, more for best rated (a few 10s mean little).</summary>
    private const int MinVotes = 20;
    private const int MinVotesBestRated = 200;

    private readonly ILiteCollection<BsonDocument> _imdbIds = db.Database.GetCollection("KinoTmdbImdb");
    private readonly ConcurrentDictionary<bool, IReadOnlyList<DiscoverGenre>> _genres = new();

    public async Task<DiscoverPage> DiscoverAsync(DiscoverRequest request, CancellationToken ct = default)
    {
        if (Key() is not { } key) return new DiscoverPage([], request.Page, 0, 0);

        using var json = await GetAsync(Address(request, key, DateTime.UtcNow), ct).ConfigureAwait(false);
        var root = json.RootElement;
        var kind = request.Series ? "tv" : "movie";
        var results = root.TryGetProperty("results", out var r) ? r.EnumerateArray().ToList() : [];

        var items = await Task.WhenAll(results.Select(async t =>
        {
            var id = t.GetProperty("id").GetInt32();
            var title = Str(t, request.Series ? "name" : "title") ?? Str(t, request.Series ? "original_name" : "original_title") ?? $"TMDb {id}";
            var date = Str(t, request.Series ? "first_air_date" : "release_date");
            var votes = t.TryGetProperty("vote_count", out var vc) ? vc.GetInt32() : 0;
            double? score = votes > 0 && t.TryGetProperty("vote_average", out var va) ? Math.Round(va.GetDouble(), 1, MidpointRounding.AwayFromZero) : null;
            return new TrendingItem(id, title, date is { Length: >= 4 } ? date[..4] : null,
                await ImdbIdAsync(kind, id, key, ct).ConfigureAwait(false),
                Str(t, "poster_path") is { } poster ? PosterBase + poster : null, score, Str(t, "overview"), []);
        })).ConfigureAwait(false);

        return new DiscoverPage(items,
            root.TryGetProperty("page", out var p) ? p.GetInt32() : request.Page,
            root.TryGetProperty("total_pages", out var tp) ? tp.GetInt32() : 0,
            root.TryGetProperty("total_results", out var tr) ? tr.GetInt32() : 0);
    }

    /// <summary>The /discover address for the request (internal for tests). Dates are whole years; Newest stops at today.</summary>
    internal static string Address(DiscoverRequest request, string key, DateTime todayUtc)
    {
        var series = request.Series;
        var dateField = series ? "first_air_date" : "primary_release_date";
        var query = new List<string>
        {
            $"api_key={Uri.EscapeDataString(key)}",
            "include_adult=false",
            "language=en-US",
            $"page={request.Page}",
            $"with_keywords={string.Join("|", request.KeywordIds)}",
            "sort_by=" + request.Sort switch
            {
                DiscoverSort.BestRated => "vote_average.desc",
                DiscoverSort.Newest => $"{dateField}.desc",
                _ => "popularity.desc",
            },
            $"vote_count.gte={(request.Sort == DiscoverSort.BestRated ? MinVotesBestRated : MinVotes)}",
        };
        if (request.GenreId is { } genre) query.Add($"with_genres={genre}");
        if (request.YearFrom is { } from) query.Add($"{dateField}.gte={from}-01-01");
        var to = request.YearTo is { } y ? $"{y}-12-31" : null;
        // Newest: nothing that hasn't come out yet.
        if (request.Sort == DiscoverSort.Newest)
        {
            var today = todayUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            to = to is null || string.CompareOrdinal(today, to) < 0 ? today : to;
        }
        if (to is not null) query.Add($"{dateField}.lte={to}");
        if (request.MinRating is { } min) query.Add($"vote_average.gte={min.ToString(CultureInfo.InvariantCulture)}");
        return $"{Api}discover/{(series ? "tv" : "movie")}?{string.Join("&", query)}";
    }

    public async Task<IReadOnlyList<DiscoverGenre>> GetGenresAsync(bool series, CancellationToken ct = default)
    {
        if (_genres.TryGetValue(series, out var known)) return known;
        if (Key() is not { } key) return [];

        using var json = await GetAsync($"{Api}genre/{(series ? "tv" : "movie")}/list?language=en-US&api_key={Uri.EscapeDataString(key)}", ct).ConfigureAwait(false);
        var genres = json.RootElement.GetProperty("genres").EnumerateArray()
            .Select(g => new DiscoverGenre(g.GetProperty("id").GetInt32(), Str(g, "name") ?? ""))
            .Where(g => g.Name.Length > 0)
            .OrderBy(g => g.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        return _genres[series] = genres;
    }

    /// <summary>The title's IMDb id: from KinoTmdbImdb, else asked once (/external_ids) and kept. Null when TMDb has none.</summary>
    private async Task<string?> ImdbIdAsync(string kind, int tmdbId, string key, CancellationToken ct)
    {
        var id = $"{kind}:{tmdbId}";
        if (_imdbIds.FindById(id) is { } known) return known["ImdbId"].AsString is { Length: > 0 } imdb ? imdb : null;

        using var json = await GetAsync($"{Api}{kind}/{tmdbId}/external_ids?api_key={Uri.EscapeDataString(key)}", ct).ConfigureAwait(false);
        var found = Str(json.RootElement, "imdb_id");
        _imdbIds.Upsert(new BsonDocument { ["_id"] = id, ["ImdbId"] = found ?? "" });
        return found;
    }

    private async Task<JsonDocument> GetAsync(string url, CancellationToken ct)
    {
        using var response = await http.GetAsync(url, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
    }

    private string? Key() => string.IsNullOrWhiteSpace(settings.Current.TMDBkey) ? null : settings.Current.TMDBkey.Trim();

    private static string? Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String && v.GetString() is { Length: > 0 } s ? s.Trim() : null;
}
