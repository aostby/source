using System.Globalization;
using Kolibri.Kino.Controllers.Usage;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Help → API usage report: Kino's calls to OMDb, TMDb, SubDL and Plex per day (last 30 days, a month)
/// or per month (a year, all time).
/// </summary>
public partial class UsageReportForm : AsyncForm
{
    private static readonly string[] Periods = ["Last 30 days", "Month", "Year", "All"];

    private readonly ApiUsageController _usage;
    private bool _filling;

    public UsageReportForm(ApiUsageController usage)
    {
        _usage = usage;
        InitializeComponent();
        _filling = true;
        cboPeriod.Items.AddRange(Periods);
        cboPeriod.SelectedIndex = 0;
        cboMonth.Items.AddRange(CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Where(m => m.Length > 0).ToArray<object>());
        cboMonth.SelectedIndex = DateTime.Today.Month - 1;
        cboYear.Items.Add(DateTime.Today.Year);
        cboYear.SelectedIndex = 0;
        _filling = false;
        UpdateFilters();
        lblNote.Text = "Counts the calls Kino makes (every Kino using this database); SilverScreen's calls aren't counted. " +
                       $"OMDb's free keys allow {ApiUsageController.OmdbFreeDailyLimit:N0} calls a day: days above that are red. " +
                       "TMDb has no daily limit, only a limit per second. Plex is free.";
    }

    public override string HelpTopic => "usage";

    private async void UsageReportForm_Load(object sender, EventArgs e) => await LoadReportAsync();

    private async void Filter_Changed(object? sender, EventArgs e)
    {
        if (_filling) return;
        UpdateFilters();
        await LoadReportAsync();
    }

    private async void btnRefresh_Click(object sender, EventArgs e) => await LoadReportAsync();

    private UsagePeriod SelectedPeriod()
    {
        var year = cboYear.SelectedItem is int y ? y : DateTime.Today.Year;
        return cboPeriod.SelectedIndex switch
        {
            1 => UsagePeriod.ForMonth(year, cboMonth.SelectedIndex + 1),
            2 => UsagePeriod.ForYear(year),
            3 => UsagePeriod.All,
            _ => UsagePeriod.Last30Days,
        };
    }

    /// <summary>Year for Month and Year; month only for Month.</summary>
    private void UpdateFilters()
    {
        lblYear.Enabled = cboYear.Enabled = cboPeriod.SelectedIndex is 1 or 2;
        lblMonth.Enabled = cboMonth.Enabled = cboPeriod.SelectedIndex == 1;
    }

    private Task LoadReportAsync() =>
        RunAsync("Loading usage…", async ct =>
        {
            var report = await _usage.GetReportAsync(SelectedPeriod(), ct);
            ShowYears(report.Years);
            grid.DataSource = report.Rows.ToList();
            FormatColumns(report.Period);

            var t = report.Today;
            lblToday.Text = $"Today ({t.Period}):  OMDb {t.OMDb:N0}  ·  TMDb {t.TMDb:N0}  ·  SubDL {t.SubDL:N0}  ·  Plex {t.Plex:N0}" +
                            (t.Errors > 0 ? $"  ·  {t.Errors:N0} failed" : "");
            var s = report.Total;
            ShowStatus($"Total for {report.Period.Describe()}:  OMDb {s.OMDb:N0}  ·  TMDb {s.TMDb:N0}  ·  SubDL {s.SubDL:N0}  ·  Plex {s.Plex:N0}" +
                       $"  ·  total {s.Total:N0}, {s.Errors:N0} failed");
        });

    /// <summary>The years with data (and this year), keeping the selected one.</summary>
    private void ShowYears(IReadOnlyList<int> years)
    {
        var selected = cboYear.SelectedItem as int?;
        if (cboYear.Items.Cast<int>().SequenceEqual(years)) return;
        _filling = true;
        cboYear.Items.Clear();
        foreach (var year in years) cboYear.Items.Add(year);
        cboYear.SelectedItem = selected is { } y && years.Contains(y) ? y : years[0];
        _filling = false;
    }

    private void FormatColumns(UsagePeriod period)
    {
        foreach (DataGridViewColumn column in grid.Columns)
        {
            if (column.DataPropertyName == nameof(UsageRow.Period))
            {
                column.HeaderText = period.ByMonth ? "Month" : "Day";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                continue;
            }
            column.DefaultCellStyle.Format = "N0";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.Width = 90;
        }
    }

    /// <summary>Days over OMDb's free daily limit in red; zeros grey, so the days with calls stand out.</summary>
    private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || grid.Rows[e.RowIndex].DataBoundItem is not UsageRow row) return;
        var column = grid.Columns[e.ColumnIndex].DataPropertyName;
        if (e.Value is 0) e.CellStyle.ForeColor = SystemColors.GrayText;
        if (column == nameof(UsageRow.OMDb) && !SelectedPeriod().ByMonth && row.OMDb > ApiUsageController.OmdbFreeDailyLimit)
            e.CellStyle.ForeColor = Color.Firebrick;
        if (column == nameof(UsageRow.Errors) && row.Errors > 0)
            e.CellStyle.ForeColor = Color.Firebrick;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        filters.Enabled = !busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;
}
