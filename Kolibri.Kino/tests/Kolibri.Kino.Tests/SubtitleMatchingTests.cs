using Kolibri.Kino.Core;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class SubtitleMatchingTests : IDisposable
{
    private const string Movie = @"X:\2026\La.Bataille.de.Gaulle.L.Age.de.Fer. (2026) .FRENCH.1080p.WEBRip.EAC3.5.1.x264-GL0P.mkv";
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-match-").FullName;

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    [Fact]
    public void Other_releases_do_not_fit()
    {
        // What SubDL gave for this movie (2026-10-09): none is for the GL0P WEBRip.
        string[] subs =
        [
            @"Subs\La Bataille De Gaulle Lage de fer 2026 1080p AMZN WEB-DL DD 5 1 H 264-playWEB_[eng].srt",
            @"Subs\La Bataille de Gaulle 1 (2026) v2 EN.srt",
            @"Subs\La bataille de Gaulle L'âge de fer (2026).Eng.srt",
        ];
        Assert.Null(SubtitleMatching.BestMatch(Movie, subs));
    }

    [Fact]
    public void The_same_release_fits_best_then_the_same_group()
    {
        string[] subs =
        [
            @"Subs\Other.Movie.2026.1080p.WEB.H264-GL0P.srt",                                                   // same group
            @"Subs\La Bataille de Gaulle L Age de Fer (2026) FRENCH 1080p WEBRip EAC3 5 1 x264-GL0P.eng.srt",  // same release
            @"Subs\La.Bataille.de.Gaulle.L.Age.de.Fer. (2026) .FRENCH.1080p.WEBRip.EAC3.5.1.x264-GL0P.sub",    // not .srt
        ];
        Assert.Equal(subs[1], SubtitleMatching.BestMatch(Movie, subs));
        Assert.Equal(subs[0], SubtitleMatching.BestMatch(Movie, [subs[0], subs[2]]));
    }

    [Theory]
    [InlineData("Movie.2020.1080p.BluRay.x264-SPARKS", "sparks")]
    [InlineData("Movie 2020 720p WEB-DL-NTb", "ntb")]
    [InlineData("Spider-Man (2002)", null)]       // a title with a dash is not a release group
    [InlineData("Movie (2020)", null)]
    public void Release_groups(string name, string? group) => Assert.Equal(group, SubtitleMatching.ReleaseGroup(name));

    [Fact]
    public void A_fitting_subtitle_is_copied_next_to_the_movie_once()
    {
        var movie = Path.Combine(_folder, "Movie.2020.1080p.BluRay.x264-SPARKS.mkv");
        File.WriteAllText(movie, "video");
        var subs = Directory.CreateDirectory(Path.Combine(_folder, "Subs")).FullName;
        File.WriteAllText(Path.Combine(subs, "Movie 2020 1080p BluRay x264-SPARKS [eng].srt"), new string('x', 1500));
        var folder = new FileSystemSubtitleFolder();

        var copied = folder.CopyMatchFromSubs(movie);

        Assert.Equal(Path.Combine(_folder, "Movie.2020.1080p.BluRay.x264-SPARKS.srt"), copied);
        Assert.Equal(Core.Models.SubtitleState.SrtFile, folder.GetStatus(movie).State);
        Assert.Null(folder.CopyMatchFromSubs(movie)); // already there: never overwritten
    }

    [Fact]
    public void Without_a_fit_the_largest_is_offered_first_and_copied_when_chosen()
    {
        var movie = Path.Combine(_folder, "Movie (2020).mkv");
        File.WriteAllText(movie, "video");
        var subs = Directory.CreateDirectory(Path.Combine(_folder, "Subs")).FullName;
        File.WriteAllText(Path.Combine(subs, "small.srt"), new string('x', 500));
        File.WriteAllText(Path.Combine(subs, "large.srt"), new string('x', 5000));
        File.WriteAllText(Path.Combine(subs, "notes.txt"), new string('x', 9000));
        var folder = new FileSystemSubtitleFolder();

        var files = folder.GetSubsFiles(movie);
        Assert.Equal(["large.srt", "small.srt"], files.Select(f => Path.GetFileName(f.Path)));
        Assert.Equal(5000, files[0].Bytes);
        Assert.Null(folder.CopyMatchFromSubs(movie));

        var copied = folder.CopyNextToMovie(movie, files[0].Path);
        Assert.Equal(Path.Combine(_folder, "Movie (2020).srt"), copied);
        Assert.Equal(5000, new FileInfo(copied).Length);
        Assert.Throws<IOException>(() => folder.CopyNextToMovie(movie, files[1].Path)); // never overwritten
    }
}
