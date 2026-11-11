using Kolibri.Kino.KMDb;
using Kolibri.Kino.SilverScreenCopy;
using LiteDB;
using Newtonsoft.Json.Linq;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public class SilverScreenSyncTests
{
    private static JObject Doc(string json) => JObject.Parse(json, new JsonLoadSettings());

    [Fact]
    public void A_new_document_is_taken_as_it_is()
    {
        var (merged, changed) = SilverScreenSync.Merge(null, Doc("""{"_id":"tt1","Title":"A"}"""));
        Assert.True(changed);
        Assert.Equal("A", (string?)merged["Title"]);
    }

    [Fact]
    public void Missing_values_are_filled_from_the_other_side_and_kept_values_stay()
    {
        var stored = Doc("""{"_id":"tt1","Title":"Stored","Plot":"N/A","Genre":null,"Cast":[]}""");
        var incoming = Doc("""{"_id":"tt1","Title":"Incoming","Plot":"A plot","Genre":"Drama","Cast":["A"],"Extra":"new"}""");
        var (merged, changed) = SilverScreenSync.Merge(stored, incoming);
        Assert.True(changed);
        Assert.Equal("Stored", (string?)merged["Title"]);
        Assert.Equal("A plot", (string?)merged["Plot"]);
        Assert.Equal("Drama", (string?)merged["Genre"]);
        Assert.Single((JArray)merged["Cast"]!);
        Assert.Equal("new", (string?)merged["Extra"]);
    }

    [Fact]
    public void Empty_incoming_values_never_wipe_stored_ones()
    {
        var stored = Doc("""{"_id":"tt1","Title":"Stored","Updated":"2026-01-01T00:00:00Z"}""");
        var incoming = Doc("""{"_id":"tt1","Title":"","Updated":"2026-06-01T00:00:00Z"}""");
        var (merged, _) = SilverScreenSync.Merge(stored, incoming);
        Assert.Equal("Stored", (string?)merged["Title"]);
    }

    [Theory]
    [InlineData("Updated")]
    [InlineData("Fetched")]
    [InlineData("Checked")]
    [InlineData("Added")]
    public void The_newer_document_wins_when_both_have_a_time(string field)
    {
        var older = Doc($$"""{"_id":"x","Rating":6,"{{field}}":"2026-11-01T10:00:00.000Z"}""");
        var newer = Doc($$"""{"_id":"x","Rating":8,"{{field}}":"2026-11-02T10:00:00.000Z"}""");

        Assert.Equal(8, (int)SilverScreenSync.Merge(older, newer).Merged["Rating"]!);
        var (keep, changed) = SilverScreenSync.Merge(newer, older);
        Assert.Equal(8, (int)keep["Rating"]!);
        Assert.False(changed);
    }

    [Fact]
    public void Without_a_time_on_both_sides_stored_values_are_kept()
    {
        var stored = Doc("""{"_id":"x","Watched":"N"}""");
        var incoming = Doc("""{"_id":"x","Watched":"Y","Added":"2026-11-02T10:00:00Z"}""");
        Assert.Equal("N", (string?)SilverScreenSync.Merge(stored, incoming).Merged["Watched"]);
    }

    [Fact]
    public void The_same_document_again_changes_nothing()
    {
        var doc = Doc("""{"_id":"x","Title":"A","Added":"2026-11-02T10:00:00Z"}""");
        Assert.False(SilverScreenSync.Merge(doc, (JObject)doc.DeepClone()).Changed);
    }

    [Theory]
    [InlineData("Title", true)]
    [InlineData("KmdbTest_2", true)]
    [InlineData("_modified", false)]
    [InlineData("bad name", false)]
    [InlineData("x`; DROP", false)]
    public void Only_plain_names_become_columns(string name, bool ok) => Assert.Equal(ok, SilverScreenSync.IsFieldName(name));

    [Fact]
    public void Json_values_become_column_values()
    {
        Assert.Equal(42L, SilverScreenRepository.ToColumnValue(new JValue(42), "int"));
        Assert.Equal(7.5, SilverScreenRepository.ToColumnValue(new JValue(7.5), "double"));
        Assert.Equal(true, SilverScreenRepository.ToColumnValue(new JValue(true), "tinyint"));
        Assert.Equal(new DateTime(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc),
            SilverScreenRepository.ToColumnValue(new JValue("2026-11-06T10:00:00.000Z"), "datetime"));
        Assert.Equal("""[{"a":1}]""", SilverScreenRepository.ToColumnValue(JArray.Parse("""[{"a":1}]"""), "longtext"));
        Assert.Equal("text", SilverScreenRepository.ToColumnValue(new JValue("text"), "varchar"));
        Assert.Null(SilverScreenRepository.ToColumnValue(JValue.CreateNull(), "varchar"));
        Assert.Equal("TINYINT(1) NULL", SilverScreenRepository.SqlTypeFor(new JValue(true), "x"));
        Assert.StartsWith("LONGTEXT", SilverScreenRepository.SqlTypeFor(new JArray(), "x"));
    }
}

