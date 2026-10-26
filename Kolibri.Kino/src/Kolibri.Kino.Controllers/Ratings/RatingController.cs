using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Controllers.Ratings;

/// <param name="Rating">What's stored now; null after clearing.</param>
/// <param name="Message">One sentence for the status line: saved in Kino, and what Plex did.</param>
public sealed record RatingSaveResult(MovieRating? Rating, string Message);

/// <summary>What importing a ratings file did.</summary>
/// <param name="Added">Titles that had no rating here.</param>
/// <param name="Updated">Titles whose rating here was older than the file's.</param>
/// <param name="KeptNewer">Titles whose rating here was as new or newer, so it stayed.</param>
/// <param name="Invalid">Entries without an IMDb id or with a rating outside 1–10.</param>
/// <param name="SentToPlex">Of the added and updated ones, how many Plex took.</param>
/// <param name="PlexNote">Why Plex didn't take them all, if it didn't (the Kino part is done).</param>
public sealed record RatingImportResult(int Added, int Updated, int KeptNewer, int Invalid, int SentToPlex, string? PlexNote);

/// <summary>
/// "My rating": your own rating (half stars, 1–10 as Plex has them) and review of a title. Kept in Kino, and sent to
/// Plex as your private rating there. Exported to a file and imported, to carry ratings from one Kino database to another
/// (the NAS's and the PC's), since they aren't kept in step.
/// </summary>
public sealed partial class RatingController(IRatingRepository ratings, IMediaServerLibrary plex, ILogger<RatingController> logger)
{
    /// <summary>Marks a ratings file, so another JSON file isn't taken for one.</summary>
    public const string FileFormat = "kolibri-kino-ratings";

    public bool PlexConfigured => plex.IsConfigured;

    public Task<MovieRating?> GetAsync(string imdbId, CancellationToken ct = default) => ratings.GetAsync(imdbId, ct);

    public Task<IReadOnlyList<MovieRating>> GetAllAsync(CancellationToken ct = default) => ratings.GetAllAsync(ct);

    /// <summary>
    /// Stores the rating (1–10) and review in Kino, then sets the same rating on Plex when Plex has the title. A Plex
    /// failure doesn't undo the Kino part; the message says what happened.
    /// </summary>
    public async Task<RatingSaveResult> SaveAsync(string imdbId, int rating, string? review, string? title, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        if (rating is < MovieRating.Min or > MovieRating.Max)
            throw new ArgumentOutOfRangeException(nameof(rating), rating, "A rating is from half a star to five stars (1–10).");

        var saved = new MovieRating(imdbId, rating, Clean(review), Clean(title), DateTime.UtcNow);
        await ratings.SaveAsync(saved, ct).ConfigureAwait(false);
        logger.LogInformation("Rated {ImdbId} ({Title}) {Rating}/10", imdbId, saved.Title, rating);

        var kino = $"Saved your rating, {StarsText(saved.Stars)}.";
        return new RatingSaveResult(saved, await SendToPlexAsync(imdbId, rating, ct).ConfigureAwait(false) is { } note ? $"{kino} {note}" : kino);
    }

    /// <summary>Removes your rating and review from Kino, and your rating from Plex.</summary>
    public async Task<RatingSaveResult> ClearAsync(string imdbId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        await ratings.RemoveAsync(imdbId, ct).ConfigureAwait(false);
        logger.LogInformation("Cleared the rating of {ImdbId}", imdbId);
        const string kino = "Your rating is removed.";
        return new RatingSaveResult(null, await SendToPlexAsync(imdbId, null, ct).ConfigureAwait(false) is { } note ? $"{kino} {note}" : kino);
    }

    /// <summary>What Plex did with the rating, as a sentence; null when Plex isn't set up. Never throws for Plex's sake.</summary>
    private async Task<string?> SendToPlexAsync(string imdbId, int? rating, CancellationToken ct)
    {
        if (!plex.IsConfigured) return null;
        try
        {
            if (!await plex.RateAsync(imdbId, rating, ct).ConfigureAwait(false)) return "Plex doesn't have it, so it's in Kino only.";
            logger.LogInformation("Set the Plex rating of {ImdbId} to {Rating}", imdbId, rating?.ToString() ?? "none");
            return rating is null ? "Removed on Plex too." : "Also set on Plex (only you see it).";
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException
                                       or TaskCanceledException && !ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Setting the Plex rating of {ImdbId} failed", imdbId);
            return $"Not set on Plex: {ex.Message}";
        }
    }

