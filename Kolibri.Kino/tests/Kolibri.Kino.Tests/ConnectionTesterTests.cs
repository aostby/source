using System.Net;
using System.Text;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class ConnectionTesterTests
{
    // Only Uranus and Prowlarr have their settings here, so no other service is asked (nothing goes on the network).
    private static UserSettings Settings(string? prowlarrKey = "secret", string? uranusUrl = "https://example.test/KolibriKino/") => new()
    {
        OMDBkey = "", TMDBkey = "", SUBDLkey = null, XPlexServerName = null, XPlexToken = null,
        UranusUrl = uranusUrl, UranusApiKey = "token",
        ProwlarrUrl = "http://nas:9696/", ProwlarrApiKey = prowlarrKey,
    };

    [Fact]
    public async Task Each_service_says_what_is_missing_and_Uranus_and_Prowlarr_are_tried()
    {
        var asked = new List<string>();
        var tester = new ConnectionTester(new HttpClient(new Handler(request =>
        {
            lock (asked) asked.Add($"{request.RequestUri!.AbsoluteUri} {request.Headers.GetValues("X-Api-Key").Single()}");
            return request.RequestUri.AbsolutePath.EndsWith("/status", StringComparison.Ordinal)
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{"appName":"Prowlarr","version":"2.1.5"}""", Encoding.UTF8, "application/json") }
                : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{"status":"ok","files":[]}""") };
        })));

        var checks = (await tester.TestAsync(Settings())).ToDictionary(c => c.Service);

        Assert.Equal(["http://nas:9696/api/v1/system/status secret", "https://example.test/KolibriKino/get.php token"], asked.Order(StringComparer.Ordinal));
        Assert.Equal(("key", false), (checks["OMDb"].Missing, checks["OMDb"].Ok));
        Assert.Equal(("key", true), (checks["TMDb"].Missing, checks["TMDb"].Ok));
        Assert.Equal("key", checks["SubDL"].Missing);
        Assert.Equal("server name and token", checks["Plex"].Missing);
        Assert.Equal((null, true), (checks["Uranus"].Missing, checks["Uranus"].Ok));
        Assert.Equal((null, true, "Connected to http://nas:9696/ (Prowlarr 2.1.5)."), (checks["Prowlarr"].Missing, checks["Prowlarr"].Ok, checks["Prowlarr"].Message));
    }

    [Fact]
    public async Task Missing_or_refused_Prowlarr_and_Uranus_settings_are_named()
    {
        var tester = new ConnectionTester(new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized))));

        var checks = (await tester.TestAsync(Settings(prowlarrKey: " ", uranusUrl: "problembar.net"))).ToDictionary(c => c.Service);

        Assert.Equal(("API key", false), (checks["Prowlarr"].Missing, checks["Prowlarr"].Ok));
        Assert.Equal("valid URL (http or https)", checks["Uranus"].Missing);

        var refused = (await tester.TestAsync(Settings())).ToDictionary(c => c.Service);
        Assert.Equal((null, false), (refused["Prowlarr"].Missing, refused["Prowlarr"].Ok));
        Assert.Contains("rejected the API key", refused["Prowlarr"].Message);
        Assert.Contains("rejected the token", refused["Uranus"].Message);
    }

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => Task.FromResult(respond(request));
    }
}
