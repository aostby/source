using Kolibri.Kino.Controllers.Library;

namespace Kolibri.Kino.Tests;

public sealed class SortByYearTests : IDisposable
{
    private readonly string _root = Directory.CreateTempSubdirectory("kino-sortyear-").FullName;
    private static readonly DateTime Today = new(2026, 11, 4);

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private string Touch(string relative)
    {
        var path = Path.Combine(_root, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "");
        return path;
    }

    [Theory]
    [InlineData("Grindhouse (2007)", 2007)]
    [InlineData("Blade Runner 2049 (2017)", 2017)]
    [InlineData("Blade.Runner.2049.2017.1080p.BluRay", 2017)]
    [InlineData("2001 A Space Odyssey 1968", 1968)]
    [InlineData("1917.2019.1080p", 2019)]
    [InlineData("Heat [1995] 720p", 1995)]
    [InlineData("Blade Runner 2049", null)]
    [InlineData("Movie 1080p", null)]
    public void The_year_is_read_from_the_name(string name, int? year) => Assert.Equal(year, SortByYear.YearOf(name, 2028));

    [Fact]
    public void Folders_go_whole_to_their_year_and_loose_files_get_a_folder_of_their_own()
    {
        Touch(@"Grindhouse (2007)\Grindhouse.mkv");
        Touch(@"Grindhouse (2007)\Sample\sample.mkv");
        Touch(@"No year here\movie.mkv");
        Touch(@"Not a movie (2001)\readme.txt");
        Touch(@"Heat.1995.720p.mkv");
        Touch(@"Heat.1995.720p.en.srt");
        Touch(@"Heat.1995.720p.nfo");
        Touch(@"Other.txt");
        Touch(@"Unknown.mkv");
        Touch(@"2010\Old (2010)\old.mkv"); // a year folder from before: left alone

        var plan = SortByYear.Plan(_root, Today);

        Assert.Equal(["No year here", "Unknown.mkv"], plan.NoYear);
        Assert.Equal(2, plan.Moves.Count);
        var result = SortByYear.Execute(plan);

        Assert.Equal((2, 0), (result.Moved, result.Errors.Count));
        var target = Path.Combine(_root, SortByYear.TargetName);
        Assert.True(File.Exists(Path.Combine(target, @"2007\Grindhouse (2007)\Grindhouse.mkv")));
        Assert.True(File.Exists(Path.Combine(target, @"2007\Grindhouse (2007)\Sample\sample.mkv")));
        Assert.False(Directory.Exists(Path.Combine(_root, "Grindhouse (2007)")));
        foreach (var name in new[] { "Heat.1995.720p.mkv", "Heat.1995.720p.en.srt", "Heat.1995.720p.nfo" })
        {
            Assert.True(File.Exists(Path.Combine(target, @"1995\Heat.1995.720p", name)), name);
            Assert.False(File.Exists(Path.Combine(_root, name)), name);
        }
        Assert.True(File.Exists(Path.Combine(_root, "Other.txt")));
        Assert.True(File.Exists(Path.Combine(_root, @"Not a movie (2001)\readme.txt")));
        Assert.True(File.Exists(Path.Combine(_root, @"2010\Old (2010)\old.mkv")));
    }

    [Fact]
    public void A_place_taken_already_is_left_and_said()
    {
        Touch(@"Heat (1995)\heat.mkv");
        Touch(@"_byYear\1995\Heat (1995)\heat.mkv");

        var plan = SortByYear.Plan(_root, Today);

        Assert.Empty(plan.Moves);
        Assert.Equal(["Heat (1995)"], plan.Taken);
    }

    [Fact]
    public void A_folder_without_a_year_takes_it_from_its_biggest_movie_file()
    {
        Touch(@"Heat\Heat.1995.mkv");
        File.WriteAllText(Path.Combine(_root, @"Heat\Heat.1995.mkv"), new string('x', 100));
        Touch(@"Heat\trailer 2020.mkv");

        var move = Assert.Single(SortByYear.Plan(_root, Today).Moves);

        Assert.Equal(1995, move.Year);
    }
}
