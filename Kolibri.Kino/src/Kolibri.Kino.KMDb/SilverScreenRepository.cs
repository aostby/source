using System.Collections.Concurrent;
using System.Data.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using OMDbApiNet.Model;

namespace Kolibri.Kino.KMDb;

/// <summary>A copied LiteDB collection: its table, how many rows, and when it was copied.</summary>
public sealed record SilverScreenCollection(string Name, string SourceFile, int Rows, DateTime CopiedUtc);

/// <summary>One page of documents from a collection.</summary>
public sealed record SilverScreenPage(string Collection, int Page, int PageSize, int Total, IReadOnlyList<JObject> Items);

/// <summary>An image (poster, still, profile) from the SilverScreen image cache.</summary>
public sealed record SilverScreenImage(byte[] Data, DateTime? Added);

/// <summary>
/// One image of a title: its key in the cache, its type (poster, backdrop, still, landscape, thumb, clearlogo …),
/// season/episode for stills, where it came from (kino, kodi, tmdb, plex, omdb, web), its size, and its address here.
/// </summary>
public sealed record ArtInfo(string Id, string Type, int? Season, int? Episode, string? Source, int? Width, int? Height, long Bytes, string Url);

/// <summary>The SilverScreen library copied to MariaDB (database "silverscreen", one table per LiteDB collection).</summary>
public interface ISilverScreenRepository
{
    Task<IReadOnlyList<SilverScreenCollection>> CollectionsAsync(CancellationToken ct = default);
    Task<JObject?> GetAsync(string collection, string id, CancellationToken ct = default);
    /// <summary>A page of documents, optionally only where a field has a value, or changed at or after <paramref name="since"/> (UTC).</summary>
    Task<SilverScreenPage?> ListAsync(string collection, int page, int pageSize, string? field, string? value, DateTime? since = null, CancellationToken ct = default);
    Task<SilverScreenImage?> ImageAsync(string id, CancellationToken ct = default);
    Task<byte[]?> BinaryAsync(string collection, string id, string column, CancellationToken ct = default);

    /// <summary>The library's OMDb item for a title (collection Item), or null.</summary>
    Task<Item?> ItemAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The library's OMDb episode (collection Episode), or null.</summary>
    Task<Episode?> EpisodeAsync(string imdbId, CancellationToken ct = default);

    /// <summary>Release dates of a season's episodes by IMDb id (KolibriSeason, OMDb's format "2008-01-20").</summary>
    Task<IReadOnlyDictionary<string, string>> SeasonReleasedAsync(string seriesId, int season, CancellationToken ct = default);

    /// <summary>
    /// Where the library says a title's poster is, best first: TMDb poster paths (Movie, TvShow; "/abc.jpg"), then the
    /// poster addresses of Item, Episode, WatchListItem and KinoWishlist. Unfiltered (Plex addresses included).
    /// </summary>
    Task<IReadOnlyList<string>> PosterSourcesAsync(string imdbId, CancellationToken ct = default);

    /// <summary>The best image of a type for a title (stills: of one episode), or null. Sources in order kino, kodi, tmdb, plex, omdb, web.</summary>
    Task<SilverScreenImage?> ArtAsync(string imdbId, string type, int? season = null, int? episode = null, CancellationToken ct = default);

    /// <summary>All images of a title (for a series also its episodes' stills), best first per type.</summary>
    Task<IReadOnlyList<ArtInfo>> ArtListAsync(string imdbId, CancellationToken ct = default);

    /// <summary>Which of these ids have a poster in the cache (by IMDb id and type, or keyed by the id itself).</summary>
    Task<IReadOnlySet<string>> ExistingImagesAsync(IReadOnlyCollection<string> ids, CancellationToken ct = default);
}

