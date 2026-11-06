using System.Reflection;
using Kolibri.Kino.KMDb;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using MySqlConnector;
using Newtonsoft.Json;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Kolibri.Kino.KMDb: OMDb's API on the IMDb data sets in MariaDB. Settings (environment variables in Docker):
//   ConnectionStrings__Imdb (or IMDB_CONNECTION)  the MariaDB database with the IMDb tables (read-only is enough)
//   Kmdb__ApiKey (or KMDB_API_KEY)                 the key clients send (the Uranus key)
//   Kmdb__PlexToken (or KMDB_PLEX_TOKEN)           optional: Plex token, for posters only Plex has
//   OTEL_EXPORTER_OTLP_ENDPOINT                    optional: also send metrics and traces over OTLP (Grafana Alloy, Tempo …)
// Locally: dotnet user-secrets set "ConnectionStrings:Imdb" "…" / "Kmdb:ApiKey" "…" (never in appsettings or git).
//
// Monitoring, all without the key: /ping (running), /health (databases, JSON; 503 when the IMDb database is down),
// /metrics (Prometheus/OpenMetrics for Grafana).

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration["IMDB_CONNECTION"] ?? builder.Configuration.GetConnectionString("Imdb");
if (string.IsNullOrWhiteSpace(connection))
    throw new InvalidOperationException("No database: set ConnectionStrings__Imdb (or IMDB_CONNECTION).");

builder.Services.Configure<KmdbOptions>(builder.Configuration.GetSection("Kmdb"));
builder.Services.PostConfigure<KmdbOptions>(o =>
{
    o.ApiKey ??= builder.Configuration["KMDB_API_KEY"];
    o.PlexToken ??= builder.Configuration["KMDB_PLEX_TOKEN"];
});

var cs = new MySqlConnectionStringBuilder(connection)
{
    // A lookup should answer quickly or not at all; the pool is shared by all requests.
    ConnectionTimeout = 10,
    DefaultCommandTimeout = 30,
};
// Named pools: the metrics label them pool_name="imdb" / "silverscreen" instead of the connection string.
builder.Services.AddMySqlDataSource(cs.ConnectionString, b => b.UseName("imdb"));
builder.Services.AddSingleton<IImdbRepository, MariaDbImdbRepository>();

// The SilverScreen library copied to MariaDB (tools/Kolibri.Kino.SilverScreenCopy): the same server, database
// "silverscreen", unless ConnectionStrings__SilverScreen says otherwise.
var silverScreen = new MySqlConnectionStringBuilder(builder.Configuration.GetConnectionString("SilverScreen") is { Length: > 0 } own ? own : cs.ConnectionString)
{
    ConnectionTimeout = 10,
    DefaultCommandTimeout = 30,
};
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("SilverScreen"))) silverScreen.Database = "silverscreen";
builder.Services.AddSingleton(_ => new SilverScreenRepository(new MySqlDataSourceBuilder(silverScreen.ConnectionString).UseName("silverscreen").Build()));
builder.Services.AddSingleton<ISilverScreenRepository>(sp => sp.GetRequiredService<SilverScreenRepository>());
builder.Services.AddSingleton<ISilverScreenWriter>(sp => sp.GetRequiredService<SilverScreenRepository>());
builder.Services.AddSingleton<SilverScreenEnricher>();
// Posters missing from the image cache are fetched from TMDb's public image server (or another public address).
builder.Services.AddHttpClient(PosterService.HttpClientName, c =>
{
    c.Timeout = TimeSpan.FromSeconds(15);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Kolibri.Kino.KMDb");
});
builder.Services.AddSingleton<PosterService>();
// Plex's current posters by IMDb id (only with Kmdb:PlexToken), loaded in the background and every 6 hours.
builder.Services.AddHttpClient(PlexPosterIndex.HttpClientName, c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddSingleton<PlexPosterIndex>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PlexPosterIndex>());
builder.Services.AddSingleton<OmdbService>();

// Health: each dependency on its own, re-checked every 30 s in the background for the metrics.
builder.Services.AddSingleton<HealthState>();
builder.Services.AddSingleton<IHealthCheckPublisher, HealthStatePublisher>();
builder.Services.Configure<HealthCheckPublisherOptions>(o =>
{
    o.Delay = TimeSpan.FromSeconds(5);
    o.Period = TimeSpan.FromSeconds(30);
});
builder.Services.AddHealthChecks()
    .AddCheck<ImdbHealthCheck>("imdb", HealthStatus.Unhealthy, ["db"], TimeSpan.FromSeconds(15))
    .AddCheck<SilverScreenHealthCheck>("silverscreen", HealthStatus.Degraded, ["db"], TimeSpan.FromSeconds(15))
    .AddCheck<PlexHealthCheck>("plex", HealthStatus.Degraded, ["plex"]);

