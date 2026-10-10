using Kolibri.Kino.Controllers.Library;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class LibraryFilterTests
{
    private static readonly Item Avatar = new()
    {
        ImdbId = "tt0499549", Title = "Avatar", Year = "2009", Genre = "Action, Adventure, Fantasy", ImdbRating = "7.9",
        Director = "James Cameron", Actors = "Sam Worthington, Zoë Saldaña, Sigourney Weaver",
    };

    private static readonly Item Unknown = new() { ImdbId = "tt1", Title = "Unknown", Year = "N/A", Genre = "N/A", ImdbRating = "N/A" };

    [Fact]
    public void Filters_combine_and_an_empty_filter_passes_everything()
    {
        Assert.True(new LibraryFilter().IsEmpty);
        Assert.True(new LibraryFilter().Matches(Unknown));
        Assert.True(new LibraryFilter(Genre: "adventure", YearFrom: 2000, YearTo: 2009, MinRating: 7.5, Person: "cameron").Matches(Avatar));
        Assert.False(new LibraryFilter(Genre: "Adventure", MinRating: 8).Matches(Avatar));
    }

    [Theory]
    [InlineData("Adventure", true)]
    [InlineData("Advent", false)] // a whole genre, not part of one
    [InlineData("Drama", false)]
    public void A_genre_is_one_of_the_items_genres(string genre, bool expected)
    {
        Assert.Equal(expected, new LibraryFilter(Genre: genre).Matches(Avatar));
    }

    [Theory]
    [InlineData("zoe saldana", true)] // case and accents don't matter
    [InlineData("Weaver", true)]
    [InlineData("James Cameron", true)] // directors too
    [InlineData("Tom Hanks", false)]
    public void A_person_is_found_among_actors_and_director(string person, bool expected)
    {
        Assert.Equal(expected, new LibraryFilter(Person: person).Matches(Avatar));
    }

    [Fact]
    public void Items_without_year_or_rating_fail_those_filters()
    {
        Assert.False(new LibraryFilter(YearFrom: 1900).Matches(Unknown));
        Assert.False(new LibraryFilter(MinRating: 1).Matches(Unknown));
        Assert.Empty(LibraryFilter.GenresOf(Unknown));
    }

    [Fact]
    public void A_series_counts_from_its_first_year()
    {
        var series = new Item { Year = "2019–2023" };

        Assert.Equal(2019, LibraryFilter.YearOf(series));
        Assert.True(new LibraryFilter(YearFrom: 2010, YearTo: 2019).Matches(series));
    }

    [Fact]
    public void Genres_are_counted_and_alphabetical()
    {
        var drama = new Item { Genre = "Drama, Adventure" };

        Assert.Equal([("Action", 1), ("Adventure", 2), ("Drama", 1), ("Fantasy", 1)], LibraryFilter.Genres([Avatar, drama, Unknown]));
    }
}
