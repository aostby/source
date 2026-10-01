using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

/// <summary>Cases are real folder and file names from the library.</summary>
public sealed class MovieFileNameParserTests
{
    private readonly MovieFileNameParser _parser = new();

    [Fact]
    public void Imdb_tag_in_folder_name_is_used()
    {
        var parsed = _parser.Parse(@"X:\Spillefilmer\Baby Einstein {imdb-tt9808494}\06BE_MACDONALD.avi");

        Assert.Equal("tt9808494", parsed.ImdbId);
    }

    [Fact]
    public void Year_and_title_come_from_folder_when_file_has_no_year()
    {
        var parsed = _parser.Parse(@"X:\JUL\Arthurs julegaverace (2011) - Arthur Christmas\Arthur Christmas.avi");

        Assert.Equal(2011, parsed.Year);
        Assert.Contains("Arthur Christmas", parsed.Titles);
        Assert.Contains(parsed.Titles, t => t.StartsWith("Arthurs julegaverace", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Release_tags_are_not_part_of_the_title()
    {
        var parsed = _parser.Parse(@"X:\2019\Spies In Disguise (2019) [1080p] [BluRay] [5.1] [YTS.MX]\Spies.In.Disguise.2019.1080p.BluRay.x264.AAC5.1-[YTS.MX].mp4");

        Assert.Equal("Spies In Disguise", parsed.Titles[0]);
        Assert.Equal(2019, parsed.Year);
    }

    [Fact]
    public void Language_and_ripper_tags_are_dropped()
    {
        var parsed = _parser.Parse(@"X:\2009\Arthur Og Maltazards Hevn (2009) Norsk Tale - Nicemaniac\Arthur Og Maltazards Hevn (2009) Norsk Tale - Nicemaniac.avi");

        Assert.Equal("Arthur Og Maltazards Hevn", parsed.Titles[0]);
        Assert.DoesNotContain(parsed.Titles, t => t.Contains("Nicemaniac", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(2009, parsed.Year);
    }

    [Fact]
    public void Language_words_smileys_and_bracketed_years_never_become_titles()
    {
        var parsed = _parser.Parse(@"X:\JUL\Eselets Shrektakulære jul  (2010) Norsk Tale - Nicemaniac =)\Eselets Shrektakulære jul  (2010) Filmen - Norsk Tale - Nicemaniac.avi");
        var dragons = _parser.Parse(@"X:\JUL\Dragetemmeren - Dragons Gift of the Night Fury (2011)\Dragetemmeren - Dragons Gift of the Night Fury.avi");
        var mikke = _parser.Parse(@"X:\JUL\Disney Mikkes Jul I Andeby - Norge - Norwegian - Nicemaniac\Mikkes Jul I Andeby - Norge - Norwegian - Nicemaniac.avi");

        Assert.DoesNotContain("=)", parsed.Titles);
        Assert.Contains("Dragons Gift of the Night Fury", dragons.Titles);
        Assert.All(mikke.Titles, t => Assert.DoesNotContain("Norwegian", t, StringComparison.OrdinalIgnoreCase));
        Assert.Equal("Mikkes Jul I Andeby", mikke.Titles[0]);
    }

    [Theory]
    [InlineData(@"X:\Sauen Shaun 01\Shaun.The.Sheep.E06.Still.life (thecodeishere).avi")]
    [InlineData(@"X:\SauenShaun_06\SauenShaun_S06E02.mkv")]
    public void Episodes_are_recognised(string path) => Assert.True(_parser.Parse(path).LooksLikeSeries);

    [Fact]
    public void Bare_imdb_id_in_file_name_is_used()
    {
        Assert.Equal("tt0133093", _parser.Parse(@"X:\Movies\The Matrix tt0133093.mkv").ImdbId);
    }
}
