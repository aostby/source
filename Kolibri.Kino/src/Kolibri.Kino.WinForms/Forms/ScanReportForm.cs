using System.ComponentModel;
using System.Diagnostics;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Scanning;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Result of a folder scan (replaces SilverScreen's CurrentLog text box).
/// </summary>
public partial class ScanReportForm : Form, IWritesToExcel
{
    private const string ChangesAndProblems = "Changes and problems";
    private const string Everything = "Everything";

    private readonly ScanReport _report;
    private readonly Func<string, bool>? _findMovie;
    private readonly HashSet<string> _linkedByHand = new(StringComparer.OrdinalIgnoreCase);
    private List<ReportRow> _rows = [];

    /// <param name="findMovie">Opens the manual lookup for a file; returns true when it was linked.</param>
    public ScanReportForm(ScanReport report, Func<string, bool>? findMovie = null)
    {
        _report = report;
        _findMovie = findMovie;
        InitializeComponent();
        this.Describe(
            (btnOpenFolder, "Shows the file in Explorer; for a duplicate, also the file the movie is already linked to, to compare them."),
            (btnFindMovie, "Finds the movie for the selected file by hand, as Find movie for file in Local movies."));
        btnExcel.ToolTipText = "Saves the rows shown as a file that opens in Excel, named after the Show choice, and opens it.";
        Kolibri.Kino.WinForms.Controls.AppIcon.Apply(this);
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvEntries.AutoGenerateColumns = true;
        btnFindMovie.Visible = findMovie is not null;

        Text = $"Scan report – {report.Folder}";
        lblSummary.Text = Summary(report);
        cboShow.Items.Add(ChangesAndProblems);
        cboShow.Items.Add(Everything);
        foreach (var outcome in Enum.GetValues<ScanOutcome>().Where(o => report.Count(o) > 0))
            cboShow.Items.Add(outcome);
        cboShow.SelectedIndex = 0;
    }

    private static string Summary(ScanReport r)
    {
        var parts = Enum.GetValues<ScanOutcome>()
            .Where(o => r.Count(o) > 0)
            .Select(o => $"{r.Count(o)} {Describe(o)}");
        var summary = $"{r.Entries.Count} file(s): {string.Join(", ", parts)}.";
        if (r.StoppedReason is not null) summary += $"  Stopped: {r.StoppedReason}";
        foreach (var note in r.Notes) summary += $"  {note}.";
        return summary;
    }

    private static string Describe(ScanOutcome o) => o switch
    {
        ScanOutcome.AlreadyLinked => "already linked",
        ScanOutcome.Linked => "linked",
        ScanOutcome.Refreshed => "refreshed",
        ScanOutcome.Duplicate => "duplicate",
        ScanOutcome.OtherPart => "other part",
        ScanOutcome.Skipped => "skipped",
        ScanOutcome.NotFound => "not found",
        _ => "failed",
    };

    private void cboShow_SelectedIndexChanged(object sender, EventArgs e)
    {
        IEnumerable<ScanEntry> entries = cboShow.SelectedItem switch
        {
            ScanOutcome outcome => _report.Entries.Where(x => x.Outcome == outcome),
            Everything => _report.Entries,
            _ => _report.Entries.Where(x => x.Outcome is not (ScanOutcome.AlreadyLinked or ScanOutcome.OtherPart)),
        };

        _rows = entries
            .OrderBy(x => x.Outcome)
            .ThenBy(x => x.FilePath, StringComparer.OrdinalIgnoreCase)
            .Select(ReportRow.From)
            .Select(r => _linkedByHand.Contains(r.File) ? r with { Outcome = "linked by hand" } : r)
            .ToList();
        bindingSource.SetRows(_rows);
        lblStatus.Text = $"{_rows.Count} row(s) shown.";
        btnExcel.Enabled = _rows.Count > 0;
        if (dgvEntries.Columns[nameof(ReportRow.File)] is { } file) file.FillWeight = 250;
        if (dgvEntries.Columns[nameof(ReportRow.Detail)] is { } detail) detail.FillWeight = 200;
    }

    /// <summary>
    /// Shows the file in Explorer; for a duplicate also the file the movie is already linked to, so the two can be compared.
    /// </summary>
    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        if (dgvEntries.CurrentItem() is not ReportRow { Entry: var entry }) return;

        ShowInExplorer(entry.FilePath);
        if (entry.LinkedFile is { } other
            && !string.Equals(Path.GetDirectoryName(other), Path.GetDirectoryName(entry.FilePath), StringComparison.OrdinalIgnoreCase))
            ShowInExplorer(other);
    }

    /// <summary>Opens the folder with the file selected; just the folder if the file is gone.</summary>
    private void ShowInExplorer(string path)
    {
        if (!Explorer.Show(path)) lblSummary.Text = $"Not found: {path}";
    }

    private void dgvEntries_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (btnFindMovie.Visible && btnFindMovie.Enabled) btnFindMovie.PerformClick();
        else btnOpenFolder.PerformClick();
    }

    private void dgvEntries_SelectionChanged(object sender, EventArgs e)
    {
        var row = dgvEntries.CurrentItem() as ReportRow;
        btnFindMovie.Enabled = row is { Entry.Outcome: not ScanOutcome.Skipped } && !_linkedByHand.Contains(row.Entry.FilePath);
        btnOpenFolder.Text = row?.Entry.LinkedFile is null ? "Open folder" : "Open both folders";
    }

    private void btnFindMovie_Click(object sender, EventArgs e)
    {
        if (_findMovie is null || dgvEntries.CurrentItem() is not ReportRow row) return;
        if (_findMovie(row.Entry.FilePath))
        {
            _linkedByHand.Add(row.Entry.FilePath);
            cboShow_SelectedIndexChanged(cboShow, EventArgs.Empty);
        }
    }

    /// <summary>Saves the rows shown as CSV, named after the Show choice ("Changes and problems - Movies - 2026-10-14.csv"), and opens it in Excel.</summary>
    private void btnExcel_Click(object sender, EventArgs e) => WriteToExcel();

    public void WriteToExcel()
    {
        if (_rows.Count == 0) return;
        var csv = new System.Text.StringBuilder();
        csv.AppendLine(Csv.Line("Outcome", "Title", "IMDb id", "Source", "File", "Detail"));
        foreach (var r in _rows)
            csv.AppendLine(Csv.Line(r.Outcome, r.Title, r.ImdbId, r.Source, r.File, r.Detail));

        if (ExcelExport.Save(this, csv.ToString(), cboShow.Text, ExcelExport.FolderName(_report.Folder)) is { } file)
            lblStatus.Text = $"Saved {Path.GetFileName(file)}.";
    }

    internal sealed record ReportRow(
        string Outcome, string Title, string ImdbId, string Source, string File, string Detail,
        [property: Browsable(false)] ScanEntry Entry)
    {
        /// <summary>Title falls back to the file name, so rows without a known movie still say what they are.</summary>
        public static ReportRow From(ScanEntry e) =>
            new(Describe(e.Outcome), string.IsNullOrWhiteSpace(e.Title) ? Path.GetFileNameWithoutExtension(e.FilePath) : e.Title,
                e.ImdbId ?? "", e.Source ?? "", e.FilePath, e.Detail ?? "", e);
    }
}
