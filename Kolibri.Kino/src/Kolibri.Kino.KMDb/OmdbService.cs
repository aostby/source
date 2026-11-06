using System.Text.RegularExpressions;
using OMDbApiNet.Model;

namespace Kolibri.Kino.KMDb;

/// <summary>The query parameters of OMDb's API (https://www.omdbapi.com/#parameters). Names are case-insensitive.</summary>
public sealed record OmdbRequest
{
    /// <summary>An IMDb id (tt0133093).</summary>
    public string? I { get; init; }
    /// <summary>A title to look up (best match).</summary>
    public string? T { get; init; }
    /// <summary>A title to search for (10 per page).</summary>
    public string? S { get; init; }
    /// <summary>movie, series, episode or game.</summary>
    public string? Type { get; init; }
    /// <summary>Year of release (the first year for a series).</summary>
    public int? Y { get; init; }
    /// <summary>Season number; with i= or t= of a series.</summary>
    public int? Season { get; init; }
    /// <summary>Episode number; with Season.</summary>
    public int? Episode { get; init; }
    /// <summary>Page of a search, 1–100.</summary>
    public int? Page { get; init; }
}

/// <summary>
/// Answers OMDb requests from the IMDb database with OMDbApiNet's models (<see cref="Item"/>, <see cref="OMDbApiNet.Model.Episode"/>,
/// <see cref="Season"/>, <see cref="SearchList"/>), so a client written for OMDb reads the answers as they are.
/// </summary>
/// <remarks>
/// IMDb's data sets have no plot, poster, age rating, release date, country, language, awards or Metascore: those
/// fields are "N/A" unless the SilverScreen library has the title (<see cref="SilverScreenEnricher"/>). Actors come
/// from title_principals, which holds the cast only for some titles.
/// </remarks>
public sealed partial class OmdbService(IImdbRepository db, SilverScreenEnricher? library = null, KmdbMetrics? metrics = null)
{
    public const int PageSize = 10;
    private const int MaxPeople = 5;
    private const int MaxActors = 3;

    /// <param name="request">OMDb's parameters.</param>
    /// <param name="baseUrl">This service's address as the client sees it (http://greenlantern:8086), for poster links.</param>
    /// <param name="ct"></param>
    public async Task<object> GetAsync(OmdbRequest request, string? baseUrl = null, CancellationToken ct = default)
    {
        var kind = Kind(request);
        try
        {
            var answer = await AnswerWithLibraryAsync(request, baseUrl, ct).ConfigureAwait(false);
            metrics?.OmdbRequest(kind, answer is ErrorResponse ? "not_found" : "found");
            return answer;
        }
        catch
        {
            metrics?.OmdbRequest(kind, "error");
            throw;
        }
    }

    /// <summary>What a request asks for, for the metrics: search, episode, season, id or title.</summary>
    internal static string Kind(OmdbRequest r) =>
        !string.IsNullOrWhiteSpace(r.S) ? "search"
        : r.Season is not null && r.Episode is not null ? "episode"
        : r.Season is not null ? "season"
        : !string.IsNullOrWhiteSpace(r.I) ? "id"
        : "title";

    private async Task<object> AnswerWithLibraryAsync(OmdbRequest request, string? baseUrl, CancellationToken ct)
    {
        var answer = await AnswerAsync(request, ct).ConfigureAwait(false);
        if (library is null) return answer;

        // A title IMDb's files don't have yet (newer than the last import) may be in the library.
        if (answer is ErrorResponse { Error: "Error getting data." } && request.I?.Trim() is { } id && request.Season is null
            && await library.ItemOnlyInLibraryAsync(id, baseUrl, ct).ConfigureAwait(false) is { } item)
            return item;
        return await library.EnrichAsync(answer, baseUrl, ct).ConfigureAwait(false);
    }

    private async Task<object> AnswerAsync(OmdbRequest request, CancellationToken ct)
    {
        string[]? types = null;
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            if (!OmdbFormat.TitleTypes.TryGetValue(request.Type.Trim(), out types))
                return OmdbFormat.Error("Invalid type! Use movie, series, episode or game.");
        }

        if (!string.IsNullOrWhiteSpace(request.S))
            return await SearchAsync(request.S.Trim(), types ?? OmdbFormat.DefaultSearchTypes, request.Y, request.Page ?? 1, ct).ConfigureAwait(false);

