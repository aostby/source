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
        _watchlists = new WatchlistController(_repo, _files, _settings, _plex, _plex, new NoPosters(),
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
    public async Task List_choices_say_whether_a_list_is_in_Kino_on_Plex_or_both()
    {
        _omdb.Add(Movie("tt0133093", "The Matrix"));
        await _watchlists.AddAsync("tt0133093", "Sci-fi");
        _plex.Playlists["sci-FI"] = [];
        _plex.Playlists["Favoritter"] = [];

        var lists = await _watchlists.GetListChoicesAsync();

        Assert.Null(lists.PlexError);
        Assert.Equal([new WatchlistChoice("Favoritter", false, true), new WatchlistChoice("Jul", true, false), new WatchlistChoice("Sci-fi", true, true)],
            lists.Choices);
    }

    [Fact]
    public async Task Adding_to_only_a_Plex_playlist_leaves_Kinos_watchlists_alone()
    {
        var matrix = Movie("tt0133093", "The Matrix");
        _plex.Add(matrix);
        _plex.Playlists["Favoritter"] = [];

        var result = await _watchlists.AddToListsAsync("tt0133093", "Favoritter", toKino: false, toPlex: true);

        Assert.Equal(("The Matrix", (AddToWatchlistResult?)null, 1, false), (result.Title, result.Kino, result.Plex!.Added, result.Plex.Created));
        Assert.Equal(["tt0133093"], _plex.Playlists["Favoritter"]);
        Assert.Empty(await _watchlists.GetItemsAsync("Favoritter"));
    }

    [Fact]
    public async Task Adding_to_both_makes_a_missing_Plex_playlist_and_reports_titles_Plex_lacks()
    {
        _omdb.Add(Movie("tt0133093", "The Matrix")); // not on Plex

        var result = await _watchlists.AddToListsAsync("tt0133093", "Jul", toKino: true, toPlex: true);

        Assert.NotNull(result.Kino);
        Assert.Single(await _watchlists.GetItemsAsync("Jul"));
        Assert.Equal(["tt0133093"], result.Plex!.NotOnServer);
    }

    [Fact]
    public async Task Sync_from_Plex_adds_what_the_playlist_has_and_marks_what_Plex_has_watched()
    {
        foreach (var (id, title) in new[] { ("tt1", "On both"), ("tt2", "Only on Plex"), ("tt3", "Only in Kino"), ("tt4", "Watched in Kino, not on Plex") })
        {
            _plex.Add(Movie(id, title));
            _omdb.Add(Movie(id, title));
        }
        await _watchlists.AddAsync("tt1", "Jul");
        await _watchlists.AddAsync("tt3", "Jul");
        await _watchlists.AddAsync("tt4", "Jul");
        await _watchlists.SetWatchedAsync("tt4", true);
        _plex.Playlists["Jul"] = ["tt1", "tt2", "tt4"];
        _plex.Watched.UnionWith(["tt1", "tt2"]);

        var result = await _watchlists.SyncFromPlexAsync("Jul");

        Assert.Equal((true, 2), (result.PlaylistFound, result.MarkedWatched));
        Assert.Equal(["Only on Plex"], result.Added);
        var items = (await _watchlists.GetItemsAsync("Jul")).ToDictionary(i => i.ImdbId, i => i.Watched);
        // tt4 is watched in Kino but not on Plex: it stays watched.
        Assert.Equal(new Dictionary<string, string> { ["tt1"] = "Y", ["tt2"] = "Y", ["tt3"] = "N", ["tt4"] = "Y" }, items);
    }

    [Fact]
    public async Task Sync_from_Plex_without_such_a_playlist_makes_it_from_the_watchlist()
    {
        _plex.Add(Movie("tt1", "On Plex"));
        _omdb.Add(Movie("tt2", "Not on Plex"));
        await _watchlists.AddAsync("tt1", "Watched");
        await _watchlists.AddAsync("tt2", "Watched");

        var result = await _watchlists.SyncFromPlexAsync("Watched");

        Assert.False(result.PlaylistFound);
        Assert.Equal((1, true), (result.CreatedOnPlex!.Added, result.CreatedOnPlex.Created));
        Assert.Equal(["tt2"], result.CreatedOnPlex.NotOnServer);
        Assert.Equal(["tt1"], _plex.Playlists["Watched"]);
        Assert.Equal(2, (await _watchlists.GetItemsAsync("Watched")).Count);
    }

    [Fact]
    public void Plex_playlist_items_give_imdb_id_and_watched_only_when_seen_to_the_end()
    {
        // Shaped like HTPC's /playlists/45683/items?includeGuids=1 on 2026-10-09.
        var doc = System.Xml.Linq.XDocument.Parse("""
            <MediaContainer>
              <Video title="Carlito's Way" year="1993" type="movie" viewCount="1" lastViewedAt="1755031203"><Guid id="imdb://tt0106519"/></Video>
              <Video title="Hangman" year="2017" type="movie" lastViewedAt="1748790473"><Guid id="imdb://tt3011960"/></Video>
              <Video title="Pilot" type="episode" viewCount="2"><Guid id="tvdb://1"/></Video>
            </MediaContainer>
            """);

        var entries = PlexLibrary.EntriesFrom(doc);

        Assert.Equal([("tt0106519", true), ("tt3011960", false), (null, true)], entries.Select(e => (e.ImdbId, e.Watched)));
    }

    [Fact]
    public async Task Marking_watched_also_marks_it_on_Plex_when_Plex_has_it()
    {
        _plex.Add(Movie("tt1", "On Plex"));

        Assert.Equal("Also marked watched on Plex.", await _watchlists.MarkWatchedOnPlexAsync("tt1", "On Plex"));
        Assert.Contains("tt1", _plex.Watched);
        Assert.Equal("Not Here isn't on Plex, so it's watched in Kino only.", await _watchlists.MarkWatchedOnPlexAsync("tt2", "Not Here"));

        Assert.Equal("Plex had it as watched already.", await _watchlists.MarkWatchedOnPlexAsync("tt1", "On Plex"));

        _plex.IsConfigured = false;
        Assert.Null(await _watchlists.MarkWatchedOnPlexAsync("tt1", "On Plex"));
    }

    [Fact]
    public async Task Watched_movies_move_to_Watched_and_are_marked_on_Plex_without_a_playlist()
    {
        foreach (var (id, title) in new[] { ("tt1", "Seen, on Plex"), ("tt2", "Seen, not on Plex"), ("tt3", "Not seen"), ("tt4", "Seen, Plex knows") })
            _omdb.Add(Movie(id, title));
        _plex.Add(Movie("tt1", "Seen, on Plex")).Add(Movie("tt4", "Seen, Plex knows"))
            .Add(Movie("tt5", "Seen on Plex, on a list")).Add(Movie("tt6", "Seen on Plex only"));
        _plex.Watched.UnionWith(["tt4", "tt5", "tt6"]);
        _plex.Playlists["Jul"] = ["tt1", "tt4"];
        await _watchlists.AddAsync("tt5", "Western");
        await _watchlists.AddAsync("tt1", "Jul");
        await _watchlists.AddAsync("tt2", "Sci-fi");
        await _watchlists.AddAsync("tt3", "Jul");
        await _watchlists.AddAsync("tt4", "Jul");
        foreach (var id in new[] { "tt1", "tt2", "tt4" }) await _watchlists.SetWatchedAsync(id, true);

        var result = await _watchlists.MoveWatchedToListAsync();

        Assert.Equal(["Seen on Plex only", "Seen on Plex, on a list", "Seen, Plex knows", "Seen, not on Plex", "Seen, on Plex"],
            (await _watchlists.GetItemsAsync("Watched")).Select(i => i.Title).Order(StringComparer.Ordinal));
        Assert.All(await _watchlists.GetItemsAsync("Watched"), i => Assert.Equal("Y", i.Watched));
        Assert.Equal(["Not seen"], (await _watchlists.GetItemsAsync("Jul")).Select(i => i.Title));
        Assert.Equal((4, 0, 1, 1), (result.Moved.Count, result.AlreadyThere, result.AddedFromPlex.Count, result.MarkedFromPlex));
        Assert.Equal((1, 2), (result.PlexMarked, result.PlexAlreadyWatched)); // tt1 marked; tt4 and tt5 watched on Plex already
        Assert.Equal(["Seen, not on Plex"], result.NotOnPlex);
        Assert.Equal(["tt1", "tt4"], _plex.Playlists["Jul"]); // Plex playlists stay as they are
        Assert.DoesNotContain("Watched", _plex.Playlists.Keys);

        var again = await _watchlists.MoveWatchedToListAsync();
        Assert.Equal((0, 5, 0), (again.Moved.Count, again.AlreadyThere, again.AddedFromPlex.Count));
    }

    [Fact]
    public async Task Plex_playlists_not_in_Kino_become_watchlists_leaving_out_titles_on_other_lists()
    {
        foreach (var (id, title) in new[] { ("tt1", "New"), ("tt2", "On Jul"), ("tt3", "Watched on Plex"), ("tt4", "Only on Jul's playlist"), ("tt5", "On Watched") })
        {
            _plex.Add(Movie(id, title));
            _omdb.Add(Movie(id, title));
        }
        await _watchlists.AddAsync("tt2", "Jul");
        await _watchlists.AddAsync("tt5", "Watched");
        _plex.Playlists["Tom Hanks"] = ["tt1", "tt2", "tt3", "tt5"];
        _plex.Playlists["jul"] = ["tt4"];          // Kino has Jul: left alone
        _plex.Playlists["Only taken"] = ["tt2"];   // everything on other lists: not made
        _plex.Playlists["Empty"] = [];
        _plex.Watched.Add("tt3");

        var result = await _watchlists.ImportPlexPlaylistsAsync();

        Assert.Equal(["New"], (await _watchlists.GetItemsAsync("Tom Hanks")).Select(i => i.Title));
        Assert.Equal(["On Watched", "Watched on Plex"], (await _watchlists.GetItemsAsync("Watched")).Select(i => i.Title).Order(StringComparer.Ordinal));
        Assert.Equal("Y", (await _watchlists.GetItemsAsync("Watched")).Single(i => i.ImdbId == "tt3").Watched);
        Assert.Equal(["On Jul"], (await _watchlists.GetItemsAsync("Jul")).Select(i => i.Title));
        Assert.DoesNotContain("Only taken", await _watchlists.GetListNamesAsync());
        Assert.DoesNotContain("Empty", await _watchlists.GetListNamesAsync());
        Assert.Equal((1, 1, 3), (result.Added, result.ToWatched, result.Skipped));

        Assert.Empty((await _watchlists.ImportPlexPlaylistsAsync()).Lists.Where(l => l.Added + l.ToWatched > 0)); // nothing new the second time
    }

    [Fact]
    public async Task Mark_watched_moves_the_movie_to_Watched_keeping_it_once()
    {
        _omdb.Add(Movie("tt1", "The Expendables 2"));
        await _watchlists.AddAsync("tt1", "Jason Statham");

        Assert.Equal("Jason Statham", await _watchlists.MarkWatchedAndMoveAsync("tt1"));
        Assert.Empty(await _watchlists.GetItemsAsync("Jason Statham"));
        Assert.Equal("Y", Assert.Single(await _watchlists.GetItemsAsync("Watched")).Watched);
        Assert.Null(await _watchlists.MarkWatchedAndMoveAsync("tt1")); // on Watched already
        Assert.Null(await _watchlists.MarkWatchedAndMoveAsync("tt9")); // on no watchlist
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
