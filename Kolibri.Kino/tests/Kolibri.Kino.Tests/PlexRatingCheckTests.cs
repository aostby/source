using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

/// <summary>Plex rates good movies too high: ratings above 8.5 from Plex are replaced by IMDb's own, from OMDb.</summary>
public sealed class PlexRatingCheckTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly FakePlex _plex = new();
    private readonly FakeOmdb _omdb = new();
    private readonly MovieLinker _linker;

    public PlexRatingCheckTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _linker = new MovieLinker(new LiteDbFileItemRepository(_db), _movies, _plex, _omdb, NullLogger<MovieLinker>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Theory]
    [InlineData("9.1", "8.2")]
    [InlineData("8.6", "9.0")]   // IMDb's own is used also when it's higher: it's the correct one
    public async Task Above_8_5_OMDbs_IMDb_rating_replaces_Plexs(string plexRating, string imdbRating)
    {
        _plex.Add(Movie("tt0000001", "Good", rating: plexRating));
        _omdb.Add(Movie("tt0000001", "Good", rating: imdbRating));

        var item = (await _linker.FindByImdbIdAsync("tt0000001", usePlex: true, default))!;

        Assert.Equal((imdbRating, 1), (item.ImdbRating, _omdb.Calls));
    }

    [Theory]
    [InlineData("8.5", "7.0", 0)]   // not above 8.5: OMDb isn't asked
    [InlineData("N/A", "7.0", 0)]
    [InlineData("9.1", "N/A", 1)]   // OMDb has no rating for it
    public async Task Otherwise_Plexs_rating_stays(string plexRating, string imdbRating, int omdbCalls)
    {
        _plex.Add(Movie("tt0000002", "Fine", rating: plexRating));
        _omdb.Add(Movie("tt0000002", "Fine", rating: imdbRating));

        var item = (await _linker.FindByImdbIdAsync("tt0000002", usePlex: true, default))!;

        Assert.Equal((plexRating, omdbCalls), (item.ImdbRating, _omdb.Calls));
    }

    [Fact]
    public async Task OMDb_unavailable_or_not_knowing_the_movie_keeps_Plexs_rating()
    {
        _plex.Add(Movie("tt0000003", "Unknown to OMDb", rating: "9.0"));
        Assert.Equal("9.0", (await _linker.FindByImdbIdAsync("tt0000003", usePlex: true, default))!.ImdbRating);

        _omdb.Add(Movie("tt0000003", "Unknown to OMDb", rating: "7.5"));
        _omdb.UnavailableMessage = "Request limit reached!";
        Assert.Equal("9.0", (await _linker.CheckPlexRatingAsync(_plex.Movies[0], default)).ImdbRating);
    }

    [Fact]
    public async Task Library_entries_and_series_are_not_checked()
    {
        // A library entry's rating may have been set by hand; it's never second-guessed.
        await _movies.UpsertAsync(Movie("tt0000004", "Set by hand", rating: "9.6"));
        _plex.Add(Movie("tt0000005", "A series", type: "series", rating: "9.4"));
        _omdb.Add(Movie("tt0000004", "Set by hand", rating: "7.0")).Add(Movie("tt0000005", "A series", type: "series", rating: "7.0"));

        Assert.Equal("9.6", (await _linker.FindByImdbIdAsync("tt0000004", usePlex: true, default))!.ImdbRating);
        Assert.Equal("9.4", (await _linker.CheckPlexRatingAsync(_plex.Movies[0], default)).ImdbRating);
        Assert.Equal(0, _omdb.Calls);
    }
}
