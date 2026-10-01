using Kolibri.Kino.Data;
using TMDbLib.Objects.Search;

namespace Kolibri.Kino.Tests;

public sealed class TmdbImdbIdResolverTests
{
    [Fact]
    public void Prefers_same_title_same_year()
    {
        SearchMovie[] results = [Movie(1, "The Lion King", 2019), Movie(2, "The Lion King", 1994)];

        Assert.Equal(2, TmdbImdbIdResolver.Pick(results, "the lion king", 1994)?.Id);
    }

    [Fact]
    public void Accepts_original_title_and_one_year_off()
    {
        SearchMovie[] results = [Movie(7, "Arthur Christmas", 2011, original: "Arthurs julegaverace")];

        Assert.Equal(7, TmdbImdbIdResolver.Pick(results, "Arthurs julegaverace", 2010)?.Id);
    }

    [Fact]
    public void Rejects_different_titles_even_if_popular_in_that_year()
    {
        SearchMovie[] results = [Movie(1, "Frozen", 2013), Movie(2, "Gravity", 2013)];

        Assert.Null(TmdbImdbIdResolver.Pick(results, "Frost", 2013));
    }

    private static SearchMovie Movie(int id, string title, int year, string? original = null) =>
        new() { Id = id, Title = title, OriginalTitle = original ?? title, ReleaseDate = new DateTime(year, 6, 1) };
}
