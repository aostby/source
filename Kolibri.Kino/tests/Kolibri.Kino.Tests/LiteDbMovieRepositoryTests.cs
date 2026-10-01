using Kolibri.Kino.Data;
using LiteDB;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class LiteDbMovieRepositoryTests : IDisposable
{
    private readonly TempDatabase _temp = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public async Task Upsert_then_get_round_trips_item()
    {
        using var db = new KinoDatabase(_temp.Path);
        var repo = new LiteDbMovieRepository(db);

        var inserted = await repo.UpsertAsync(Movie("tt0133093", "The Matrix"));
        var updated = await repo.UpsertAsync(Movie("tt0133093", "The Matrix (1999)"));
        var loaded = await repo.GetByImdbIdAsync("tt0133093");

        Assert.True(inserted);
        Assert.False(updated);
        Assert.Equal("The Matrix (1999)", loaded?.Title);
    }

    [Fact]
    public async Task Search_is_case_insensitive_and_filters_by_type()
    {
        using var db = new KinoDatabase(_temp.Path);
        var repo = new LiteDbMovieRepository(db);
        await repo.UpsertAsync(Movie("tt0133093", "The Matrix"));
        await repo.UpsertAsync(Movie("tt0903747", "Breaking Bad", type: "series"));

        var hits = await repo.SearchByTitleAsync("matrix");
        var series = await repo.GetAllAsync("Series");

        Assert.Equal("tt0133093", Assert.Single(hits).ImdbId);
        Assert.Equal("tt0903747", Assert.Single(series).ImdbId);
    }

    [Fact]
    public async Task GetByImdbIds_returns_only_known_ids()
    {
        using var db = new KinoDatabase(_temp.Path);
        var repo = new LiteDbMovieRepository(db);
        await repo.UpsertAsync(Movie("tt0133093", "The Matrix"));

        var found = await repo.GetByImdbIdsAsync(["tt0133093", "tt0000000", "tt0133093"]);

        Assert.Equal("The Matrix", Assert.Single(found).Value.Title);
    }

    [Fact]
    public async Task Reads_items_written_the_SilverScreen_way()
    {
        // Same call LiteDBController.InsertItem makes: explicit _id = ImdbId in collection "Item".
        using (var legacy = new LiteDatabase(_temp.Path))
            legacy.GetCollection<Item>("Item").Insert("tt0110912", Movie("tt0110912", "Pulp Fiction"));

        using var db = new KinoDatabase(_temp.Path);
        var repo = new LiteDbMovieRepository(db);

        Assert.Equal("Pulp Fiction", (await repo.GetByImdbIdAsync("tt0110912"))?.Title);
    }

    [Fact]
    public async Task Delete_removes_item()
    {
        using var db = new KinoDatabase(_temp.Path);
        var repo = new LiteDbMovieRepository(db);
        await repo.UpsertAsync(Movie("tt0133093", "The Matrix"));

        Assert.True(await repo.DeleteAsync("tt0133093"));
        Assert.Null(await repo.GetByImdbIdAsync("tt0133093"));
    }

    internal static Item Movie(string imdbId, string title, string type = "movie", string rating = "7.0", string year = "1999") =>
        new() { ImdbId = imdbId, Title = title, Type = type, Year = year, ImdbRating = rating, Response = "True" };
}