// Metrics (and traces): OpenTelemetry. /metrics for Prometheus → Grafana; OTLP as well when an endpoint is configured.
builder.Services.AddSingleton<KmdbMetrics>();
var otlp = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
var telemetry = builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("kolibri-kino-kmdb", serviceVersion: typeof(KmdbMetrics).Assembly.GetName().Version?.ToString()))
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation()   // http.server.request.duration per route and status
         .AddRuntimeInstrumentation()      // GC, memory, thread pool
         .AddMeter(KmdbMetrics.MeterName)  // KMDb's own: OMDb lookups, sync, health, library rows
         .AddMeter("MySqlConnector")       // connection pool and command times
         .AddPrometheusExporter();
        if (otlp) m.AddOtlpExporter();
    });
if (otlp)
    telemetry.WithTracing(t => t.AddAspNetCoreInstrumentation(o => o.Filter = c => !ApiKeyMiddleware.IsMonitoring(c.Request.Path))
        .AddSource("MySqlConnector")
        .AddOtlpExporter());

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    // OMDbApiNet's models carry OMDb's names ([JsonProperty("imdbID")]), so Newtonsoft writes OMDb's JSON.
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    // Library documents keep dates as the ISO text GET gives, so a sync compares text with text.
    o.SerializerSettings.DateParseHandling = DateParseHandling.None;
    o.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kolibri.Kino.KMDb",
        Version = "v1",
        Description = "OMDb's API on IMDb's data sets (https://datasets.imdbws.com/). Click Authorize and enter the API key. "
                      + "Data from IMDb is for personal, non-commercial use.",
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = ApiKeyMiddleware.HeaderName,
        Description = "The Uranus API key (kino_config.php). OMDb clients can send it as apikey= instead.",
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("ApiKey", doc)] = [] });
    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) c.IncludeXmlComments(xml);
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (string.IsNullOrWhiteSpace(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<KmdbOptions>>().Value.ApiKey))
    app.Logger.LogError("No API key (Kmdb__ApiKey / KMDB_API_KEY): every request will be refused.");

// A database that is down or too slow gives OMDb's error shape (503), not a stack trace.
app.UseExceptionHandler(error => error.Run(async context =>
{
    var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    app.Logger.LogWarning("Request {Path} failed: {Message}", context.Request.Path, ex?.Message);
    var timeout = ex is MySqlException { ErrorCode: MySqlErrorCode.CommandTimeoutExpired } or MySqlException { ErrorCode: MySqlErrorCode.QueryInterrupted };
    context.Response.StatusCode = ex is MySqlException or TimeoutException ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json; charset=utf-8";
    var message = timeout ? "The database took too long. Try a longer search text."
        : ex is MySqlException ? "The database is not available." : "Something went wrong.";
    await context.Response.WriteAsync(JsonConvert.SerializeObject(OmdbFormat.Error(message)));
}));

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kolibri.Kino.KMDb v1");
    c.DocumentTitle = "Kolibri.Kino.KMDb";
});
app.UseMiddleware<ApiKeyMiddleware>();

// Monitoring, no key needed (ApiKeyMiddleware.IsOpen).
// /ping: the service runs. Never touches the databases, so it answers even when they are down.
app.MapMethods("/ping", ["GET", "HEAD"], (HealthState state) => Results.Ok(new
{
    status = "ok",
    service = "Kolibri.Kino.KMDb",
    version = state.Version,
    startedUtc = state.StartedUtc,
    uptimeSeconds = (long)(DateTime.UtcNow - state.StartedUtc).TotalSeconds,
})).WithTags("Monitoring").WithSummary("The service runs (no key, no database check).");

// /health: both databases checked now. 200 Healthy, 200 Degraded (SilverScreen library down), 503 Unhealthy (IMDb down).
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = HealthResponse.WriteAsync });

// /metrics: Prometheus/OpenMetrics text for Prometheus or Grafana Alloy to scrape.
app.MapPrometheusScrapingEndpoint("/metrics");

app.MapControllers();
app.Run();
