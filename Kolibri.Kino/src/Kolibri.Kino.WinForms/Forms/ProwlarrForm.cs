using System.ComponentModel;
using System.Diagnostics;
using Kolibri.Kino.Controllers.Prowlarr;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// File → Prowlarr: the wishlist Kino on the web sent to Uranus (or a wishlist file: CSV, JSON or XML) as a list to tick,
/// searched on Prowlarr (on the NAS) by title and year. For each title the releases that are it, in a wanted quality
/// and with enough seeders, most seeders first (appsettings.json, Kino:Prowlarr); <b>Show left out</b> shows the rest
/// and why. <b>Get release</b> opens the magnet link, or saves and opens the .torrent file, in your torrent program;
/// a big one (over 6 GB) asks first. Titles in the file without an IMDb id are looked up in the library, on OMDb and on
/// TMDb. Laid out like Library → Search: the list on top, the releases of the selected title below.
/// </summary>
public sealed class ProwlarrForm : AsyncForm
{
    public override string HelpTopic => "prowlarr";

    private readonly ProwlarrController _prowlarr;
    private readonly WebWishlist _web;
    private readonly IServiceProvider _services;
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 20000 };
    private bool _busy;

    private readonly Button btnUranus = TopButton("From Uranus");
    private readonly Button btnFile = TopButton("Open file…");
    private readonly Button btnLookUp = TopButton("Look up missing");
    private readonly Button btnSearch = TopButton("Search Prowlarr", gap: 24);
    private readonly Button btnGet = TopButton("Get release");
    private readonly TextBox txtManual = new() { Width = 240, Margin = new Padding(24, 4, 3, 3), PlaceholderText = "Search for, e.g. Bosch.Legacy.S01E10" };
    private readonly Button btnManual = TopButton("Search this");
    private readonly CheckBox chkAll = new() { Text = "Select all", AutoSize = true, Checked = true, Margin = new Padding(24, 7, 3, 3) };
    private readonly CheckBox chkLeftOut = new() { Text = "Show left out", AutoSize = true, Margin = new Padding(16, 7, 3, 3) };
    private readonly DataGridView dgvWanted = Grid(readOnly: false);
    private readonly DataGridView dgvReleases = Grid(readOnly: true);
    private readonly BindingSource wantedSource = new();
    private readonly BindingSource releasesSource = new();
    private readonly Label lblReleases = new() { Dock = DockStyle.Top, Height = 24, Padding = new Padding(4, 6, 4, 0), AutoEllipsis = true };
    private readonly ToolStripStatusLabel lblStatus = new() { Spring = true, TextAlign = ContentAlignment.MiddleLeft };

    // The yellow bar at the top while Prowlarr's URL or API key is missing (as Search's bar for the shared default keys).
    private readonly Panel configBanner = new() { Dock = DockStyle.Top, Height = 44, BackColor = Color.LightYellow, Padding = new Padding(9, 4, 9, 4), Visible = false };
    private readonly Label lblConfig = new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
    private readonly LinkLabel lnkSettings = new() { Text = "Open Settings…", AutoSize = true, Dock = DockStyle.Right, Padding = new Padding(12, 10, 0, 0) };

    private List<WantedRow> _rows = [];
    private string _source = "";

    public ProwlarrForm(ProwlarrController prowlarr, WebWishlist web, IServiceProvider services)
    {
        _prowlarr = prowlarr;
        _web = web;
        _services = services;
        Text = "Prowlarr";
        ClientSize = new Size(1150, 760);
        StartPosition = FormStartPosition.CenterParent;

        var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 8, 8, 4), WrapContents = true }; // wraps in a narrow window
        top.Controls.AddRange([btnUranus, btnFile, btnLookUp, chkAll, btnSearch, btnGet, chkLeftOut, txtManual, btnManual]);
        btnUranus.Click += async (_, _) => await LoadFromUranusAsync();
        btnFile.Click += async (_, _) => await OpenFileAsync();
        btnLookUp.Click += async (_, _) => await RunAsync("Looking up titles…", async ct =>
        {
            await LookUpMissingAsync(ct);
            await ShowReleaseDatesAsync(ct);
        });
        btnSearch.Click += async (_, _) => await SearchAsync();
        btnGet.Click += async (_, _) => await GetAsync();
        btnManual.Click += async (_, _) => await SearchManualAsync();
        txtManual.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await SearchManualAsync();
        };
        _toolTip.SetToolTip(btnManual, "Puts what you typed at the top of the list and searches Prowlarr for it (Enter does the same): " +
                                       "Bosch.Legacy.S01E10 or Bosch Legacy S01E10 (an episode), The Bear S03 (a series), " +
                                       "Grindhouse (2007) or Grindhouse.2007 (a movie of that year), or just a title.");
        chkLeftOut.CheckedChanged += (_, _) => ShowReleases();
        chkAll.CheckedChanged += (_, _) => TickAll();
        _toolTip.SetToolTip(chkAll, "Ticks every title you don't have (those in SilverScreen stay unticked); untick it to clear every tick and pick titles one by one.");
        _toolTip.SetToolTip(btnFile, "Reads a wishlist file: CSV (Kino's own, or another program's), JSON or XML. Each title needs a title " +
                                     "and a year, under names that clearly mean that (Title, Name …; Year, Released …).");
        _toolTip.SetToolTip(btnLookUp, "Finds the IMDb id and type (movie or series) of the titles without one, by title and year, in your " +
                                       "library, on OMDb and on TMDb.");
        _toolTip.SetToolTip(btnGet, "Opens the selected release's magnet link, or saves its .torrent file and opens it, in your torrent " +
                                    "program (double-click does the same). One bigger than " +
                                    $"{_prowlarr.Options.ConfirmAboveGb:0.#} GB asks first.");
        _toolTip.SetToolTip(chkLeftOut, "Also shows the releases Kino left out (another title, another year, quality, too few seeders …), with why.");

        dgvWanted.DataSource = wantedSource;
        dgvReleases.DataSource = releasesSource;
        dgvWanted.Columns.Add(new DataGridViewLinkColumn
        {
            Name = ImdbColumn, HeaderText = "IMDb", Text = "IMDb", UseColumnTextForLinkValue = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, ToolTipText = "Opens the title on IMDb in a window of its own.",
        });
        dgvWanted.CellContentClick += Wanted_CellContentClick;
        dgvWanted.CellDoubleClick += Wanted_CellDoubleClick;
        dgvWanted.CellFormatting += Wanted_CellFormatting;
        dgvWanted.SelectionChanged += (_, _) => ShowReleases();
        // A tick counts at once, not when the cell is left.
        dgvWanted.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (dgvWanted.IsCurrentCellDirty) dgvWanted.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        dgvWanted.DataBindingComplete += (_, _) => ShapeWantedColumns();
        dgvReleases.DataBindingComplete += (_, _) => ShapeReleaseColumns();
        dgvReleases.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await GetAsync(); };
        dgvReleases.CellFormatting += Releases_CellFormatting;

        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 330 };
        split.Panel1.Controls.Add(dgvWanted);
        split.Panel2.Controls.AddRange([dgvReleases, lblReleases]);

        configBanner.Controls.AddRange([lblConfig, lnkSettings]);
        lnkSettings.LinkClicked += (_, _) =>
        {
            using var form = ActivatorUtilities.CreateInstance<SettingsForm>(_services);
            if (form.ShowDialog(this) == DialogResult.OK) ShowStatus("Settings saved.");
            ShowButtonStates();
        };

        var status = new StatusStrip();
        status.Items.Add(lblStatus);
        // Docked from the last one added: the status bar at the bottom, the yellow bar at the very top, the buttons under it.
        Controls.AddRange([split, top, configBanner, status]);
        ShowButtonStates();
    }

    private static Button TopButton(string text, int gap = 3) => new() { Text = text, AutoSize = true, Margin = new Padding(gap, 3, 3, 3) };

    private static DataGridView Grid(bool readOnly) => new()
    {
        Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = readOnly, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = SystemColors.Window,
    };

    /// <summary>The wishlist on Uranus when it's set up (Settings, category Uranus), else a file.</summary>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_web.IsConfigured) await LoadFromUranusAsync();
        else
        {
            ShowStatus("Uranus isn't set up (Settings, category Uranus): choose a wishlist file.");
            await OpenFileAsync();
        }
    }

    /// <summary>Settings may have changed while the window was open: the buttons follow them.</summary>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        ShowButtonStates();
    }

    /// <summary>The buttons, and the yellow bar saying what's missing in Settings while Prowlarr's URL or API key is.</summary>
    private void ShowButtonStates()
    {
        var problem = _prowlarr.ConfigProblem;
        configBanner.Visible = problem is not null;
        if (problem is not null)
            lblConfig.Text = $"Prowlarr can't be used: Settings has {problem}. Set them under Prowlarr in Settings " +
                             "(the API key is in Prowlarr: Settings → General → Security).";

        btnUranus.Enabled = !_busy && _web.IsConfigured;
        btnSearch.Enabled = btnGet.Enabled = btnManual.Enabled = txtManual.Enabled = !_busy && problem is null;
        btnFile.Enabled = btnLookUp.Enabled = chkAll.Enabled = chkLeftOut.Enabled = !_busy;
        _toolTip.SetToolTip(btnUranus, _web.IsConfigured
            ? "Fetches the newest wishlist Kino on the web sent to Uranus (problembar.net)."
            : "Set the Uranus URL and token in Settings (category Uranus) to fetch the wishlist Kino on the web sent there.");
        _toolTip.SetToolTip(btnSearch, _prowlarr.IsConfigured
            ? "Searches Prowlarr for each ticked title by title and year (series: by title; episodes: by S01E02), and keeps the releases that are it."
            : "Set the Prowlarr URL and API key in Settings (category Prowlarr) to search Prowlarr.");
    }

    private Task LoadFromUranusAsync() =>
        RunAsync("Fetching the wishlist from Uranus…", async ct => await ShowWantedAsync(await _web.GetAsync(ct), "the wishlist on Uranus", ct));

    private async Task OpenFileAsync()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Wishlist to search Prowlarr for",
            Filter = "Wishlists (CSV, JSON, XML)|*.csv;*.txt;*.json;*.xml|All files|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) is { } home && Directory.Exists(Path.Combine(home, "Downloads"))
                ? Path.Combine(home, "Downloads") : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        await RunAsync($"Reading {Path.GetFileName(dialog.FileName)}…", async ct =>
        {
            WishlistFileRead read;
            try
            {
                // Shared, so it reads also while the file is open in Excel (which locks it). UTF-8 unless the file says otherwise.
                await using var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                read = WishlistFile.Parse(await reader.ReadToEndAsync(ct));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
            {
                ShowStatus($"Error: {Path.GetFileName(dialog.FileName)} can't be used: {ex.Message}");
                MessageBox.Show(this, ex.Message, "Open wishlist file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await ShowWantedAsync(read, Path.GetFileName(dialog.FileName), ct);
        });
    }

    /// <summary>
    /// Shows the titles read (one per title and year), says which you have (in SilverScreen with a file: not ticked),
    /// then looks up those without an IMDb id.
    /// </summary>
    private async Task ShowWantedAsync(WishlistFileRead read, string source, CancellationToken ct)
    {
        _source = source;
        _rows = read.Items
            .DistinctBy(i => string.IsNullOrEmpty(i.ImdbId) ? $"{i.Title.ToLowerInvariant()}|{i.Year}" : i.ImdbId.ToLowerInvariant())
            .Select(i => new WantedRow { Item = i })
            .ToList();
        var have = await _prowlarr.HaveAsync(_rows.Select(r => r.Item.TitleImdbId), ct);
        foreach (var row in _rows)
        {
            row.Have = !row.Item.IsEpisode && have.Contains(row.Item.ImdbId);
            row.Search = chkAll.Checked && !row.Have;
        }
        wantedSource.SetRows(_rows);
        ShowReleases();
        ShowStatus($"{_rows.Count} title(s) from {source} ({read.Format})" + (read.Skipped > 0 ? $"; {read.Skipped} without a title or year left out" : "") +
                   $"; {_rows.Count(r => r.Have)} in SilverScreen already (not ticked).");
        if (_rows.Any(NeedsLookUp)) await LookUpMissingAsync(ct);
        await ShowReleaseDatesAsync(ct);
    }

    /// <summary>
    /// The release date of the titles you don't have, where a day is known (library, else OMDb); one not out yet is
    /// light salmon. Not worth failing the list over, so a failure only says so in the status bar.
    /// </summary>
    private async Task ShowReleaseDatesAsync(CancellationToken ct)
    {
        try
        {
            var dates = await _prowlarr.ReleaseDatesAsync(_rows.Where(r => !r.Have).Select(r => r.Item), ct);
            foreach (var row in _rows) row.Released = dates.TryGetValue(row.Item.ImdbId, out var d) ? d : null;
            dgvWanted.Refresh();
            var coming = _rows.Count(r => r.NotOutYet);
            if (coming > 0) ShowStatus($"{lblStatus.Text} {coming} not released yet (salmon).");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            ShowStatus($"Release dates couldn't be fetched: {ex.Message}");
        }
    }

    /// <summary>Select all: every title you don't have ticked, or (unticked) no title, to pick them one by one.</summary>
    private void TickAll()
    {
        dgvWanted.EndEdit();
        foreach (var row in _rows) row.Search = chkAll.Checked && !row.Have;
        dgvWanted.Refresh();
    }

    private static bool NeedsLookUp(WantedRow row) => string.IsNullOrEmpty(row.Item.ImdbId) || string.IsNullOrEmpty(row.Item.Type);

    /// <summary>The IMDb id and type of titles without them, one at a time (OMDb and TMDb are asked for each).</summary>
    private async Task LookUpMissingAsync(CancellationToken ct)
    {
        var missing = _rows.Where(NeedsLookUp).ToList();
        if (missing.Count == 0)
        {
            ShowStatus("Every title has its IMDb id and type.");
            return;
        }
        int found = 0, failed = 0;
        foreach (var (row, index) in missing.Select((r, i) => (r, i)))
        {
            ShowStatus($"Looking up {index + 1} of {missing.Count}: {row.Title} ({row.Year})…");
            try
            {
                if (await _prowlarr.LookUpAsync(row.Item, ct) is not { } item) continue;
                row.Item = item;
                row.Have = (await _prowlarr.HaveAsync([item.ImdbId], ct)).Count > 0;
                if (row.Have) row.Search = false;
                found++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Log.LogWarning(ex, "Looking up {Title} ({Year}) failed", row.Title, row.Year);
                failed++;
            }
            dgvWanted.Refresh();
        }
        wantedSource.ResetBindings(false);
        ShowStatus($"Looked up {missing.Count} title(s) without an IMDb id or type: {found} found" +
                   (failed > 0 ? $", {failed} failed (see Help → Log)" : "") +
                   (missing.Count - found - failed > 0 ? $", {missing.Count - found - failed} not found (searched as movies, by title and year)." : "."));
    }

    /// <summary>Searches Prowlarr for each ticked title in turn and shows the first one's releases.</summary>
    private async Task SearchAsync()
    {
        dgvWanted.EndEdit();
        var ticked = _rows.Where(r => r.Search).ToList();
        if (ticked.Count == 0)
        {
            ShowStatus("Tick the titles to search for (the Search column).");
            return;
        }
        await SearchRowsAsync(ticked);
    }

    /// <summary>
    /// What's typed in the box as a title of its own (see <see cref="ReleaseMatch.FromText"/>), put at the top of the
    /// list and searched at once; the same search again finds the row that's there.
    /// </summary>
    private async Task SearchManualAsync()
    {
        if (string.IsNullOrWhiteSpace(txtManual.Text))
        {
            ShowStatus("Type what to search Prowlarr for, e.g. Bosch.Legacy.S01E10 or Grindhouse (2007).");
            txtManual.Focus();
            return;
        }
        var wanted = ReleaseMatch.FromText(txtManual.Text);
        var row = _rows.FirstOrDefault(r => string.Equals(r.Title, wanted.Title, StringComparison.OrdinalIgnoreCase) && r.Year == wanted.Year);
        if (row is null)
        {
            row = new WantedRow { Item = wanted };
            _rows.Insert(0, row);
            wantedSource.SetRows(_rows);
        }
        row.Search = true;
        await SearchRowsAsync([row]);
        Select(row);
    }

    private async Task SearchRowsAsync(IReadOnlyList<WantedRow> ticked)
    {
        await RunAsync("Searching Prowlarr…", async ct =>
        {
            int searched = 0, failed = 0;
            foreach (var row in ticked)
            {
                ShowStatus($"Searching Prowlarr {searched + failed + 1} of {ticked.Count}: \"{ReleaseMatch.Query(row.Item)}\"…");
                try
                {
                    row.Result = await _prowlarr.SearchAsync(row.Item, ct);
                    row.Found = $"{row.Result.Best.Count} of {row.Result.All.Count}";
                    searched++;
                }
                catch (InvalidOperationException ex) when (searched > 0)
                {
                    // Prowlarr answered before, so this one title failed (an indexer error, a time-out): note it and go on.
                    Log.LogWarning(ex, "Prowlarr search for {Title} failed", row.Title);
                    row.Found = "Error: " + ex.Message;
                    failed++;
                }
                dgvWanted.Refresh();
                if (searched + failed == 1) Select(row); // the first one's releases while the rest are searched
                else if (ReferenceEquals(dgvWanted.CurrentItem(), row)) ShowReleases();
            }
            var releases = ticked.Sum(r => r.Result?.Best.Count ?? 0);
            var none = ticked.Count(r => r.Result is { Best.Count: 0 });
            ShowStatus($"Searched Prowlarr for {searched} title(s): {releases} release(s) to choose from" +
                       (none > 0 ? $"; {none} title(s) with none (tick Show left out to see why)" : "") +
                       (failed > 0 ? $"; {failed} search(es) failed." : "."));
        });
    }

    private void Select(WantedRow row)
    {
        var index = wantedSource.IndexOf(row);
        if (index >= 0 && index < dgvWanted.Rows.Count) dgvWanted.CurrentCell = dgvWanted.Rows[index].Cells.Cast<DataGridViewCell>().First(c => c.Visible);
        ShowReleases();
    }

    /// <summary>The selected title's releases: the wanted ones, or all of them with Show left out.</summary>
    private void ShowReleases()
    {
        if (dgvWanted.CurrentItem() is not WantedRow { Result: { } result })
        {
            releasesSource.SetRows(Array.Empty<ReleaseRow>());
            lblReleases.Text = dgvWanted.CurrentItem() is WantedRow row
                ? $"{row.Title} ({row.Year}): not searched yet. Tick it and click Search Prowlarr."
                : "Releases";
            return;
        }
        var shown = chkLeftOut.Checked ? result.All : result.Best;
        releasesSource.SetRows(shown.Select(c => new ReleaseRow { Check = c }));
        // Most seeders first, with the header's arrow, unless a column was clicked to sort otherwise (that sort is kept).
        if (releasesSource.List is IBindingList { IsSorted: false }
            && dgvReleases.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.DataPropertyName == nameof(ReleaseRow.Seeders)) is { } seeders)
            dgvReleases.Sort(seeders, ListSortDirection.Descending);
        var wanted = result.All.Count(c => c.LeftOut is null);
        lblReleases.Text = $"{result.Wanted.Title} ({result.Wanted.Year}), searched as \"{result.Query}\": " +
                           (chkLeftOut.Checked
                               ? $"all {result.All.Count} release(s), the {wanted} wanted first."
                               : $"the best {result.Best.Count} of {wanted} wanted, of {result.All.Count} found.");
    }

    /// <summary>
    /// The selected release in your torrent program: its magnet link opened, or its .torrent file saved (Kino:Prowlarr:
    /// TorrentFolder, else Downloads) and opened. One that's big, or that Kino left out, asks first.
    /// </summary>
    private async Task GetAsync()
    {
        if (_prowlarr.ConfigProblem is { } problem)
        {
            ShowStatus($"Prowlarr can't be used: Settings has {problem}.");
            return;
        }
        if (dgvReleases.CurrentItem() is not ReleaseRow { Check: var check })
        {
            ShowStatus("Select a release in the list below first (search for a title to get some).");
            return;
        }
        var release = check.Release;
        var limit = _prowlarr.Options.ConfirmAboveGb;
        var warnings = new[]
        {
            release.SizeGb > limit ? $"It is {SizeText(release.Size)}, more than {limit:0.#} GB." : null,
            check.LeftOut is { } why ? $"Kino left it out: {why}." : null,
        }.OfType<string>().ToList();
        if (warnings.Count > 0 && MessageBox.Show(this,
                $"{release.Title}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, warnings)}{Environment.NewLine}{Environment.NewLine}Get it anyway?",
                "Get release", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;

        await RunAsync($"Getting {release.Title} from {release.Indexer}…", async ct =>
        {
            var link = await _prowlarr.GetLinkAsync(release, ct);
            if (link.Magnet is { } magnet)
            {
                if (Open(magnet)) ShowStatus($"Opened the magnet link for {release.Title} ({SizeText(release.Size)}).");
                else
                {
                    Clipboard.SetText(magnet);
                    ShowStatus($"No program opens magnet links here; the magnet link for {release.Title} is on the clipboard.");
                    MessageBox.Show(this, $"{release.Indexer} only has a magnet link for {release.Title}, and no program on this PC opens " +
                                          "magnet links (a torrent program such as qBittorrent does).\n\nThe magnet link is on the clipboard: " +
                                          "paste it into your torrent program.", "Get release", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            var folder = string.IsNullOrWhiteSpace(_prowlarr.Options.TorrentFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")
                : Environment.ExpandEnvironmentVariables(_prowlarr.Options.TorrentFolder.Trim());
            Directory.CreateDirectory(folder);
            var file = Path.Combine(folder, link.FileName);
            for (var n = 2; File.Exists(file); n++) file = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(link.FileName)} ({n}).torrent");
            await File.WriteAllBytesAsync(file, link.Torrent!, ct);
            ShowStatus(Open(file)
                ? $"Saved and opened {Path.GetFileName(file)} ({SizeText(release.Size)})."
                : $"Saved {file}; no program opens .torrent files here.");
        });
    }

    /// <summary>"820 MB" under 1 GB, else "1.8 GB" (1 GB = 1,000 MB = 1,000,000,000 bytes, as Prowlarr counts).</summary>
    private static string SizeText(long bytes) =>
        bytes < 1_000_000_000 ? $"{bytes / 1e6:0} MB" : $"{bytes / 1e9:0.0} GB";

    /// <summary>Opens a magnet link or a file with the program Windows has for it; false when there is none.</summary>
    private bool Open(string target)
    {
        // Without a program for it Windows doesn't fail: it offers the Microsoft Store. So ask the registry first.
        var isMagnet = target.StartsWith("magnet:", StringComparison.OrdinalIgnoreCase);
        if (!(isMagnet ? HasProtocolHandler("magnet") : HasFileHandler(Path.GetExtension(target))))
        {
            Log.LogInformation("No program for {What} on this PC", isMagnet ? "magnet links" : Path.GetExtension(target) + " files");
            return false;
        }
        try
        {
            Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            return true;
        }
        catch (Win32Exception ex)
        {
            Log.LogWarning(ex, "No program opened {What}", target.StartsWith("magnet:", StringComparison.OrdinalIgnoreCase) ? "a magnet link" : target);
            return false;
        }
    }

    /// <summary>A program is registered for links like magnet:… (the user's choice, or a protocol with an open command).</summary>
    private static bool HasProtocolHandler(string scheme)
    {
        using var choice = Microsoft.Win32.Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{scheme}\UserChoice");
        if (choice?.GetValue("ProgId") is string { Length: > 0 }) return true;
        using var command = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($@"{scheme}\shell\open\command");
        return command?.GetValue(null) is string { Length: > 0 };
    }

    /// <summary>A program is registered for files with this extension (.torrent).</summary>
    private static bool HasFileHandler(string extension)
    {
        using var choice = Microsoft.Win32.Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\UserChoice");
        if (choice?.GetValue("ProgId") is string { Length: > 0 }) return true;
        using var type = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
        if (type?.GetValue(null) is string { Length: > 0 } progId)
        {
            using var command = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($@"{progId}\shell\open\command");
            if (command?.GetValue(null) is string { Length: > 0 }) return true;
        }
        using var openWith = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($@"{extension}\OpenWithProgids");
        return openWith?.GetValueNames().Length > 0;
    }

    private const string ImdbColumn = "imdbLink";

    /// <summary>The "IMDb" link: the title's IMDb page in a web window of its own (an episode: its series).</summary>
    private void Wanted_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvWanted.Columns[e.ColumnIndex].Name != ImdbColumn || dgvWanted.ItemAt(e.RowIndex) is not WantedRow row) return;
        if (string.IsNullOrEmpty(row.Item.ImdbId))
            WebPageForm.Show(this, $"https://www.imdb.com/find/?q={Uri.EscapeDataString($"{row.Title} {row.Year}")}");
        else
            WebPageForm.Show(this, $"https://www.imdb.com/title/{row.Item.TitleImdbId}/");
    }

    /// <summary>Double-click (not on the tick box): the title's details, where it has an IMDb id.</summary>
    private void Wanted_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgvWanted.Columns[e.ColumnIndex].DataPropertyName == nameof(WantedRow.Search)
            || dgvWanted.ItemAt(e.RowIndex) is not WantedRow row) return;
        if (string.IsNullOrEmpty(row.Item.ImdbId))
            ShowStatus($"{row.Title} has no IMDb id; Look up missing finds it (or the IMDb link searches IMDb).");
        else
            ActivatorUtilities.CreateInstance<MovieDetailsForm>(_services, row.Item.TitleImdbId).ShowFrom(this);
    }

    /// <summary>In SilverScreen: green, as on the Wishlist. Searched with nothing wanted found: grey.</summary>
    private void Wanted_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || dgvWanted.ItemAt(e.RowIndex) is not WantedRow row) return;
        if (row.Have) e.CellStyle.BackColor = Color.Honeydew;
        else if (row.NotOutYet) e.CellStyle.BackColor = NotOutYetColor;
        else if (row.Result is { Best.Count: 0 } || row.Found.StartsWith("Error", StringComparison.Ordinal)) e.CellStyle.ForeColor = SystemColors.GrayText;
    }

    /// <summary>Not released yet (a known date after today): light salmon, as "in the library, no file" on the Wishlist.</summary>
    private static readonly Color NotOutYetColor = Color.FromArgb(255, 218, 208);

    /// <summary>Left out: grey. Bigger than the limit: the size in dark orange (getting it asks first).</summary>
    private void Releases_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.CellStyle is null || dgvReleases.ItemAt(e.RowIndex) is not ReleaseRow row) return;
        if (dgvReleases.Columns[e.ColumnIndex].DataPropertyName == nameof(ReleaseRow.Size))
        {
            e.Value = SizeText(row.Size);
            e.FormattingApplied = true;
        }
        if (row.Check.LeftOut is not null) e.CellStyle.ForeColor = SystemColors.GrayText;
        else if (dgvReleases.Columns[e.ColumnIndex].DataPropertyName == nameof(ReleaseRow.Size) && row.Check.Release.SizeGb > _prowlarr.Options.ConfirmAboveGb)
            e.CellStyle.ForeColor = Color.DarkOrange;
    }

    private void ShapeWantedColumns()
    {
        foreach (DataGridViewColumn column in dgvWanted.Columns)
        {
            column.ReadOnly = column.DataPropertyName != nameof(WantedRow.Search);
            if (column.DataPropertyName == nameof(WantedRow.Title)) column.FillWeight = 300;
            else if (column.DataPropertyName == nameof(WantedRow.Found)) column.FillWeight = 120;
            else if (column.Name != ImdbColumn) column.FillWeight = 60;
            if (column.DataPropertyName == nameof(WantedRow.Released)) column.DefaultCellStyle.Format = "yyyy-MM-dd";
        }
        if (dgvWanted.Columns[ImdbColumn] is { } imdb) imdb.DisplayIndex = dgvWanted.Columns.Count - 1;
    }

    private void ShapeReleaseColumns()
    {
        foreach (DataGridViewColumn column in dgvReleases.Columns)
        {
            column.FillWeight = column.DataPropertyName switch
            {
                nameof(ReleaseRow.Release) => 400,
                nameof(ReleaseRow.LeftOut) => 140,
                nameof(ReleaseRow.Indexer) => 90,
                _ => 50,
            };
            if (column.DataPropertyName == nameof(ReleaseRow.Size)) column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (column.DataPropertyName == nameof(ReleaseRow.Published)) column.DefaultCellStyle.Format = "yyyy-MM-dd";
            if (column.DataPropertyName == nameof(ReleaseRow.LeftOut)) column.Visible = chkLeftOut.Checked;
        }
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        _busy = busy;
        dgvWanted.ReadOnly = busy;
        ShowButtonStates();
        if (!busy) ShapeWantedColumns();
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }

    /// <summary>A wanted title's columns: tick it to search for it.</summary>
    public sealed class WantedRow
    {
        [Browsable(false)] public WishlistItem Item { get; set; } = null!;
        [Browsable(false)] public ProwlarrSearch? Result { get; set; }

        public bool Search { get; set; }
        public string Title => Item.Title;
        public string? Year => Item.Year;
        public string? Type => Item.Type;
        [DisplayName("IMDb id")] public string ImdbId => Item.ImdbId;
        [DisplayName("In SilverScreen")] public bool Have { get; set; }

        /// <summary>The release date, when a day is known (not just the year); empty otherwise.</summary>
        public DateTime? Released { get; set; }

        [Browsable(false)] public bool NotOutYet => Released is { } d && d > DateTime.Today;

        /// <summary>"8 of 190": the wanted releases shown of those Prowlarr found; empty until searched.</summary>
        [DisplayName("Releases")] public string Found { get; set; } = "";
    }

    /// <summary>A release's columns.</summary>
    public sealed class ReleaseRow
    {
        [Browsable(false)] public ReleaseCheck Check { get; init; } = null!;
        public int Seeders => Check.Release.Seeders;
        /// <summary>In bytes, so it sorts by size; shown as "820 MB" or "1.8 GB" (see <see cref="SizeText"/>).</summary>
        public long Size => Check.Release.Size;
        public string Quality => Check.Quality ?? "";
        public string Release => Check.Release.Title;
        public string Indexer => Check.Release.Indexer;
        public string Link => Check.Release.Links;
        public DateTime? Published => Check.Release.Published;
        [DisplayName("Left out")] public string LeftOut => Check.LeftOut ?? "";
    }
}
