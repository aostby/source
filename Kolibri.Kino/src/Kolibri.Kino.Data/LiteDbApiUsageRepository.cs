using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data.Storage;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoApiUsage": one document per day, _id = "yyyy-MM-dd" (local date, so ids sort by date),
/// with { Services: { OMDb: { Calls, Errors }, TMDb: … } }. Adding reads the day and writes it back with the sums.
/// </summary>
public sealed class LiteDbApiUsageRepository(KinoDatabase db) : IApiUsageRepository
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly IDocumentCollection _days = db.Collection("KinoApiUsage");
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task AddAsync(IReadOnlyCollection<ApiUsageEntry> entries, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            foreach (var day in entries.GroupBy(e => e.Date))
            {
                var id = Id(day.Key);
                var doc = await _days.FindByIdAsync(id, ct).ConfigureAwait(false) ?? new BsonDocument { ["_id"] = id };
                var services = doc.TryGetValue("Services", out var s) && s.IsDocument ? s.AsDocument : new BsonDocument();
                foreach (var entry in day)
                {
                    var sum = ToCount(services.TryGetValue(entry.Service, out var stored) ? stored : null) + entry.Count;
                    services[entry.Service] = new BsonDocument { ["Calls"] = sum.Calls, ["Errors"] = sum.Errors };
                }
                doc["Services"] = services;
                await _days.UpsertAsync(doc, ct).ConfigureAwait(false);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<ApiUsageDay?> GetDayAsync(DateOnly date, CancellationToken ct = default) =>
        await _days.FindByIdAsync(Id(date), ct).ConfigureAwait(false) is { } doc ? FromDocument(doc) : null;

    public Task<IReadOnlyList<ApiUsageDay>> GetDaysAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default) =>
        // One small document per day, so a few hundred a year: filtered in memory.
        GetDaysCoreAsync(from, to, ct);

    private async Task<IReadOnlyList<ApiUsageDay>> GetDaysCoreAsync(DateOnly? from, DateOnly? to, CancellationToken ct) =>
        (await _days.FindAllAsync(ct).ConfigureAwait(false))
            .Select(FromDocument).OfType<ApiUsageDay>()
            .Where(d => (from is null || d.Date >= from) && (to is null || d.Date <= to))
            .OrderBy(d => d.Date).ToList();

    private static string Id(DateOnly date) => date.ToString(DateFormat, CultureInfo.InvariantCulture);

    private static ApiUsageDay? FromDocument(BsonDocument doc)
    {
        if (!DateOnly.TryParseExact(doc["_id"].AsString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return null;
        var services = doc.TryGetValue("Services", out var s) && s.IsDocument ? s.AsDocument : new BsonDocument();
        return new ApiUsageDay(date, services.ToDictionary(p => p.Key, p => ToCount(p.Value)));
    }

    private static ApiCallCount ToCount(BsonValue? value) =>
        value is { IsDocument: true } ? new ApiCallCount(Int(value["Calls"]), Int(value["Errors"])) : ApiCallCount.Zero;

    private static int Int(BsonValue value) => value.IsNumber ? value.AsInt32 : 0;
}