public class SilverScreenReadTests
{
    [Theory]
    [InlineData("http://HTPC:32400/library/metadata/1/thumb/2?X-Plex-Token=abc", "http://HTPC:32400/library/metadata/1/thumb/2")]
    [InlineData("http://HTPC:32400/photo?X-Plex-Token=abc&width=300", "http://HTPC:32400/photo?width=300")]
    [InlineData("http://HTPC:32400/photo?width=300&X-Plex-Token=abc", "http://HTPC:32400/photo?width=300")]
    [InlineData("http://HTPC:32400/photo?a=1&X-Plex-Token=abc&b=2", "http://HTPC:32400/photo?a=1&b=2")]
    public void Plex_tokens_are_removed_from_addresses(string url, string expected) =>
        Assert.Equal(expected, SilverScreenRepository.WithoutPlexToken(url));

    [Fact]
    public void Plex_tokens_are_removed_deep_in_documents()
    {
        var doc = JObject.Parse("""{"Poster":"http://h/x?X-Plex-Token=secret","Episodes":[{"Still":"http://h/y?X-Plex-Token=secret"}]}""");
        SilverScreenRepository.RemovePlexTokens(doc);
        Assert.DoesNotContain("secret", doc.ToString());
    }

    [Fact]
    public void Images_and_other_binaries_get_their_own_address()
    {
        Assert.Equal("/silverscreen/image/tt0133093", SilverScreenRepository.BinaryPath("Image", "tt0133093", "Data"));
        Assert.Equal("/silverscreen/image/https%3A%2F%2Fimage.tmdb.org%2Fa.jpg", SilverScreenRepository.BinaryPath("Image", "https://image.tmdb.org/a.jpg", "Data"));
        Assert.Equal("/silverscreen/WatchListItem/tt1/Picture", SilverScreenRepository.BinaryPath("WatchListItem", "tt1", "Picture"));
    }

    [Theory]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg")]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D }, "image/png")]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif")]
    [InlineData(new byte[] { 0x52, 0x49, 0x46, 0x46, 1, 2, 3, 4, 0x57, 0x45, 0x42, 0x50 }, "image/webp")]
    [InlineData(new byte[] { 1, 2, 3 }, "application/octet-stream")]
    public void Image_types_are_told_from_the_first_bytes(byte[] data, string type) =>
        Assert.Equal(type, SilverScreenController.ContentType(data));

    [Theory]
    [InlineData("/silverscreen/poster/tt1", "GET", true)]
    [InlineData("/silverscreen/image/tt1", "HEAD", true)]
    [InlineData("/silverscreen/image/tt1", "PUT", false)]
    [InlineData("/silverscreen/Item/tt1", "GET", false)]
    [InlineData("/silverscreen", "GET", false)]
    public void Only_reading_images_is_open(string path, string method, bool open) =>
        Assert.Equal(open, ApiKeyMiddleware.IsOpen(new Microsoft.AspNetCore.Http.PathString(path), method));

    [Fact]
    public void Library_documents_are_read_by_their_CSharp_names()
    {
        var episode = JObject.Parse("""{"_id":"tt2301451","SeasonNumber":"5","EpisodeNumber":"14","ImdbId":"tt2301451","SeriesId":"tt0903747"}""")
            .ToObject<Episode>(OmdbJson.Reader)!;
        Assert.Equal(("5", "14", "tt0903747"), (episode.SeasonNumber, episode.EpisodeNumber, episode.SeriesId));
    }
}

