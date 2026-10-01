using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using LiteDB;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class WatchlistControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly LiteDbWatchListRepository _repo;
    private readonly FakeOmdb _omdb = new();
    private readonly FakePlex _plex = new();
    private readonly InMemorySettingsStore _settings = new(new UserSettings { FavoriteWatchList = "Jul" });
    private readonly WatchlistController _watchlists;

    public WatchlistControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _repo = new LiteDbWatchListRepository(_db);
        _watchlists = new WatchlistController(_repo, _files, _settings, _plex, new NoPosters(),
            new MovieLinker(_files, _movies, _plex, _omdb), NullLogger<WatchlistController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task Adds_with_details_and_the_folder_of_the_linked_file()
    {
        await _movies.UpsertAsync(Movie("tt0133093", "The Matrix", rating: "8.7"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0133093", FullName = @"X:\Movies\The Matrix (1999)\matrix.mkv" });

        var result = await _watchlists.AddAsync("tt0133093", "Jul");

        var item = Assert.Single(await _watchlists.GetItemsAsync("Jul"));
        Assert.Equal((null, "The Matrix", "8.7", "N", @"X:\Movies\The Matrix (1999)", "https://www.imdb.com/title/tt0133093"),
            (result.MovedFrom, item.Title, item.ImdbRating, item.Watched, item.FilePath, item.Trailer));
    }

    [Fact]
    public async Task A_movie_is_on_one_list_at_a_time()
    {
        _omdb.Add(Movie("tt0133093", "The Matrix"));
        await _watchlists.AddAsync("tt0133093", "Jul");

        var result = await _watchlists.AddAsync("tt0133093", "Sci-fi");

        Assert.Equal("Jul", result.MovedFrom);
        Assert.Empty(await _watchlists.GetItemsAsync("Jul"));
        Assert.Single(await _watchlists.GetItemsAsync("Sci-fi"));
    }

    [Fact]
    public async Task Reads_SilverScreen_entries_and_keeps_their_picture_when_marking_watched()
    {
        // Written like SilverScreen's WatchListAdd (typed WatchListItem incl. Picture) and AddWatchListName (placeholder).
        _db.Database.GetCollection("WatchListItem").Insert("tt0110912", new BsonDocument
        {
            ["ImdbId"] = "tt0110912", ["Title"] = "Pulp Fiction", ["WatchListName"] = "Jul", ["Watched"] = "N",
            ["Picture"] = new byte[] { 1, 2, 3 },
        });
        _db.Database.GetCollection("WatchListItem").Insert(new BsonDocument { ["WatchListName"] = "Empty list" });

        Assert.Equal(["Empty list", "Jul"], await _watchlists.GetListNamesAsync());
        Assert.True(await _watchlists.SetWatchedAsync("tt0110912", watched: true));

        var doc = _db.Database.GetCollection("WatchListItem").FindById("tt0110912");
        Assert.Equal("Y", doc["Watched"].AsString);
        Assert.Equal([1, 2, 3], doc["Picture"].AsBinary);
    }

    [Fact]
    public async Task The_favorite_list_always_shows_and_new_lists_can_be_empty()
    {
        await _watchlists.CreateListAsync("Påske");

        Assert.Equal(["Jul", "Påske"], await _watchlists.GetListNamesAsync());
        Assert.Empty(await _watchlists.GetItemsAsync("Påske"));
    }

    [Fact]
    public async Task Picking_a_list_makes_it_the_favorite()
    {
        await _watchlists.SetFavoriteAsync("Sci-fi");

        Assert.Equal("Sci-fi", _settings.Current.FavoriteWatchList);
        Assert.Equal("Sci-fi", _watchlists.FavoriteList);
    }

    [Fact]
    public async Task Copies_to_Plex_and_removes_from_Plex_when_asked()
    {
        _plex.Add(Movie("tt0133093", "The Matrix"));
        _omdb.Add(Movie("tt0110912", "Pulp Fiction"));
        await _watchlists.AddAsync("tt0133093", "Jul");
        await _watchlists.AddAsync("tt0110912", "Jul");

        var sync = await _watchlists.CopyToPlexAsync("Jul");
        Assert.Equal((1, true), (sync.Added, sync.Created));
        Assert.Equal(["tt0110912"], sync.NotOnServer);

        Assert.True(await _watchlists.RemoveAsync("tt0133093", "Jul", alsoFromPlex: true));
        Assert.Empty(_plex.Playlists["Jul"]);
        Assert.Single(await _watchlists.GetItemsAsync("Jul"));
    }

    [Fact]
    public void Csv_quotes_separators_and_hides_NA()
    {
        var csv = WatchlistController.ToCsv([new WatchListItem { Title = "Me; Myself \"and\" I", Year = "2000", ImdbRating = "N/A", Watched = "Y", ImdbId = "tt1" }]);

        Assert.Contains("\"Me; Myself \"\"and\"\" I\";2000;;;;Yes;tt1", csv);
    }

    private sealed class NoPosters : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
