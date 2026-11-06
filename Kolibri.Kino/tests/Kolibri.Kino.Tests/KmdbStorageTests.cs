using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Kolibri.Kino.Data.Storage;
using LiteDB;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public class BsonJsonTests
{
    [Fact]
    public void Documents_go_to_KMDb_as_plain_json_and_come_back_the_same()
    {
        var doc = new BsonDocument
        {
            ["_id"] = "tt0133093",
            ["Title"] = "The Matrix",
            ["Rating"] = 8,
            ["Votes"] = 2_291_970L,
            ["Score"] = 8.7,
            ["Watched"] = true,
            ["Updated"] = new DateTime(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc),
            ["Episodes"] = new BsonArray { new BsonDocument { ["AirDate"] = new DateTime(2008, 1, 20, 0, 0, 0, DateTimeKind.Utc) } },
            ["Picture"] = new byte[] { 1, 2, 3 },
        };
        var json = BsonJson.Serialize(doc);
        Assert.DoesNotContain("Picture", json);
        Assert.Contains("\"Updated\":\"2026-11-06T10:00:00.000Z\"", json);

        using var parsed = JsonDocument.Parse(json);
        var back = BsonJson.ToDocument(parsed.RootElement);
        Assert.Equal("The Matrix", back["Title"].AsString);
        Assert.Equal(8, back["Rating"].AsInt32);
        Assert.Equal(2_291_970L, back["Votes"].AsInt64);
        Assert.Equal(8.7, back["Score"].AsDouble);
        Assert.True(back["Watched"].AsBoolean);
        Assert.True(back["Updated"].IsDateTime);
        Assert.Equal(new DateTime(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc), back["Updated"].AsDateTime.ToUniversalTime());
        Assert.True(back["Episodes"].AsArray[0]["AirDate"].IsDateTime);
    }

    [Theory]
    [InlineData("1999-03-31", false)]
    [InlineData("31 Mar 1999", false)]
    [InlineData("2026-11-06T10:00:00Z", true)]
    [InlineData("2026-11-06T10:00:00.123Z", true)]
    public void Only_KMDbs_date_text_becomes_a_date(string text, bool isDate)
    {
        using var json = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(new { v = text }));
        Assert.Equal(isDate, BsonJson.ToDocument(json.RootElement)["v"].IsDateTime);
    }

    [Fact]
    public void Kmdb_binary_addresses_are_not_document_fields()
    {
        Assert.True(BsonJson.IsBinaryAddress("/silverscreen/WatchListItem/tt1/Picture"));
        Assert.False(BsonJson.IsBinaryAddress("https://image.tmdb.org/x.jpg"));
    }
}