public class SilverScreenEnricherTests
{
    [Fact]
    public void Library_values_fill_only_what_IMDb_lacks()
    {
        var item = new Item
        {
            Title = "The Matrix", Plot = "N/A", Rated = "N/A", Released = "N/A", Metascore = "N/A", Director = "Lana Wachowski, Lilly Wachowski",
            Dvd = "N/A", Ratings = [new Rating { Source = "Internet Movie Database", Value = "8.7/10" }],
        };
        var known = new Item
        {
            Plot = "Neo …", Rated = "R", Released = "1999-03-31", Metascore = "73", Director = "Someone else", Dvd = "N/A",
            Ratings = [new Rating { Source = "Internet Movie Database", Value = "8.6/10" }, new Rating { Source = "Rotten Tomatoes", Value = "83%" }],
        };
        SilverScreenEnricher.FillFrom(item, Normalized(known));
        Assert.Equal(("Neo …", "R", "31 Mar 1999", "73"), (item.Plot, item.Rated, item.Released, item.Metascore));
        Assert.Equal("Lana Wachowski, Lilly Wachowski", item.Director);
        Assert.Equal("N/A", item.Dvd);
        Assert.Equal(["Internet Movie Database:8.7/10", "Rotten Tomatoes:83%"], item.Ratings.Select(r => $"{r.Source}:{r.Value}"));
    }

    [Theory]
    [InlineData("73", "73")]
    [InlineData("100", "100")]
    [InlineData("192326", null)]
    [InlineData("N/A", null)]
    public void Only_real_Metascores_are_used(string value, string? expected) => Assert.Equal(expected, SilverScreenEnricher.Metascore(value));

    [Theory]
    [InlineData("1999-03-31", "31 Mar 1999")]
    [InlineData("31 Mar 1999", "31 Mar 1999")]
    public void Dates_get_OMDb_format(string value, string expected) => Assert.Equal(expected, SilverScreenEnricher.OmdbDate(value));

    [Theory]
    [InlineData("http://HTPC:32400/library/metadata/1/thumb/2", true)]
    [InlineData("https://m.media-amazon.com/images/M/x.jpg", false)]
    public void Plex_posters_are_not_handed_out(string url, bool plex) => Assert.Equal(plex, SilverScreenEnricher.IsPlexAddress(url));

    // FillFrom gets the library's values as stored; the enricher cleans dates and Metascores on the way.
    private static Item Normalized(Item known)
    {
        known.Released = SilverScreenEnricher.OmdbDate(known.Released);
        known.Metascore = SilverScreenEnricher.Metascore(known.Metascore);
        return known;
    }
}

public class SilverScreenCopyTests
{
    [Fact]
    public void Columns_follow_the_documents()
    {
        var plan = new ColumnPlan();
        plan.Add(new BsonDocument { ["_id"] = "tt1", ["Title"] = "A", ["Episodes"] = new BsonArray { new BsonDocument { ["N"] = 1 } }, ["Added"] = DateTime.UtcNow });
        plan.Add(new BsonDocument { ["_id"] = "tt2", ["Title"] = new string('x', 500), ["Rating"] = 7, ["Data"] = new byte[] { 1 } });
        plan.Add(new BsonDocument { ["_id"] = "tt3", ["Rating"] = 7.5 });
        var columns = plan.Columns().ToDictionary(c => c.Name);

        Assert.Equal("_id", plan.Columns()[0].Name);
        Assert.Equal(ColumnKind.Text, columns["_id"].Kind);
        Assert.Contains("utf8mb4_bin", columns["_id"].SqlType);
        Assert.Equal("TEXT NULL", columns["Title"].SqlType);
        Assert.Equal(ColumnKind.Json, columns["Episodes"].Kind);
        Assert.Equal(ColumnKind.DateTime, columns["Added"].Kind);
        Assert.Equal(ColumnKind.Double, columns["Rating"].Kind);
        Assert.Equal(ColumnKind.Blob, columns["Data"].Kind);
    }

    [Fact]
    public void Nested_data_becomes_plain_json()
    {
        var value = new BsonDocument
        {
            ["When"] = new DateTime(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc),
            ["Big"] = 5L,
            ["List"] = new BsonArray { 1, "a", BsonValue.Null },
        };
        Assert.Equal("""{"When":"2026-11-06T10:00:00.000Z","Big":5,"List":[1,"a",null]}""", Values.ToJson(value));
    }

    [Fact]
    public void Unset_dates_become_null()
    {
        Assert.Null(Values.ToColumn(new BsonValue(DateTime.MinValue), ColumnKind.DateTime));
        Assert.Equal("5f0c8e6a7e0e7200011eb39a", Values.ToColumn(new BsonValue(new ObjectId("5f0c8e6a7e0e7200011eb39a")), ColumnKind.Text));
    }

