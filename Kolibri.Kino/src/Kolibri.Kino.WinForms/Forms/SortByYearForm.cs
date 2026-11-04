using System.Diagnostics;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// File → Tools → Sort folder by year (SilverScreen's SortMoviesDesktopForm, "Large movie folder …"): choose a folder
/// with many movies, and Sort moves each movie folder in it to "_byYear\{year}\" by the year in its name. A movie file
/// lying loose in the folder gets a folder of its own named as the file (what SilverScreen's "Single file move" box
/// did, now always). See <see cref="SortByYear"/>.
/// </summary>
public sealed class SortByYearForm : AsyncForm
{
    public override string HelpTopic => "sort-by-year";

    private readonly ISettingsStore _settings;
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 20000 };
    private readonly Button btnChoose = new() { Text = "Choose folder…", Width = 360, Height = 26, Location = new Point(12, 24) };
    private readonly Button btnSort = new() { Text = "Sort", Width = 80, Height = 26, Location = new Point(382, 24), Enabled = false };
    private readonly Label lblLoose = new()
    {
        Text = "Loose movie files get a folder of their own, named as the file.", AutoSize = true, Location = new Point(476, 30),
    };
    private readonly LinkLabel lnkFolder = new() { Text = "Movie folder", AutoSize = true, Location = new Point(14, 58) };
    private readonly TextBox txtLog = new()
    {
        Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false,
        BackColor = SystemColors.Window, Font = new Font(FontFamily.GenericMonospace, 9f),
    };
    private readonly ToolStripStatusLabel lblStatus = new() { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
    private string? _folder;

    public SortByYearForm(ISettingsStore settings)
    {
        _settings = settings;
        Text = "Sort folders by year";
        ClientSize = new Size(900, 480);
        StartPosition = FormStartPosition.CenterParent;

        var group = new GroupBox
        {
            Text = "Large movie folder with many movie files – create year folders and move the movie folders into them (year from the folder name)",
            Dock = DockStyle.Top, Height = 90, Padding = new Padding(8),
        };
        group.Controls.AddRange([btnChoose, btnSort, lblLoose, lnkFolder]);
        var top = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(8, 8, 8, 2) };
        top.Controls.Add(group);
        var log = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 2, 8, 8) };
        log.Controls.Add(txtLog);
        var status = new StatusStrip();
        status.Items.Add(lblStatus);
        Controls.AddRange([log, top, status]);

        btnChoose.Click += async (_, _) => await ChooseAsync();
        btnSort.Click += async (_, _) => await SortAsync();
        lnkFolder.LinkClicked += (_, _) =>
        {
            if (_folder is not null && Directory.Exists(_folder)) Process.Start(new ProcessStartInfo(_folder) { UseShellExecute = true });
        };
        _toolTip.SetToolTip(btnChoose, "Choose the folder with the movies; the list below shows what Sort will do, before anything is moved.");
        _toolTip.SetToolTip(btnSort, $"Moves each movie folder to {SortByYear.TargetName}\\<year>\\ in the chosen folder, by the year in its " +
                                     "name (else in its biggest movie file's name). Asks first.");
        _toolTip.SetToolTip(lblLoose, "A movie file lying in the chosen folder itself (Heat.1995.720p.mkv) is moved to " +
                                      $"{SortByYear.TargetName}\\1995\\Heat.1995.720p\\, with its subtitles and other files of the same name.");
        _toolTip.SetToolTip(lnkFolder, "Opens the folder in Explorer.");
        ShowStatus("Choose the folder to sort.");
    }

    private async Task ChooseAsync()
    {
        var start = _folder ?? _settings.Current.UserFilePaths.MoviesSourcePath;
        using var dialog = new FolderBrowserDialog
        {
            Description = "The folder with the movies to sort by year",
            UseDescriptionForTitle = true,
            InitialDirectory = start is not null && Directory.Exists(start) ? start : "",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _folder = dialog.SelectedPath;
        lnkFolder.Text = _folder;
        await PreviewAsync();
    }

    /// <summary>What Sort will do, in the list; nothing is moved.</summary>
    private async Task<YearPlan?> PreviewAsync()
    {
        if (_folder is not { } folder) return null;
        YearPlan? plan = null;
        await RunAsync($"Looking through {folder}…", async ct => plan = await Task.Run(() => SortByYear.Plan(folder), ct));
        btnSort.Enabled = plan is { Moves.Count: > 0 };
        if (plan is null) return null;

        var lines = new List<string>();
        var folders = plan.Moves.Count(m => !m.IsFile);
        var files = plan.Moves.Count - folders;
        lines.Add($"{folders} folder(s) and {files} loose file(s) to move to {plan.Target}:");
        lines.AddRange(plan.Moves.OrderBy(m => m.Year).Select(m =>
            $"  {m.Year}  {Path.GetFileName(m.Source)}" + (m.IsFile ? $"  → folder {Path.GetFileName(m.Destination)}" : "") +
            (m.Companions.Count > 0 ? $" (+ {string.Join(", ", m.Companions.Select(Path.GetFileName))})" : "")));
        if (plan.NoYear.Count > 0)
        {
            lines.Add("");
            lines.Add($"No year in the name, left where they are ({plan.NoYear.Count}):");
            lines.AddRange(plan.NoYear.Select(n => "  " + n));
        }
        if (plan.Taken.Count > 0)
        {
            lines.Add("");
            lines.Add($"In the year folder already, left where they are ({plan.Taken.Count}):");
            lines.AddRange(plan.Taken.Select(n => "  " + n));
        }
        txtLog.Lines = lines.ToArray();
        ShowStatus(plan.Moves.Count == 0
            ? "Nothing to move."
            : $"{folders} folder(s) and {files} file(s) to move; {plan.NoYear.Count} without a year. Click Sort to move them.");
        return plan;
    }

    private async Task SortAsync()
    {
        // Looked through again, in case the folder changed since.
        if (await PreviewAsync() is not { Moves.Count: > 0 } plan) return;
        if (MessageBox.Show(this,
                $"Move {plan.Moves.Count} folder(s) and file(s) in{Environment.NewLine}{plan.Root}{Environment.NewLine}" +
                $"to year folders in {SortByYear.TargetName}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

        YearResult? result = null;
        var progress = new Progress<string>(s => ShowStatus($"Moving {s}"));
        await RunAsync("Moving…", async ct => result = await Task.Run(() => SortByYear.Execute(plan, progress, ct), ct));
        btnSort.Enabled = false;
        if (result is null) return;

        var lines = new List<string> { $"Moved {result.Moved} of {plan.Moves.Count} to {plan.Target}." };
        if (result.Errors.Count > 0)
        {
            lines.Add("");
            lines.Add($"Not moved ({result.Errors.Count}):");
            lines.AddRange(result.Errors.Select(e => "  " + e));
        }
        txtLog.Lines = lines.ToArray();
        ShowStatus($"Moved {result.Moved} of {plan.Moves.Count}" + (result.Errors.Count > 0 ? $"; {result.Errors.Count} failed (see the list)." : "."));
        _folder = plan.Target;
        lnkFolder.Text = plan.Target;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        btnChoose.Enabled = !busy;
        if (busy) btnSort.Enabled = false;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }
}
