using System.Text;
using System.Threading.Channels;
using Kolibri.Kino.Controllers.Ratings;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Blazor;

/// <summary>The lists <see cref="WebSync"/> sends.</summary>
public enum WebSyncFile
{
    Wishlist,
    Ratings,
}

/// <summary>
/// Sends the wishlist and your ratings to the web service (<see cref="KinoOptions.WebSync"/>) after they change, as the
/// same files the Download buttons give. Pages only say what changed; the file is made and posted here, in the
/// background, a few seconds later (so a run of changes goes once). A failure is logged and the next change tries
/// again; the page never waits for it, and what's saved in Kino is never touched.
/// </summary>
public sealed class WebSync(IServiceProvider services, IHttpClientFactory http, IOptions<KinoOptions> options, ILogger<WebSync> logger)
    : BackgroundService
{
    private readonly Channel<WebSyncFile> _changes = Channel.CreateUnbounded<WebSyncFile>(new UnboundedChannelOptions { SingleReader = true });

    private WebSyncOptions Settings => options.Value.WebSync;

    /// <summary>The list has changed: send it soon. Does nothing when the web service isn't set up.</summary>
    public void Changed(WebSyncFile file)
    {
        if (Settings.IsConfigured) _changes.Writer.TryWrite(file);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Settings.IsConfigured)
        {
            logger.LogInformation("Web sync is off (Kino:WebSync:Url or ApiKey not set)");
            return;
        }
        try
        {
            while (await _changes.Reader.WaitToReadAsync(stoppingToken))
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(0, Settings.DelaySeconds)), stoppingToken);
                var pending = new HashSet<WebSyncFile>();
                while (_changes.Reader.TryRead(out var file)) pending.Add(file);
                foreach (var file in pending) await SendAsync(file, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task SendAsync(WebSyncFile file, CancellationToken ct)
    {
        try
        {
            using var scope = services.CreateScope();
            var (content, contentType, name) = file switch
            {
                WebSyncFile.Wishlist => (
                    KinoEndpoints.CsvBytes(WishlistController.ToCsv(await scope.ServiceProvider.GetRequiredService<WishlistController>().GetAsync(ct))),
                    "text/csv; charset=utf-8",
                    $"Wishlist - {DateTime.Now:yyyy-MM-dd}.csv"),
                _ => (
                    Encoding.UTF8.GetBytes(await scope.ServiceProvider.GetRequiredService<RatingController>().ExportAsync(ct)),
                    "application/json; charset=utf-8",
                    $"Kino ratings - {DateTime.Now:yyyy-MM-dd}.json"),
            };

            var url = Settings.Url.Trim();
            using var request = new HttpRequestMessage(HttpMethod.Post,
                url + (url.Contains('?') ? '&' : '?') + "name=" + Uri.EscapeDataString(name));
            request.Headers.Add("X-Api-Key", Settings.ApiKey.Trim());
            request.Content = new ByteArrayContent(content);
            request.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);

            using var client = http.CreateClient(nameof(WebSync));
            client.Timeout = TimeSpan.FromSeconds(30);
            using var response = await client.SendAsync(request, ct);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Sent {File} ({Bytes} bytes) to the web service", name, content.Length);
                return;
            }
            var answer = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning("The web service didn't take {File}: {Status} {Answer}", name, (int)response.StatusCode,
                answer.Length > 300 ? answer[..300] : answer);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Sending the {List} to the web service failed", file);
        }
    }
}
