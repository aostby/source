using System.Text.Json;
using System.Text.RegularExpressions;
using Kolibri.Kino.Core;
using MySqlConnector;

namespace Kolibri.Kino.SilverScreenCopy;

/// <summary>What an image is: the title it belongs to and its type.</summary>
public sealed record ImageMeta(string? ImdbId, string Type, int? Season = null, int? Episode = null, int? TmdbId = null, string? Source = null);

/// <summary>
/// Fills in ImdbId, ImageType, Season/Episode, TmdbId, Source, SourceUrl, Width and Height for the images copied from
/// Kino's cache, from their keys and the rest of the library:
/// <list type="bullet">
/// <item>tt0133093 → that title's poster; tt0903747_S01E02 → the episode's still; tmdb-person-123 → a profile photo;
/// tmdb-tv-/tmdb-movie-123 → the poster of that TMDb title.</item>
/// <item>An address (the image was cached by where it came from) → matched against the library's poster, backdrop,
/// still and profile addresses: Movie, TvShow (and its seasons), KinoSeason, Credits, Item, Episode, WatchListItem,
/// KinoWishlist, KinoSimilar, KinoDiscover/KinoTrending. TMDb addresses match on the file name, whatever the size.</item>
/// <item>Anything else: a poster when portrait, a backdrop when landscape, with no title.</item>
/// </list>
/// Rerunnable; it only writes these columns (not _modified, so a sync doesn't see 2,900 changed images).
/// </summary>
public sealed partial class ImageClassifier(MySqlConnection connection, TextWriter log)
{
    private readonly Dictionary<string, ImageMeta> _byAddress = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _tmdbToImdb = new(StringComparer.OrdinalIgnoreCase);

    public async Task RunAsync(CancellationToken ct = default)
    {
        await ImageTable.EnsureColumnsAsync(connection, ct);
        await LoadLibraryAsync(ct);
        log.WriteLine($"  known image addresses: {_byAddress.Count:n0}, TMDb→IMDb ids: {_tmdbToImdb.Count:n0}");

        var rows = new List<(string Id, ImageMeta Meta, string? Url, int? W, int? H)>();
        await using (var cmd = connection.CreateCommand())
        {
            // The first 64 KB hold the header with the size, also in JPEGs with large EXIF blocks.
            cmd.CommandText = "SELECT `_id`, SUBSTRING(`Data`, 1, 65536) FROM `Image` WHERE `Data` IS NOT NULL AND (`Source` IS NULL OR `Source` <> 'kodi')";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var id = reader.GetString(0);
                var head = reader.IsDBNull(1) ? [] : (byte[])reader.GetValue(1);
                var size = ImageInfo.Size(head);
                var meta = Classify(id, size);
                rows.Add((id, meta, IsAddress(id) ? WithoutToken(id) : null, size?.Width, size?.Height));
            }
        }

        await using (var tx = await connection.BeginTransactionAsync(ct))
        {
            foreach (var r in rows)
            {
                await using var cmd = new MySqlCommand("""
                    UPDATE `Image` SET `ImdbId` = @imdb, `ImageType` = @type, `Season` = @season, `Episode` = @episode,
                           `TmdbId` = @tmdb, `Source` = @source, `SourceUrl` = @url, `Width` = @w, `Height` = @h
                     WHERE `_id` = @id
                    """, connection, tx);
                cmd.Parameters.AddWithValue("@id", r.Id);
                cmd.Parameters.AddWithValue("@imdb", r.Meta.ImdbId);
                cmd.Parameters.AddWithValue("@type", r.Meta.Type);
                cmd.Parameters.AddWithValue("@season", r.Meta.Season);
                cmd.Parameters.AddWithValue("@episode", r.Meta.Episode);
                cmd.Parameters.AddWithValue("@tmdb", r.Meta.TmdbId);
                cmd.Parameters.AddWithValue("@source", r.Meta.Source ?? "kino");
                cmd.Parameters.AddWithValue("@url", r.Url);
                cmd.Parameters.AddWithValue("@w", r.W);
                cmd.Parameters.AddWithValue("@h", r.H);
                await cmd.ExecuteNonQueryAsync(ct);
            }
            await tx.CommitAsync(ct);
        }

