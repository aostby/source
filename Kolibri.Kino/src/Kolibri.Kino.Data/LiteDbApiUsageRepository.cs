using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;

namespace Kolibri.Kino.Data;

/// <summary>
/// "KinoApiUsage": one document per day, _id = "yyyy-MM-dd" (local date, so ids sort by date),
/// with { Services: { OMDb: { Calls, Errors }, TMDb: … } }. Adding reads the day and writes it back with the sums.
/// </summary>
public sealed class LiteDbApiUsageRepository(KinoDatabase db) : IApiUsageRepository
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly ILiteCollection<BsonDocument> _days = db.Database.GetCollection("KinoApiUsage");
    private readonly Lock _lock = new();

    public Task AddAsync(IReadOnlyCollection<ApiUsageEntry> entries, CancellationToken ct = default) =>
        Task.Run(() =>
        {
            lock (_lock)
            {
                foreach (var day in entries.GroupBy(e => e.Date))
                {
                    var id = Id(day.Key);
                    var doc = _days.FindById(id) ?? new BsonDocument { ["_id"] = id };
                    var services = doc.TryGetValue("Services", out var s) && s.IsDocument ? s.AsDocument : new BsonDocument();
                    foreach (var entry in day)
                    {
                        var sum = ToCount(services.TryGetValue(entry.Service, out var stored) ? stored : null) + entry.Count;
                        services[entry.Service] = new BsonDocument { ["Calls"] = sum.Calls, ["Errors"] = sum.Errors };
                    }
                    doc["Services"] = services;
                    _days.Upsert(doc);
                }
            }
        }, ct);

    public Task<ApiUsageDay?> GetDayAsync(DateOnly date, CancellationToken ct = default) =>
        Task.Run(() => _days.FindById(Id(date)) is { } doc ? FromDocument(doc) : null, ct);

    public Task<IReadOnlyList<ApiUsageDay>> GetDaysAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default) =>
        // One small document per day, so a few hundred a year: filtered in memory.
        Task.Run(() => (IReadOnlyList<ApiUsageDay>)_days.FindAll().ToList()
            .Select(FromDocument).OfType<ApiUsageDay>()
            .Where(d => (from is null || d.Date >= from) && (to is null || d.Date <= to))
            .OrderBy(d => d.Date).ToList(), ct);

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
