using Kolibri.Kino.Controllers.Playback;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class PlaybackTests
{
    [Fact]
    public async Task The_linked_file_is_found_through_the_path_mapping()
    {
        var folder = Directory.CreateTempSubdirectory("kino-play-");
        try
        {
            File.WriteAllText(Path.Combine(folder.FullName, "Film.2026.mp4"), "");
            var controller = Controller(
                new FileItem { ImdbId = "tt1", FullName = @"\\NAS\Movies\Film.2026.mp4" },
                new KinoOptions { PathMappings = [new PathMapping { From = @"\\NAS\Movies", To = folder.FullName }] });

            var file = await controller.GetFileAsync("tt1");

            Assert.NotNull(file);
            Assert.Equal(Path.Combine(folder.FullName, "Film.2026.mp4"), file.LocalPath);
            Assert.Equal(("video/mp4", true), (file.ContentType, file.LikelyPlays));
        }
        finally
        {
            folder.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task A_series_folder_or_a_missing_file_has_nothing_to_play()
    {
        var folder = Directory.CreateTempSubdirectory("kino-play-");
        try
        {
            Assert.Null(await Controller(new FileItem { ImdbId = "tt1", FullName = folder.FullName }).GetFileAsync("tt1"));
            Assert.Null(await Controller(new FileItem { ImdbId = "tt1", FullName = Path.Combine(folder.FullName, "gone.mkv") }).GetFileAsync("tt1"));
            Assert.Null(await Controller(null).GetFileAsync("tt1"));
        }
        finally
        {
            folder.Delete(recursive: true);
        }
    }

    [Theory]
    [InlineData("a.MP4", "video/mp4", true)]
    [InlineData("a.webm", "video/webm", true)]
    [InlineData("a.mkv", "video/x-matroska", false)]
    [InlineData("a.avi", "video/x-msvideo", false)]
    public void Browsers_are_expected_to_play_mp4_and_webm(string name, string type, bool likely)
    {
        Assert.Equal((type, likely), PlaybackController.TypeOf(name));
    }

    [Fact]
    public async Task Plex_gives_the_page_for_its_own_movies_and_nothing_when_it_fails()
    {
        var plex = new FakePlex().Add(new Item { ImdbId = "tt1", Title = "Film" });

        Assert.Equal("http://plex/web/tt1", await Controller(null, plex: plex).GetPlexPageAsync("tt1"));
        Assert.Null(await Controller(null, plex: plex).GetPlexPageAsync("tt2"));

        plex.Throws = new HttpRequestException("HTPC is off");
        Assert.Null(await Controller(null, plex: plex).GetPlexPageAsync("tt1"));

        Assert.Null(await Controller(null, plex: new FakePlex { IsConfigured = false }.Add(new Item { ImdbId = "tt1" })).GetPlexPageAsync("tt1"));
    }

    [Theory]
    [InlineData("http://HTPC:32400/web/index.html#!/server/abc/details?key=%2Flibrary%2Fmetadata%2F1",
        "http://htpc.local:32400/web/index.html#!/server/abc/details?key=%2Flibrary%2Fmetadata%2F1")]
    [InlineData("http://htpc:32400/web", "http://htpc.local:32400/web")] // case doesn't matter
    [InlineData("http://htpc2:32400/web", "http://htpc2:32400/web")] // another host: unchanged
    [InlineData("not a url", "not a url")]
    public void Links_for_the_browser_use_the_name_it_can_look_up(string url, string expected)
    {
        var options = new KinoOptions { BrowserHosts = [new HostMapping { From = "HTPC", To = "htpc.local" }] };

        Assert.Equal(expected, options.ForBrowser(url));
    }

    [Fact]
    public async Task Plex_pages_are_sent_with_the_browser_host()
    {
        var plex = new FakePlex().Add(new Item { ImdbId = "tt1", Title = "Film" });
        var options = new KinoOptions { BrowserHosts = [new HostMapping { From = "plex", To = "plex.local" }] };

        Assert.Equal("http://plex.local/web/tt1", await Controller(null, options, plex).GetPlexPageAsync("tt1"));
    }

    [Fact]
    public void A_plex_poster_is_fetched_resized_with_the_token()
    {
        Assert.Equal(
            "http://htpc:32400/photo/:/transcode?width=600&height=900&minSize=1&upscale=0&url=%2Flibrary%2Fmetadata%2F1%2Fthumb%2F2&X-Plex-Token=secret",
            PlexLibrary.PosterSized("http://HTPC:32400/library/metadata/1/thumb/2?X-Plex-Token=secret"));
        Assert.Null(PlexLibrary.PosterSized("N/A"));
    }

    [Fact]
    public void The_plex_web_address_opens_the_movie_details()
    {
        Assert.Equal(
            "http://HTPC:32400/web/index.html#!/server/abc123/details?key=%2Flibrary%2Fmetadata%2F45454",
            PlexLibrary.WebPage("http://HTPC:32400", "abc123", "45454"));
    }

    [Fact]
    public async Task A_series_lists_its_episodes_by_season_and_plays_one_by_its_key()
    {
        var folder = Directory.CreateTempSubdirectory("kino-series-");
        try
        {
            var s2 = folder.CreateSubdirectory("Season 2");
            var s1 = folder.CreateSubdirectory("Season 1");
            foreach (var path in new[] { Path.Combine(s2.FullName, "Show.S02E01.mkv"), Path.Combine(s1.FullName, "Show.S01E02.mp4"),
                         Path.Combine(s1.FullName, "Show.S01E01.mp4"), Path.Combine(folder.FullName, "Behind the scenes.mp4"), Path.Combine(s1.FullName, "notes.txt") })
                File.WriteAllText(path, "");
            var controller = Controller(new FileItem { ImdbId = "tt1", FullName = folder.FullName });

            var episodes = await controller.GetEpisodesAsync("tt1");

            Assert.Equal(["S01E01", "S01E02", "S02E01", "Behind the scenes.mp4"], episodes.Select(e => e.Label));
            Assert.Equal("Season 1/Show.S01E02.mp4", episodes[1].Key);
            var file = await controller.GetFileAsync("tt1", episodes[1].Key);
            Assert.Equal(Path.Combine(s1.FullName, "Show.S01E02.mp4"), file?.LocalPath);
            Assert.True(await controller.HasLocalVideoAsync("tt1"));
        }
        finally
        {
            folder.Delete(recursive: true);
        }
    }

    [Theory]
    [InlineData("../secret.mp4")]
    [InlineData("Season 1/../../secret.mp4")]
    [InlineData("/etc/passwd")]
    [InlineData(@"C:\Windows\win.ini")]
    public void An_episode_key_cannot_reach_outside_the_series_folder(string key)
    {
        var folder = Path.Combine(Path.GetTempPath(), "kino-series-root");

        Assert.Null(PlaybackController.Inside(folder, key));
    }

    [Fact]
    public void Plex_shows_are_keyed_by_their_imdb_id()
    {
        var shows = System.Xml.Linq.XDocument.Parse("""
            <MediaContainer>
              <Directory ratingKey="161449" title="MobLand"><Guid id="imdb://tt31510819"/></Directory>
              <Directory ratingKey="1" title="No Guid Show"/>
            </MediaContainer>
            """);

        Assert.Equal([("tt31510819", "161449")], PlexLibrary.ShowKeysFrom(shows));
    }

    [Fact]
    public void Plex_episodes_are_keyed_by_season_and_number_and_found_for_a_file()
    {
        var leaves = System.Xml.Linq.XDocument.Parse("""
            <MediaContainer>
              <Video ratingKey="501" parentIndex="1" index="1" title="Pilot"/>
              <Video ratingKey="502" parentIndex="1" index="2" title="Two"/>
              <Video ratingKey="999" title="No numbers"/>
            </MediaContainer>
            """);

        var keys = PlexLibrary.EpisodeKeysFrom(leaves);

        Assert.Equal(new Dictionary<(int, int), string> { [(1, 1)] = "501", [(1, 2)] = "502" }, keys);
        Assert.Equal("502", PlaybackController.PlexPageOf(new PlayableEpisode("S1/e2.mkv", 1, [2, 3], "e2.mkv"), keys));
        Assert.Null(PlaybackController.PlexPageOf(new PlayableEpisode("x.mkv", 0, [], "x.mkv"), keys));
    }

    private static PlaybackController Controller(FileItem? link, KinoOptions? options = null, FakePlex? plex = null) =>
        new(new OneFile(link), new EpisodeFileFinder(new FileSystemMediaScanner()), plex ?? new FakePlex(), new InMemorySettingsStore(),
            Options.Create(options ?? new KinoOptions()), NullLogger<PlaybackController>.Instance);

    private sealed class OneFile(FileItem? link) : IFileItemRepository
    {
        public Task<FileItem?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) => Task.FromResult(link?.ImdbId == imdbId ? link : null);
        public Task<IReadOnlyList<FileItem>> GetAllAsync(CancellationToken ct = default) => Task.FromResult<IReadOnlyList<FileItem>>(link is null ? [] : [link]);
        public Task<IReadOnlyList<FileItem>> GetInFolderAsync(string folder, CancellationToken ct = default) => throw new NotSupportedException();
        public Task UpsertAsync(FileItem file, CancellationToken ct = default) => throw new NotSupportedException();
        public Task<bool> DeleteAsync(string imdbId, CancellationToken ct = default) => throw new NotSupportedException();
    }
}