public sealed class KmdbStorageTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly FakeKmdb _kmdb = new();
    private readonly KmdbConnection _connection;

    public KmdbStorageTests()
    {
        _db = new KinoDatabase(_temp.Path);
        var settings = new InMemorySettingsStore(new UserSettings { KmdbUrl = "http://kmdb.test:8086/", UranusApiKey = "key" });
        var client = new KmdbClient(settings, Options.Create(new KinoOptions()), _kmdb);
        _connection = new KmdbConnection(client, _db, settings, NullLogger<KmdbConnection>.Instance);
    }

    [Fact]
    public async Task With_KMDb_answering_the_library_is_read_from_it_and_written_to_both()
    {
        _kmdb.Add("Item", """{"_id":"tt0133093","ImdbId":"tt0133093","Title":"The Matrix","Type":"movie"}""");
        Assert.True((await _connection.CheckAsync()).UsesKmdb);

        var movies = new LiteDbMovieRepository(_db);
        Assert.Equal("The Matrix", (await movies.GetByImdbIdAsync("tt0133093"))?.Title);
        // It came from KMDb: the LiteDB file doesn't have it.
        Assert.Null(_db.Database.GetCollection("Item").FindById("tt0133093"));

        await movies.UpsertAsync(new Item { ImdbId = "tt0234215", Title = "The Matrix Reloaded", Type = "movie" });
        Assert.True(_kmdb.Has("Item", "tt0234215"));
        Assert.NotNull(_db.Database.GetCollection("Item").FindById("tt0234215"));
        Assert.Empty(await _db.Pending.AllAsync());
    }

    [Fact]
    public async Task Without_KMDb_the_LiteDB_file_is_used_and_changes_are_sent_when_it_answers()
    {
        _kmdb.Down = true;
        var state = await _connection.CheckAsync();
        Assert.False(state.UsesKmdb);
        Assert.Contains("ping", state.Message);

        var ratings = new LiteDbRatingRepository(_db);
        await ratings.SaveAsync(new MovieRating("tt0133093", 8, "Good", "The Matrix", DateTime.UtcNow));
        await ratings.RemoveAsync("tt0000001");
        Assert.Equal(2, (await _db.Pending.AllAsync()).Count);
        Assert.False(_kmdb.Has("KinoRating", "tt0133093"));

        _kmdb.Down = false;
        _kmdb.Add("KinoRating", """{"_id":"tt0000001","Rating":3}""");
        Assert.True((await _connection.CheckAsync()).UsesKmdb);
        Assert.True(_kmdb.Has("KinoRating", "tt0133093"));
        Assert.False(_kmdb.Has("KinoRating", "tt0000001"));
        Assert.Empty(await _db.Pending.AllAsync());
    }

    [Fact]
    public async Task A_failing_call_falls_back_to_the_LiteDB_file_at_once()
    {
        Assert.True((await _connection.CheckAsync()).UsesKmdb);
        _db.Database.GetCollection("KinoWishlist").Upsert(new BsonDocument { ["_id"] = "tt1", ["Title"] = "Local", ["Added"] = DateTime.UtcNow });
        _kmdb.Down = true;

        var wishlist = new LiteDbWishlistRepository(_db);
        Assert.Equal("Local", (await wishlist.GetAsync("tt1"))?.Title);
        Assert.False(_connection.UseKmdb);
    }

    [Fact]
    public async Task Without_a_KMDb_url_nothing_is_kept_for_it()
    {
        var settings = new InMemorySettingsStore(new UserSettings());
        using var plain = new TempDatabase();
        using var db = new KinoDatabase(plain.Path);
        var connection = new KmdbConnection(new KmdbClient(settings, Options.Create(new KinoOptions()), _kmdb), db, settings, NullLogger<KmdbConnection>.Instance);
        Assert.False((await connection.CheckAsync()).UsesKmdb);

        await new LiteDbRatingRepository(db).SaveAsync(new MovieRating("tt1", 7, null, null, DateTime.UtcNow));
        Assert.Empty(await db.Pending.AllAsync());
        Assert.Equal(0, _kmdb.Calls);
    }

    [Fact]
    public async Task A_wrong_key_keeps_the_LiteDB_file()
    {
        _kmdb.Key = "other";
        var state = await _connection.CheckAsync();
        Assert.False(state.UsesKmdb);
        Assert.Contains("key", state.Message, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    /// <summary>KMDb's /ping and /silverscreen API, in memory: collections of JSON documents by _id.</summary>
    private sealed class FakeKmdb : HttpMessageHandler
    {
        private readonly Dictionary<string, Dictionary<string, JsonObject>> _collections = new(StringComparer.OrdinalIgnoreCase);
        public bool Down { get; set; }
        public string Key { get; set; } = "key";
        public int Calls { get; private set; }

        public void Add(string collection, string json)
        {
            var doc = JsonNode.Parse(json)!.AsObject();
            Docs(collection)[doc["_id"]!.ToString()] = doc;
        }

        public bool Has(string collection, string id) => Docs(collection).ContainsKey(id);

        private Dictionary<string, JsonObject> Docs(string collection) =>
            _collections.TryGetValue(collection, out var docs) ? docs : _collections[collection] = new Dictionary<string, JsonObject>(StringComparer.Ordinal);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            if (Down) throw new HttpRequestException("Connection refused");
            var path = request.RequestUri!.AbsolutePath.Trim('/').Split('/').Select(Uri.UnescapeDataString).ToArray();
            if (path is ["ping"]) return Json("""{"status":"ok"}""");
            if (request.Headers.TryGetValues("X-Api-Key", out var keys) && keys.Single() != Key)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            switch (request.Method.Method, path)
            {
                case ("GET", ["silverscreen"]):
                    return Json("[]");
                case ("GET", ["silverscreen", var c]):
                    var items = new JsonArray(Docs(c).Values.Select(d => (JsonNode)d.DeepClone()).ToArray());
                    return Json(new JsonObject { ["Collection"] = c, ["Page"] = 1, ["PageSize"] = 500, ["Total"] = items.Count, ["Items"] = items }.ToJsonString());
                case ("GET", ["silverscreen", var c, var id]):
                    return Docs(c).TryGetValue(id, out var doc) ? Json(doc.ToJsonString()) : new HttpResponseMessage(HttpStatusCode.NotFound);
                case ("PUT", ["silverscreen", var c, var id]):
                    Docs(c)[id] = JsonNode.Parse(await request.Content!.ReadAsStringAsync(ct))!.AsObject();
                    return Json(Docs(c)[id].ToJsonString());
                case ("PUT", ["silverscreen", var c]):
                    foreach (var d in JsonNode.Parse(await request.Content!.ReadAsStringAsync(ct))!.AsArray())
                        Docs(c)[d!["_id"]!.ToString()] = d.AsObject().DeepClone().AsObject();
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                case ("DELETE", ["silverscreen", var c, var id]):
                    return Docs(c).Remove(id) ? new HttpResponseMessage(HttpStatusCode.NoContent) : new HttpResponseMessage(HttpStatusCode.NotFound);
                default:
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        private static HttpResponseMessage Json(string json) => new(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
    }
}
