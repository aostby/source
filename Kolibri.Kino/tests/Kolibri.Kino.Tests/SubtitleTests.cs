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
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
    }

    private sealed class FakeSubtitles(params SubtitleInfo[] hits) : ISubtitleProvider
    {
        public (string ImdbId, string Languages) Searched { get; private set; }

        public Task<IReadOnlyList<SubtitleInfo>> SearchAsync(string imdbId, string languages, CancellationToken ct = default)
        {
            Searched = (imdbId, languages);
            return Task.FromResult<IReadOnlyList<SubtitleInfo>>(hits);
        }

        public Task<byte[]> DownloadAsync(SubtitleInfo subtitle, CancellationToken ct = default) =>
            subtitle.DownloadUrl.Contains("broken")
                ? throw new HttpRequestException("404")
                : Task.FromResult(Zip(($"{subtitle.ReleaseName}.srt", "1\n00:00:01,000 --> 00:00:02,000\nHi\n")));
    }
}