        TitleRow? title;
        if (!string.IsNullOrWhiteSpace(request.I))
        {
            var id = request.I.Trim();
            if (!IsImdbId(id)) return OmdbFormat.Error("Incorrect IMDb ID.");
            title = await db.GetTitleAsync(id, ct).ConfigureAwait(false);
            if (title is null) return OmdbFormat.Error("Error getting data.");
        }
        else if (!string.IsNullOrWhiteSpace(request.T))
        {
            var lookupTypes = request.Season is null ? types ?? OmdbFormat.AllTypes : OmdbFormat.TitleTypes["series"];
            title = await db.FindByTitleAsync(request.T.Trim(), lookupTypes, request.Y, ct).ConfigureAwait(false);
            if (title is null) return OmdbFormat.Error(request.Season is null ? "Movie not found!" : "Series or season not found!");
        }
        else
        {
            return OmdbFormat.Error("Incorrect IMDb ID.");
        }

        if (request.Season is { } season)
        {
            if (OmdbFormat.OmdbType(title.TitleType) != "series")
                return OmdbFormat.Error(request.Episode is null ? "Series or season not found!" : "Series or episode not found!");
            if (request.Episode is { } number)
            {
                var episodeId = await db.EpisodeIdAsync(title.Id, season, number, ct).ConfigureAwait(false);
                var episode = episodeId is null ? null : await db.GetTitleAsync(episodeId, ct).ConfigureAwait(false);
                return episode is null
                    ? OmdbFormat.Error("Series or episode not found!")
                    : await EpisodeAsync(episode, new EpisodeLink(title.Id, season, number), ct).ConfigureAwait(false);
            }
            return await SeasonAsync(title, season, ct).ConfigureAwait(false);
        }

