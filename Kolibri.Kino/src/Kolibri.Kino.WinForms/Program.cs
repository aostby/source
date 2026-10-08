using Kolibri.Kino.Controllers;
using Kolibri.Kino.Data;
using Kolibri.Kino.WinForms.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kolibri.Kino.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        // API keys and the Plex token are user settings in the database (Settings window), not configuration.
        // The database itself: the file chosen in Settings, else appsettings.json (see DatabaseLocation).
        builder.Configuration[DatabaseLocation.ConfigKey] = DatabaseLocation.Resolve(builder.Configuration);

        builder.Services
            .AddKinoData(builder.Configuration)
            .AddKinoControllers()
            .AddTransient<MainForm>();

        using var host = builder.Build();
        host.Services.StartApiUsageCounting();
        KinoLog.Factory = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
        Application.Run(host.Services.GetRequiredService<MainForm>());
    }
}
