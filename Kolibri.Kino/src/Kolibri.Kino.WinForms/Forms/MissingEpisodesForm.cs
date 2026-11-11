using System.ComponentModel;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Controllers.Wishlist;
using Microsoft.Extensions.Logging;
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
    private readonly WishlistController _wishlist;
    private readonly IServiceProvider _services;
    private IReadOnlyList<MissingEpisodesRow> _rows = [];
    private bool _busy;
    private readonly Button btnAddSelected = new() { Text = "Add selected to wishlist", AutoSize = true, Margin = new Padding(24, 6, 3, 3), Enabled = false };
    private readonly Button btnAddAll = new() { Text = "Add all missing to wishlist", AutoSize = true, Margin = new Padding(3, 6, 3, 3), Enabled = false };

    public MissingEpisodesForm(LocalSeriesController controller, WishlistController wishlist, IServiceProvider services)
    {
        _controller = controller;
        _wishlist = wishlist;
        _services = services;
        InitializeComponent();
        toolbar.Controls.AddRange([btnAddSelected, btnAddAll]);
        dgvSeries.MultiSelect = true;
        btnAddSelected.Click += async (_, _) => await AddToWishlistAsync(SelectedRows());
        btnAddAll.Click += async (_, _) => await AddToWishlistAsync(ShownRows().Where(r => r.Missing.Count > 0).ToList(), ask: true);
        dgvSeries.SelectionChanged += (_, _) => ShowButtons();
        dgvSeries.ContextMenuStrip = NewSeriesMenu();
        dgvSeries.CellMouseDown += (_, e) =>
        {
            // A right-click on a row not selected selects it alone; on a selected one it keeps the selection.
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || dgvSeries.Rows[e.RowIndex].Selected) return;
            dgvSeries.ClearSelection();
            dgvSeries.Rows[e.RowIndex].Selected = true;
        };
        this.Describe(
            (btnRun, "Goes through every series folder and compares the files with the episode lists in the database (Kino's, else SilverScreen's). Only a series with no list is fetched online, once."),
            (chkRefreshOngoing, "Fetches ongoing series' lists again when they are over 3 days old and have no episode still to air, to include episodes aired since. About one TMDb request per series."),
            (chkShowComplete, "Also lists the series where every aired episode is on disk."));
        btnCancel.ToolTipText = "Stops the report; what is done so far is shown.";
        this.Describe(
            (btnAddSelected, "Puts the missing episodes of the selected series on the wishlist (Ctrl- or Shift-click to select more), " +
                             "so File → Prowlarr searches for them. Episodes on it already are left as they are."),
            (btnAddAll, "Puts every missing episode in the list on the wishlist (asks first). Episodes on it already are left as they are."));
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
        ShowButtons();
        foreach (var (name, weight) in new[] { (nameof(ReportRow.Title), 150), (nameof(ReportRow.MissingEpisodes), 300), (nameof(ReportRow.Note), 120) })
            if (dgvSeries.Columns[name] is { } column) column.FillWeight = weight;
        foreach (var name in new[] { nameof(ReportRow.Year), nameof(ReportRow.Aired), nameof(ReportRow.OnDisk), nameof(ReportRow.Missing) })
            if (dgvSeries.Columns[name] is { } column) column.FillWeight = 40;
        if (dgvSeries.Columns[nameof(ReportRow.MissingEpisodes)] is { } list) list.HeaderText = "Missing episodes";
        if (dgvSeries.Columns[nameof(ReportRow.OnDisk)] is { } onDisk) onDisk.HeaderText = "On disk";

        var withMissing = rows.Where(r => r.Missing.Count > 0).ToList();
        var noList = rows.Count(r => r.Source is null);
        var fetched = rows.Count(r => r.FetchedOnline);
        ShowStatus($"{withMissing.Count} of {rows.Count} series miss episodes: {withMissing.Sum(r => r.Missing.Count):N0} in all, "
            + $"{withMissing.Where(r => !r.Ongoing).Sum(r => r.Missing.Count):N0} in ended series (red)."
            + (noList > 0 ? $" {noList} series have no episode list." : "")
            + $" {(fetched == 0 ? "All lists" : $"{rows.Count - fetched} lists")} from the database"
            + (fetched > 0 ? $", {fetched} fetched online." : "."));
    }

    private IEnumerable<MissingEpisodesRow> ShownRows() =>
        Enumerable.Range(0, dgvSeries.Rows.Count).Select(dgvSeries.ItemAt).OfType<ReportRow>().Select(r => r.Source);

    private List<MissingEpisodesRow> SelectedRows() =>
        dgvSeries.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index)
            .Select(r => dgvSeries.ItemAt(r.Index)).OfType<ReportRow>().Select(r => r.Source)
            .Where(r => r.Missing.Count > 0).ToList();

    private void ShowButtons()
    {
        btnAddSelected.Enabled = !_busy && SelectedRows().Count > 0;
        btnAddAll.Enabled = !_busy && _rows.Any(r => r.Missing.Count > 0);
    }

    private ContextMenuStrip NewSeriesMenu()
    {
        var menu = new ContextMenuStrip();
        var add = new ToolStripMenuItem("Add missing episodes to wishlist");
        add.Click += async (_, _) => await AddToWishlistAsync(SelectedRows());
        menu.Items.Add(add);
        menu.Opening += (_, e) =>
        {
            var selected = SelectedRows();
            if (_busy || selected.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            var count = selected.Sum(r => r.Missing.Count);
            add.Text = selected.Count == 1
                ? $"Add the {count} missing episode(s) of {selected[0].Series.Item.Title} to wishlist"
                : $"Add the {count} missing episode(s) of {selected.Count} series to wishlist";
        };
        return menu;
    }

    /// <summary>
    /// Puts the missing episodes of these series on the wishlist (as the series page's Add to wishlist does), so
    /// File → Prowlarr finds them; those on it already are left. Says how many were added.
    /// </summary>
    private async Task AddToWishlistAsync(IReadOnlyList<MissingEpisodesRow> series, bool ask = false)
    {
        if (series.Count == 0) return;
        var total = series.Sum(r => r.Missing.Count);
        if (ask && MessageBox.Show(this, $"Put {total:N0} missing episode(s) of {series.Count} series on the wishlist?", Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int added = 0, failed = 0;
        await RunAsync("Adding to the wishlist…", async ct =>
        {
            foreach (var (row, index) in series.Select((r, i) => (r, i)))
            {
                ShowStatus($"Adding to the wishlist, series {index + 1} of {series.Count}: {row.Series.Item.Title}…");
                try
                {
                    added += await _wishlist.AddEpisodesAsync(row.Series.Item.ImdbId, row.Missing, ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Log.LogWarning(ex, "Adding the missing episodes of {ImdbId} {Title} to the wishlist failed", row.Series.Item.ImdbId, row.Series.Item.Title);
                    failed++;
                }
            }
        });
        ShowStatus($"{added:N0} of {total:N0} missing episode(s) put on the wishlist" +
                   (total - added > 0 ? $"; {total - added:N0} were on it already" : "") +
                   (failed > 0 ? $"; {failed} series failed (see the log)." : ". File → Prowlarr searches for them."));
    }

    /// <summary>Local series, filtered on the series' title, with the series and its episodes shown.</summary>
    private void dgvSeries_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvSeries.ItemAt(e.RowIndex) is not ReportRow { Source.Series.Item: var item }) return;
        this.ShowSingle<LocalSeriesForm>(_services).ShowSeries(item.Title ?? item.ImdbId, item.ImdbId);
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
        _busy = busy;
        btnRun.Enabled = chkRefreshOngoing.Enabled = !busy;
        progress.Visible = btnCancel.Visible = busy;
        ShowButtons();
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
