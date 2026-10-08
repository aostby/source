using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Controllers.Usage;

public enum UsagePeriodKind
{
    Last30Days,
    Month,
    Year,
    All,
}

/// <summary>What the usage report covers: the last 30 days (default), a month, a year, or everything.</summary>
public sealed record UsagePeriod(UsagePeriodKind Kind, int Year = 0, int Month = 0)
{
    public static UsagePeriod Last30Days { get; } = new(UsagePeriodKind.Last30Days);
    public static UsagePeriod All { get; } = new(UsagePeriodKind.All);
    public static UsagePeriod ForMonth(int year, int month) => new(UsagePeriodKind.Month, year, month);
    public static UsagePeriod ForYear(int year) => new(UsagePeriodKind.Year, year);

    /// <summary>Per day for 30 days and a month; per month for a year and everything.</summary>
    public bool ByMonth => Kind is UsagePeriodKind.Year or UsagePeriodKind.All;

    public string Describe() => Kind switch
    {
        UsagePeriodKind.Last30Days => "the last 30 days",
        UsagePeriodKind.Month => new DateTime(Year, Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture),
        UsagePeriodKind.Year => Year.ToString(CultureInfo.InvariantCulture),
        _ => "all time",
    };
}

/// <summary>One line of the report: a day ("2026-10-09") or a month ("2026-10"). Properties are the grid's columns.</summary>
public sealed class UsageRow
{
    public string Period { get; init; } = "";
    public int OMDb { get; init; }
    public int TMDb { get; init; }
    public int SubDL { get; init; }
    public int Plex { get; init; }
    public int Total => OMDb + TMDb + SubDL + Plex;
    public int Errors { get; init; }

    internal static UsageRow From(string period, IEnumerable<ApiUsageDay> days)
    {
        var list = days.ToList();
        int Calls(string service) => list.Sum(d => d.For(service).Calls);
        return new UsageRow
        {
            Period = period,
            OMDb = Calls(ApiServices.Omdb),
            TMDb = Calls(ApiServices.Tmdb),
            SubDL = Calls(ApiServices.SubDl),
            Plex = Calls(ApiServices.Plex),
            Errors = list.Sum(d => ApiServices.All.Sum(s => d.For(s).Errors)),
        };
    }
}

/// <summary>The rows (newest first), their sum, today's calls, and the years there is data for (for the filter).</summary>
public sealed record UsageReport(UsagePeriod Period, IReadOnlyList<UsageRow> Rows, UsageRow Total, UsageRow Today, IReadOnlyList<int> Years);

/// <summary>
/// The API usage report: Kino's calls to OMDb, TMDb, SubDL and Plex per day or month. Only calls Kino makes are
/// counted (by every Kino using this database); SilverScreen's are not. OMDb's free keys allow 1,000 calls a day.
/// </summary>
public sealed class ApiUsageController(IApiUsageRepository repository, IApiUsageTracker tracker)
{
    public const int OmdbFreeDailyLimit = 1000;

    public Task<UsageReport> GetReportAsync(UsagePeriod period, CancellationToken ct = default) =>
        GetReportAsync(period, DateOnly.FromDateTime(DateTime.Now), ct);

    internal async Task<UsageReport> GetReportAsync(UsagePeriod period, DateOnly today, CancellationToken ct)
    {
        await tracker.FlushAsync(ct).ConfigureAwait(false);

        var (from, to) = Range(period, today);
        var days = await repository.GetDaysAsync(from, to, ct).ConfigureAwait(false);
        var all = period.Kind == UsagePeriodKind.All ? days : await repository.GetDaysAsync(null, null, ct).ConfigureAwait(false);

        // Every day (or month) of the period up to today, also those without calls, so gaps show.
        var first = from ?? (days.Count > 0 ? days[0].Date : today);
        var last = to is { } end && end < today ? end : today;
        var rows = period.ByMonth ? MonthRows(days, first, last) : DayRows(days, first, last);

        var years = all.Select(d => d.Date.Year).Append(today.Year).Distinct().OrderDescending().ToList();
        var todayCalls = all.Where(d => d.Date == today);
        return new UsageReport(period, rows, UsageRow.From("Total", days), UsageRow.From(Day(today), todayCalls), years);
    }

    private static (DateOnly? From, DateOnly? To) Range(UsagePeriod period, DateOnly today) => period.Kind switch
    {
        UsagePeriodKind.Last30Days => (today.AddDays(-29), today),
        UsagePeriodKind.Month => (new DateOnly(period.Year, period.Month, 1), new DateOnly(period.Year, period.Month, 1).AddMonths(1).AddDays(-1)),
        UsagePeriodKind.Year => (new DateOnly(period.Year, 1, 1), new DateOnly(period.Year, 12, 31)),
        _ => (null, null),
    };

    private static List<UsageRow> DayRows(IReadOnlyList<ApiUsageDay> days, DateOnly first, DateOnly last)
    {
        var byDate = days.ToDictionary(d => d.Date);
        var rows = new List<UsageRow>();
        for (var date = last; date >= first; date = date.AddDays(-1))
            rows.Add(UsageRow.From(Day(date), byDate.TryGetValue(date, out var day) ? [day] : []));
        return rows;
    }

    private static List<UsageRow> MonthRows(IReadOnlyList<ApiUsageDay> days, DateOnly first, DateOnly last)
    {
        var rows = new List<UsageRow>();
        for (var month = new DateOnly(last.Year, last.Month, 1); month >= new DateOnly(first.Year, first.Month, 1); month = month.AddMonths(-1))
            rows.Add(UsageRow.From(month.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                days.Where(d => d.Date.Year == month.Year && d.Date.Month == month.Month)));
        return rows;
    }

    private static string Day(DateOnly date) => date.ToString("yyyy-MM-dd ddd", CultureInfo.InvariantCulture);
}
