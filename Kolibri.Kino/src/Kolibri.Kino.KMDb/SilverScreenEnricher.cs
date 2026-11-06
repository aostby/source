using System.Globalization;
using MySqlConnector;
using OMDbApiNet.Model;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// Fills in what IMDb's data sets lack (plot, poster, age rating, release date, country, language, awards, Metascore,
/// Rotten Tomatoes …) from the SilverScreen library when it has the title. IMDb's own fields (title, year, rating,
/// votes, genres, crew) stay as the data sets give them: they are refreshed daily, the library is not.
/// </summary>
/// <remarks>
/// Posters become this service's /silverscreen/poster/{id} when the library's image cache holds one; otherwise the
/// library's own poster address, unless that points at Plex (it would need the owner's Plex token).
/// If the library database can't be reached, answers go out from IMDb's data alone.
/// </remarks>
public sealed class SilverScreenEnricher(ISilverScreenRepository library, ILogger<SilverScreenEnricher> log, KmdbMetrics? metrics = null, PosterService? posters = null)
{
    public async Task<object> EnrichAsync(object answer, string? baseUrl, CancellationToken ct = default)
    {
        try
        {
            switch (answer)
            {
                case Item item: await ItemAsync(item, baseUrl, ct).ConfigureAwait(false); break;
                case Episode episode: await EpisodeAsync(episode, baseUrl, ct).ConfigureAwait(false); break;
                case Season season: await SeasonAsync(season, ct).ConfigureAwait(false); break;
                case SearchList list: await SearchAsync(list, baseUrl, ct).ConfigureAwait(false); break;
            }
        }
        catch (MySqlException ex)
        {
            log.LogWarning("SilverScreen data left out: {Message}", ex.Message);
        }
        return answer;
    }

    /// <summary>The library's own item for a title IMDb's files don't have (yet), or null.</summary>
    public async Task<Item?> ItemOnlyInLibraryAsync(string imdbId, string? baseUrl, CancellationToken ct = default)
    {
        try
        {
            var item = await library.ItemAsync(imdbId, ct).ConfigureAwait(false);
            if (item is null || !OmdbJson.HasValue(item.Title)) return null;
            metrics?.Enriched("library_only");
            item.ImdbId ??= imdbId;
            item.Response = "True";
            item.Error = null;
            item.Released = OmdbDate(item.Released) ?? OmdbFormat.NotAvailable;
            item.Metascore = Metascore(item.Metascore) ?? OmdbFormat.NotAvailable;
            item.Poster =await PosterAsync(imdbId, item.Poster, baseUrl, ct).ConfigureAwait(false);
            return item;
        }
        catch (MySqlException ex)
        {
            log.LogWarning("SilverScreen lookup of {Id} failed: {Message}", imdbId, ex.Message);
            return null;
        }
    }

    private async Task ItemAsync(Item item, string? baseUrl, CancellationToken ct)
    {
        var known = await library.ItemAsync(item.ImdbId, ct).ConfigureAwait(false);
        if (known is not null)
        {
            FillFrom(item, known);
            metrics?.Enriched("item");
        }
        item.Poster = await PosterAsync(item.ImdbId, known?.Poster, baseUrl, ct).ConfigureAwait(false);
    }

    private async Task EpisodeAsync(Episode episode, string? baseUrl, CancellationToken ct)
    {
        var known = await library.EpisodeAsync(episode.ImdbId, ct).ConfigureAwait(false);
        if (known is not null)
        {
            metrics?.Enriched("episode");
            episode.Rated = Pick(episode.Rated, known.Rated);
            episode.Released = Pick(episode.Released, OmdbDate(known.Released));
            episode.Plot = Pick(episode.Plot, known.Plot);
            episode.Language = Pick(episode.Language, known.Language);
            episode.Country = Pick(episode.Country, known.Country);
            episode.Awards = Pick(episode.Awards, known.Awards);
            episode.Metascore = Pick(episode.Metascore, Metascore(known.Metascore));
            episode.Actors = Pick(episode.Actors, known.Actors);
            episode.Ratings = MergeRatings(episode.Ratings, known.Ratings);
        }
        episode.Poster = await PosterAsync(episode.ImdbId, known?.Poster, baseUrl, ct).ConfigureAwait(false);
    }

