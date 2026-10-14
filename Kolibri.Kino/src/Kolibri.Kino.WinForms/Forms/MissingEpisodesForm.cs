using System.ComponentModel;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Which aired episodes have no file, for every series with a folder. Ongoing series first, then ended ones, newest
/// first within each; an ended series' missing episodes are in red, since nothing more is coming for it.
/// </summary>
public partial class MissingEpisodesForm : AsyncForm
{
    public override string HelpTopic => "missing-episodes";

    private readonly LocalSeriesController _controller;
    private IReadOnlyList<MissingEpisodesRow> _rows = [];

    public MissingEpisodesForm(LocalSeriesController controller)
    {
        _controller = controller;
        InitializeComponent();
        this.Describe(
            (btnRun, "Goes through every series folder and compares the files with the episode lists. Lists already fetched are used; a series without one is fetched once."),
            (chkRefreshOngoing, "Fetches the episode lists of ongoing series again, to include episodes aired since they were fetched. One TMDb (or OMDb) request per season."),
            (chkShowComplete, "Also lists the series where every aired episode is on disk."));
        btnCancel.ToolTipText = "Stops the report; what is done so far is shown.";
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvSeries.AutoGenerateColumns = true;
    }

    private async void MissingEpisodesForm_Load(object sender, EventArgs e) => await MakeReportAsync();

    private async void btnRun_Click(object sender, EventArgs e) => await MakeReportAsync();

    private void btnCancel_Click(object sender, EventArgs e) => CancelRunning();

    private void chkShowComplete_CheckedChanged(object sender, EventArgs e) => ShowRows(_rows);

    private async Task MakeReportAsync()
    {
        var refresh = chkRefreshOngoing.Checked;
        var onProgress = new Progress<(int Done, int Total, string Title)>(p =>
        {
            progress.Maximum = Math.Max(1, p.Total);
            progress.Value = Math.Min(p.Done, progress.Maximum);
            if (p.Title.Length > 0) ShowStatus($"Series {p.Done + 1} of {p.Total}: {p.Title}");
        });

        await RunAsync("Reading the series…", async ct =>
        {
            _rows = await _controller.GetMissingEpisodesAsync(refresh, onProgress, ct);
            ShowRows(_rows);
        });
    }

    private void ShowRows(IReadOnlyList<MissingEpisodesRow> rows)
    {
        var shown = rows.Where(r => chkShowComplete.Checked || r.Missing.Count > 0).Select(ReportRow.From).ToList();
        bindingSource.SetRows(shown);
        foreach (var (name, weight) in new[] { (nameof(ReportRow.Title), 150), (nameof(ReportRow.MissingEpisodes), 300), (nameof(ReportRow.Note), 120) })
            if (dgvSeries.Columns[name] is { } column) column.FillWeight = weight;
        foreach (var name in new[] { nameof(ReportRow.Year), nameof(ReportRow.Aired), nameof(ReportRow.OnDisk), nameof(ReportRow.Missing) })
            if (dgvSeries.Columns[name] is { } column) column.FillWeight = 40;
        if (dgvSeries.Columns[nameof(ReportRow.MissingEpisodes)] is { } list) list.HeaderText = "Missing episodes";
        if (dgvSeries.Columns[nameof(ReportRow.OnDisk)] is { } onDisk) onDisk.HeaderText = "On disk";

        var withMissing = rows.Where(r => r.Missing.Count > 0).ToList();
        var noList = rows.Count(r => r.Source is null);
        ShowStatus($"{withMissing.Count} of {rows.Count} series miss episodes: {withMissing.Sum(r => r.Missing.Count):N0} in all, "
            + $"{withMissing.Where(r => !r.Ongoing).Sum(r => r.Missing.Count):N0} in ended series (red)."
            + (noList > 0 ? $" {noList} series have no episode list." : ""));
    }

    /// <summary>An ended series' missing episodes in red: they have all aired, and nothing more is coming.</summary>
    private void dgvSeries_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || dgvSeries.ItemAt(e.RowIndex) is not ReportRow { Source: { Ongoing: false, Missing.Count: > 0 } }) return;
        e.CellStyle.ForeColor = Color.Firebrick;
        var column = dgvSeries.Columns[e.ColumnIndex].DataPropertyName;
        if (column is nameof(ReportRow.Missing) or nameof(ReportRow.MissingEpisodes))
            e.CellStyle.Font = new Font(dgvSeries.Font, FontStyle.Bold);
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        btnRun.Enabled = chkRefreshOngoing.Enabled = !busy;
        progress.Visible = btnCancel.Visible = busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    internal sealed record ReportRow(
        string Status, string Year, string Title, int Aired, int OnDisk, int Missing, string MissingEpisodes, string Note,
        [property: Browsable(false)] MissingEpisodesRow Source)
    {
        public static ReportRow From(MissingEpisodesRow r) => new(
            r.Ongoing ? "Ongoing" : "Ended", r.Series.Item.Year ?? "", r.Series.Item.Title ?? r.Series.Item.ImdbId,
            r.Aired, r.OnDisk, r.Missing.Count, LocalSeriesController.Describe(r.Missing),
            r.Note ?? (r.Source is null ? "" : $"List from {r.Source}"), r);
    }
}