    /// <summary>Every rating as a JSON file for <see cref="ImportAsync"/> in another Kino.</summary>
    public async Task<string> ExportAsync(CancellationToken ct = default)
    {
        var all = (await ratings.GetAllAsync(ct).ConfigureAwait(false))
            .OrderBy(r => r.Title ?? r.ImdbId, StringComparer.CurrentCultureIgnoreCase)
            .Select(r => new RatingEntry(r.ImdbId, r.Title, r.Rating, r.Review, r.UpdatedUtc))
            .ToList();
        return JsonSerializer.Serialize(new RatingFile(FileFormat, 1, DateTime.UtcNow, all), Json);
    }

    /// <summary>
    /// Takes the ratings from an exported file: a title gets the file's rating when it has none here or an older one;
    /// a rating here that's as new or newer stays. The added and updated ones are sent to Plex too, when it's set up
    /// (it stops at the first Plex failure; the Kino part is done by then).
    /// </summary>
    public async Task<RatingImportResult> ImportAsync(string json, CancellationToken ct = default)
    {
        RatingFile? file;
        try
        {
            file = JsonSerializer.Deserialize<RatingFile>(json, Json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"This isn't a Kino ratings file: {ex.Message}", ex);
        }
        if (file is null || file.Format != FileFormat)
            throw new InvalidOperationException($"This isn't a Kino ratings file (no \"format\": \"{FileFormat}\").");

        var here = (await ratings.GetAllAsync(ct).ConfigureAwait(false)).ToDictionary(r => r.ImdbId, StringComparer.OrdinalIgnoreCase);
        int added = 0, updated = 0, keptNewer = 0, invalid = 0;
        var changed = new List<MovieRating>();
        foreach (var entry in file.Ratings ?? [])
        {
            ct.ThrowIfCancellationRequested();
            if (entry.ImdbId is not { } id || !ImdbId().IsMatch(id) || entry.Rating is < MovieRating.Min or > MovieRating.Max)
            {
                invalid++;
                continue;
            }
            var incoming = new MovieRating(id, entry.Rating, Clean(entry.Review), Clean(entry.Title), DateTime.SpecifyKind(entry.Updated, DateTimeKind.Utc));
            if (here.TryGetValue(id, out var existing))
            {
                if (existing.UpdatedUtc >= incoming.UpdatedUtc)
                {
                    keptNewer++;
                    continue;
                }
                updated++;
            }
            else added++;
            await ratings.SaveAsync(incoming, ct).ConfigureAwait(false);
            here[id] = incoming;
            changed.Add(incoming);
        }
        logger.LogInformation("Imported ratings: {Added} added, {Updated} updated, {Kept} kept (newer here), {Invalid} invalid",
            added, updated, keptNewer, invalid);

        int sent = 0;
        string? plexNote = null;
        if (plex.IsConfigured)
        {
            foreach (var rating in changed)
            {
                try
                {
                    if (await plex.RateAsync(rating.ImdbId, rating.Rating, ct).ConfigureAwait(false)) sent++;
                }
                catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException
                                               or TaskCanceledException && !ct.IsCancellationRequested)
                {
                    plexNote = $"Plex stopped after {sent}: {ex.Message}";
                    logger.LogWarning(ex, "Sending imported ratings to Plex stopped at {ImdbId}", rating.ImdbId);
                    break;
                }
            }
            if (plexNote is null && sent < changed.Count) plexNote = $"{changed.Count - sent} aren't on Plex.";
        }
        return new RatingImportResult(added, updated, keptNewer, invalid, sent, plexNote);
    }

    /// <summary>"3½ stars", "1 star", "½ star".</summary>
    public static string StarsText(double stars)
    {
        var whole = (int)Math.Floor(stars);
        var text = (whole > 0 ? whole.ToString() : "") + (stars - whole >= 0.5 ? "½" : "");
        return $"{text} {(stars is > 0 and <= 1 ? "star" : "stars")}";
    }

    private static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private sealed record RatingFile(string? Format, int Version, DateTime Exported, List<RatingEntry>? Ratings);

    private sealed record RatingEntry(string? ImdbId, string? Title, int Rating, string? Review, DateTime Updated);

    [GeneratedRegex(@"^tt\d{5,10}$")]
    private static partial Regex ImdbId();
}
