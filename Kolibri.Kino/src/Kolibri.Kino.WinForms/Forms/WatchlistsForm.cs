using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Watchlists (port of SilverScreen's WatchlistForm): one list at a time, filter, mark watched, remove,
/// details with poster, copy/export, and copy to a Plex playlist of the same name.
/// </summary>
public partial class WatchlistsForm : AsyncForm
{
    public override string HelpTopic => "watchlists";

    private readonly WatchlistController _watchlists;
    private IReadOnlyList<WatchListItem> _items = [];
    private CancellationTokenSource? _posterCts;

    /// <summary>In the details panel, after Open folder and Play: the same as the toolbar's Mark watched.</summary>
    private readonly Button btnWatchedHere = new() { AutoSize = true, Text = "Mark watched", Enabled = false, UseVisualStyleBackColor = true };
    private bool _loadingNames;

    public WatchlistsForm(WatchlistController watchlists)
    {
        _watchlists = watchlists;
        InitializeComponent();
        SiteIcons.Imdb(btnImdb);
        this.Describe(
            (btnNewList, "Makes a new, empty Kino watchlist."),
            (btnWatched, WatchedTip),
            (btnRemove, "Removes the selected movie from this watchlist, and if you want from the Plex playlist of the same name. Asks first."),
            (btnImdb, "Opens the selected movie's page on IMDb."),
            (btnCopy, "Copies the movies shown (title, year, IMDb rating, genre, watched) to the clipboard as a table, ready to paste into Excel, a document or an e-mail."),
            (btnExport, "Saves the movies shown as a CSV file, which opens in Excel."),
            (btnPlex, "Adds every movie on this watchlist to the Plex playlist of the same name (made if missing). Movies already there, or not on Plex, are skipped."),
            (btnSyncFromPlex, "The other way: adds the titles in the Plex playlist of the same name that aren't on this watchlist, " +
                              "and marks titles watched on Plex as watched here. Titles only on this watchlist are kept. " +
                              "If Plex has no such playlist, it's made from this watchlist."),
            (btnOpenOnPlex, "Opens the Plex playlist of this name in Plex's own web app, in a window in Kino " +
                            "(Plex may ask you to sign in the first time)."));
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvItems.AutoGenerateColumns = true;
        btnPlex.Enabled = watchlists.PlexConfigured;
        // Needs the Plex server and token from Settings.
        btnSyncFromPlex.Visible = btnOpenOnPlex.Visible = watchlists.PlexConfigured;
        btnWatchedHere.Click += btnWatchedHere_Click;
        details.AddButton(btnWatchedHere);
        this.Describe((btnWatchedHere, WatchedTip));
    }

    private const string WatchedTip =
        "Mark watched: marks the selected movie watched and moves it to the Watched list; with Plex set up, you're asked whether to " +
        "mark it watched on Plex too. Mark not watched: only takes the mark away (in Kino).";

    private string? CurrentList => cboLists.SelectedItem as string;

    private WatchRow? Selected => dgvItems.CurrentItem() as WatchRow;

    private async void WatchlistsForm_Load(object sender, EventArgs e) => await LoadListNamesAsync(_watchlists.FavoriteList);

    private async Task LoadListNamesAsync(string select)
    {
        await RunAsync("Loading watchlists…", async ct =>
        {
            var names = await _watchlists.GetListNamesAsync(ct);
            _loadingNames = true;
            cboLists.Items.Clear();
            cboLists.Items.AddRange([.. names]);
            _loadingNames = false;
            cboLists.SelectedItem = names.FirstOrDefault(n => string.Equals(n, select, StringComparison.OrdinalIgnoreCase)) ?? names.FirstOrDefault();
        });
    }

    private async void cboLists_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loadingNames || CurrentList is not { } list) return;
        await RunAsync($"Loading {list}…", async ct =>
        {
            // As SilverScreen: the list you pick becomes the favorite (the default next time).
            await _watchlists.SetFavoriteAsync(list, ct);
            _items = await _watchlists.GetItemsAsync(list, ct);
            ApplyFilter();
        });
    }

    private async void btnNewList_Click(object sender, EventArgs e)
    {
        var name = Microsoft.VisualBasic.Interaction.InputBox("Name of the new watchlist:", "New watchlist", $"{DateTime.Now:yyyy-MM-dd} list").Trim();
        if (name.Length == 0) return;
        await RunAsync("Creating list…", ct => _watchlists.CreateListAsync(name, ct));
        await LoadListNamesAsync(name);
    }

    private void Filter_Changed(object sender, EventArgs e)
    {
        if (sender is RadioButton { Checked: false }) return;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var text = txtFilter.Text.Trim();
        var shown = _items
            .Where(i => text.Length == 0 || i.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase))
            .Where(i => rbAll.Checked || (rbWatched.Checked ? i.Watched == "Y" : i.Watched != "Y"))
            .Select(WatchRow.From)
            .ToList();

        bindingSource.SetRows(shown);
        if (dgvItems.Columns[nameof(WatchRow.Title)] is { } title) title.FillWeight = 250;
        details.Clear();
        btnWatchedHere.Enabled = false;
        ShowStatus($"{shown.Count} of {_items.Count} movie(s) in {CurrentList}; {_items.Count(i => i.Watched != "Y")} not watched.");
    }

    private async void dgvItems_SelectionChanged(object sender, EventArgs e)
    {
        if (Selected is not { } row) return;
        details.ShowItem(row.Item);
        btnWatched.Text = btnWatchedHere.Text = row.Item.Watched == "Y" ? "Mark not watched" : "Mark watched";
        btnWatchedHere.Enabled = true;
        LoadPoster(row.Item);

        // The linked file, so Open folder and Play work.
        try
        {
            if (await _watchlists.GetFilePathAsync(row.Item.ImdbId) is { } path && ReferenceEquals(Selected, row) && !IsGone)
                details.ShowItem(row.Item, path, File.Exists(path));
        }
        catch (Exception ex) // only for the buttons; the details are shown already
        {
            ShowStatus($"File of {row.Title}: {ex.Message}");
        }
    }

    private async void btnWatchedHere_Click(object? sender, EventArgs e) => await ToggleWatchedAsync();

    private async void LoadPoster(WatchListItem item)
    {
        _posterCts?.Cancel();
        _posterCts?.Dispose();
        _posterCts = new CancellationTokenSource();
        var ct = _posterCts.Token;
        try
        {
            var bytes = await _watchlists.GetPosterAsync(item, ct);
            if (bytes is null || ct.IsCancellationRequested) return;
            using var stream = new MemoryStream(bytes);
            using var image = Image.FromStream(stream);
            details.SetPoster(new Bitmap(image));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ShowStatus($"Poster for {item.Title}: {ex.Message}");
        }
    }

    private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        // SilverScreen's colors: green for watched, pink for not watched.
        if (e.CellStyle is not null && dgvItems.ItemAt(e.RowIndex) is WatchRow row)
            e.CellStyle.BackColor = row.Item.Watched == "Y" ? Color.Honeydew : Color.MistyRose;
    }

    private async void btnWatched_Click(object sender, EventArgs e) => await ToggleWatchedAsync();

    /// <summary>
    /// Both Mark watched buttons. Watched: marked, moved to the Watched list (where watched movies are kept), and, if you
    /// say yes, marked watched on Plex. Not watched: only the mark goes, in Kino.
    /// </summary>
    private async Task ToggleWatchedAsync()
    {
        if (Selected is not { } row || CurrentList is not { } list) return;
        var title = row.Title;
        var imdbId = row.Item.ImdbId;

        if (row.Item.Watched == "Y")
        {
            await RunAsync("Saving…", async ct =>
            {
                if (!await _watchlists.SetWatchedAsync(imdbId, false, ct)) return;
                row.Item.Watched = "N";
                ApplyFilter();
                ShowStatus($"{title} marked not watched.");
            });
            return;
        }

        string? movedFrom = null;
        var saved = false;
        await RunAsync("Saving…", async ct =>
        {
            movedFrom = await _watchlists.MarkWatchedAndMoveAsync(imdbId, ct);
            _items = await _watchlists.GetItemsAsync(list, ct);
            ApplyFilter();
            saved = true;
        });
        if (!saved || IsGone) return;
        var status = movedFrom is null ? $"{title} marked watched." : $"{title} marked watched and moved from {movedFrom} to {WatchlistController.WatchedList}.";
        ShowStatus(status);

        if (!_watchlists.PlexConfigured
            || MessageBox.Show(this, $"{title} is marked watched in Kino.{Environment.NewLine}{Environment.NewLine}Mark it watched on Plex too?",
                   "Mark watched", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        await RunAsync("Telling Plex…", async ct =>
        {
            if (await _watchlists.MarkWatchedOnPlexAsync(imdbId, title, ct) is { } plex) ShowStatus($"{status} {plex}");
        });
    }

    private async void btnRemove_Click(object sender, EventArgs e)
    {
        if (Selected is not { } row || CurrentList is not { } list) return;

        if (MessageBox.Show(this, $"Remove {row.Title} from {list}?", "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        var alsoPlex = _watchlists.PlexConfigured
            && MessageBox.Show(this, $"Also remove it from the Plex playlist \"{list}\"?", "Remove", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;

        await RunAsync("Removing…", async ct =>
        {
            var removedFromPlex = await _watchlists.RemoveAsync(row.Item.ImdbId, list, alsoPlex, ct);
            _items = _items.Where(i => i.ImdbId != row.Item.ImdbId).ToList();
            ApplyFilter();
            if (alsoPlex) ShowStatus(removedFromPlex ? $"Removed {row.Title} here and in Plex." : $"Removed {row.Title}; it wasn't in the Plex playlist.");
        });
    }

    private async void btnPlex_Click(object sender, EventArgs e)
    {
        if (CurrentList is not { } list) return;
        await RunAsync($"Copying {list} to Plex…", async ct =>
        {
            var result = await _watchlists.CopyToPlexAsync(list, ct);
            var text = $"Plex playlist \"{list}\"{(result.Created ? " created" : "")}: {result.Added} added, {result.AlreadyThere} already there.";
            if (result.NotOnServer.Count > 0) text += $" {result.NotOnServer.Count} not on the Plex server.";
            ShowStatus(text);
        });
    }

    /// <summary>Plex playlist → this watchlist: missing titles added, titles watched on Plex marked watched. Shows what changed.</summary>
    /// <summary>The Plex playlist of this name in Plex Web, in a web window (as SilverScreen's browser windows).</summary>
    private async void btnOpenOnPlex_Click(object sender, EventArgs e)
    {
        if (CurrentList is not { } list) return;
        string? page = null;
        await RunAsync($"Finding {list} on Plex…", async ct => page = await _watchlists.GetPlexPlaylistPageAsync(list, ct));
        if (IsGone) return;
        if (page is null)
        {
            ShowStatus($"Plex has no playlist named \"{list}\". Copy to Plex playlist makes it.");
            return;
        }
        WebPageForm.Show(this, page);
        ShowStatus($"Opened the Plex playlist \"{list}\".");
    }

    private async void btnSyncFromPlex_Click(object sender, EventArgs e)
    {
        if (CurrentList is not { } list) return;
        SyncFromPlexResult? result = null;
        await RunAsync($"Syncing {list} from Plex…", async ct =>
        {
            result = await _watchlists.SyncFromPlexAsync(list, ct);
            _items = await _watchlists.GetItemsAsync(list, ct);
            ApplyFilter();
        });
        if (IsGone || result is not { } r) return;

        if (!r.PlaylistFound)
        {
            var made = r.CreatedOnPlex;
            ShowStatus(made is null or { Added: 0 }
                ? $"Plex had no playlist named \"{list}\", and none of its movies are on Plex, so none was made."
                : $"Plex had no playlist named \"{list}\"; made it with {made.Added} movie(s)" +
                  (made.NotOnServer.Count > 0 ? $" ({made.NotOnServer.Count} aren't on Plex)." : "."));
            return;
        }

        var summary = $"Synced \"{list}\" from Plex: {r.Added.Count} added, {r.MarkedWatched} marked watched" +
                      (r.Skipped.Count > 0 ? $", {r.Skipped.Count} skipped" : "") + (r.Failed.Count > 0 ? $", {r.Failed.Count} failed" : "") + ".";
        ShowStatus(summary);

        var details = new List<string>();
        if (r.Moved.Count > 0) details.Add("Moved from another watchlist (a title is on one list at a time):" + Lines(r.Moved));
        if (r.Skipped.Count > 0) details.Add("Skipped, no IMDb id (e.g. episodes):" + Lines(r.Skipped));
        if (r.Failed.Count > 0) details.Add("Couldn't be added:" + Lines(r.Failed));
        if (details.Count > 0)
            MessageBox.Show(this, summary + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, details),
                "Sync from Plex playlist", MessageBoxButtons.OK, MessageBoxIcon.Information);

        static string Lines(IEnumerable<string> items) =>
            string.Concat(items.Take(15).Select(i => Environment.NewLine + "  " + i)) + (items.Count() > 15 ? Environment.NewLine + "  …" : "");
    }

    private void btnImdb_Click(object sender, EventArgs e)
    {
        if (Selected is { } row)
            Process.Start(new ProcessStartInfo($"https://www.imdb.com/title/{row.Item.ImdbId}/") { UseShellExecute = true });
    }

    private void dgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => btnImdb.PerformClick();

    private void btnCopy_Click(object sender, EventArgs e)
    {
        var rows = bindingSource.List.OfType<WatchRow>().ToList();
        if (rows.Count == 0) return;
        var text = new StringBuilder("Title\tYear\tIMDb rating\tGenre\tWatched\r\n");
        foreach (var r in rows) text.Append($"{r.Title}\t{r.Year}\t{r.Rating}\t{r.Genre}\t{r.Watched}\r\n");
        Clipboard.SetText(text.ToString());
        ShowStatus($"Copied {rows.Count} movie(s) to the clipboard.");
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        var rows = bindingSource.List.OfType<WatchRow>().Select(r => r.Item).ToList();
        if (rows.Count == 0) return;

        using var dialog = new SaveFileDialog { Filter = "CSV (Excel)|*.csv", FileName = $"{CurrentList}.csv" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        File.WriteAllText(dialog.FileName, WatchlistController.ToCsv(rows), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        ShowStatus($"Exported {rows.Count} movie(s) to {dialog.FileName}.");
        Explorer.Show(dialog.FileName); // the folder, with the new file selected
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
        base.OnFormClosed(e);
    }

    internal sealed record WatchRow(
        string Title, string Year, string Rating, string Genre, string Runtime, string Watched,
        [property: Browsable(false)] WatchListItem Item)
    {
        public static WatchRow From(WatchListItem i) =>
            new(i.Title, i.Year ?? "", i.ImdbRating ?? "", i.Genre ?? "", i.Runtime ?? "", i.Watched == "Y" ? "Yes" : "No", i);
    }
}
