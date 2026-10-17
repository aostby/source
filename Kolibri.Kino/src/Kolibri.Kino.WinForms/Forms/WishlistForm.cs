using System.ComponentModel;
using System.Diagnostics;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Library → Wishlist: titles wanted, perhaps not owned. Search looks in SilverScreen.db first, then OMDb, and says which
/// titles are in SilverScreen and which are to get; the list says the same (looked up each time) and is written to a
/// file with "In SilverScreen" as true/false. Titles found on OMDb aren't put in the library, so they have no file.
/// </summary>
public partial class WishlistForm : AsyncForm, IWritesToExcel
{
    public override string HelpTopic => "wishlist";

    private readonly WishlistController _wishlist;
    private readonly IServiceProvider _services;
    private IReadOnlyList<WishlistRow> _rows = [];

    public WishlistForm(WishlistController wishlist, IServiceProvider services)
    {
        _wishlist = wishlist;
        _services = services;
        InitializeComponent();
        this.Describe(
            (btnSearch, "Searches SilverScreen.db (your library) first, then OMDb and TMDb. Enter does the same. \"Operation Kabul, 2025\" or " +
                        "\"Operation Kabul (2025)\" keeps to that year; an IMDb id or link finds that title."),
            (btnAdd, "Puts the selected search hit on the wishlist. A title from OMDb isn't added to the library, so it has no file."),
            (btnRemove, "Takes the selected title off the wishlist."),
            (btnImdb, "Opens the selected title's page on IMDb."),
            (btnImport, "Reads a wishlist CSV (e.g. Download CSV on the web app's Wishlist page) and adds its entries as they are. " +
                        "Entries whose IMDb id is on the wishlist already are skipped."));
        // Columns come from the row types; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvHits.AutoGenerateColumns = dgvWishlist.AutoGenerateColumns = true;
        // An "IMDb" link on each row, to check in a window of its own that it's the right title.
        foreach (var grid in new[] { dgvHits, dgvWishlist })
        {
            grid.Columns.Add(new DataGridViewLinkColumn
            {
                Name = ImdbColumn, HeaderText = "IMDb", Text = "IMDb", UseColumnTextForLinkValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, ToolTipText = "Opens the title on IMDb in a window of its own.",
            });
            grid.CellContentClick += Grid_CellContentClick;
        }
    }

    private bool _loaded;
    private string? _searchAfterLoad;

    private async void WishlistForm_Load(object sender, EventArgs e)
    {
        await LoadListAsync();
        _loaded = true;
        if (_searchAfterLoad is { } query)
        {
            _searchAfterLoad = null;
            txtSearch.Text = query;
            await SearchAsync();
        }
    }

    /// <summary>
    /// Puts <paramref name="query"/> ("Title, Year", e.g. from Similar movies in a details window) in the search box and
    /// searches; when the window is just opening, after the list has loaded (a search would cancel the loading).
    /// </summary>
    public async void SearchFor(string query)
    {
        if (!_loaded)
        {
            _searchAfterLoad = query;
            return;
        }
        txtSearch.Text = query;
        await SearchAsync();
    }

    private Task LoadListAsync(string? status = null) =>
        RunAsync("Loading the wishlist…", async ct =>
        {
            _rows = await _wishlist.GetAsync(ct);
            wishlistSource.SetRows(_rows.Select(WishRow.From));
            Widen(dgvWishlist, nameof(WishRow.Title));
            var toGet = _rows.Count(r => !r.InSilverScreen);
            ShowStatus(status ?? $"{_rows.Count} title(s) on the wishlist: {toGet} to get, {_rows.Count - toGet} in SilverScreen.");
        });

    private async void btnSearch_Click(object sender, EventArgs e) => await SearchAsync();

    private async Task SearchAsync()
    {
        var text = txtSearch.Text.Trim();
        if (text.Length == 0)
        {
            ShowStatus("Type a title (or an IMDb id) to search for.");
            txtSearch.Focus();
            return;
        }

        await RunAsync($"Searching SilverScreen and OMDb for \"{text}\"…", async ct =>
        {
            var result = await _wishlist.SearchAsync(text, ct);
            hitsSource.SetRows(result.Hits.Select(HitRow.From));
            Widen(dgvHits, nameof(HitRow.Title));
            var have = result.Hits.Count(h => h.InSilverScreen);
            ShowStatus($"{result.Hits.Count} hit(s): {have} in SilverScreen, {result.Hits.Count - have} to get." +
                       (result.OnlineError is null ? "" : $" OMDb couldn't be searched: {result.OnlineError}"));
        });
    }

