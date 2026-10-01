using Kolibri.Kino.Controllers;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.Tests;

/// <summary>
/// Builds the same registrations as Program.cs, so a service the container can't create fails here, not at startup.
/// </summary>
public sealed class CompositionTests : IDisposable
{
    private readonly TempDatabase _temp = new();

    public void Dispose() => _temp.Dispose();

    [Fact]
    public void Every_registered_service_and_controller_can_be_created()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Kino:LiteDbPath"] = _temp.Path })
            .Build();
        var services = new ServiceCollection()
            .AddLogging()
            .AddKinoData(configuration)
            .AddKinoControllers();

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        foreach (var descriptor in services.Where(d => !d.ServiceType.IsGenericTypeDefinition))
            Assert.NotNull(provider.GetService(descriptor.ServiceType));
    }
}
