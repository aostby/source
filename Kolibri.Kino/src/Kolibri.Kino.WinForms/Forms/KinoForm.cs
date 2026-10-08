using System.ComponentModel;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Settings;
using Microsoft.Extensions.DependencyInjection;
using OMDbApiNet.Model;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Kino's search window (opened in <see cref="MainForm"/> at start): search the library or OMDb, and open the other windows.
/// Thin UI: collects input, awaits the controller, shows the result. No data access here.
/// </summary>
public partial class KinoForm : AsyncForm
{
    public override string HelpTopic => "search";

    private static readonly string[] VisibleColumns =
        [nameof(Item.Title), nameof(Item.Year), nameof(Item.Type), nameof(Item.ImdbRating), nameof(Item.Genre), nameof(Item.ImdbId)];

    private readonly MovieController _movies;
    private readonly SettingsController _settings;
    private readonly IServiceProvider _services;
    private bool _showingOnlineResults;

    public KinoForm(MovieController movies, SettingsController settings, IServiceProvider services)
    {
        _movies = movies;
        _settings = settings;
        _services = services;
        InitializeComponent();
        lblDefaultKeys.Text = SettingsController.DefaultKeysMessage;
        UpdateDefaultKeysBanner();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvItems.AutoGenerateColumns = true;
    }

    private async void KinoForm_Load(object sender, EventArgs e) =>
        await RunAsync("Loading library…", async ct => ShowLocal(await _movies.GetLocalAsync(ct: ct)));

    private async void btnSearchLocal_Click(object sender, EventArgs e) =>
        await RunAsync("Searching library…", async ct => ShowLocal(await _movies.SearchLocalAsync(txtSearch.Text, ct)));

    private async void btnSearchOnline_Click(object sender, EventArgs e)
    {
        // OMDb can't list everything, so it needs a title. (An empty OMDb key can't happen: empty means the shared test key.)
        if (string.IsNullOrWhiteSpace(txtSearch.Text))
        {
            ShowStatus("Enter a title (or part of one) to search OMDb.");
            txtSearch.Focus();
            return;
        }
        await RunAsync("Searching OMDb…", async ct => ShowOnline(await _movies.SearchOnlineAsync(txtSearch.Text, ct)));
    }

    private async void btnImport_Click(object sender, EventArgs e)
    {
        if (dgvItems.CurrentItem() is not SearchItem selected) return;

        await RunAsync($"Importing {selected.Title}…", async ct =>
        {
            var item = await _movies.ImportAsync(selected.ImdbId, ct);
            ShowStatus(item is null ? $"OMDb has no details for {selected.ImdbId}." : $"Saved {item.Title} ({item.Year}).");
        });
    }

    // Inside the MDI main window, like its menu items (Controls/Mdi.cs).
    private void btnLocalMovies_Click(object sender, EventArgs e) => this.ShowSingle<LocalMoviesForm>(_services);

    private void btnLocalSeries_Click(object sender, EventArgs e) => this.ShowSingle<LocalSeriesForm>(_services);

    private void btnWatchlists_Click(object sender, EventArgs e) => this.ShowSingle<WatchlistsForm>(_services);

    private void btnSettings_Click(object sender, EventArgs e)
    {
        using var form = ActivatorUtilities.CreateInstance<SettingsForm>(_services);
        if (form.ShowDialog(this) == DialogResult.OK) ShowStatus("Settings saved.");
        UpdateDefaultKeysBanner();
    }

    private void lnkSetKeys_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => btnSettings.PerformClick();

    /// <summary>The yellow bar asking for real keys, shown while a shared default key is in use (it still works).</summary>
    internal void UpdateDefaultKeysBanner() => defaultKeysBanner.Visible = _settings.UsesDefaultKeys;

    private void btnAddToWatchlist_Click(object sender, EventArgs e)
    {
        var (imdbId, title) = dgvItems.CurrentItem() switch
        {
            Item item => (item.ImdbId, item.Title),
            SearchItem hit => (hit.ImdbId, hit.Title),
            _ => (null, null),
        };
        if (imdbId is null) return;

        using var dialog = ActivatorUtilities.CreateInstance<AddToWatchlistDialog>(_services, imdbId, title ?? imdbId);
        if (dialog.ShowDialog(this) == DialogResult.OK) ShowStatus(dialog.ResultMessage);
    }

