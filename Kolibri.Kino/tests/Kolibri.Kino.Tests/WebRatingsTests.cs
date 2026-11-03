using System.Net;
using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kolibri.Kino.Tests;

public sealed class WebRatingsTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbRatingRepository _repository;
    private readonly RatingController _ratings;
    private readonly FakeUranus _uranus = new();

    public WebRatingsTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _repository = new LiteDbRatingRepository(_db);
        _ratings = new RatingController(_repository, new FakePlex { IsConfigured = false }, NullLogger<RatingController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    private WebRatings Web(string? url, string? key) =>
        new(new InMemorySettingsStore(new UserSettings { UranusUrl = url, UranusApiKey = key }), new HttpClient(_uranus), _ratings);

    [Theory]
    [InlineData("https://problembar.net/uranus/apps/KolibriKino/", "https://problembar.net/uranus/apps/KolibriKino/get.php")]
    [InlineData("https://problembar.net/uranus/apps/KolibriKino", "https://problembar.net/uranus/apps/KolibriKino/get.php")]
    [InlineData(" https://problembar.net/uranus/apps/KolibriKino/post.php ", "https://problembar.net/uranus/apps/KolibriKino/get.php")]
    [InlineData("https://problembar.net/uranus/apps/KolibriKino/get.php?type=Wishlist", "https://problembar.net/uranus/apps/KolibriKino/get.php")]
    [InlineData("problembar.net/uranus", null)]
    [InlineData("ftp://problembar.net/", null)]
    [InlineData("", null)]
    public void The_Uranus_address_is_get_php_in_the_folder(string url, string? expected) =>
        Assert.Equal(expected, new UserSettings { UranusUrl = url }.GetUranusGetUrl());

    [Theory]
    [InlineData("https://example.test/", "secret", true)]
    [InlineData("https://example.test/", " ", false)]
    [InlineData("", "secret", false)]
    [InlineData(null, null, false)]
    public void It_needs_both_the_URL_and_the_token(string? url, string? key, bool configured) =>
        Assert.Equal(configured, Web(url, key).IsConfigured);

    [Fact]
    public async Task Import_fetches_the_newest_ratings_file_with_the_token_and_takes_it_in()
    {
        await _ratings.SaveAsync("tt0082010", 6, null, "An American Werewolf in London");
        _uranus.Body = $$"""
            {"format":"kolibri-kino-ratings","ratings":[
              {"imdbId":"tt0082010","rating":9,"title":"An American Werewolf in London","updated":"{{DateTime.UtcNow.AddMinutes(5):O}}"},
              {"imdbId":"tt0087332","rating":8,"title":"Ghostbusters","updated":"2026-11-01T10:00:00Z"}]}
            """;

        var result = await Web("https://example.test/KolibriKino/", " secret ").ImportAsync();

        Assert.Equal("https://example.test/KolibriKino/get.php?type=Kino%20ratings&format=json", _uranus.Url);
        Assert.Equal("secret", _uranus.Key);
        Assert.Equal((1, 1), (result.Added, result.Updated));
        Assert.Equal(9, (await _repository.GetAsync("tt0082010"))!.Rating);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, "Uranus has no ratings yet")]
    [InlineData(HttpStatusCode.Unauthorized, "didn't accept the token")]
    [InlineData(HttpStatusCode.InternalServerError, "Uranus answered 500")]
    public async Task A_refusal_is_said_in_words(HttpStatusCode status, string expected)
    {
        _uranus.Status = status;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Web("https://example.test/", "secret").ImportAsync());
        Assert.Contains(expected, error.Message);
    }

    [Fact]
    public async Task Without_the_settings_nothing_is_asked()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => Web("https://example.test/", null).ImportAsync());
        Assert.Null(_uranus.Url);
    }

    private sealed class FakeUranus : HttpMessageHandler
    {
        public HttpStatusCode Status = HttpStatusCode.OK;
        public string Body = "{}";
        public string? Url;
        public string? Key;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Url = request.RequestUri!.AbsoluteUri;
            Key = request.Headers.TryGetValues("X-Api-Key", out var values) ? values.Single() : null;
            return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(Body) });
        }
    }
}
