using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class OmdbErrorsTests
{
    [Fact]
    public void A_key_error_with_the_test_key_says_to_get_your_own() =>
        Assert.Contains("Get a free key of your own", OmdbErrors.WithKeyHint("Request limit reached!", usesTestKey: true));

    [Fact]
    public void A_key_error_with_your_own_key_says_to_check_it() =>
        Assert.EndsWith("Check the OMDb key in Settings.", OmdbErrors.WithKeyHint("Invalid API key!", usesTestKey: false));

    [Fact]
    public void Other_errors_are_left_as_they_are() =>
        Assert.Equal("OMDb: Too many results.", OmdbErrors.WithKeyHint("Too many results.", usesTestKey: true));
}