    private void txtSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        btnSearch.PerformClick();
    }

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        if (dgvHits.CurrentItem() is not HitRow { Hit: var hit }) return;
        if (hit.OnWishlist)
        {
            ShowStatus($"{hit.Title} is on the wishlist already.");
            return;
        }

        string? added = null;
        await RunAsync($"Adding {hit.Title}…", async ct =>
        {
            var item = await _wishlist.AddAsync(hit.ImdbId, ct);
            added = $"Added {item.Title} to the wishlist ({(hit.InSilverScreen ? "in SilverScreen already" : "to get")}).";
        });
        if (added is null || IsGone) return;
        foreach (var row in hitsSource.List.OfType<HitRow>().Where(r => r.Hit.ImdbId == hit.ImdbId)) row.OnWishlist = true;
        dgvHits.Refresh();
        await LoadListAsync(added);
    }

    private async void btnRemove_Click(object sender, EventArgs e)
    {
        if (dgvWishlist.CurrentItem() is not WishRow { Row: var row }) return;
        await RunAsync($"Removing {row.Item.Title}…", ct => _wishlist.RemoveAsync(row.Item.ImdbId, ct));
        if (!IsGone) await LoadListAsync($"Removed {row.Item.Title} from the wishlist.");
    }

    private const string ImdbColumn = "imdbLink";

    /// <summary>The "IMDb" link: the title's IMDb page in a web window of its own (as Find movie's Open on IMDb).</summary>
    private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || sender is not DataGridView grid || grid.Columns[e.ColumnIndex].Name != ImdbColumn) return;
        var imdbId = grid.ItemAt(e.RowIndex) switch
        {
            HitRow h => h.Hit.ImdbId,
            WishRow w => w.Row.Item.TitleImdbId, // an episode: its series
            _ => null,
        };
        if (imdbId is not null) WebPageForm.Show(this, $"https://www.imdb.com/title/{imdbId}/");
    }

    /// <summary>A wishlist CSV (from the web app, or Write to Excel…) onto this wishlist; IMDb ids already on it are skipped.</summary>
    private async void btnImport_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Import wishlist",
            Filter = "CSV (Excel)|*.csv|All files|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) is { } home && Directory.Exists(Path.Combine(home, "Downloads"))
                ? Path.Combine(home, "Downloads") : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        WishlistImport? result = null;
        await RunAsync($"Importing {Path.GetFileName(dialog.FileName)}…", async ct =>
        {
            try
            {
                // Shared, so it reads also while the file is open in Excel (which locks it). UTF-8; the BOM is skipped.
                await using var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                var csv = await reader.ReadToEndAsync(ct);
                result = await _wishlist.ImportCsvAsync(csv, ct);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
            {
                MessageBox.Show(this, ex.Message, "Import wishlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        });
        if (result is null) return;
        await LoadListAsync();
        ShowStatus($"Imported {result.Added} entr{(result.Added == 1 ? "y" : "ies")} from {Path.GetFileName(dialog.FileName)}; " +
                   $"{result.AlreadyThere} already on the wishlist" + (result.Invalid > 0 ? $", {result.Invalid} without an IMDb id." : "."));
    }

    private void btnImdb_Click(object sender, EventArgs e)
    {
        var imdbId = (ActiveControl == dgvHits ? (dgvHits.CurrentItem() as HitRow)?.Hit.ImdbId : null)
                     ?? (dgvWishlist.CurrentItem() as WishRow)?.Row.Item.TitleImdbId;
        if (imdbId is not null) Process.Start(new ProcessStartInfo($"https://www.imdb.com/title/{imdbId}/") { UseShellExecute = true });
    }

    /// <summary>
    /// "Write to Excel…" (bottom right) and Ctrl+S: the whole wishlist (not the grid as shown), with "In SilverScreen" as
    /// true/false to sort or filter on.
    /// </summary>
    public void WriteToExcel()
    {
        if (_rows.Count == 0)
        {
            ShowStatus("The wishlist is empty; nothing to write.");
            return;
        }
        if (ExcelExport.Save(this, WishlistController.ToCsv(_rows), "Wishlist") is { } file)
            ShowStatus($"Wrote {_rows.Count} title(s) to {Path.GetFileName(file)}.");
    }

    /// <summary>Double-click: the title's details (from OMDb when it isn't in SilverScreen).</summary>
    private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || sender is not DataGridView grid) return;
        var imdbId = grid.ItemAt(e.RowIndex) switch
        {
            HitRow h => h.Hit.ImdbId,
            WishRow w => w.Row.Item.TitleImdbId, // an episode: its series
            _ => null,
        };
        if (imdbId is not null) ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, imdbId).ShowFrom(this);
    }

    /// <summary>In SilverScreen: green. To get: light orange.</summary>
    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || sender is not DataGridView grid) return;
        bool? have = grid.ItemAt(e.RowIndex) switch
        {
            HitRow h => h.InSilverScreen,
            WishRow w => w.InSilverScreen,
            _ => null,
        };
        if (have is { } h2) e.CellStyle.BackColor = h2 ? Color.Honeydew : Color.PapayaWhip;
    }

    private static void Widen(DataGridView grid, string column)
    {
        if (grid.Columns[column] is { } c) c.FillWeight = 300;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        searchBar.Enabled = listButtons.Enabled = !busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    /// <summary>A search hit's columns.</summary>
    public sealed class HitRow
    {
        [Browsable(false)] public WishlistHit Hit { get; init; } = null!;
        public string Title => Hit.Title;
        public string? Year => Hit.Year;
        public string? Type => Hit.Type;
        [DisplayName("In SilverScreen")] public bool InSilverScreen => Hit.InSilverScreen;
        public string Where => Hit.InSilverScreen ? "In SilverScreen" : "To get";
        [DisplayName("On wishlist")] public bool OnWishlist { get; set; }

        public static HitRow From(WishlistHit hit) => new() { Hit = hit, OnWishlist = hit.OnWishlist };
    }

    /// <summary>A wishlist entry's columns.</summary>
    public sealed class WishRow
    {
        [Browsable(false)] public WishlistRow Row { get; init; } = null!;
        public string Title => Row.Item.Title;
        public string? Year => Row.Item.Year;
        public string? Type => Row.Item.Type;
        [DisplayName("IMDb rating")] public string? Rating => Row.Item.ImdbRating;
        public string? Genre => Row.Item.Genre;
        [DisplayName("In SilverScreen")] public bool InSilverScreen => Row.InSilverScreen;
        public string? File => Row.FilePath;
        public DateTime Added => Row.Item.Added;

        public static WishRow From(WishlistRow row) => new() { Row = row };
    }
}
