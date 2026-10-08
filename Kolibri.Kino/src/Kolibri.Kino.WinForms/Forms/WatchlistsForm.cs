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
    private bool _loadingNames;

    public WatchlistsForm(WatchlistController watchlists)
    {
        _watchlists = watchlists;
        InitializeComponent();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvItems.AutoGenerateColumns = true;
        btnPlex.Enabled = watchlists.PlexConfigured;
    }

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
        ShowStatus($"{shown.Count} of {_items.Count} movie(s) in {CurrentList}; {_items.Count(i => i.Watched != "Y")} not watched.");
    }

    private void dgvItems_SelectionChanged(object sender, EventArgs e)
    {
        if (Selected is not { } row) return;
        details.ShowItem(row.Item);
        btnWatched.Text = row.Item.Watched == "Y" ? "Mark not watched" : "Mark watched";
        LoadPoster(row.Item);
    }

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

    private async void btnWatched_Click(object sender, EventArgs e)
    {
        if (Selected is not { } row) return;
        var watched = row.Item.Watched != "Y";
        await RunAsync("Saving…", async ct =>
        {
            if (await _watchlists.SetWatchedAsync(row.Item.ImdbId, watched, ct))
            {
                row.Item.Watched = watched ? "Y" : "N";
                ApplyFilter();
            }
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
