using System.IO.Compression;
using System.Net;
using System.Text;
using Kolibri.Kino.Controllers.Subtitles;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class SubtitleTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-subs-").FullName;
    private readonly FileSystemSubtitleFolder _subs = new();

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private string Movie(string name = "Movie (2020).mkv")
    {
        var path = Path.Combine(_folder, name);
        File.WriteAllText(path, "video");
        return path;
    }

    [Fact]
    public void Status_follows_SilverScreen()
    {
        Assert.Equal(SubtitleState.NoFile, _subs.GetStatus(Path.Combine(_folder, "gone.mkv")).State);

        var mkv = Movie();
        var avi = Movie("Other.avi");
        Assert.Equal(SubtitleState.NoneMaybeBuiltIn, _subs.GetStatus(mkv).State);
        Assert.Equal(SubtitleState.None, _subs.GetStatus(avi).State);

        File.WriteAllText(Path.Combine(_folder, "Movie (2020).en.srt"), "tiny"); // under 1,000 bytes: ignored
        Assert.Equal(SubtitleState.NoneMaybeBuiltIn, _subs.GetStatus(mkv).State);

        Directory.CreateDirectory(Path.Combine(_folder, "Subs"));
        File.WriteAllText(Path.Combine(_folder, "Subs", "a.srt"), "x");
        Assert.Equal(new SubtitleStatus(SubtitleState.SubsFolder, Path.Combine(_folder, "Subs"), 1), _subs.GetStatus(mkv));

        var srt = Path.Combine(_folder, "Movie (2020).srt");
        File.WriteAllText(srt, new string('x', 2000));
        Assert.Equal(new SubtitleStatus(SubtitleState.SrtFile, srt), _subs.GetStatus(mkv));
    }

    [Fact]
    public async Task Zips_unpack_only_subtitles_into_Subs_without_overwriting_or_escaping()
    {
        var mkv = Movie();
        var zip = Zip(("Movie.srt", "one"), ("../../evil.srt", "two"), ("readme.txt", "ad"), ("sub/Movie.srt", "three"));

        var written = await _subs.SaveZipAsync(mkv, zip);

        Assert.Equal(3, written);
        Assert.Equal(["Movie (2).srt", "Movie.srt", "evil.srt"],
            Directory.GetFiles(Path.Combine(_folder, "Subs")).Select(Path.GetFileName).Order(StringComparer.Ordinal));
        Assert.False(File.Exists(Path.Combine(Path.GetDirectoryName(_folder)!, "evil.srt")));
    }

    [Fact]
    public async Task Downloading_saves_every_hit_and_goes_on_after_a_failure()
    {
        var mkv = Movie();
        var provider = new FakeSubtitles(
            new SubtitleInfo("Good.Release", "EN", "/a.zip", false, null),
            new SubtitleInfo("Broken.Release", "NO", "/broken.zip", false, null),
            new SubtitleInfo("Other.Release", "NO", "/b.zip", false, null));
        var settings = new InMemorySettingsStore(new UserSettings { SUBDLkey = "key", SubtitleLanguages = " NO,EN " });
        var controller = new SubtitleController(provider, _subs, settings);

        var result = await controller.DownloadAsync("tt0133093", mkv);

        Assert.Equal(("NO,EN", 3, 2, 2, Path.Combine(_folder, "Subs")), (result.Languages, result.Found, result.Saved, result.Files, result.Folder));
        Assert.Single(result.Failed);
        Assert.Equal(("tt0133093", "NO,EN"), provider.Searched);
    }

    [Fact]
    public async Task SubDL_answers_become_hits_nothing_or_a_key_error()
    {
        var settings = new InMemorySettingsStore(new UserSettings { SUBDLkey = "key" });

        var hits = await Provider("""{"status":true,"subtitles":[{"release_name":"The.Matrix.1999","language":"EN","url":"/subtitle/1-2.zip","hi":true}]}""", settings)
            .SearchAsync("tt0133093", "EN");
        Assert.Equal(new SubtitleInfo("The.Matrix.1999", "EN", "/subtitle/1-2.zip", true, null), Assert.Single(hits));

        Assert.Empty(await Provider("""{"status":false,"error":"can't find movie or tv"}""", settings).SearchAsync("tt0000001", "EN"));

        var refused = await Assert.ThrowsAsync<MovieInfoUnavailableException>(() =>
            Provider("""{"status":false,"error":"Invalid API key"}""", settings, HttpStatusCode.Forbidden).SearchAsync("tt0133093", "EN"));
        Assert.Equal("SubDL: Invalid API key", refused.Message);

        await Assert.ThrowsAsync<MovieInfoUnavailableException>(() =>
            Provider("{}", new InMemorySettingsStore(new UserSettings { SUBDLkey = " " })).SearchAsync("tt0133093", "EN"));
    }

    [Theory]
    [InlineData("The.Lowdown.2025.S01E03.1080p.WEB.h264-ETHEL.mkv", 1, 3)]
    [InlineData("show s2.e10 something.srt", 2, 10)]
    [InlineData("Show - 1x02 - Title.srt", 1, 2)]
    public void Episode_codes_are_read_from_names(string name, int season, int episode) =>
        Assert.Equal((season, episode), SubtitleMatching.EpisodeCode(name));

    [Theory]
    [InlineData("Movie.2020.1920x1080.x264-GRP.mkv")]
    [InlineData("The.Lowdown.2025.S01.COMPLETE.srt")]
    public void Names_without_an_episode_have_no_code(string name) => Assert.Null(SubtitleMatching.EpisodeCode(name));

    [Fact]
    public async Task An_episode_gets_its_own_Subs_folder_with_only_its_files_from_a_season_pack()
    {
        var episode = Movie("Show.S01E02.1080p.WEB.h264-GRP.mkv");
        var other = Movie("Show.S01E03.1080p.WEB.h264-GRP.mkv");
        var zip = Zip(("Show.S01E01.1080p.WEB.h264-GRP.srt", "1"), ("Show.S01E02.1080p.WEB.h264-GRP.srt", "2"),
            ("Show.S01E03.1080p.WEB.h264-GRP.srt", "3"), ("English.srt", "?"));

        Assert.Equal(2, await _subs.SaveZipAsync(episode, zip));

        var folder = Path.Combine(_folder, "Subs", "Show.S01E02.1080p.WEB.h264-GRP");
        Assert.Equal(folder, _subs.SubsFolder(episode));
        Assert.Equal(["English.srt", "Show.S01E02.1080p.WEB.h264-GRP.srt"],
            Directory.GetFiles(folder).Select(Path.GetFileName).Order(StringComparer.Ordinal));
        Assert.Equal(SubtitleState.SubsFolder, _subs.GetStatus(episode).State);
        Assert.Equal(SubtitleState.NoneMaybeBuiltIn, _subs.GetStatus(other).State); // its own folder is still empty
        Assert.Equal(Path.Combine(_folder, "Show.S01E02.1080p.WEB.h264-GRP.srt"), _subs.CopyMatchFromSubs(episode));
    }

    [Fact]
    public void A_season_pack_subtitle_for_another_episode_never_fits()
    {
        var video = "Show.S01E02.1080p.WEB.h264-GRP.mkv";
        Assert.Null(SubtitleMatching.BestMatch(video, ["Show.S01E03.1080p.WEB.h264-GRP.srt"]));
        Assert.Equal("x/Show.S01E02.WEB-GRP.srt", SubtitleMatching.BestMatch(video, ["Show.S01E03.1080p.WEB.h264-GRP.srt", "x/Show.S01E02.WEB-GRP.srt"]));
    }

    [Fact]
    public async Task Episode_downloads_search_SubDL_by_series_season_and_episode()
    {
        var mkv = Movie("Show.S02E05.mkv");
        var provider = new FakeSubtitles(new SubtitleInfo("Show.S02E05", "EN", "/a.zip", false, null));
        var controller = new SubtitleController(provider, _subs, new InMemorySettingsStore(new UserSettings { SUBDLkey = "key", SubtitleLanguages = "EN" }));

        var result = await controller.DownloadEpisodeAsync("tt0903747", 2, 5, mkv);

        Assert.Equal(("tt0903747 S2E5", "EN"), provider.Searched);
        Assert.Equal(Path.Combine(_folder, "Subs", "Show.S02E05"), result.Folder);
        Assert.Equal(Path.Combine(_folder, "Show.S02E05.srt"), result.Copied);
    }

    [Fact]
    public async Task SubDL_episode_search_asks_for_tv_with_season_and_episode()
    {
        var handler = new StaticHandler("""{"status":true,"subtitles":[]}""", HttpStatusCode.OK);
        await new SubDlSubtitleProvider(new HttpClient(handler), new InMemorySettingsStore(new UserSettings { SUBDLkey = "key" }))
            .SearchEpisodeAsync("tt31289994", 1, 3, "EN");
        Assert.Contains("imdb_id=tt31289994&type=tv&season_number=1&episode_number=3&languages=EN", handler.LastUrl);
    }

    [Fact]
    public async Task A_season_download_skips_episodes_with_a_srt_and_fetches_a_season_pack_once()
    {
        var e1 = Movie("Show.S01E01.WEB-GRP.mkv");
        var e2 = Movie("Show.S01E02.WEB-GRP.mkv");
        var e3 = Movie("Show.S01E03.WEB-GRP.mkv");
        File.WriteAllText(Path.Combine(_folder, "Show.S01E03.WEB-GRP.srt"), new string('x', 2000));
        var provider = new FakeSubtitles(new SubtitleInfo("Show.S01.WEB-GRP", "EN", "/pack.zip", false, null))
        {
            Pack = Zip(("Show.S01E01.WEB-GRP.srt", "1"), ("Show.S01E02.WEB-GRP.srt", "2"), ("Show.S01E03.WEB-GRP.srt", "3")),
        };
        var controller = new SubtitleController(provider, _subs, new InMemorySettingsStore(new UserSettings { SUBDLkey = "key" }));

        var result = await controller.DownloadSeasonAsync("tt1", 1, [new(3, e3), new(2, e2), new(1, e1)]);

        Assert.Equal([1, 2], result.Episodes.Select(r => r.Episode.Episode));
        Assert.Equal(3, Assert.Single(result.HadSubtitles).Episode);
        Assert.All(result.Episodes, r => Assert.Equal(1, r.Result.Files));
        Assert.Equal(1, provider.Downloads); // the pack, once
        Assert.True(File.Exists(Path.Combine(_folder, "Show.S01E01.WEB-GRP.srt")));
        Assert.True(File.Exists(Path.Combine(_folder, "Show.S01E02.WEB-GRP.srt")));
    }

    [Fact]
    public async Task Too_many_requests_is_tried_once_more_then_stops_the_run()
    {
        var settings = new InMemorySettingsStore(new UserSettings { SUBDLkey = "key" });
        var hit = new SubtitleInfo("Show.S01E01", "EN", "/a.zip", false, null);

        var once = new SequenceHandler(HttpStatusCode.TooManyRequests, HttpStatusCode.OK);
        Assert.Equal([1, 2, 3], await new SubDlSubtitleProvider(new HttpClient(once), settings).DownloadAsync(hit));
        Assert.Equal(2, once.Calls);

        var always = new SequenceHandler(HttpStatusCode.TooManyRequests, HttpStatusCode.TooManyRequests);
        var refused = await Assert.ThrowsAsync<MovieInfoUnavailableException>(() =>
            new SubDlSubtitleProvider(new HttpClient(always), settings).DownloadAsync(hit));
        Assert.Contains("429", refused.Message);
    }

    [Fact]
    public async Task A_season_download_stops_when_SubDL_refuses_more_and_keeps_what_it_got()
    {
        var e1 = Movie("Show.S01E01.WEB-GRP.mkv");
        var e2 = Movie("Show.S01E02.WEB-GRP.mkv");
        var provider = new FakeSubtitles(new SubtitleInfo("Show.S01.WEB-GRP", "EN", "/limit.zip", false, null));
        var controller = new SubtitleController(provider, _subs, new InMemorySettingsStore(new UserSettings { SUBDLkey = "key" }));

        var result = await controller.DownloadSeasonAsync("tt1", 1, [new(1, e1), new(2, e2)]);

        var (episode, first) = Assert.Single(result.Episodes); // E02 isn't tried
        Assert.Equal((1, 0), (episode.Episode, first.Saved));
        Assert.StartsWith("S01E01: SubDL: too many", result.Stopped);
    }

    private sealed class SequenceHandler(params HttpStatusCode[] codes) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var response = new HttpResponseMessage(codes[Math.Min(Calls++, codes.Length - 1)]) { Content = new ByteArrayContent([1, 2, 3]) };
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
            return Task.FromResult(response);
        }
    }

    private static SubDlSubtitleProvider Provider(string json, ISettingsStore settings, HttpStatusCode status = HttpStatusCode.OK) =>
        new(new HttpClient(new StaticHandler(json, status)), settings);

    private static byte[] Zip(params (string Name, string Text)[] entries)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            foreach (var (name, text) in entries)
                using (var writer = new StreamWriter(archive.CreateEntry(name).Open()))
                    writer.Write(text);
        return stream.ToArray();
    }

    private sealed class StaticHandler(string json, HttpStatusCode status) : HttpMessageHandler
    {
        public string LastUrl { get; private set; } = "";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            LastUrl = request.RequestUri!.ToString();
            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
        }
    }

    private sealed class FakeSubtitles(params SubtitleInfo[] hits) : ISubtitleProvider
    {
        public (string ImdbId, string Languages) Searched { get; private set; }

        public Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, CancellationToken ct = default)
        {
            Searched = (imdbId, languages);
            return Task.FromResult<IReadOnlyList<SubtitleInfo>>(hits);
        }

        public Task<IReadOnlyList<SubtitleInfo>> SearchEpisodeAsync(string seriesImdbId, int season, int episode, string languages,
            CancellationToken ct = default) =>
            SearchAsync($"{seriesImdbId} S{season}E{episode}", languages, ct);

        public byte[]? Pack { get; init; }
        public int Downloads { get; private set; }

        public Task<byte[]> DownloadAsync(SubtitleInfo subtitle, CancellationToken ct = default) =>
            ++Downloads > 0 && subtitle.DownloadUrl.Contains("limit")
                ? throw new MovieInfoUnavailableException("SubDL: too many downloads (HTTP 429).")
                : subtitle.DownloadUrl.Contains("broken")
                ? throw new HttpRequestException("404")
                : Pack is { } pack ? Task.FromResult(pack) : Task.FromResult(Zip(($"{subtitle.ReleaseName}.srt", "1\n00:00:01,000 --> 00:00:02,000\nHi\n")));
    }
}