        foreach (var g in rows.GroupBy(r => (r.Meta.Type, HasTitle: r.Meta.ImdbId is not null || r.Meta.TmdbId is not null)).OrderBy(g => g.Key.Type))
            log.WriteLine($"  {g.Key.Type,-12} {(g.Key.HasTitle ? "with title" : "no title"),-10} {g.Count(),6:n0}");
        log.WriteLine($"  classified {rows.Count:n0} images");

        // What belongs to nothing the library knows (no title, no TMDb id, not one of Kino's own icons) is not used:
        // removed from MariaDB. Kino's LiteDB cache is not touched.
        await using (var prune = connection.CreateCommand())
        {
            prune.CommandText = "DELETE FROM `Image` WHERE `ImdbId` IS NULL AND `TmdbId` IS NULL AND (`ImageType` IS NULL OR `ImageType` <> 'icon')";
            var removed = await prune.ExecuteNonQueryAsync(ct);
            log.WriteLine($"  removed {removed:n0} images that belong to nothing in the library");
        }
    }

    /// <summary>What an image cached under <paramref name="key"/> is.</summary>
    internal ImageMeta Classify(string key, (int Width, int Height)? size)
    {
        if (TitleId().IsMatch(key)) return new ImageMeta(key, "poster", Source: "kino");
        if (EpisodeKey().Match(key) is { Success: true } e)
            return new ImageMeta(e.Groups[1].Value, "still", int.Parse(e.Groups[2].Value), int.Parse(e.Groups[3].Value), Source: "kino");
        if (PersonKey().Match(key) is { Success: true } p) return new ImageMeta(null, "profile", TmdbId: int.Parse(p.Groups[1].Value), Source: "tmdb");
        if (TmdbTitleKey().Match(key) is { Success: true } t)
        {
            var tmdbId = int.Parse(t.Groups[2].Value);
            return new ImageMeta(_tmdbToImdb.GetValueOrDefault($"{t.Groups[1].Value}:{tmdbId}"), "poster", TmdbId: tmdbId, Source: "tmdb");
        }
        if (IsAddress(key))
        {
            var source = SourceOf(key);
            if (_byAddress.TryGetValue(AddressKey(key), out var known)) return known with { Source = source };
            return new ImageMeta(null, size is { } s ? ImageInfo.TypeFromShape(s.Width, s.Height) : "poster", Source: source);
        }
        if (key.StartsWith("button", StringComparison.OrdinalIgnoreCase)) return new ImageMeta(null, "icon", Source: "kino");
        return new ImageMeta(null, size is { } z ? ImageInfo.TypeFromShape(z.Width, z.Height) : "poster", Source: "kino");
    }

    /// <summary>
    /// The key an address is matched on: TMDb image addresses by their file name (w500 and original are the same image),
    /// others whole, without a Plex token, in lower case.
    /// </summary>
    internal static string AddressKey(string url)
    {
        var clean = WithoutToken(url).Trim();
        if (clean.StartsWith('/')) return "tmdb:" + clean.TrimStart('/').ToLowerInvariant();
        if (Uri.TryCreate(clean, UriKind.Absolute, out var uri) && uri.Host.Contains("tmdb.org", StringComparison.OrdinalIgnoreCase))
            return "tmdb:" + uri.Segments[^1].ToLowerInvariant();
        return clean.ToLowerInvariant();
    }

    internal static string SourceOf(string url) =>
        url.Contains("tmdb.org", StringComparison.OrdinalIgnoreCase) || url.Contains("themoviedb.org", StringComparison.OrdinalIgnoreCase) ? "tmdb"
        : url.Contains(":32400", StringComparison.Ordinal) ? "plex"
        : url.Contains("media-amazon.com", StringComparison.OrdinalIgnoreCase) ? "omdb"
        : url.Contains("fanart.tv", StringComparison.OrdinalIgnoreCase) ? "fanart"
        : "web";

    internal static bool IsAddress(string key) => key.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || key.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    internal static string WithoutToken(string url) => PlexToken().Replace(url, m => m.Value.StartsWith('?') && m.Value.EndsWith('&') ? "?" : m.Value.StartsWith('?') ? "" : m.Value.EndsWith('&') ? "&" : "").TrimEnd('&');

    private void Know(string? address, ImageMeta meta)
    {
        if (string.IsNullOrWhiteSpace(address) || address == "N/A") return;
        _byAddress.TryAdd(AddressKey(address), meta);
    }

    /// <summary>Every image address the library knows, with the title and type it stands for.</summary>
    private async Task LoadLibraryAsync(CancellationToken ct)
    {
        await ForEachAsync("SELECT `_id`, `ImdbId` FROM `KinoTmdbImdb`", r =>
        {
            // Keys like "movie:603" / "tv:1396" (or "movie-603"): the TMDb kind and id.
            var key = r.GetString(0).Replace('-', ':');
            if (!r.IsDBNull(1)) _tmdbToImdb.TryAdd(key, r.GetString(1));
        }, ct);
        await ForEachAsync("SELECT `_id`, `ImdbId`, `PosterPath`, `BackdropPath` FROM `Movie` WHERE `ImdbId` IS NOT NULL", r =>
        {
            var imdb = r.GetString(1);
            _tmdbToImdb.TryAdd($"movie:{r.GetValue(0)}", imdb);
            Know(Str(r, 2), new ImageMeta(imdb, "poster"));
            Know(Str(r, 3), new ImageMeta(imdb, "backdrop"));
        }, ct);
        await ForEachAsync("SELECT `_id`, JSON_VALUE(`ExternalIds`, '$.ImdbId'), `PosterPath`, `BackdropPath`, `Seasons` FROM `TvShow`", r =>
        {
            var imdb = Str(r, 1);
            if (imdb is null) return;
            _tmdbToImdb.TryAdd($"tv:{r.GetValue(0)}", imdb);
            Know(Str(r, 2), new ImageMeta(imdb, "poster"));
            Know(Str(r, 3), new ImageMeta(imdb, "backdrop"));
            foreach (var season in Json(Str(r, 4)))
                Know(Prop(season, "PosterPath"), new ImageMeta(imdb, "poster", Season: Int(season, "SeasonNumber")));
        }, ct);
        await ForEachAsync("SELECT `SeriesId`, `Episodes` FROM `KinoSeason`", r =>
        {
            foreach (var e in Json(Str(r, 1)))
                Know(Prop(e, "StillUrl"), new ImageMeta(r.GetString(0), "still", Int(e, "SeasonNumber"), Int(e, "EpisodeNumber")));
        }, ct);
        await ForEachAsync("SELECT `Cast`, `Crew`, `Base64Data` FROM `Credits`", r =>
        {
            foreach (var p in Json(Str(r, 0)).Concat(Json(Str(r, 1))))
                Know(Prop(p, "ProfilePath"), new ImageMeta(null, "profile", TmdbId: Int(p, "_id")));
            // SilverScreen kept TMDb's credits as base64 JSON: {"cast":[{"id":…,"profile_path":"/x.jpg"}],"crew":[…]}.
            var credits = FromBase64Json(Str(r, 2));
            foreach (var list in new[] { "cast", "crew" })
                if (credits is { ValueKind: JsonValueKind.Object } c && c.TryGetProperty(list, out var people) && people.ValueKind == JsonValueKind.Array)
                    foreach (var p in people.EnumerateArray())
                        Know(Prop(p, "profile_path"), new ImageMeta(null, "profile", TmdbId: Int(p, "id")));
        }, ct);
        await ForEachAsync("SELECT `Base64Data` FROM `ReviewList`", r =>
        {
            // SilverScreen's TMDb reviews: [{"author_details":{"avatar_path":"/x.jpg"}}]: the reviewers' pictures.
            if (FromBase64Json(Str(r, 0)) is { ValueKind: JsonValueKind.Array } reviews)
                foreach (var review in reviews.EnumerateArray())
                    if (review.ValueKind == JsonValueKind.Object && review.TryGetProperty("author_details", out var author))
                        Know(Prop(author, "avatar_path"), new ImageMeta(null, "avatar"));
        }, ct);
        await ForEachAsync("SELECT `_id`, `Poster` FROM `Item`", r => Know(Str(r, 1), new ImageMeta(r.GetString(0), "poster")), ct);
        await ForEachAsync("SELECT `SeriesId`, `SeasonNumber`, `EpisodeNumber`, `Poster` FROM `Episode`", r =>
            Know(Str(r, 3), new ImageMeta(Str(r, 0), "still", ParseInt(Str(r, 1)), ParseInt(Str(r, 2)))), ct);
        await ForEachAsync("SELECT `ImdbId`, `Poster` FROM `WatchListItem` WHERE `ImdbId` IS NOT NULL", r => Know(Str(r, 1), new ImageMeta(r.GetString(0), "poster")), ct);
        await ForEachAsync("SELECT `_id`, `Poster` FROM `KinoWishlist`", r =>
        {
            var id = r.GetString(0);
            Know(Str(r, 1), EpisodeKey().Match(id) is { Success: true } m
                ? new ImageMeta(m.Groups[1].Value, "still", int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value))
                : new ImageMeta(id, "poster"));
        }, ct);
        await ForEachAsync("SELECT `Hits` FROM `KinoSimilar`", r =>
        {
            foreach (var h in Json(Str(r, 0))) Know(Prop(h, "Poster"), new ImageMeta(Prop(h, "ImdbId"), "poster"));
        }, ct);
        foreach (var table in new[] { "KinoDiscover", "KinoTrending" })
            await ForEachAsync($"SELECT `Items` FROM `{table}`", r =>
            {
                foreach (var i in Json(Str(r, 0))) Know(Prop(i, "Image"), new ImageMeta(Prop(i, "ImdbId"), "poster", TmdbId: Int(i, "TmdbId")));
            }, ct);
    }

    private async Task ForEachAsync(string sql, Action<MySqlDataReader> row, CancellationToken ct)
    {
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct)) row(reader);
        }
        catch (MySqlException ex) when (ex.ErrorCode is MySqlErrorCode.NoSuchTable or MySqlErrorCode.BadFieldError)
        {
            log.WriteLine($"  (skipped: {ex.Message})");
        }
    }

    private static string? Str(MySqlDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i), System.Globalization.CultureInfo.InvariantCulture);

    private static IEnumerable<JsonElement> Json(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];
        using var doc = JsonDocument.Parse(text);
        return doc.RootElement.ValueKind == JsonValueKind.Array ? doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList() : [];
    }

    private static JsonElement? FromBase64Json(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return null;
        try
        {
            using var doc = JsonDocument.Parse(Convert.FromBase64String(base64));
            return doc.RootElement.Clone();
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            return null;
        }
    }

    private static string? Prop(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static int? Int(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n) ? n : null;

    private static int? ParseInt(string? s) => int.TryParse(s, out var n) ? n : null;

    [GeneratedRegex("^tt[0-9]{7,10}$")]
    private static partial Regex TitleId();

    [GeneratedRegex("^(tt[0-9]{7,10})_S([0-9]{1,3})E([0-9]{1,4})$")]
    private static partial Regex EpisodeKey();

    [GeneratedRegex("^tmdb-person-([0-9]+)$")]
    private static partial Regex PersonKey();

    [GeneratedRegex("^tmdb-(tv|movie)-([0-9]+)$")]
    private static partial Regex TmdbTitleKey();

    [GeneratedRegex("[?&]X-Plex-Token=[^&\"\\s]*&?", RegexOptions.IgnoreCase)]
    private static partial Regex PlexToken();
}
