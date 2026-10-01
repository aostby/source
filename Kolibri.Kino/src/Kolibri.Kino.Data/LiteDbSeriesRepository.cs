using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// Season/episode storage. Kino's cache is "KinoSeason" (_id = "{seriesId}_{season}"); SilverScreen's data is read
/// from "KolibriSeason" (same _id form, OMDb season with an episode list) and "Episode" (_id = episode IMDb id,
/// OMDb details), and never written. "KinoIgnoredFolder" (_id = folder path) holds folders "Find new series" skips.
/// </summary>
public sealed class LiteDbSeriesRepository(KinoDatabase db) : ISeriesRepository
{
    private readonly ILiteCollection<BsonDocument> _kino = db.Database.GetCollection("KinoSeason");
    private readonly ILiteCollection<BsonDocument> _silverScreenSeasons = db.Database.GetCollection("KolibriSeason");
    private readonly ILiteCollection<BsonDocument> _silverScreenEpisodes = db.Database.GetCollection("Episode");
    private readonly ILiteCollection<BsonDocument> _ignored = db.Database.GetCollection("KinoIgnoredFolder");

    public Task<IReadOnlySet<string>> GetIgnoredFoldersAsync(CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlySet<string>)_ignored.FindAll().Select(d => d["_id"].AsString).ToHashSet(StringComparer.OrdinalIgnoreCase), ct);

    public Task IgnoreFolderAsync(string folder, CancellationToken ct = default) =>
        Task.Run(() => _ignored.Upsert(new BsonDocument { ["_id"] = folder, ["Ignored"] = DateTime.UtcNow }), ct);

    public Task<IReadOnlyList<SeriesSeason>> GetCachedSeasonsAsync(string seriesImdbId, CancellationToken ct = default) =>
        Task.Run(() => (IReadOnlyList<SeriesSeason>)_kino.Find(Query.EQ("SeriesId", seriesImdbId))
            .Select(FromKinoDocument)
            .OrderBy(s => s.SeasonNumber == 0 ? int.MaxValue : s.SeasonNumber)
            .ToList(), ct);

    public Task<IReadOnlyList<SeriesSeason>> GetSilverScreenSeasonsAsync(string seriesImdbId, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            // ToList first: in Shared mode LiteDB closes the file after each operation, so looking up episodes while
            // this query's cursor is still open fails with "engine instance already disposed".
            var seasonDocs = _silverScreenSeasons.Find(Query.EQ("SeriesId", seriesImdbId)).ToList();
            var seasons = new List<SeriesSeason>();
            foreach (var doc in seasonDocs)
            {
                ct.ThrowIfCancellationRequested();
                if (Int(doc["SeasonNumber"]) is not { } number) continue;

                var episodes = doc["Episodes"].IsArray
                    ? doc["Episodes"].AsArray.Where(e => e.IsDocument).Select(e => FromOmdbEpisode(e.AsDocument, number)).OfType<SeriesEpisode>().ToList()
                    : [];
                seasons.Add(new SeriesSeason(number, null, episodes.OrderBy(e => e.EpisodeNumber).ToList(), "OMDb"));
            }
            return (IReadOnlyList<SeriesSeason>)seasons.OrderBy(s => s.SeasonNumber).ToList();
        }, ct);

    public Task SaveSeasonsAsync(string seriesImdbId, IReadOnlyList<SeriesSeason> seasons, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            _kino.DeleteMany(Query.EQ("SeriesId", seriesImdbId));
            _kino.InsertBulk(seasons.Select(s => ToKinoDocument(seriesImdbId, s)));
        }, ct);

    /// <summary>A season-list episode enriched with the episode's own OMDb record, when SilverScreen fetched it.</summary>
    private SeriesEpisode? FromOmdbEpisode(BsonDocument listed, int season)
    {
        if (Int(listed["Episode"]) is not { } number) return null;

        var imdbId = Str(listed["ImdbId"]);
        var details = imdbId is null ? null : _silverScreenEpisodes.FindById(imdbId);
        return new SeriesEpisode(
            season,
            number,
            Str(listed["Title"]) ?? Str(details?["Title"]) ?? $"Episode {number}",
            Date(listed["Released"]) ?? Date(details?["Released"]),
            Str(details?["Plot"]),
            Double(listed["ImdbRating"]) ?? Double(details?["ImdbRating"]),
            Int(Str(details?["Runtime"])?.Split(' ')[0]),
            null,
            imdbId);
    }

    private static BsonDocument ToKinoDocument(string seriesId, SeriesSeason season) => new()
    {
        ["_id"] = $"{seriesId}_{season.SeasonNumber}",
        ["SeriesId"] = seriesId,
        ["SeasonNumber"] = season.SeasonNumber,
        ["Name"] = season.Name,
        ["Source"] = season.Source,
        ["Fetched"] = DateTime.UtcNow,
        ["Episodes"] = new BsonArray(season.Episodes.Select(e => new BsonDocument
        {
            ["SeasonNumber"] = e.SeasonNumber,
            ["EpisodeNumber"] = e.EpisodeNumber,
            ["Title"] = e.Title,
            ["AirDate"] = e.AirDate is { } d ? d : BsonValue.Null,
            ["Plot"] = e.Plot,
            ["Rating"] = e.Rating is { } r ? r : BsonValue.Null,
            ["RuntimeMinutes"] = e.RuntimeMinutes is { } m ? m : BsonValue.Null,
            ["StillUrl"] = e.StillUrl,
            ["ImdbId"] = e.ImdbId,
        })),
    };

    private static SeriesSeason FromKinoDocument(BsonDocument doc) => new(
        doc["SeasonNumber"].AsInt32,
        Str(doc["Name"]),
        doc["Episodes"].AsArray.Select(v => v.AsDocument).Select(e => new SeriesEpisode(
            e["SeasonNumber"].AsInt32,
            e["EpisodeNumber"].AsInt32,
            Str(e["Title"]) ?? "",
            e["AirDate"].IsDateTime ? e["AirDate"].AsDateTime : null,
            Str(e["Plot"]),
            e["Rating"].IsNumber ? e["Rating"].AsDouble : null,
            e["RuntimeMinutes"].IsNumber ? e["RuntimeMinutes"].AsInt32 : null,
            Str(e["StillUrl"]),
            Str(e["ImdbId"]))).ToList(),
        Str(doc["Source"]) ?? "cache");

    private static string? Str(BsonValue? v) => v is { IsString: true } && v.AsString is { Length: > 0 } s && s != "N/A" ? s : null;

    private static int? Int(BsonValue? v) => Int(Str(v));

    private static int? Int(string? s) => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null;

    private static double? Double(BsonValue? v) =>
        double.TryParse(Str(v), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

    /// <summary>OMDb dates come as "2019-05-17" (season lists) or "10 Mar 1997" (episode records).</summary>
    private static DateTime? Date(BsonValue? v) =>
        DateTime.TryParse(Str(v), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var d) ? d : null;
}