    private async Task SeasonAsync(Season season, CancellationToken ct)
    {
        var first = season.Episodes?.FirstOrDefault()?.ImdbId;
        if (first is null || !int.TryParse(season.SeasonNumber, out var number)) return;
        // The season's series id isn't in OMDb's Season; the library's KolibriSeason is keyed by it.
        var seriesId = (await library.EpisodeAsync(first, ct).ConfigureAwait(false))?.SeriesId;
        if (!OmdbJson.HasValue(seriesId)) return;
        var released = await library.SeasonReleasedAsync(seriesId!, number, ct).ConfigureAwait(false);
        if (released.Count > 0) metrics?.Enriched("season");
        foreach (var e in season.Episodes!)
            if (e.ImdbId is not null && released.TryGetValue(e.ImdbId, out var date)) e.Released = Pick(e.Released, date);
    }

    private async Task SearchAsync(SearchList list, string? baseUrl, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(baseUrl) || list.SearchResults is not { Count: > 0 } hits) return;
        var withImage = await AvailableAsync(hits.Select(h => h.ImdbId).ToList(), ct).ConfigureAwait(false);
        foreach (var hit in hits.Where(h => withImage.Contains(h.ImdbId)))
            hit.Poster = PosterUrl(baseUrl, hit.ImdbId);
        if (withImage.Count > 0) metrics?.Enriched("search");
    }

    /// <summary>Copies the library's values into the fields IMDb's data sets leave "N/A".</summary>
    internal static void FillFrom(Item item, Item known)
    {
        item.Rated = Pick(item.Rated, known.Rated);
        item.Released = Pick(item.Released, OmdbDate(known.Released));
        item.Plot = Pick(item.Plot, known.Plot);
        item.Language = Pick(item.Language, known.Language);
        item.Country = Pick(item.Country, known.Country);
        item.Awards = Pick(item.Awards, known.Awards);
        item.Metascore = Pick(item.Metascore, Metascore(known.Metascore));
        item.Actors = Pick(item.Actors, known.Actors);
        item.Director = Pick(item.Director, known.Director);
        item.Writer = Pick(item.Writer, known.Writer);
        if (item.Dvd is not null) item.Dvd = Pick(item.Dvd, known.Dvd);
        if (item.BoxOffice is not null) item.BoxOffice = Pick(item.BoxOffice, known.BoxOffice);
        if (item.Production is not null) item.Production = Pick(item.Production, known.Production);
        if (item.Website is not null) item.Website = Pick(item.Website, known.Website);
        item.Ratings = MergeRatings(item.Ratings, known.Ratings);
    }

    /// <summary>IMDb's rating from the data sets (current), plus the library's other sources (Rotten Tomatoes, Metacritic).</summary>
    internal static List<Rating> MergeRatings(List<Rating>? current, List<Rating>? known)
    {
        var merged = new List<Rating>(current ?? []);
        foreach (var r in known ?? [])
            if (OmdbJson.HasValue(r.Source) && OmdbJson.HasValue(r.Value)
                && !merged.Any(m => string.Equals(m.Source, r.Source, StringComparison.OrdinalIgnoreCase)))
                merged.Add(r);
        return merged;
    }

    private async Task<string> PosterAsync(string? imdbId, string? knownPoster, string? baseUrl, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(imdbId) && !string.IsNullOrEmpty(baseUrl)
            && (await AvailableAsync([imdbId], ct).ConfigureAwait(false)).Contains(imdbId))
            return PosterUrl(baseUrl, imdbId);
        return OmdbJson.HasValue(knownPoster) && !IsPlexAddress(knownPoster!) ? knownPoster! : OmdbFormat.NotAvailable;
    }

    /// <summary>Ids with a poster /silverscreen/poster can serve: cached, or (with the poster service) to be fetched.</summary>
    private Task<IReadOnlySet<string>> AvailableAsync(IReadOnlyCollection<string> ids, CancellationToken ct) =>
        posters is not null ? posters.AvailableAsync(ids, ct) : library.ExistingImagesAsync(ids, ct);

    internal static string PosterUrl(string baseUrl, string imdbId) => $"{baseUrl.TrimEnd('/')}/silverscreen/poster/{Uri.EscapeDataString(imdbId)}";

    /// <summary>Plex addresses (…:32400/library/…) only work with the owner's token, which is never handed out.</summary>
    internal static bool IsPlexAddress(string url) =>
        url.Contains(":32400", StringComparison.Ordinal) || url.Contains("X-Plex-Token", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Library items that came from Plex hold dates as 1999-03-31 and Plex's own ids in Metascore; OMDb writes
    /// "31 Mar 1999" and a Metascore of 0–100.
    /// </summary>
    internal static string? OmdbDate(string? value) =>
        DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)
            : value;

    internal static string? Metascore(string? value) =>
        int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var score) && score <= 100 ? value : null;

    private static string? Pick(string? current, string? known) => OmdbJson.HasValue(current) ? current : OmdbJson.HasValue(known) ? known : current;
}
