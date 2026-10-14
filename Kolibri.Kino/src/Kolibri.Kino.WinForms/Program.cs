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

        // appsettings.json is also built into the .exe, under the file next to it, so the released .exe runs on its own.
        using var defaultsJson = typeof(Program).Assembly.GetManifestResourceStream("Kolibri.Kino.WinForms.appsettings.json")!;
        var defaults = new ConfigurationBuilder().AddJsonStream(defaultsJson).Build();
        builder.Configuration.Sources.Insert(0, new ChainedConfigurationSource { Configuration = defaults });

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
        Application.AddMessageFilter(new Controls.ExcelExport.Shortcut());
        Application.Run(host.Services.GetRequiredService<MainForm>());
    }
}
