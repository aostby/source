using System.Text;
using Kolibri.Kino.Controllers.Playback;

namespace Kolibri.Kino.Tests;

public sealed class PlaybackSubtitlesTests
{
    [Fact]
    public void The_standard_srt_is_first_and_on_by_default()
    {
        var found = PlaybackSubtitles.Find(@"X:\Movies\Film (2026)\Film.2026.mkv",
            ["Film.2026.mkv", "Film.2026.en.srt", "Film.2026.srt", "Film.2026.nor.HI.srt", "Other.srt", "Film.2026.nfo"], "NO,EN");

        Assert.Equal(["Film.2026.srt", "Film.2026.nor.HI.srt", "Film.2026.en.srt"], found.Select(s => s.FileName));
        Assert.Equal(["Standard", "Norsk (HI)", "English"], found.Select(s => s.Label));
        Assert.Equal([true, false, false], found.Select(s => s.IsDefault));
        Assert.Equal("no", found[1].Language);
    }

    [Fact]
    public void Without_a_standard_srt_the_preferred_language_is_shown()
    {
        var found = PlaybackSubtitles.Find("/media/Film.mp4", ["Film.eng.srt", "Film.no.srt"], "NO,EN");

        Assert.Equal(("Film.no.srt", true), (found[0].FileName, found[0].IsDefault));
        Assert.Empty(PlaybackSubtitles.Find("/media/Film.mp4", ["Film.mp4", "Film.nfo"], "NO,EN"));
    }

    [Fact]
    public void Srt_becomes_WebVTT()
    {
        const string srt = "\uFEFF1\r\n00:00:01,500 --> 00:00:03,250\r\n{\\an8}Hei på deg\r\n\r\n2\r\n00:00:04,000 --> 00:00:05,000\r\nJa\r\n";

        Assert.Equal("WEBVTT\n\n1\n00:00:01.500 --> 00:00:03.250\nHei på deg\n\n2\n00:00:04.000 --> 00:00:05.000\nJa\n", PlaybackSubtitles.ToWebVtt(srt));
        Assert.Equal("WEBVTT\n\nx", PlaybackSubtitles.ToWebVtt("WEBVTT\n\nx"));
    }

    [Fact]
    public void Old_western_encoded_subtitles_keep_their_letters()
    {
        Assert.Equal("Blåbærsyltetøy", PlaybackSubtitles.ReadText(Encoding.Latin1.GetBytes("Blåbærsyltetøy")));
        Assert.Equal("Blåbærsyltetøy", PlaybackSubtitles.ReadText(Encoding.UTF8.GetBytes("Blåbærsyltetøy")));
    }
}