/// <summary>
/// <see cref="ISilverScreenRepository"/> on MariaDB. The tables are made by tools/Kolibri.Kino.SilverScreenCopy: one
/// column per LiteDB field, nested data as JSON text (LONGTEXT), binary data as LONGBLOB, dates in UTC.
/// </summary>
public sealed partial class SilverScreenRepository(MySqlDataSource dataSource) : ISilverScreenRepository
{
    /// <summary>Collections never served: the settings hold API keys, the Plex token and the database password.</summary>
    public static readonly IReadOnlySet<string> Hidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "UserSettings" };

    private const string StatusTable = "_CopyStatus";

    // Column types per table, read once (the copy tool rebuilds tables, so a restart picks up new columns).
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _columns = new(StringComparer.OrdinalIgnoreCase);

    public async Task<IReadOnlyList<SilverScreenCollection>> CollectionsAsync(CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"SELECT collection_name, source_file, row_count, copied_utc FROM `{StatusTable}` ORDER BY collection_name", conn);
        var list = new List<SilverScreenCollection>();
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
        {
            var name = reader.GetString(0);
            if (Hidden.Contains(name)) continue;
            list.Add(new SilverScreenCollection(name, reader.GetString(1), reader.GetInt32(2), DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc)));
        }
        return list;
    }

    public async Task<JObject?> GetAsync(string collection, string id, CancellationToken ct = default)
    {
        var columns = await ColumnsAsync(collection, ct).ConfigureAwait(false);
        if (columns is null) return null;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"SELECT {SelectList(columns)} FROM {Q(collection)} WHERE `_id` = @id", conn);
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        return await reader.ReadAsync(ct).ConfigureAwait(false) ? ToJson(reader, columns, collection) : null;
    }

    public async Task<SilverScreenPage?> ListAsync(string collection, int page, int pageSize, string? field, string? value, DateTime? since = null, CancellationToken ct = default)
    {
        var columns = await ColumnsAsync(collection, ct).ConfigureAwait(false);
        if (columns is null) return null;
        var where = "";
        if (!string.IsNullOrWhiteSpace(field))
        {
            var column = columns.Keys.FirstOrDefault(c => string.Equals(c, field, StringComparison.OrdinalIgnoreCase));
            if (column is null || columns[column] is "longblob" or "longtext")
                throw new ArgumentException($"'{field}' is not a field of {collection} that can be filtered on.");
            where = value is null ? $"WHERE {Q(column)} IS NULL" : $"WHERE {Q(column)} = @value";
        }
        if (since is not null)
        {
            // Rows written through KMDb carry _modified; rows straight from the copy tool count as changed when copied.
            var changed = columns.ContainsKey(ModifiedColumn)
                ? $"COALESCE(`{ModifiedColumn}`, (SELECT copied_utc FROM `{StatusTable}` WHERE collection_name = @collection)) >= @since"
                : $"(SELECT copied_utc FROM `{StatusTable}` WHERE collection_name = @collection) >= @since";
            where += (where.Length == 0 ? "WHERE " : " AND ") + changed;
        }

        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"""
            SELECT {SelectList(columns)}, COUNT(*) OVER () AS `__total`
              FROM {Q(collection)} {where}
             ORDER BY `_id`
             LIMIT @take OFFSET @skip
            """, conn);
        if (value is not null) cmd.Parameters.AddWithValue("@value", value);
        cmd.Parameters.AddWithValue("@since", since?.ToUniversalTime());
        cmd.Parameters.AddWithValue("@collection", collection);
        cmd.Parameters.AddWithValue("@take", pageSize);
        cmd.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
        var items = new List<JObject>();
        var total = 0;
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
        {
            items.Add(ToJson(reader, columns, collection));
            total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("__total")), CultureInfo.InvariantCulture);
        }
        if (items.Count == 0) total = await CountAsync(conn, collection, where, value, since, ct).ConfigureAwait(false);
        return new SilverScreenPage(collection, page, pageSize, total, items);
    }

    public async Task<SilverScreenImage?> ImageAsync(string id, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand("SELECT `Data`, `Added` FROM `Image` WHERE `_id` = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        if (!await reader.ReadAsync(ct).ConfigureAwait(false) || reader.IsDBNull(0)) return null;
        return new SilverScreenImage((byte[])reader.GetValue(0), reader.IsDBNull(1) ? null : DateTime.SpecifyKind(reader.GetDateTime(1), DateTimeKind.Utc));
    }

    public async Task<IReadOnlyList<string>> PosterSourcesAsync(string imdbId, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand("""
            SELECT src FROM (
              SELECT 1 AS o, `PosterPath` AS src FROM `Movie` WHERE `ImdbId` = @id
              UNION ALL SELECT 2, `PosterPath` FROM `TvShow` WHERE JSON_VALUE(`ExternalIds`, '$.ImdbId') = @id
              UNION ALL SELECT 3, `Poster` FROM `Item` WHERE `_id` = @id
              UNION ALL SELECT 4, `Poster` FROM `Episode` WHERE `_id` = @id
              UNION ALL SELECT 5, `Poster` FROM `WatchListItem` WHERE `ImdbId` = @id
              UNION ALL SELECT 6, `Poster` FROM `KinoWishlist` WHERE `_id` = @id
            ) s
            WHERE src IS NOT NULL AND src <> '' AND src <> 'N/A'
            ORDER BY o
            """, conn);
        cmd.Parameters.AddWithValue("@id", imdbId);
        var sources = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false)) sources.Add(reader.GetString(0));
        return sources;
    }

    /// <summary>The order sources are preferred in: what Kino showed, Kodi's chosen art, then fetched ones.</summary>
    internal const string SourceOrder = "FIELD(COALESCE(`Source`, 'kino'), 'kino', 'kodi', 'tmdb', 'plex', 'omdb', 'web')";

    public async Task<SilverScreenImage?> ArtAsync(string imdbId, string type, int? season = null, int? episode = null, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"""
            SELECT `Data`, `Added` FROM `Image`
             WHERE `ImdbId` = @imdb AND `ImageType` = @type AND `Data` IS NOT NULL
               AND `Season` <=> @season AND `Episode` <=> @episode
             ORDER BY {SourceOrder}, `Width` DESC
             LIMIT 1
            """, conn);
        cmd.Parameters.AddWithValue("@imdb", imdbId);
        cmd.Parameters.AddWithValue("@type", type);
        cmd.Parameters.AddWithValue("@season", season);
        cmd.Parameters.AddWithValue("@episode", episode);
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        if (!await reader.ReadAsync(ct).ConfigureAwait(false)) return null;
        return new SilverScreenImage((byte[])reader.GetValue(0), reader.IsDBNull(1) ? null : DateTime.SpecifyKind(reader.GetDateTime(1), DateTimeKind.Utc));
    }

    public async Task<IReadOnlyList<ArtInfo>> ArtListAsync(string imdbId, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"""
            SELECT `_id`, `ImageType`, `Season`, `Episode`, `Source`, `Width`, `Height`, LENGTH(`Data`)
              FROM `Image`
             WHERE `ImdbId` = @imdb AND `Data` IS NOT NULL
             ORDER BY `ImageType`, `Season`, `Episode`, {SourceOrder}, `Width` DESC
            """, conn);
        cmd.Parameters.AddWithValue("@imdb", imdbId);
        var list = new List<ArtInfo>();
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
        {
            var id = reader.GetString(0);
            list.Add(new ArtInfo(id, reader.GetString(1), NullableInt(reader, 2), NullableInt(reader, 3), reader.IsDBNull(4) ? null : reader.GetString(4),
                NullableInt(reader, 5), NullableInt(reader, 6), reader.GetInt64(7), BinaryPath("Image", id, "Data")));
        }
        return list;
    }

    private static int? NullableInt(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i), CultureInfo.InvariantCulture);

    public async Task<IReadOnlySet<string>> ExistingImagesAsync(IReadOnlyCollection<string> ids, CancellationToken ct = default)
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        var wanted = ids.Where(i => !string.IsNullOrEmpty(i)).Distinct(StringComparer.Ordinal).ToList();
        if (wanted.Count == 0) return found;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand { Connection = conn };
        var names = string.Join(", ", wanted.Select((id, i) => { cmd.Parameters.AddWithValue($"@i{i}", id); return $"@i{i}"; }));
        cmd.CommandText = $"""
            SELECT `ImdbId` FROM `Image` WHERE `ImdbId` IN ({names}) AND `ImageType` = 'poster' AND `Season` IS NULL AND `Data` IS NOT NULL
            UNION SELECT `_id` FROM `Image` WHERE `_id` IN ({names}) AND `Data` IS NOT NULL
            """;
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false)) found.Add(reader.GetString(0));
        return found;
    }

    public async Task<byte[]?> BinaryAsync(string collection, string id, string column, CancellationToken ct = default)
    {
        var columns = await ColumnsAsync(collection, ct).ConfigureAwait(false);
        var name = columns?.Keys.FirstOrDefault(c => string.Equals(c, column, StringComparison.OrdinalIgnoreCase));
        if (columns is null || name is null || columns[name] != "longblob") return null;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand($"SELECT {Q(name)} FROM {Q(collection)} WHERE `_id` = @id", conn);
        cmd.Parameters.AddWithValue("@id", IdValue(columns, id));
        return await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false) as byte[];
    }

    public async Task<Item?> ItemAsync(string imdbId, CancellationToken ct = default) =>
        (await GetAsync("Item", imdbId, ct).ConfigureAwait(false))?.ToObject<Item>(OmdbJson.Reader);

    public async Task<Episode?> EpisodeAsync(string imdbId, CancellationToken ct = default) =>
        (await GetAsync("Episode", imdbId, ct).ConfigureAwait(false))?.ToObject<Episode>(OmdbJson.Reader);

    public async Task<IReadOnlyDictionary<string, string>> SeasonReleasedAsync(string seriesId, int season, CancellationToken ct = default)
    {
        var released = new Dictionary<string, string>(StringComparer.Ordinal);
        var doc = await GetAsync("KolibriSeason", $"{seriesId}_{season}", ct).ConfigureAwait(false);
        if (doc?["Episodes"] is not JArray episodes) return released;
        foreach (var e in episodes.OfType<JObject>())
        {
            var id = (string?)e["ImdbId"];
            var date = (string?)e["Released"];
            if (!string.IsNullOrEmpty(id) && OmdbJson.HasValue(date)) released[id] = date!;
        }
        return released;
    }

    /// <summary>Column name → MariaDB data type for a collection's table; null when it isn't a served collection.</summary>
    private async Task<IReadOnlyDictionary<string, string>?> ColumnsAsync(string collection, CancellationToken ct)
    {
        if (Hidden.Contains(collection) || collection.StartsWith('_') || collection.Contains("__", StringComparison.Ordinal)) return null;
        if (_columns.TryGetValue(collection, out var cached)) return cached;

        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand("""
            SELECT column_name, data_type FROM information_schema.columns
             WHERE table_schema = DATABASE() AND table_name = @t
             ORDER BY ordinal_position
            """, conn);
        cmd.Parameters.AddWithValue("@t", collection);
        var columns = new Dictionary<string, string>(StringComparer.Ordinal);
        await using (var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false))
        {
            while (await reader.ReadAsync(ct).ConfigureAwait(false)) columns[reader.GetString(0)] = reader.GetString(1);
        }
        if (columns.Count == 0 || !columns.ContainsKey("_id")) return null;
        _columns[collection] = columns;
        return columns;
    }

    private static async Task<int> CountAsync(MySqlConnection conn, string collection, string where, string? value, DateTime? since, CancellationToken ct)
    {
        await using var cmd = new MySqlCommand($"SELECT COUNT(*) FROM {Q(collection)} {where}", conn);
        if (value is not null) cmd.Parameters.AddWithValue("@value", value);
        cmd.Parameters.AddWithValue("@since", since?.ToUniversalTime());
        cmd.Parameters.AddWithValue("@collection", collection);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false), CultureInfo.InvariantCulture);
    }

    /// <summary>Binary columns are not read in lists and documents; they are fetched on their own (images).</summary>
    private static string SelectList(IReadOnlyDictionary<string, string> columns) =>
        string.Join(", ", columns.Select(c => c.Value == "longblob" ? $"{Q(c.Key)} IS NOT NULL AS {Q(c.Key)}" : Q(c.Key)));

    private static object IdValue(IReadOnlyDictionary<string, string> columns, string id) =>
        columns["_id"] is "int" or "bigint" && long.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : id;

    /// <summary>
    /// A row as the LiteDB document it came from: JSON columns as objects and arrays, dates as ISO UTC, TINYINT(1) as
    /// true/false. A binary field becomes the address that serves it (or null). Plex tokens in URLs are removed.
    /// </summary>
    internal static JObject ToJson(DbDataReader reader, IReadOnlyDictionary<string, string> columns, string collection, bool removeTokens = true)
    {
        var doc = new JObject();
        var id = Convert.ToString(reader.GetValue(reader.GetOrdinal("_id")), CultureInfo.InvariantCulture) ?? "";
        foreach (var (name, type) in columns)
        {
            // Bookkeeping columns (_modified) are not part of the LiteDB document.
            if (name.StartsWith('_') && name != "_id") continue;
            var i = reader.GetOrdinal(name);
            if (reader.IsDBNull(i)) { doc[name] = JValue.CreateNull(); continue; }
            doc[name] = type switch
            {
                "longtext" => ParseJson(reader.GetString(i)),
                "longblob" => Convert.ToInt64(reader.GetValue(i), CultureInfo.InvariantCulture) != 0
                    ? BinaryPath(collection, id, name)
                    : JValue.CreateNull(),
                "datetime" => DateTime.SpecifyKind(reader.GetDateTime(i), DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                "tinyint" => Convert.ToInt32(reader.GetValue(i), CultureInfo.InvariantCulture) != 0,
                _ => JToken.FromObject(reader.GetValue(i)),
            };
        }
        if (removeTokens) RemovePlexTokens(doc);
        return doc;
    }

    /// <summary>Where a binary field is served: images at /silverscreen/image/{id}, other binaries by collection and field.</summary>
    internal static string BinaryPath(string collection, string id, string column) =>
        string.Equals(collection, "Image", StringComparison.OrdinalIgnoreCase) && column == "Data"
            ? $"/silverscreen/image/{Uri.EscapeDataString(id)}"
            : $"/silverscreen/{collection}/{Uri.EscapeDataString(id)}/{column}";

    private static JToken ParseJson(string text)
    {
        try { return JToken.Parse(text); }
        catch (Newtonsoft.Json.JsonReaderException) { return text; }
    }

    /// <summary>Removes X-Plex-Token=… from every text value: library posters point at Plex with the owner's token.</summary>
    internal static void RemovePlexTokens(JToken token)
    {
        var values = token is JContainer container ? container.DescendantsAndSelf() : [token];
        foreach (var value in values.OfType<JValue>().Where(v => v.Type == JTokenType.String).ToList())
        {
            var text = (string)value.Value!;
            if (text.Contains("X-Plex-Token", StringComparison.OrdinalIgnoreCase)) value.Value = WithoutPlexToken(text);
        }
    }

    internal static string WithoutPlexToken(string text) => PlexToken().Replace(text, m => m.Value.StartsWith('?') && m.Value.EndsWith('&') ? "?" : m.Value.StartsWith('?') ? "" : m.Value.EndsWith('&') ? "&" : "").TrimEnd('&');

    [GeneratedRegex("[?&]X-Plex-Token=[^&\"\\s]*&?", RegexOptions.IgnoreCase)]
    private static partial Regex PlexToken();

    internal static string Q(string name) => "`" + name.Replace("`", "``") + "`";
}
