using System.Text.Json;
using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class RatingControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbRatingRepository _ratings;
    private readonly FakePlex _plex = new();
    private readonly RatingController _controller;

    public RatingControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _ratings = new LiteDbRatingRepository(_db);
        _controller = new RatingController(_ratings, _plex, NullLogger<RatingController>.Instance);
        _plex.Add(Movie("tt0082010", "An American Werewolf in London"));
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task Saving_keeps_the_rating_and_review_in_Kino_and_sets_the_rating_on_Plex()
    {
        var result = await _controller.SaveAsync("tt0082010", 7, "  Still funny, still scary.  ", "An American Werewolf in London");

        var stored = (await _controller.GetAsync("tt0082010"))!;
        Assert.Equal((7, 3.5, "Still funny, still scary."), (stored.Rating, stored.Stars, stored.Review));
        Assert.Equal(7, _plex.Ratings["tt0082010"]);
        Assert.Equal("Saved your rating, 3½ stars. Also set on Plex (only you see it).", result.Message);
    }

    [Fact]
    public async Task Plex_not_having_it_or_failing_keeps_the_rating_in_Kino()
    {
        Assert.EndsWith("Plex doesn't have it, so it's in Kino only.", (await _controller.SaveAsync("tt0000001", 4, null, "Not on Plex")).Message);

        _plex.Throws = new HttpRequestException("500 Internal Server Error");
        Assert.EndsWith("Not set on Plex: 500 Internal Server Error", (await _controller.SaveAsync("tt0082010", 10, null, null)).Message);
        Assert.Equal(10, (await _controller.GetAsync("tt0082010"))!.Rating);

        _plex.IsConfigured = false;
        Assert.Equal("Saved your rating, 1 star.", (await _controller.SaveAsync("tt0082010", 2, null, null)).Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public async Task A_rating_is_half_a_star_to_five_stars(int rating) =>
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _controller.SaveAsync("tt0082010", rating, null, null));

    [Fact]
    public async Task Clearing_removes_it_in_Kino_and_on_Plex()
    {
        await _controller.SaveAsync("tt0082010", 8, "Good", null);

        var result = await _controller.ClearAsync("tt0082010");

        Assert.Null(await _controller.GetAsync("tt0082010"));
        Assert.Null(_plex.Ratings["tt0082010"]);
        Assert.Equal("Your rating is removed. Removed on Plex too.", result.Message);
    }

    [Fact]
    public async Task Import_takes_new_and_newer_ratings_keeps_newer_ones_here_and_sends_the_changes_to_Plex()
    {
        var now = DateTime.UtcNow;
        await _ratings.SaveAsync(new MovieRating("tt0000001", 6, "Here, newer", "Kept", now));
        await _ratings.SaveAsync(new MovieRating("tt0082010", 4, null, "Older here", now.AddDays(-10)));

        // As exported from the NAS's Kino.
        var file = JsonSerializer.Serialize(new
        {
            format = RatingController.FileFormat, version = 1, exported = now,
            ratings = new object[]
            {
                new { imdbId = "tt0000001", title = "Kept", rating = 2, updated = now.AddDays(-1) },
                new { imdbId = "tt0082010", title = "Older here", rating = 9, review = "Better than I remembered", updated = now.AddDays(-2) },
                new { imdbId = "tt0000003", title = "New", rating = 5, updated = now.AddDays(-3) },
                new { imdbId = "nonsense", title = "Bad id", rating = 5, updated = now },
                new { imdbId = "tt0000004", title = "Bad rating", rating = 12, updated = now },
            },
        });

        var result = await _controller.ImportAsync(file);

        Assert.Equal((1, 1, 1, 2), (result.Added, result.Updated, result.KeptNewer, result.Invalid));
        Assert.Equal((6, 9, 5), ((await _controller.GetAsync("tt0000001"))!.Rating, (await _controller.GetAsync("tt0082010"))!.Rating,
            (await _controller.GetAsync("tt0000003"))!.Rating));
        Assert.Equal("Better than I remembered", (await _controller.GetAsync("tt0082010"))!.Review);
        Assert.Equal((1, "1 aren't on Plex."), (result.SentToPlex, result.PlexNote)); // tt0000003 isn't on the fake Plex
        Assert.Equal(9, _plex.Ratings["tt0082010"]);
    }

    [Fact]
    public async Task An_exported_file_imports_into_another_Kino()
    {
        await _controller.SaveAsync("tt0082010", 7, "Review", "An American Werewolf in London");
        var exported = await _controller.ExportAsync();

        using var otherTemp = new TempDatabase();
        using var otherDb = new KinoDatabase(otherTemp.Path);
        var other = new RatingController(new LiteDbRatingRepository(otherDb), new FakePlex { IsConfigured = false }, NullLogger<RatingController>.Instance);
        var result = await other.ImportAsync(exported);

        Assert.Equal(1, result.Added);
        var copied = (await other.GetAsync("tt0082010"))!;
        Assert.Equal((7, "Review", "An American Werewolf in London"), (copied.Rating, copied.Review, copied.Title));
        Assert.Equal(0, (await other.ImportAsync(exported)).Added + (await other.ImportAsync(exported)).Updated); // the same file again: nothing new
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"format":"something-else","ratings":[]}""")]
    public async Task Other_files_are_refused(string json) =>
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.ImportAsync(json));

    [Theory]
    [InlineData(0.5, "½ star")]
    [InlineData(1, "1 star")]
    [InlineData(3.5, "3½ stars")]
    [InlineData(5, "5 stars")]
    public void Stars_read_as_text(double stars, string expected) => Assert.Equal(expected, RatingController.StarsText(stars));
}
