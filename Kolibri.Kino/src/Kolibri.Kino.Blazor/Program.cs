using Kolibri.Kino.Blazor;
using Kolibri.Kino.Blazor.Components;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Data;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// The same layers as the WinForms app: pages call controllers, never data classes.
// API keys and the Plex token are user settings in the database (Settings page), not configuration.
builder.Services
    .AddKinoData(builder.Configuration)
    .AddKinoControllers()
    .AddScoped<LastSearch>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// In the container the keys that protect form tokens are kept in the data folder, so a restart doesn't break open pages.
if (builder.Configuration["DataProtectionKeysPath"] is { Length: > 0 } keysPath)
    builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(keysPath));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
// No HTTPS redirect: on the NAS the app is plain HTTP on the LAN, and a reverse proxy adds HTTPS if wanted.
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapKinoEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
