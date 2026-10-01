using System.Net;
using Kolibri.Kino.Data;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class CachedPosterProviderTests : IDisposable
{
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 1, 2, 3];

    private readonly TempDatabase _temp = new();
    private readonly CountingHandler _handler = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public async Task Downloads_once_then_serves_from_cache_across_connections()
    {
        var item = new Item { ImdbId = "tt0133093", Poster = "https://example.test/matrix.jpg" };

        using (var store = new LiteDbImageStore(_temp.Path))
            Assert.Equal(Jpeg, await Provider(store).GetPosterAsync(item));

        // A new connection stands in for a new process: the old GetHashCode keys failed exactly here.
        using (var store = new LiteDbImageStore(_temp.Path))
            Assert.Equal(Jpeg, await Provider(store).GetPosterAsync(item));

        Assert.Equal(1, _handler.Calls);
    }

    [Fact]
    public async Task Stores_bytes_under_the_imdb_id_without_growing_on_repeat()
    {
        var item = new Item { ImdbId = "tt0133093", Poster = "https://example.test/matrix.jpg" };
        using var store = new LiteDbImageStore(_temp.Path);
        var provider = Provider(store);

        for (var i = 0; i < 5; i++) await provider.GetPosterAsync(item);

        Assert.Equal(Jpeg, await store.GetAsync("tt0133093"));
        Assert.Equal(1, _handler.Calls);
    }

    [Fact]
    public async Task Reuses_posters_SilverScreen_cached_under_their_url()
    {
        var item = new Item { ImdbId = "tt0133093", Poster = "https://example.test/matrix.jpg" };
        using var store = new LiteDbImageStore(_temp.Path);
        await store.SaveAsync(item.Poster, Jpeg);

        Assert.Equal(Jpeg, await Provider(store).GetPosterAsync(item));
        Assert.Equal(0, _handler.Calls);
    }

    [Theory]
    [InlineData("N/A")]
    [InlineData("")]
    [InlineData(null)]
    public async Task No_poster_url_means_no_request(string? poster)
    {
        using var store = new LiteDbImageStore(_temp.Path);

        Assert.Null(await Provider(store).GetPosterAsync(new Item { ImdbId = "tt0133093", Poster = poster }));
        Assert.Equal(0, _handler.Calls);
    }

    private CachedPosterProvider Provider(LiteDbImageStore store) => new(store, new HttpClient(_handler, disposeHandler: false));

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Jpeg) });
        }
    }
}