    [Fact]
    public void The_table_has_the_key_lookups_and_the_modified_column()
    {
        var sql = Copier.CreateTable("Item__new", [new Column("_id", ColumnKind.Text, "VARCHAR(768) NOT NULL"),
            new Column("ImdbId", ColumnKind.Text, "VARCHAR(255) NULL")], "Item");
        Assert.Contains("PRIMARY KEY (`_id`)", sql);
        Assert.Contains("KEY `ix_ImdbId` (`ImdbId`)", sql);
        Assert.Contains("`_modified` DATETIME(3)", sql);
    }
}

public class PosterServiceTests
{
    [Theory]
    [InlineData("https://image.tmdb.org/t/p/w500/jzaweEESl54ejxo3EE5uNL8lgTn.jpg", true)]
    [InlineData("https://m.media-amazon.com/images/M/x.jpg", true)]
    [InlineData("http://HTPC:32400/library/metadata/152394/thumb/1", false)]
    [InlineData("http://x/y?X-Plex-Token=abc", false)]
    [InlineData("N/A", false)]
    [InlineData(@"C:\TEMP\poster.jpg", false)]
    [InlineData("ftp://host/poster.jpg", false)]
    public void Only_public_addresses_are_fetched(string url, bool fetchable) => Assert.Equal(fetchable, PosterService.IsFetchable(url));

    [Fact]
    public async Task TMDb_paths_come_first_and_Plex_is_left_out()
    {
        var service = new PosterService(new Sources(), null!, null!, Microsoft.Extensions.Logging.Abstractions.NullLogger<PosterService>.Instance);
        var sources = await service.SourcesAsync("tt1401143", default);
        Assert.Equal(["https://image.tmdb.org/t/p/w500/jzaweEESl54ejxo3EE5uNL8lgTn.jpg", "https://m.media-amazon.com/x.jpg"], sources);
    }

    private sealed class Sources : ISilverScreenRepository
    {
        public Task<IReadOnlyList<string>> PosterSourcesAsync(string imdbId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<string>>(["/jzaweEESl54ejxo3EE5uNL8lgTn.jpg", "http://HTPC:32400/library/metadata/152394/thumb/1",
                "https://m.media-amazon.com/x.jpg", "/jzaweEESl54ejxo3EE5uNL8lgTn.jpg"]);

        public Task<IReadOnlyList<SilverScreenCollection>> CollectionsAsync(CancellationToken ct = default) => throw new NotSupportedException();
        public Task<JObject?> GetAsync(string collection, string id, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<SilverScreenPage?> ListAsync(string collection, int page, int pageSize, string? field, string? value, DateTime? since = null, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<SilverScreenImage?> ImageAsync(string id, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<byte[]?> BinaryAsync(string collection, string id, string column, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<Item?> ItemAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<Episode?> EpisodeAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<IReadOnlyDictionary<string, string>> SeasonReleasedAsync(string seriesId, int season, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<IReadOnlySet<string>> ExistingImagesAsync(IReadOnlyCollection<string> ids, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<SilverScreenImage?> ArtAsync(string imdbId, string type, int? season = null, int? episode = null, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<ArtInfo>> ArtListAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
    }
}

public class PlexPosterIndexTests
{
    [Fact]
    public void Posters_are_found_by_the_imdb_guid()
    {
        var list = System.Xml.Linq.XDocument.Parse("""
            <MediaContainer>
              <Video ratingKey="1" thumb="/library/metadata/1/thumb/111"><Guid id="tmdb://603" /><Guid id="imdb://tt0133093" /></Video>
              <Directory ratingKey="2" thumb="/library/metadata/2/thumb/222"><Guid id="imdb://tt0903747" /></Directory>
              <Video ratingKey="3" thumb="/library/metadata/3/thumb/333"><Guid id="tvdb://1" /></Video>
              <Video ratingKey="4"><Guid id="imdb://tt0000004" /></Video>
            </MediaContainer>
            """);
        Assert.Equal([("tt0133093", "/library/metadata/1/thumb/111"), ("tt0903747", "/library/metadata/2/thumb/222")],
            PlexPosterIndex.ThumbsFrom(list).ToList());
    }

    [Fact]
    public void Plex_posters_are_resized_by_Plex_and_never_carry_the_token()
    {
        var url = PosterService.PlexPosterUrl("http://HTPC:32400/library/metadata/1/thumb/111?X-Plex-Token=secret");
        Assert.Equal("http://htpc:32400/photo/:/transcode?width=500&height=750&minSize=1&upscale=0&url=%2Flibrary%2Fmetadata%2F1%2Fthumb%2F111", url);
    }
}
