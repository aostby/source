using System.Net;
using System.Text.Json;
using Kolibri.Kino.Blazor;
using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Core;
using Kolibri.Kino.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Tests;

public sealed class WebSyncTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly RatingController _ratings;
    private readonly FakeServer _server = new();

    public WebSyncTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _ratings = new RatingController(new LiteDbRatingRepository(_db), new FakePlex(), NullLogger<RatingController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Fact]
    public async Task A_change_posts_the_ratings_file_with_the_key_and_the_downloads_name()
    {
        await _ratings.SaveAsync("tt0082010", 7, "Still funny.", "An American Werewolf in London");
        using var sync = Sync(new WebSyncOptions { Url = "https://example.test/post.php", ApiKey = "secret", DelaySeconds = 0 });
        await sync.StartAsync(CancellationToken.None);

        sync.Changed(WebSyncFile.Ratings);
        sync.Changed(WebSyncFile.Ratings);
        var request = await _server.NextAsync();

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal($"https://example.test/post.php?name=Kino%20ratings%20-%20{DateTime.Now:yyyy-MM-dd}.json", request.Url);
        Assert.Equal("secret", request.ApiKey);
        using var json = JsonDocument.Parse(request.Body);
        Assert.Equal("tt0082010", json.RootElement.GetProperty("ratings")[0].GetProperty("imdbId").GetString());
        await sync.StopAsync(CancellationToken.None);
        Assert.Equal(1, _server.Count);
    }

    [Fact]
    public async Task A_failing_server_is_logged_and_the_next_change_is_sent_again()
    {
        _server.Fail = true;
        using var sync = Sync(new WebSyncOptions { Url = "https://example.test/post.php", ApiKey = "secret", DelaySeconds = 0 });
        await sync.StartAsync(CancellationToken.None);

        sync.Changed(WebSyncFile.Ratings);
        await _server.NextAsync();
        _server.Fail = false;
        sync.Changed(WebSyncFile.Ratings);
        await _server.NextAsync();

        await sync.StopAsync(CancellationToken.None);
        Assert.Equal(2, _server.Count);
    }

    [Fact]
    public async Task Without_a_key_nothing_is_sent()
    {
        using var sync = Sync(new WebSyncOptions { Url = "https://example.test/post.php", DelaySeconds = 0 });
        await sync.StartAsync(CancellationToken.None);

        sync.Changed(WebSyncFile.Ratings);
        await Task.Delay(200);

        await sync.StopAsync(CancellationToken.None);
        Assert.Equal(0, _server.Count);
    }

    private WebSync Sync(WebSyncOptions settings)
    {
        var services = new ServiceCollection().AddSingleton(_ratings).BuildServiceProvider();
        return new WebSync(services, _server, Options.Create(new KinoOptions { WebSync = settings }), NullLogger<WebSync>.Instance);
    }

    private sealed record Request(HttpMethod Method, string Url, string? ApiKey, string Body);

    /// <summary>Takes the posts instead of the web service, and answers 201 (or 500 while <see cref="Fail"/>).</summary>
    private sealed class FakeServer : HttpMessageHandler, IHttpClientFactory
    {
        private readonly System.Threading.Channels.Channel<Request> _requests = System.Threading.Channels.Channel.CreateUnbounded<Request>();
        private int _count;

        public volatile bool Fail;

        public int Count => _count;

        public HttpClient CreateClient(string name) => new(this, disposeHandler: false);

        public async Task<Request> NextAsync()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            return await _requests.Reader.ReadAsync(timeout.Token);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Interlocked.Increment(ref _count);
            var body = await request.Content!.ReadAsStringAsync(ct);
            var key = request.Headers.TryGetValues("X-Api-Key", out var values) ? values.Single() : null;
            var fail = Fail;
            await _requests.Writer.WriteAsync(new Request(request.Method, request.RequestUri!.AbsoluteUri, key, body), ct);
            return new HttpResponseMessage(fail ? HttpStatusCode.InternalServerError : HttpStatusCode.Created);
        }
    }
}
