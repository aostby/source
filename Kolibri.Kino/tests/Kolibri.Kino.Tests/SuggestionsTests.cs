using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Suggestions;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class SuggestionsTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbRatingRepository _ratings;
    private readonly LiteDbSimilarCache _similar;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly SuggestionsController _controller;
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-suggestions-").FullName;

    public SuggestionsTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _ratings = new LiteDbRatingRepository(_db);
        _similar = new LiteDbSimilarCache(_db);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        var libraryFiles = new LibraryFiles(_files, new FileSystemMediaScanner(), Microsoft.Extensions.Options.Options.Create(new KinoOptions()));
        _controller = new SuggestionsController(_ratings, _similar, _movies, libraryFiles);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    /// <summary>Rated a month ago unless <paramref name="daysAgo"/> says otherwise (so not fresh).</summary>
    private Task Rate(string id, string title, int rating, int daysAgo = 30) =>
        _ratings.SaveAsync(new MovieRating(id, rating, null, title, DateTime.UtcNow.AddDays(-daysAgo)));

    private Task Like(string sourceId, params (string Id, string Title, string? Rating)[] similar) =>
        _similar.SaveAsync(new CachedSimilar(sourceId, similar.Select(s => new SimilarHit(new TmdbMovieHit(s.Id, s.Title, "2001"), s.Rating)).ToList(), DateTime.UtcNow));

    [Fact]
    public async Task Nothing_until_three_liked_movies_have_similar_movies_kept()
    {
        await Rate("tt1", "Liked 1", 9);
        await Rate("tt2", "Liked 2", 8);
        await Rate("tt3", "Not liked enough", 6); // 3 stars
        await Like("tt1", ("tt100", "A", "7.0"));
        await Like("tt2", ("tt101", "B", "7.0"));
        await Like("tt3", ("tt102", "C", "7.0"));

        Assert.Empty(await _controller.GetAsync());

        await Rate("tt4", "Liked 3", 7);
        await Like("tt4", ("tt103", "D", "7.0"));
        Assert.Equal(["A", "B", "D"], (await _controller.GetAsync()).Select(s => s.Movie.Title).Order());
    }

    [Fact]
    public async Task Like_several_favourites_first_then_IMDb_rating_and_not_what_you_have_or_have_rated()
    {
        await Rate("tt1", "Klaus", 10);
        await Rate("tt2", "Elf", 8);
        await Rate("tt3", "Home Alone", 7);
        await Rate("tt200", "Already rated", 4); // not liked, but rated: not suggested
        await Like("tt1", ("tt100", "Like two of them", "6.0"), ("tt101", "Like one, rated high", "8.5"), ("tt102", "Have it", "9.0"), ("tt200", "Already rated", "7.0"));
        await Like("tt2", ("tt100", "Like two of them", "6.0"), ("tt103", "Like one, rated low", "5.0"));
        await Like("tt3", ("tt104", "Imported, no file", "7.5"));

        var file = Path.Combine(_folder, "Have it.mkv");
        File.WriteAllText(file, "");
        await _movies.UpsertAsync(Movie("tt102", "Have it"));
        await _files.UpsertAsync(new FileItem { ImdbId = "tt102", FullName = file });
        await _movies.UpsertAsync(Movie("tt104", "Imported, no file", rating: "7.9"));

        var suggestions = await _controller.GetAsync();

        Assert.Equal(["Like two of them", "Like one, rated high", "Like one, rated low", "Imported, no file"], suggestions.Select(s => s.Movie.Title));
        var top = suggestions[0];
        Assert.Equal((18, "Because you liked Klaus and Elf"), (top.Score, SuggestionsController.BecauseText(top)));
        var imported = suggestions[3];
        Assert.Equal((true, "7.9"), (imported.InLibrary, imported.ImdbRating)); // the library's rating
    }

    [Fact]
    public async Task Twenty_of_the_best_forty_are_picked_the_same_all_day_and_differently_the_next()
    {
        foreach (var (id, title) in new[] { ("tt1", "One"), ("tt2", "Two"), ("tt3", "Three") })
        {
            await Rate(id, title, 8);
            await Like(id, Enumerable.Range(0, 20).Select(i => ($"tt{id}{i:00}", $"{title} {i}", (string?)$"{5 + i % 5}.0")).ToArray());
        }

        var monday = await _controller.GetAsync(new DateTime(2026, 11, 2));
        var mondayAgain = await _controller.GetAsync(new DateTime(2026, 11, 2));
        var tuesday = await _controller.GetAsync(new DateTime(2026, 11, 3));

        var all = await _controller.GetAllAsync();
        Assert.Equal(60, all.Count); // "See all": every one, best first
        var picked = monday.Select(s => s.Movie.ImdbId).ToList();
        Assert.Equal(all.Take(SuggestionsController.Pool).Select(s => s.Movie.ImdbId).Where(picked.Contains), picked); // from the best 40, in their order

        Assert.Equal(SuggestionsController.Shown, monday.Count);
        Assert.Equal(monday.Select(s => s.Movie.ImdbId), mondayAgain.Select(s => s.Movie.ImdbId));
        Assert.NotEqual(monday.Select(s => s.Movie.ImdbId).Order(), tuesday.Select(s => s.Movie.ImdbId).Order());
    }

    [Fact]
    public async Task Movies_like_one_rated_this_week_come_first_in_the_row_however_they_rank()
    {
        foreach (var (id, title) in new[] { ("tt1", "One"), ("tt2", "Two"), ("tt3", "Three") })
        {
            await Rate(id, title, 10);
            await Like(id, Enumerable.Range(0, 20).Select(i => ($"tt{id}{i:00}", $"{title} {i}", (string?)"8.0")).ToArray());
        }
        await Rate("tt4", "Just rated", 7, daysAgo: 2);
        await Like("tt4", Enumerable.Range(0, 10).Select(i => ($"tt9{i:00}", $"Fresh {i}", (string?)$"{5 + i / 10.0:0.0}")).ToArray());

        var all = await _controller.GetAllAsync();
        Assert.Equal(70, all.Count);
        Assert.All(all.Skip(60), s => Assert.True(s.Fresh)); // score 7 against 10: last in "See all"

        var row = await _controller.GetAsync(new DateTime(2026, 11, 2));
        Assert.Equal(SuggestionsController.Shown, row.Count);
        Assert.Equal(Enumerable.Range(0, SuggestionsController.FreshShown).Select(i => $"Fresh {9 - i}"), row.Take(SuggestionsController.FreshShown).Select(s => s.Movie.Title));
        Assert.All(row.Skip(SuggestionsController.FreshShown), s => Assert.False(s.Fresh));
    }

    [Theory]
    [InlineData(new[] { "Klaus" }, "Because you liked Klaus")]
    [InlineData(new[] { "Klaus", "Elf", "Home Alone" }, "Because you liked Klaus, Elf and Home Alone")]
    [InlineData(new[] { "Klaus", "Elf", "Home Alone", "Love Actually" }, "Because you liked Klaus, Elf and 2 more")]
    public void The_tooltip_names_the_movies_it_is_like(string[] because, string expected) =>
        Assert.Equal(expected, SuggestionsController.BecauseText(new Suggestion(new TmdbMovieHit("tt1", "X", null), null, 10, because, false)));
}
