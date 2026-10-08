using Kolibri.Kino.Controllers.Logging;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Help → Log: the status messages of Kino's windows and its controllers' messages, newest first,
/// filtered by period, level and text. Errors are red, warnings orange; the selected entry is shown in full below.
/// </summary>
public partial class LogForm : AsyncForm
{
    private static readonly (string Text, int? Days)[] Periods =
        [("Today", 1), ("Last 7 days", 7), ("Last 30 days", 30), ("All", null)];

    private static readonly (string Text, LogLevel Level)[] Levels =
    [
        ("Errors", LogLevel.Error),
        ("Warnings and errors", LogLevel.Warning),
        ("Information and up", LogLevel.Information),
        ("Everything (also Debug)", LogLevel.Trace),
    ];

    private readonly LogController _log;
    private bool _filling = true;

    public LogForm(LogController log)
    {
        _log = log;
        InitializeComponent();
        cboPeriod.Items.AddRange(Periods.Select(p => (object)p.Text).ToArray());
        cboPeriod.SelectedIndex = 0;
        cboLevel.Items.AddRange(Levels.Select(l => (object)l.Text).ToArray());
        cboLevel.SelectedIndex = 2;
        _filling = false;
        lblNote.Text = $"Kept {_log.RetentionDays} days in {_log.LogFile}. Which levels are written is set by " +
                       "Logging:LogLevel:Default in appsettings.json (Information; Debug also logs work in progress).";
    }

    public override string HelpTopic => "log";

    private async void LogForm_Load(object sender, EventArgs e) => await LoadAsync();

    private async void Filter_Changed(object? sender, EventArgs e)
    {
        if (!_filling) await LoadAsync();
    }

    private async void btnRefresh_Click(object sender, EventArgs e) => await LoadAsync();

    private async void txtSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        await LoadAsync();
    }

    private Task LoadAsync() =>
        RunAsync("Reading the log…", async ct =>
        {
            var entries = await _log.GetAsync(Periods[cboPeriod.SelectedIndex].Days, Levels[cboLevel.SelectedIndex].Level, txtSearch.Text, ct);
            grid.DataSource = entries.Select(e => new Row(e)).ToList();
            FormatColumns();
            txtDetails.Clear();
            var errors = entries.Count(e => e.Level >= LogLevel.Error);
            DisplayStatus($"{entries.Count:N0} entr{(entries.Count == 1 ? "y" : "ies")}" +
                          (errors > 0 ? $", {errors:N0} error(s)" : "") +
                          (entries.Count == LogController.MaxEntries ? $" (the newest {LogController.MaxEntries:N0}; narrow the filter for older ones)" : "") + ".");
        });

    private void FormatColumns()
    {
        if (grid.Columns[nameof(Row.Entry)] is { } entry) entry.Visible = false;
        if (grid.Columns[nameof(Row.Time)] is { } time)
        {
            time.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            time.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        if (grid.Columns[nameof(Row.Level)] is { } level) level.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        if (grid.Columns[nameof(Row.Window)] is { } window) window.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        if (grid.Columns[nameof(Row.Message)] is { } message) message.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || grid.Rows[e.RowIndex].DataBoundItem is not Row row) return;
        e.CellStyle.ForeColor = row.Entry.Level switch
        {
            >= LogLevel.Error => Color.Firebrick,
            LogLevel.Warning => Color.DarkOrange,
            <= LogLevel.Debug => SystemColors.GrayText,
            _ => e.CellStyle.ForeColor,
        };
    }

    /// <summary>The whole entry, with the error's details.</summary>
    private void grid_SelectionChanged(object sender, EventArgs e)
    {
        if (grid.CurrentRow?.DataBoundItem is not Row { Entry: var entry }) return;
        txtDetails.Text = $"{entry.Time:yyyy-MM-dd HH:mm:ss}  {entry.Level}  {entry.Source}{Environment.NewLine}{entry.Message}" +
                          (entry.Error is null ? "" : Environment.NewLine + Environment.NewLine + entry.Error);
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        filters.Enabled = !busy;
    }

    /// <summary>The count of entries is only shown, not written to the log itself.</summary>
    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    /// <summary>The grid's columns.</summary>
    public sealed class Row(LogEntry entry)
    {
        public LogEntry Entry { get; } = entry;
        public DateTime Time => Entry.Time;
        public string Level => Entry.Level.ToString();
        public string Window => Entry.Source;
        public string Message => Entry.Error is null ? Entry.Message : Entry.Message + "  (details below)";
    }
}
