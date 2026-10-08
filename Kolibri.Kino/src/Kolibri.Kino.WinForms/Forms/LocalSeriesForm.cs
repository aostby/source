using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;
using OMDbApiNet.Model;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Local series (port of SilverScreen's ShowLocalSeriesForm + DetailsFormSeries): the series in the library, the
/// selected series' details, and one tab per season with its episodes; episodes on disk are marked and playable.
/// </summary>
public partial class LocalSeriesForm : AsyncForm
{
    public override string HelpTopic => "local-series";

    private readonly LocalSeriesController _controller;
    private readonly NewSeriesController _newSeries;
    private readonly IServiceProvider _services;
    private IReadOnlyList<LocalSeries> _all = [];
    private LocalSeries? _current;
    private SeriesDetails? _details;
    private CancellationTokenSource? _posterCts;
    private CancellationTokenSource? _stillCts;

    public LocalSeriesForm(LocalSeriesController controller, NewSeriesController newSeries, IServiceProvider services)
    {
        _controller = controller;
        _newSeries = newSeries;
        _services = services;
        InitializeComponent();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvSeries.AutoGenerateColumns = true;
        episodeDetails.PosterAspect = 16.0 / 9.0;
    }

    private async void LocalSeriesForm_Load(object sender, EventArgs e) =>
        await RunAsync("Loading series…", async ct =>
        {
            _all = await _controller.GetSeriesAsync(ct);
            ApplyFilter();
        });

    /// <summary>
    /// Filters on Enter, not on every letter: each new filter selects the first match, and selecting a series
    /// loads its episodes (online the first time). Escape clears the filter.
    /// </summary>
    private async void txtFilter_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && txtFilter.TextLength > 0) txtFilter.Clear();
        else if (e.KeyCode != Keys.Enter) return;

        e.SuppressKeyPress = true; // no "ding"
        ApplyFilter();
        await ShowSelectedSeriesAsync();
    }

    /// <summary>
    /// Shows whichever series is selected after the list changed. Needed because the grid doesn't report a new
    /// selection when the first row of the new list is at the same position as the old selection
    /// (filter "allo", then "misfits": row 0 both times), which left the previous series on screen.
    /// </summary>
    private async Task ShowSelectedSeriesAsync()
    {
        if (dgvSeries.CurrentItem() is SeriesRow row)
        {
            if (!ReferenceEquals(row.Series, _current)) await ShowSeriesAsync(row.Series, refresh: false);
            return;
        }

        _current = null;
        _details = null;
        seriesDetails.Clear();
        ClearSeasons();
    }

    private void ApplyFilter()
    {
        var text = txtFilter.Text.Trim();
        var shown = _all.Where(s => text.Length == 0 || s.Item.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase)).ToList();
        bindingSource.SetRows(shown.Select(SeriesRow.From));
        if (dgvSeries.Columns[nameof(SeriesRow.Title)] is { } title) title.FillWeight = 300;
        var tooBroad = _all.Count(s => s.FolderTooBroad);
        ShowStatus($"{shown.Count} of {_all.Count} series; {_all.Count(s => !s.FolderExists)} without a folder on disk"
            + (tooBroad > 0 ? $", {tooBroad} linked to a folder that holds other series (Folder: too broad)." : "."));
    }

    private async void dgvSeries_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvSeries.CurrentItem() is SeriesRow row && !ReferenceEquals(row.Series, _current))
            await ShowSeriesAsync(row.Series, refresh: false);
    }

    private void dgvSeries_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is not null && dgvSeries.ItemAt(e.RowIndex) is SeriesRow { Series: var s } && (!s.FolderExists || s.FolderTooBroad))
            e.CellStyle.ForeColor = SystemColors.GrayText;
    }

    /// <summary>Series details at once; seasons and episodes when loaded (cache, else online; see the controller).</summary>
    private async Task ShowSeriesAsync(LocalSeries series, bool refresh)
    {
        _current = series;
        _details = null;
        seriesDetails.ShowItem(series.Item, series.FolderPath, series.FolderExists);
        LoadImage(seriesDetails, ref _posterCts, ct => _controller.GetPosterAsync(series.Item, ct));
        ClearSeasons();

        await RunAsync(refresh ? $"Fetching episodes of {series.Item.Title}…" : $"Loading episodes of {series.Item.Title}…", async ct =>
        {
            var details = await _controller.GetDetailsAsync(series, refresh, ct);
            if (!ReferenceEquals(_current, series)) return;
            _details = details;
            BuildSeasonTabs(details);
            ShowStatus(Describe(series, details));
        });
    }

    private static string Describe(LocalSeries series, SeriesDetails d)
    {
        var episodes = d.Seasons.Sum(s => s.Episodes.Count);
        var onDisk = d.Seasons.Sum(s => s.OnDiskCount);
        var from = d.Source switch
        {
            null => "no episode details",
            "OMDb" when !d.FromCache => "episodes from SilverScreen's OMDb data or OMDb",
            _ when d.FromCache => $"episodes from {d.Source} (cached)",
            _ => $"episodes from {d.Source}",
        };
        var text = $"{series.Item.Title}: {d.Seasons.Count} season(s), {episodes} episode(s), {onDisk} on disk; {from}.";
        if (!series.FolderExists) text += " Folder not found; use Set folder….";
        if (d.UnrecognisedFiles.Count > 0) text += $" {d.UnrecognisedFiles.Count} file(s) without S01E02-style names.";
        if (d.Note is not null) text += $" ({d.Note})";
        return text;
    }

    #region Season tabs

    private void ClearSeasons()
    {
        foreach (TabPage page in tabSeasons.TabPages) page.Dispose();
        tabSeasons.TabPages.Clear();
        episodeDetails.Clear();
    }

    /// <summary>One tab per season ("S01 (8/10)": on disk / episodes), like SilverScreen's DetailsFormSeries.</summary>
    private void BuildSeasonTabs(SeriesDetails details)
    {
        ClearSeasons();
        if (details.Seasons.Count == 0)
        {
            var page = new TabPage("No episodes");
            page.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                Text = details.Note ?? "No episode details are known yet. Use Refresh episodes, or check the TMDb/OMDb keys in Settings.",
            });
            tabSeasons.TabPages.Add(page);
            return;
        }

        foreach (var season in details.Seasons)
        {
            var label = season.SeasonNumber == 0 ? "Specials" : $"S{season.SeasonNumber:00}";
            var page = new TabPage($"{label} ({season.OnDiskCount}/{season.Episodes.Count})") { Tag = season };
            var grid = NewEpisodeGrid();
            page.Controls.Add(grid);
            tabSeasons.TabPages.Add(page);
            grid.DataSource = new SortableBindingList<EpisodeRow>(season.Episodes.Select(EpisodeRow.From));
            if (grid.Columns[nameof(EpisodeRow.Title)] is { } title) title.FillWeight = 300;
        }
        ShowSelectedEpisode();
    }

    private DataGridView NewEpisodeGrid()
    {
        var grid = new DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Dock = DockStyle.Fill,
            MultiSelect = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        };
        grid.SelectionChanged += (_, _) => ShowSelectedEpisode();
        grid.CellFormatting += (_, e) =>
        {
            if (e.CellStyle is not null && grid.ItemAt(e.RowIndex) is EpisodeRow { View.OnDisk: true })
                e.CellStyle.BackColor = Color.Honeydew;
        };
        grid.CellDoubleClick += (_, e) =>
        {
            if (grid.ItemAt(e.RowIndex) is EpisodeRow { View.FilePath: { } path } && File.Exists(path))
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        };
        return grid;
    }

    private void tabSeasons_SelectedIndexChanged(object sender, EventArgs e) => ShowSelectedEpisode();

    private DataGridView? CurrentEpisodeGrid => tabSeasons.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();

    /// <summary>The selected episode in the episode pane, with its screenshot and file.</summary>
    private void ShowSelectedEpisode()
    {
        if (CurrentEpisodeGrid?.CurrentItem() is not EpisodeRow row || _current is not { } series) return;

        var ep = row.View.Episode;
        var rating = ep.Rating?.ToString("0.0", CultureInfo.InvariantCulture);
        var fromImdb = _details?.Source == "OMDb";
        episodeDetails.ShowItem(new Item
        {
            Title = $"S{ep.SeasonNumber:00}E{ep.EpisodeNumber:00} · {ep.Title}",
            Year = ep.AirDate?.ToString("d MMM yyyy", CultureInfo.CurrentCulture),
            Runtime = ep.RuntimeMinutes is { } m ? $"{m} min" : null,
            // ItemDetailsControl labels ImdbRating as "IMDb"; TMDb's own score is shown as such instead.
            ImdbRating = fromImdb ? rating : null,
            Genre = !fromImdb && rating is not null ? $"TMDb {rating}" : null,
            Plot = ep.Plot,
        }, row.View.FilePath, row.View.OnDisk);

        LoadImage(episodeDetails, ref _stillCts, ct => _controller.GetStillAsync(series.Item.ImdbId, ep, ct));
    }

    #endregion

    /// <summary>Loads a picture in the background; a newer request for the same pane cancels an older one.</summary>
    private void LoadImage(ItemDetailsControl target, ref CancellationTokenSource? cts, Func<CancellationToken, Task<byte[]?>> fetch)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        _ = LoadImageAsync(target, fetch, cts.Token);
    }

    private async Task LoadImageAsync(ItemDetailsControl target, Func<CancellationToken, Task<byte[]?>> fetch, CancellationToken ct)
    {
        try
        {
            var bytes = await fetch(ct);
            if (bytes is null || ct.IsCancellationRequested) return;
            using var stream = new MemoryStream(bytes);
            using var image = Image.FromStream(stream);
            target.SetPoster(new Bitmap(image));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ShowStatus($"Picture: {ex.Message}");
        }
    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        if (_current is { } series) await ShowSeriesAsync(series, refresh: true);
    }

    /// <summary>Links the selected series to a folder (SilverScreen's "Finn" / find by id).</summary>
    private async void btnSetFolder_Click(object sender, EventArgs e)
    {
        if (_current is not { } series) return;

        using var dialog = new FolderBrowserDialog
        {
            Description = $"Folder with {series.Item.Title}",
            UseDescriptionForTitle = true,
            InitialDirectory = series.FolderExists ? series.FolderPath! : _controller.SeriesFolder ?? string.Empty,
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        LocalSeries? updated = null;
        await RunAsync("Saving folder…", async ct => updated = await _controller.SetFolderAsync(series, dialog.SelectedPath, ct));
        if (updated is null) return;

        _all = _all.Select(s => ReferenceEquals(s, series) ? updated : s).ToList();
        ApplyFilter();
        SelectSeries(updated);
        await ShowSeriesAsync(updated, refresh: false);
    }

    private void SelectSeries(LocalSeries series)
    {
        foreach (DataGridViewRow row in dgvSeries.Rows)
            if (dgvSeries.ItemAt(row.Index) is SeriesRow { Series: var s } && s.Item.ImdbId == series.Item.ImdbId)
            {
                dgvSeries.CurrentCell = row.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
                return;
            }
    }

    #region Right-click menu

    /// <summary>Right-click selects the row first, so the menu acts on the series under the mouse.</summary>
    private void dgvSeries_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            dgvSeries.CurrentCell = dgvSeries[e.ColumnIndex, e.RowIndex];
    }

    private void seriesMenu_Opening(object sender, CancelEventArgs e)
    {
        menuChangeImdbId.Enabled = _current?.FolderPath is not null;
        menuOpenImdb.Enabled = _current is not null;
        if (_current is null) e.Cancel = true;
    }

    /// <summary>
    /// Re-link the folder to the right IMDb id when the series was matched to the wrong show
    /// (as SilverScreen's right-click menu allowed), e.g. the 2015 vs the 2024 "Dark Matter".
    /// </summary>
    private async void menuChangeImdbId_Click(object sender, EventArgs e)
    {
        if (_current is not { FolderPath: { } folder } series) return;

        var input = Microsoft.VisualBasic.Interaction.InputBox(
            $"The right IMDb id for the series in{Environment.NewLine}{folder}{Environment.NewLine}{Environment.NewLine}" +
            $"Now linked to: {series.Item.Title} ({series.Item.Year}) {series.Item.ImdbId}",
            "Change IMDb id", series.Item.ImdbId).Trim();
        if (input.Length == 0 || string.Equals(input, series.Item.ImdbId, StringComparison.OrdinalIgnoreCase)) return;
        if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^tt\d{7,9}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            MessageBox.Show(this, $"\"{input}\" is not an IMDb id (tt followed by 7–9 digits, e.g. tt19231492).", "Change IMDb id");
            return;
        }

        (Item Item, string? LinkedFolder)? found = null;
        var looked = false;
        await RunAsync($"Looking up {input}…", async ct =>
        {
            found = await _controller.LookUpAsync(input, ct);
            looked = true;
        });
        if (!looked) return; // cancelled or failed; RunAsync already reported it
        if (found is not { } f)
        {
            MessageBox.Show(this, $"Nothing found for {input}.", "Change IMDb id");
            return;
        }

        var (item, linkedFolder) = f;
        if (!string.Equals(item.Type, "series", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(this, $"{item.ImdbId} is \"{item.Title}\" ({item.Year}), a {item.Type ?? "title"}, not a series.", "Change IMDb id");
            return;
        }

        var question = $"Link{Environment.NewLine}{folder}{Environment.NewLine}{Environment.NewLine}" +
                       $"to:  {item.Title} ({item.Year})  {item.ImdbId}{Environment.NewLine}" +
                       $"instead of:  {series.Item.Title} ({series.Item.Year})  {series.Item.ImdbId}?";
        if (linkedFolder is not null && !string.Equals(linkedFolder, folder, StringComparison.OrdinalIgnoreCase))
            question += $"{Environment.NewLine}{Environment.NewLine}{item.Title} is now linked to{Environment.NewLine}{linkedFolder}{Environment.NewLine}That link will be replaced.";
        if (MessageBox.Show(this, question, "Change IMDb id", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        LocalSeries? updated = null;
        await RunAsync("Changing IMDb id…", async ct =>
        {
            updated = await _controller.ChangeImdbIdAsync(series, item, ct);
            _all = await _controller.GetSeriesAsync(ct);
        });
        if (updated is null) return;

        ApplyFilter();
        var shown = _all.FirstOrDefault(s => s.Item.ImdbId == updated.Item.ImdbId) ?? updated;
        SelectSeries(shown);
        await ShowSeriesAsync(shown, refresh: false);
        ShowStatus($"{folder} is now linked to {item.Title} ({item.Year}) {item.ImdbId}. {series.Item.Title} {series.Item.ImdbId} stays in the library without a folder.");
    }

    private void menuOpenImdb_Click(object sender, EventArgs e)
    {
        if (_current is { } series)
            Process.Start(new ProcessStartInfo($"https://www.imdb.com/title/{series.Item.ImdbId}/") { UseShellExecute = true });
    }

    #endregion

    #region Find new series

    /// <summary>
    /// Links the folders in the series folder that aren't linked yet: automatically when the name has an IMDb tag
    /// or a title and year that match clearly, otherwise by asking, one folder at a time. Hold Shift to be asked
    /// about folders answered with "Don't ask again" too.
    /// </summary>
    private async void btnFindNew_Click(object sender, EventArgs e)
    {
        var includeIgnored = (ModifierKeys & Keys.Shift) != 0;
        NewFolderScan? scan = null;
        IReadOnlyList<NewSeriesResult> results = [];
        await RunAsync("Looking for new folders…", async ct =>
        {
            scan = await _newSeries.FindNewFoldersAsync(includeIgnored, ct);
            if (scan.Folders.Count == 0) return;
            var progress = new Progress<string>(s => ShowStatus($"Matching {s}"));
            results = await _newSeries.AddAutomaticallyAsync(scan.Folders, progress, ct);
        });
        if (scan is null || (scan.Folders.Count > 0 && results.Count == 0)) return; // failed or cancelled; already reported
        if (scan.Folders.Count == 0)
        {
            ShowStatus($"No new folders in {scan.Root}." + IgnoredNote(scan));
            return;
        }

        var answers = new Dictionary<NewSeriesResult, (SeriesLookupChoice Choice, Item? Item)>();
        var ask = results.Where(r => r.Status == NewSeriesStatus.NeedsInput).ToList();
        for (var i = 0; i < ask.Count; i++)
        {
            using var dialog = ActivatorUtilities.CreateInstance<SeriesLookupDialog>(
                _services, ask[i].Folder, $"{i + 1} of {ask.Count}", ask[i].Note ?? "");
            dialog.ShowDialog(this);
            answers[ask[i]] = (dialog.Choice, dialog.LinkedItem);
            if (dialog.Choice == SeriesLookupChoice.Stop) break;
        }

        await RunAsync("Loading series…", async ct =>
        {
            _all = await _controller.GetSeriesAsync(ct);
            ApplyFilter();
        });
        await ShowSelectedSeriesAsync();

        var summary = Summarize(scan, results, answers);
        ShowStatus(summary.Status);
        MessageBox.Show(this, summary.Report, "Find new series", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static (string Status, string Report) Summarize(
        NewFolderScan scan, IReadOnlyList<NewSeriesResult> results, Dictionary<NewSeriesResult, (SeriesLookupChoice Choice, Item? Item)> answers)
    {
        static string Series(Item i) => $"{i.Title} ({i.Year}) {i.ImdbId}";
        var auto = results.Where(r => r.Status == NewSeriesStatus.Linked).ToList();
        var elsewhere = results.Where(r => r.Status == NewSeriesStatus.LinkedElsewhere).ToList();
        var byHand = answers.Where(a => a.Value.Choice == SeriesLookupChoice.Linked).ToList();
        var ignored = answers.Count(a => a.Value.Choice == SeriesLookupChoice.Ignore);
        var notDone = results.Count(r => r.Status == NewSeriesStatus.NeedsInput) - byHand.Count - ignored;

        var lines = new List<string> { $"{scan.Folders.Count} folder(s) in {scan.Root} were not linked to a series.", "" };
        void Section(string title, IEnumerable<string> items)
        {
            var list = items.ToList();
            if (list.Count == 0) return;
            lines.Add($"{title}: {list.Count}");
            lines.AddRange(list.Take(15).Select(s => "   " + s));
            if (list.Count > 15) lines.Add($"   … and {list.Count - 15} more");
            lines.Add("");
        }
        Section("Linked automatically", auto.Select(r => $"{r.Folder.Name}  →  {Series(r.Item!)} {r.Note}"));
        Section("Linked by you", byHand.Select(a => $"{a.Key.Folder.Name}  →  {Series(a.Value.Item!)}"));
        Section("Already linked to another folder (not changed; use Change IMDb id or Set folder… if wrong)",
            elsewhere.Select(r => $"{r.Folder.Name}: {Series(r.Item!)} is in {r.OtherFolder}"));
        if (ignored > 0) lines.Add($"Won't be asked about again: {ignored}");
        if (notDone > 0) lines.Add($"Skipped (asked again next time): {notDone}");
        var note = IgnoredNote(scan);
        if (note.Length > 0) lines.Add(note.Trim());

        var status = $"New series: {auto.Count + byHand.Count} linked, {elsewhere.Count} already linked elsewhere, {notDone} skipped." + note;
        return (status, string.Join(Environment.NewLine, lines).TrimEnd());
    }

    private static string IgnoredNote(NewFolderScan scan) =>
        scan.IgnoredCount == 0 ? "" : $" {scan.IgnoredCount} folder(s) you chose not to be asked about again (hold Shift and click Find new series… to include them).";

    #endregion

    private void btnAddToWatchlist_Click(object sender, EventArgs e)
    {
        if (_current is not { } series) return;
        using var dialog = ActivatorUtilities.CreateInstance<AddToWatchlistDialog>(_services, series.Item.ImdbId, series.Item.Title);
        if (dialog.ShowDialog(this) == DialogResult.OK) ShowStatus(dialog.ResultMessage);
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        toolbar.Enabled = !busy;
        progress.Visible = busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _posterCts?.Cancel();
        _posterCts?.Dispose();
        _stillCts?.Cancel();
        _stillCts?.Dispose();
        base.OnFormClosed(e);
    }

    internal sealed record SeriesRow(
        string Title, string Year, int Seasons, string Rating, string Folder,
        [property: Browsable(false)] LocalSeries Series)
    {
        public static SeriesRow From(LocalSeries s) => new(
            s.Item.Title, s.Item.Year ?? "", s.TotalSeasons, s.Item.ImdbRating ?? "",
            s.FolderPath is null ? "none" : !s.FolderExists ? "missing" : s.FolderTooBroad ? "too broad" : "yes", s);
    }

    internal sealed record EpisodeRow(
        int Episode, string Title, string Aired, string Rating, string OnDisk,
        [property: Browsable(false)] EpisodeView View)
    {
        public static EpisodeRow From(EpisodeView v) => new(
            v.Episode.EpisodeNumber,
            v.Episode.Title,
            v.Episode.AirDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
            v.Episode.Rating?.ToString("0.0", CultureInfo.InvariantCulture) ?? "",
            v.OnDisk ? "yes" : "",
            v);
    }
}