    private void txtSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        btnSearchLocal.PerformClick();
    }

    private void ShowLocal(IReadOnlyList<Item> items)
    {
        _showingOnlineResults = false;
        _localResults = items;
        _onlineResults = [];
        BuildTypeFilter(items.Select(i => i.Type));
        ApplyTypeFilter();
    }

    private void ShowOnline(IReadOnlyList<SearchItem> items)
    {
        _showingOnlineResults = true;
        _onlineResults = items;
        _localResults = [];
        BuildTypeFilter(items.Select(i => i.Type));
        ApplyTypeFilter();
    }

    #region Details and rating colours

    private void dgvItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) ShowDetails();
    }

    private void menuDetails_Click(object sender, EventArgs e) => ShowDetails();

    /// <summary>All details of the selected library item or OMDb search result, in its own window.</summary>
    private void ShowDetails()
    {
        var (imdbId, _) = SelectedIdAndTitle();
        if (imdbId is null) return;
        ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, imdbId).ShowFrom(this);
    }

    /// <summary>IMDb ratings in colour: red below 6, orange 6–6.9, green from 7.</summary>
    private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || e.ColumnIndex < 0 || dgvItems.Columns[e.ColumnIndex].DataPropertyName != nameof(Item.ImdbRating)) return;
        if (RatingColors.For(e.Value as string) is { } color)
        {
            e.CellStyle.ForeColor = color;
            e.CellStyle.Font = new Font(dgvItems.Font, FontStyle.Bold);
        }
    }

    #endregion

    #region Right-click menu

    /// <summary>Right-click selects the row first, so the menu acts on the item under the mouse.</summary>
    private void dgvItems_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            dgvItems.CurrentCell = dgvItems[e.ColumnIndex, e.RowIndex];
    }

    private void itemMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var selected = dgvItems.CurrentItem();
        if (selected is null) e.Cancel = true;
        // Only library entries can be removed or have files; OMDb search results aren't in the library.
        menuRemove.Enabled = menuOpenOnDisk.Enabled = selected is Item;
    }

    private (string? ImdbId, string? Title) SelectedIdAndTitle() => dgvItems.CurrentItem() switch
    {
        Item item => (item.ImdbId, item.Title),
        SearchItem hit => (hit.ImdbId, hit.Title),
        _ => (null, null),
    };

    /// <summary>TMDb pages use TMDb's own ids, so this asks TMDb first (one request); falls back to a TMDb search.</summary>
    private async void menuOpenTmdb_Click(object sender, EventArgs e)
    {
        var (imdbId, title) = SelectedIdAndTitle();
        if (imdbId is null) return;

        string? page = null;
        await RunAsync($"Finding {title} on TMDb…", async ct => page = await _movies.GetTmdbPageAsync(imdbId, title, ct));
        if (page is null) return;
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(page) { UseShellExecute = true });
        ShowStatus(page.Contains("/search?") ? $"TMDb doesn't know {imdbId}; opened a search for \"{title}\"." : $"Opened {page}");
    }

    /// <summary>Explorer with the linked file selected, or the linked folder (series).</summary>
    private async void menuOpenOnDisk_Click(object sender, EventArgs e)
    {
        if (dgvItems.CurrentItem() is not Item item) return;

        string? path = null;
        await RunAsync($"Looking up {item.Title}…", async ct => path = await _movies.GetLinkedPathAsync(item.ImdbId, ct));

        if (path is null)
            ShowStatus($"\"{item.Title}\" isn't linked to a file or folder. Scan its folder in Local movies, or use Find movie for file.");
        else if (File.Exists(path) || Directory.Exists(path))
            Explorer.Show(path);
        else
            ShowStatus($"\"{item.Title}\" is linked to {path}, which no longer exists.");
    }

    private async void menuRemove_Click(object sender, EventArgs e)
    {
        if (dgvItems.CurrentItem() is not Item item) return;

        var what = string.IsNullOrWhiteSpace(item.Type) ? "" : $", {item.Type}";
        var answer = MessageBox.Show(this,
            $"Remove \"{item.Title}\" ({item.Year}{what}) {item.ImdbId} from the library?{Environment.NewLine}{Environment.NewLine}" +
            "Its file link is removed too. Files on disk are not touched.",
            "Remove from library", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (answer != DialogResult.Yes) return;

        await RunAsync($"Removing {item.Title}…", async ct =>
        {
            await _movies.RemoveFromLibraryAsync(item.ImdbId, ct);
            _localResults = _localResults.Where(i => i.ImdbId != item.ImdbId).ToList();
            BuildTypeFilter(_localResults.Select(i => i.Type));
            ApplyTypeFilter();
            ShowStatus($"Removed \"{item.Title}\" from the library.");
        });
    }

    private void menuOpenImdb_Click(object sender, EventArgs e)
    {
        var imdbId = dgvItems.CurrentItem() switch
        {
            Item item => item.ImdbId,
            SearchItem hit => hit.ImdbId,
            _ => null,
        };
        if (imdbId is not null)
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"https://www.imdb.com/title/{imdbId}/") { UseShellExecute = true });
    }

    #endregion

    #region Sort buttons

    /// <summary>
    /// Sort by the checked button. Genre groups by the whole genre text ("Action, Sci-Fi"), alphabetical by title
    /// within a group; Year and Rating put the newest and best first; As listed goes back to the loaded order.
    /// Clicking a column header still sorts by that column.
    /// </summary>
    private void Sort_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is not RadioButton { Checked: true } button) return;
        ApplySortChoice(button);
    }

    private void ApplySortChoice(RadioButton button)
    {
        if (bindingSource.List is not IBindingList list) return;

        if (button == rbSortNone)
        {
            bindingSource.RemoveSort();
            ApplyTypeFilter(); // rebuild in the loaded order
            return;
        }

        // Sorting is stable, so sorting by title first leaves each genre (or year, or rating) alphabetical.
        var properties = TypeDescriptor.GetProperties(list.Count > 0 ? list[0]!.GetType() : typeof(Item));
        if (properties[nameof(Item.Title)] is { } title) list.ApplySort(title, ListSortDirection.Ascending);

        var (column, direction) = button == rbSortTitle ? (nameof(Item.Title), ListSortDirection.Ascending)
            : button == rbSortYear ? (nameof(Item.Year), ListSortDirection.Descending)
            : button == rbSortRating ? (nameof(Item.ImdbRating), ListSortDirection.Descending)
            : (nameof(Item.Genre), ListSortDirection.Ascending);
        if (properties[column] is { } property) list.ApplySort(property, direction);
        else ShowStatus($"These results have no {column} to sort by.");
    }

    #endregion

    #region Type filter

    private IReadOnlyList<Item> _localResults = [];
    private IReadOnlyList<SearchItem> _onlineResults = [];

    /// <summary>Selected type (lower case), or null for All. Kept when the list reloads, if the type is still there.</summary>
    private string? _selectedType;

    /// <summary>OMDb writes "movie", SilverScreen's TMDb import wrote "Movie": compare lower-cased, and "" for none.</summary>
    private static string TypeKey(string? type) => (type ?? string.Empty).Trim().ToLowerInvariant();

    private static string TypeLabel(string key) => key switch
    {
        "" => "(no type)",
        "movie" => "Movies",
        "series" => "Series",
        "episode" => "Episodes",
        "game" => "Games",
        _ => char.ToUpperInvariant(key[0]) + key[1..],
    };

    /// <summary>One radio button for All and one per type in the list, largest first, with counts.</summary>
    private void BuildTypeFilter(IEnumerable<string?> types)
    {
        var counts = types.GroupBy(TypeKey).Select(g => (Key: g.Key, Count: g.Count())).OrderByDescending(g => g.Count).ToList();
        if (_selectedType is not null && !counts.Any(c => c.Key == _selectedType)) _selectedType = null;

        typeFilter.SuspendLayout();
        foreach (var old in typeFilter.Controls.OfType<RadioButton>().ToList())
        {
            typeFilter.Controls.Remove(old);
            old.Dispose();
        }

        AddTypeButton($"All ({counts.Sum(c => c.Count):N0})", null);
        foreach (var (key, count) in counts)
            AddTypeButton($"{TypeLabel(key)} ({count:N0})", key);
        typeFilter.ResumeLayout();

        void AddTypeButton(string text, string? key)
        {
            var button = new RadioButton
            {
                AutoSize = true,
                Text = text,
                Tag = key,
                Checked = key == _selectedType,
                Margin = new Padding(3, 3, 9, 3),
            };
            button.CheckedChanged += TypeFilter_CheckedChanged;
            typeFilter.Controls.Add(button);
        }
    }

    private void TypeFilter_CheckedChanged(object? sender, EventArgs e)
    {
        // Fires for the button going off too; react once, to the one going on.
        if (sender is not RadioButton { Checked: true } button) return;
        _selectedType = button.Tag as string;
        ApplyTypeFilter();
    }

    private void ApplyTypeFilter()
    {
        bool Wanted(string? type) => _selectedType is null || TypeKey(type) == _selectedType;

        if (_showingOnlineResults)
        {
            var shown = _onlineResults.Where(i => Wanted(i.Type)).ToList();
            Bind(shown);
            ShowStatus($"{Counted(shown.Count, _onlineResults.Count)} result(s) from OMDb. Select one and click Import.");
        }
        else
        {
            var shown = _localResults.Where(i => Wanted(i.Type)).ToList();
            Bind(shown);
            ShowStatus($"{Counted(shown.Count, _localResults.Count)} item(s) in library.");
        }

        static string Counted(int shown, int total) => shown == total ? $"{total:N0}" : $"{shown:N0} of {total:N0}";
    }

    #endregion

    /// <summary>Shows the rows sortable by clicking a column header, keeping the current sort.</summary>
    private void Bind<T>(IEnumerable<T> items)
    {
        bindingSource.SetRows(items);
        foreach (DataGridViewColumn column in dgvItems.Columns)
            column.Visible = VisibleColumns.Contains(column.DataPropertyName);
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        txtSearch.Enabled = btnSearchLocal.Enabled = btnSearchOnline.Enabled = typeFilter.Enabled = !busy;
        btnImport.Enabled = !busy && _showingOnlineResults;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;
}