        if (OmdbFormat.OmdbType(title.TitleType) == "episode")
        {
            var link = await db.GetEpisodeLinkAsync(title.Id, ct).ConfigureAwait(false);
            return await EpisodeAsync(title, link, ct).ConfigureAwait(false);
        }
        return await ItemAsync(title, ct).ConfigureAwait(false);
    }

    private async Task<object> SearchAsync(string query, IReadOnlyCollection<string> types, int? year, int page, CancellationToken ct)
    {
        if (page is < 1 or > 100) return OmdbFormat.Error("Invalid page! Use 1-100.");
        if (query.Length < 2) return OmdbFormat.Error("Too many results.");

        var hits = await db.SearchAsync(query, types, year, page, PageSize, ct).ConfigureAwait(false);
        if (hits.TooMany) return OmdbFormat.Error("Too many results.");
        if (hits.Total == 0 || hits.Rows.Count == 0) return OmdbFormat.Error("Movie not found!");

        return new SearchList
        {
            SearchResults = hits.Rows.Select(r => new SearchItem
            {
                Title = r.PrimaryTitle,
                Year = OmdbFormat.Year(r.TitleType, r.StartYear, r.EndYear),
                ImdbId = r.Id,
                Type = OmdbFormat.OmdbType(r.TitleType),
                Poster = OmdbFormat.NotAvailable,
            }).ToList(),
            TotalResults = hits.Total.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Response = "True",
        };
    }

    private async Task<Item> ItemAsync(TitleRow t, CancellationToken ct)
    {
        var series = OmdbFormat.OmdbType(t.TitleType) == "series";
        var (directors, writers, actors) = await PeopleAsync(t, ct).ConfigureAwait(false);
        int? totalSeasons = series ? await db.TotalSeasonsAsync(t.Id, ct).ConfigureAwait(false) : null;

        return new Item
        {
            Title = t.PrimaryTitle,
            Year = OmdbFormat.Year(t.TitleType, t.StartYear, t.EndYear),
            Rated = OmdbFormat.NotAvailable,
            Released = OmdbFormat.NotAvailable,
            Runtime = OmdbFormat.Runtime(t.RuntimeMinutes),
            Genre = OmdbFormat.Genre(t.Genres),
            // OMDb gives no director for a series (IMDb lists every episode's director there).
            Director = series ? OmdbFormat.NotAvailable : directors,
            Writer = writers,
            Actors = actors,
            Plot = OmdbFormat.NotAvailable,
            Language = OmdbFormat.NotAvailable,
            Country = OmdbFormat.NotAvailable,
            Awards = OmdbFormat.NotAvailable,
            Poster = OmdbFormat.NotAvailable,
            Ratings = OmdbFormat.Ratings(t.Rating),
            Metascore = OmdbFormat.NotAvailable,
            ImdbRating = OmdbFormat.Rating(t.Rating),
            ImdbVotes = OmdbFormat.Votes(t.Votes),
            ImdbId = t.Id,
            Type = OmdbFormat.OmdbType(t.TitleType),
            Dvd = series ? null : OmdbFormat.NotAvailable,
            BoxOffice = series ? null : OmdbFormat.NotAvailable,
            Production = series ? null : OmdbFormat.NotAvailable,
            Website = series ? null : OmdbFormat.NotAvailable,
            TotalSeasons = series ? totalSeasons?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? OmdbFormat.NotAvailable : null,
            Response = "True",
        };
    }

    private async Task<Episode> EpisodeAsync(TitleRow t, EpisodeLink? link, CancellationToken ct)
    {
        var (directors, writers, actors) = await PeopleAsync(t, ct).ConfigureAwait(false);
        return new Episode
        {
            Title = t.PrimaryTitle,
            Year = OmdbFormat.Year(t.TitleType, t.StartYear, t.EndYear),
            Rated = OmdbFormat.NotAvailable,
            Released = OmdbFormat.NotAvailable,
            SeasonNumber = link?.Season?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? OmdbFormat.NotAvailable,
            EpisodeNumber = link?.Episode?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? OmdbFormat.NotAvailable,
            Runtime = OmdbFormat.Runtime(t.RuntimeMinutes),
            Genre = OmdbFormat.Genre(t.Genres),
            Director = directors,
            Writer = writers,
            Actors = actors,
            Plot = OmdbFormat.NotAvailable,
            Language = OmdbFormat.NotAvailable,
            Country = OmdbFormat.NotAvailable,
            Awards = OmdbFormat.NotAvailable,
            Poster = OmdbFormat.NotAvailable,
            Ratings = OmdbFormat.Ratings(t.Rating),
            Metascore = OmdbFormat.NotAvailable,
            ImdbRating = OmdbFormat.Rating(t.Rating),
            ImdbVotes = OmdbFormat.Votes(t.Votes),
            ImdbId = t.Id,
            SeriesId = link?.SeriesId ?? OmdbFormat.NotAvailable,
            Type = "episode",
            Response = "True",
        };
    }

    private async Task<object> SeasonAsync(TitleRow series, int season, CancellationToken ct)
    {
        var episodes = await db.SeasonAsync(series.Id, season, ct).ConfigureAwait(false);
        if (episodes.Count == 0) return OmdbFormat.Error("Series or season not found!");
        var total = await db.TotalSeasonsAsync(series.Id, ct).ConfigureAwait(false);
        return new Season
        {
            Title = series.PrimaryTitle,
            SeasonNumber = season.ToString(System.Globalization.CultureInfo.InvariantCulture),
            TotalSeasons = total?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? OmdbFormat.NotAvailable,
            Episodes = episodes.Select(e => new SeasonEpisode
            {
                Title = e.Title,
                Released = OmdbFormat.NotAvailable,
                Episode = e.Episode?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? OmdbFormat.NotAvailable,
                ImdbRating = OmdbFormat.Rating(e.Rating),
                ImdbId = e.Id,
            }).ToList(),
            Response = "True",
        };
    }

    private async Task<(string Directors, string Writers, string Actors)> PeopleAsync(TitleRow t, CancellationToken ct)
    {
        var directorIds = OmdbFormat.Split(t.Directors);
        var writerIds = OmdbFormat.Split(t.Writers);
        var wanted = directorIds.Take(MaxPeople).Concat(writerIds.Take(MaxPeople)).Distinct(StringComparer.Ordinal).ToList();
        var names = await db.NamesAsync(wanted, ct).ConfigureAwait(false);
        var actors = await db.ActorsAsync(t.Id, MaxActors, ct).ConfigureAwait(false);
        return (OmdbFormat.Names(directorIds, names, MaxPeople), OmdbFormat.Names(writerIds, names, MaxPeople), OmdbFormat.Join(actors));
    }

    /// <summary>tt followed by at least seven digits, as IMDb writes title ids.</summary>
    public static bool IsImdbId(string id) => ImdbIdPattern().IsMatch(id);

    [GeneratedRegex("^tt[0-9]{7,10}$")]
    private static partial Regex ImdbIdPattern();
}
