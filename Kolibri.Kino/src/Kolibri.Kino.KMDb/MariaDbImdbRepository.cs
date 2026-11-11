using System.Data.Common;
using System.Text;
using MySqlConnector;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// <see cref="IImdbRepository"/> on the MariaDB database with the IMDb data sets (on HTPC). The tables follow the
/// files: title_basics, title_ratings, title_crew, title_episode, title_akas, title_principals, name_basics, keyed by
/// tconst/nconst. The service only reads.
/// </summary>
/// <remarks>
/// Title search matches the start of a title only: the title index makes that fast (well under a second), while
/// LIKE '%word%' reads all 12.8 million titles (about 18 s on HTPC).
/// </remarks>
public sealed class MariaDbImdbRepository(MySqlDataSource dataSource) : IImdbRepository
{
    private const string TitleColumns = """
        b.tconst, b.title_type, b.primary_title, b.original_title, b.is_adult, b.start_year, b.end_year,
        b.runtime_minutes, b.genres, r.average_rating, r.num_votes, c.directors, c.writers
        """;

    private const string TitleJoins = """
        LEFT JOIN title_ratings r ON r.tconst = b.tconst
        LEFT JOIN title_crew c ON c.tconst = b.tconst
        """;

    /// <summary>
    /// Most rows a search takes from each part (prefix, "The " prefix, alternative titles). "the" and "a" have
    /// 200,000+ titles; "star" or "love" (5–11,000) sort in about half a second.
    /// </summary>
    internal const int CandidateLimit = 100000;

    /// <summary>
    /// More hits than this is "Too many results.". Well under <see cref="CandidateLimit"/>, so a list cut off by that
    /// limit (only the alphabetically first titles) is never shown as if it were the best ones.
    /// </summary>
    internal const int TooManyResults = 50000;

    /// <summary>
    /// Alternative titles searched besides the primary title: the Nordic ones (Flåklypa Grand Prix). All regions would
    /// let "star" find Interstellar by some foreign title while the list shows the English one.
    /// </summary>
    internal const string AkaRegions = "'NO', 'SE', 'DK'";

    /// <summary>Most alternative titles a search reads before it keeps the Nordic ones.</summary>
    internal const int AkaScanLimit = 20000;

    public async Task<TitleRow?> GetTitleAsync(string imdbId, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, $"SELECT {TitleColumns} FROM title_basics b {TitleJoins} WHERE b.tconst = @id");
        cmd.Parameters.AddWithValue("@id", imdbId);
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        return await reader.ReadAsync(ct).ConfigureAwait(false) ? ReadTitle(reader) : null;
    }

    public async Task<TitleRow?> FindByTitleAsync(string title, IReadOnlyCollection<string> titleTypes, int? year, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand(null, conn);
        var types = InList(cmd, "type", titleTypes);
        cmd.CommandText = $"""
            SELECT {TitleColumns}
              FROM ((SELECT tconst FROM title_basics WHERE primary_title = @t LIMIT {CandidateLimit})
                    UNION (SELECT title_id FROM title_akas FORCE INDEX (idx_akas_title) WHERE title = @t AND region IN ({AkaRegions}) LIMIT {CandidateLimit})) m
              JOIN title_basics b ON b.tconst = m.tconst
              {TitleJoins}
             WHERE b.title_type IN ({types}) AND b.is_adult = 0 AND (@y IS NULL OR b.start_year = @y)
             ORDER BY COALESCE(r.num_votes, 0) DESC, b.tconst
             LIMIT 1
            """;
        cmd.Parameters.AddWithValue("@t", title);
        cmd.Parameters.AddWithValue("@y", year);
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        return await reader.ReadAsync(ct).ConfigureAwait(false) ? ReadTitle(reader) : null;
    }

    public async Task<SearchPage> SearchAsync(string query, IReadOnlyCollection<string> titleTypes, int? year, int page, int pageSize, CancellationToken ct = default)
    {
        var patterns = SearchPatterns(query);
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand(null, conn);
        var types = InList(cmd, "type", titleTypes);
        var parts = new List<string>();
        for (var i = 0; i < patterns.Count; i++)
        {
            cmd.Parameters.AddWithValue($"@p{i}", patterns[i]);
            parts.Add($"(SELECT tconst FROM title_basics WHERE primary_title LIKE @p{i} AND title_type IN ({types}) LIMIT {CandidateLimit})");
        }
        // The region isn't in the title index: read at most AkaScanLimit index hits, then keep the Nordic ones, so
        // "the" doesn't walk through millions of foreign titles.
        parts.Add($"(SELECT a.title_id FROM (SELECT title_id, region FROM title_akas FORCE INDEX (idx_akas_title) WHERE title LIKE @p0 LIMIT {AkaScanLimit}) a WHERE a.region IN ({AkaRegions}))");
        // COUNT(*) OVER () gives the number of hits with the one page asked for.
        cmd.CommandText = $"""
            SELECT b.tconst, b.title_type, b.primary_title, b.start_year, b.end_year, COUNT(*) OVER () AS total
              FROM ({string.Join(" UNION ", parts)}) m
              JOIN title_basics b ON b.tconst = m.tconst
              LEFT JOIN title_ratings r ON r.tconst = b.tconst
             WHERE b.title_type IN ({types}) AND b.is_adult = 0 AND (@y IS NULL OR b.start_year = @y)
             ORDER BY COALESCE(r.num_votes, 0) DESC, b.tconst
             LIMIT @take OFFSET @skip
            """;
        cmd.Parameters.AddWithValue("@y", year);
        cmd.Parameters.AddWithValue("@take", pageSize);
        cmd.Parameters.AddWithValue("@skip", (page - 1) * pageSize);

        var rows = new List<TitleRow>();
        var total = 0;
        await using (var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false))
        {
            while (await reader.ReadAsync(ct).ConfigureAwait(false))
            {
                rows.Add(new TitleRow(reader.GetString(0), reader.GetString(1), reader.GetString(2), null, false,
                    IntOrNull(reader, 3), IntOrNull(reader, 4), null, null, null, null));
                total = Convert.ToInt32(reader.GetValue(5));
            }
        }
        return total >= TooManyResults ? new SearchPage([], 0, TooMany: true) : new SearchPage(rows, total, TooMany: false);
    }

    public async Task<IReadOnlyDictionary<string, string>> NamesAsync(IReadOnlyCollection<string> nameIds, CancellationToken ct = default)
    {
        var names = new Dictionary<string, string>(StringComparer.Ordinal);
        if (nameIds.Count == 0) return names;
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = new MySqlCommand(null, conn);
        cmd.CommandText = $"SELECT nconst, primary_name FROM name_basics WHERE nconst IN ({InList(cmd, "n", nameIds)})";
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
            names[reader.GetString(0)] = reader.GetString(1);
        return names;
    }

    public async Task<IReadOnlyList<string>> ActorsAsync(string imdbId, int max, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, $"""
            SELECT n.primary_name
              FROM title_principals p JOIN name_basics n ON n.nconst = p.nconst
             WHERE p.tconst = @id AND p.category IN ('actor', 'actress', 'self')
             ORDER BY p.ordering
             LIMIT {max}
            """);
        cmd.Parameters.AddWithValue("@id", imdbId);
        var actors = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false)) actors.Add(reader.GetString(0));
        return actors;
    }

    public async Task<EpisodeLink?> GetEpisodeLinkAsync(string episodeId, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, "SELECT parent_tconst, season_number, episode_number FROM title_episode WHERE tconst = @id");
        cmd.Parameters.AddWithValue("@id", episodeId);
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        return await reader.ReadAsync(ct).ConfigureAwait(false)
            ? new EpisodeLink(reader.GetString(0), IntOrNull(reader, 1), IntOrNull(reader, 2))
            : null;
    }

    public async Task<int?> TotalSeasonsAsync(string seriesId, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, "SELECT MAX(season_number) FROM title_episode WHERE parent_tconst = @id");
        cmd.Parameters.AddWithValue("@id", seriesId);
        var value = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return value is null or DBNull ? null : Convert.ToInt32(value);
    }

    public async Task<IReadOnlyList<SeasonEpisodeRow>> SeasonAsync(string seriesId, int season, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, """
            SELECT e.tconst, e.episode_number, b.primary_title, b.start_year, r.average_rating
              FROM title_episode e
              JOIN title_basics b ON b.tconst = e.tconst
              LEFT JOIN title_ratings r ON r.tconst = e.tconst
             WHERE e.parent_tconst = @id AND e.season_number = @s
             ORDER BY e.episode_number IS NULL, e.episode_number, e.tconst
            """);
        cmd.Parameters.AddWithValue("@id", seriesId);
        cmd.Parameters.AddWithValue("@s", season);
        var rows = new List<SeasonEpisodeRow>();
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
            rows.Add(new SeasonEpisodeRow(reader.GetString(0), IntOrNull(reader, 1), reader.GetString(2), IntOrNull(reader, 3),
                reader.IsDBNull(4) ? null : reader.GetDecimal(4)));
        return rows;
    }

    public async Task<string?> EpisodeIdAsync(string seriesId, int season, int episode, CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        await using var cmd = Sql(conn, """
            SELECT tconst FROM title_episode
             WHERE parent_tconst = @id AND season_number = @s AND episode_number = @e
             ORDER BY tconst LIMIT 1
            """);
        cmd.Parameters.AddWithValue("@id", seriesId);
        cmd.Parameters.AddWithValue("@s", season);
        cmd.Parameters.AddWithValue("@e", episode);
        return await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false) as string;
    }

    public async Task<bool> PingAsync(CancellationToken ct = default)
    {
        await using var conn = await dataSource.OpenConnectionAsync(ct).ConfigureAwait(false);
        // Not just "SELECT 1": the titles must be there too (an empty or renamed table is a failure).
        await using var cmd = Sql(conn, "SELECT EXISTS (SELECT 1 FROM title_basics LIMIT 1)");
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false)) == 1;
    }

    private static MySqlCommand Sql(MySqlConnection conn, string text) => new(text, conn);

    /// <summary>
    /// LIKE patterns for a search: the words at the start of a title, and after "The ", "A " or "An " so "matrix"
    /// finds "The Matrix". The first pattern is the plain prefix (also used on alternative titles).
    /// </summary>
    internal static IReadOnlyList<string> SearchPatterns(string query)
    {
        var q = EscapeLike(query.Trim());
        var patterns = new List<string> { q + "%" };
        if (!StartsWithArticle(query.Trim()))
            patterns.AddRange(["The " + q + "%", "A " + q + "%", "An " + q + "%"]);
        return patterns;
    }

    internal static string EscapeLike(string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            if (ch is '\\' or '%' or '_') sb.Append('\\');
            sb.Append(ch);
        }
        return sb.ToString();
    }

    private static bool StartsWithArticle(string query) =>
        query.StartsWith("the ", StringComparison.OrdinalIgnoreCase)
        || query.StartsWith("a ", StringComparison.OrdinalIgnoreCase)
        || query.StartsWith("an ", StringComparison.OrdinalIgnoreCase);

    /// <summary>Adds one parameter per value and returns "@x0, @x1, …" for an IN list.</summary>
    private static string InList(MySqlCommand cmd, string prefix, IEnumerable<string> values)
    {
        var names = new List<string>();
        foreach (var value in values)
        {
            var name = $"@{prefix}{names.Count}";
            cmd.Parameters.AddWithValue(name, value);
            names.Add(name);
        }
        return names.Count == 0 ? "NULL" : string.Join(", ", names);
    }

    private static TitleRow ReadTitle(DbDataReader r) => new(
        r.GetString(0), r.GetString(1), r.GetString(2), StringOrNull(r, 3), !r.IsDBNull(4) && Convert.ToInt32(r.GetValue(4)) != 0,
        IntOrNull(r, 5), IntOrNull(r, 6), IntOrNull(r, 7), StringOrNull(r, 8),
        r.IsDBNull(9) ? null : r.GetDecimal(9), IntOrNull(r, 10), StringOrNull(r, 11), StringOrNull(r, 12));

    private static int? IntOrNull(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i));

    private static string? StringOrNull(DbDataReader r, int i) => r.IsDBNull(i) ? null : r.GetString(i);
}
