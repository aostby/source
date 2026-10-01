using System.ComponentModel;
using Kolibri.Kino.Controllers.Cleanup;
using Kolibri.Kino.Controllers.LocalMovies;
using Kolibri.Kino.Controllers.Scanning;
using Kolibri.Kino.Core;
using Microsoft.Extensions.DependencyInjection;
using OMDbApiNet.Model;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Port of SilverScreen's ShowLocalMoviesForm: the library's movies in one folder, with filters for
/// library entries whose file is gone and files on disk the library doesn't know, and a scan ("Oppdater")
/// that matches new files to movies.
/// </summary>
public partial class LocalMoviesForm : AsyncForm
{
    private readonly LocalMoviesController _controller;
    private readonly MovieScanController _scan;
    private readonly CleanupController _cleanup;
    private readonly IServiceProvider _services;
    private CancellationTokenSource? _posterCts;
    private string? _folder;
    private bool _scanning;

    public LocalMoviesForm(LocalMoviesController controller, MovieScanController scan, CleanupController cleanup, IServiceProvider services)
    {
        _controller = controller;
        _scan = scan;
        _cleanup = cleanup;
        _services = services;
        InitializeComponent();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvMovies.AutoGenerateColumns = true;
    }

    private LocalMoviesFilter SelectedFilter =>
        rbMissing.Checked ? LocalMoviesFilter.MissingFiles
        : rbNotInLibrary.Checked ? LocalMoviesFilter.NotInLibrary
        : LocalMoviesFilter.All;

    private async void LocalMoviesForm_Load(object sender, EventArgs e) =>
        await RunAsync("Loading…", async ct =>
        {
            _folder = await _controller.GetMoviesFolderAsync(ct);
            txtFolder.Text = _folder ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_folder) || !Directory.Exists(_folder))
                ShowStatus("Choose your movie folder with Browse…");
            else
                await LoadFolderAsync(ct);
        });

    private async void btnBrowse_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Movie folder",
            UseDescriptionForTitle = true,
            InitialDirectory = _folder ?? string.Empty,
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        await RunAsync("Saving folder…", async ct =>
        {
            await _controller.SetMoviesFolderAsync(dialog.SelectedPath, ct);
            _folder = txtFolder.Text = dialog.SelectedPath;
            await LoadFolderAsync(ct);
        });
    }

    private async void Filter_CheckedChanged(object sender, EventArgs e)
    {
        // Fires for the radio button going off too; react once, to the one going on.
        if (sender is RadioButton { Checked: true }) await ReloadAsync();
    }

    private async void btnRefresh_Click(object sender, EventArgs e) => await ReloadAsync();

    private async void btnScanNew_Click(object sender, EventArgs e) => await ScanAsync(ScanMode.NewFiles);

    private async void btnScanAll_Click(object sender, EventArgs e) => await ScanAsync(ScanMode.All);

    private void btnCancel_Click(object sender, EventArgs e) => CancelRunning();

    /// <summary>"Oppdater": match the folder's files to movies, then show what happened.</summary>
    private async Task ScanAsync(ScanMode mode)
    {
        if (_folder is null)
        {
            ShowStatus("Choose your movie folder with Browse… first.");
            return;
        }

        var folder = _folder;
        ScanReport? report = null;
        var onProgress = new Progress<ScanProgress>(p =>
        {
            progress.Style = ProgressBarStyle.Blocks;
            progress.Maximum = Math.Max(1, p.Total);
            progress.Value = Math.Min(p.Done, progress.Maximum);
            if (p.CurrentFile.Length > 0)
                ShowStatus($"Scanning {p.Done + 1} of {p.Total}: {Path.GetFileName(p.CurrentFile)}");
        });

        _scanning = true;
        await RunAsync("Finding video files…", async ct =>
        {
            report = await _scan.ScanAsync(folder, mode, onProgress, ct);
            // Reload even after Cancel: whatever was linked before stopping is saved.
            await LoadFolderAsync(CancellationToken.None);
        });
        _scanning = false;

        if (report is null) return;

        // SilverScreen cleaned up after every scan; Kino:Cleanup:AfterScan decides (Ask / Always / Never).
        if (report.StoppedReason is null && _cleanup.AfterScan != CleanupAfterScan.Never)
            await CleanupAsync(report.Folders, folder, confirm: _cleanup.AfterScan == CleanupAfterScan.Ask);

        ShowStatus($"Scan finished: {report.Count(ScanOutcome.Linked)} linked, {report.Count(ScanOutcome.NotFound)} not found.");
        new ScanReportForm(report, path =>
        {
            var linked = FindMovieForFile(path);
            if (linked) _ = ReloadAsync();
            return linked;
        }).Show(this);
    }

    private async void btnCleanup_Click(object sender, EventArgs e)
    {
        if (_folder is not null) await CleanupAsync([_folder], _folder, confirm: true);
    }

    /// <summary>Finds leftover files, asks when <paramref name="confirm"/> is set, then deletes them.</summary>
    private async Task CleanupAsync(IReadOnlyList<string> folders, string root, bool confirm)
    {
        CleanupPlan? plan = null;
        await RunAsync("Looking for leftover files…", async ct => plan = await _cleanup.PlanAsync(folders, ct));
        if (plan is null) return;
        if (plan.Files.Count == 0)
        {
            ShowStatus("No leftover files to clean up.");
            return;
        }

        if (confirm)
        {
            using var dialog = new CleanupConfirmForm(plan, root, _cleanup.DeleteEmptyFolders);
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                ShowStatus("Cleanup skipped; no files were deleted.");
                return;
            }
        }

        await RunAsync($"Deleting {plan.Files.Count} leftover file(s)…", async ct =>
        {
            var result = await _cleanup.ExecuteAsync(plan, ct);
            ShowStatus($"Cleanup: deleted {result.FilesDeleted} file(s) and {result.FoldersDeleted} empty folder(s)"
                + (result.Failed.Count > 0 ? $"; {result.Failed.Count} could not be deleted." : "."));
            if (result.Failed.Count > 0)
                MessageBox.Show(this, string.Join(Environment.NewLine, result.Failed.Take(30)), "Some files could not be deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        });
    }

    private async void btnFindMovie_Click(object sender, EventArgs e)
    {
        var path = dgvMovies.CurrentItem() switch
        {
            FileRow row => row.Source.FilePath,
            MovieRow row => row.Movie.FilePath,
            _ => null,
        };

        if (path is null)
        {
            using var dialog = new OpenFileDialog { Title = "Video file to find the movie for", InitialDirectory = _folder ?? string.Empty };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            path = dialog.FileName;
        }

        if (FindMovieForFile(path)) await ReloadAsync();
    }

    private void btnAddToWatchlist_Click(object sender, EventArgs e)
    {
        if (dgvMovies.CurrentItem() is not MovieRow { Movie: var movie }) return;
        using var dialog = ActivatorUtilities.CreateInstance<AddToWatchlistDialog>(_services, movie.ImdbId, movie.Title);
        if (dialog.ShowDialog(this) == DialogResult.OK) ShowStatus(dialog.ResultMessage);
    }

    private void dgvMovies_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && dgvMovies.ItemAt(e.RowIndex) is FileRow) btnFindMovie.PerformClick();
    }

    /// <summary>Opens the manual lookup for one file; true when the user linked it.</summary>
    private bool FindMovieForFile(string path)
    {
        using var form = ActivatorUtilities.CreateInstance<ManualLookupForm>(_services, path);
        return form.ShowDialog(this) == DialogResult.OK;
    }

    private async void btnRemoveMissing_Click(object sender, EventArgs e)
    {
        if (_folder is null) return;

        var answer = MessageBox.Show(this,
            $"Remove the library's links to files in{Environment.NewLine}{_folder}{Environment.NewLine}that no longer exist?" +
            $"{Environment.NewLine}{Environment.NewLine}The movie details stay in the library, so a later scan can link them again.",
            "Remove missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (answer != DialogResult.Yes) return;

        var folder = _folder;
        await RunAsync("Removing links to missing files…", async ct =>
        {
            var removed = await _scan.RemoveMissingAsync(folder, ct);
            await LoadFolderAsync(ct);
            ShowStatus($"Removed {removed} link(s) to missing files.");
        });
    }

    private Task ReloadAsync() =>
        _folder is null ? Task.CompletedTask : RunAsync("Reading folder…", LoadFolderAsync);

    private async Task LoadFolderAsync(CancellationToken ct)
    {
        var result = await _controller.LoadAsync(_folder!, SelectedFilter, ct);
        Text = $"Local movies – {result.Folder}";
        details.Clear();

        if (result.Filter == LocalMoviesFilter.NotInLibrary)
        {
            Bind(result.UnmatchedFiles.Select(f => new FileRow(Path.GetFileName(f.FilePath), Path.GetDirectoryName(f.FilePath) ?? "", f.IsMultipart, f)).ToList());
            var multipart = result.UnmatchedFiles.Count(f => f.IsMultipart);
            ShowStatus($"{result.UnmatchedFiles.Count} video file(s) not in the library ({multipart} multipart/extra). {result.LibraryFilesInFolder} library file(s) in this folder.");
        }
        else
        {
            Bind(result.Movies.Select(MovieRow.From).ToList());
            var missing = result.Movies.Count(m => !m.FileExists);
            var noDetails = result.Movies.Count(m => m.Item is null);
            ShowStatus(result.Filter == LocalMoviesFilter.MissingFiles
                ? $"{result.Movies.Count} library file(s) no longer on disk."
                : $"{result.Movies.Count} movie(s) in the library for this folder. {missing} missing on disk, {noDetails} without details.");
        }
    }

    private void Bind<T>(IReadOnlyList<T> rows)
    {
        bindingSource.SetRows(rows);
        foreach (var wide in new[] { nameof(MovieRow.Title), nameof(MovieRow.File), nameof(FileRow.Folder) })
            if (dgvMovies.Columns[wide] is { } column) column.FillWeight = 250;
    }

    private void dgvMovies_SelectionChanged(object sender, EventArgs e)
    {
        switch (dgvMovies.CurrentItem())
        {
            case MovieRow row:
                details.ShowMovie(row.Movie);
                LoadPoster(row.Movie.Item);
                break;
            case FileRow row:
                _posterCts?.Cancel();
                details.ShowFile(row.Source);
                break;
        }
    }

    /// <summary>Loads the poster in the background; a newer selection cancels an older one.</summary>
    private async void LoadPoster(Item? item)
    {
        _posterCts?.Cancel();
        _posterCts?.Dispose();
        _posterCts = new CancellationTokenSource();
        var ct = _posterCts.Token;
        if (item is null) return;

        try
        {
            var bytes = await _controller.GetPosterAsync(item, ct);
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

    private void dgvMovies_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvMovies.ItemAt(e.RowIndex) is MovieRow { OnDisk: false } && e.CellStyle is not null)
            e.CellStyle.ForeColor = SystemColors.GrayText;
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        toolbar.Enabled = !busy;
        progress.Visible = busy;
        btnCancel.Visible = busy && _scanning;
        if (!busy) progress.Style = ProgressBarStyle.Marquee;
    }

    protected override void ShowStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _posterCts?.Cancel();
        _posterCts?.Dispose();
        base.OnFormClosed(e);
    }

    internal sealed record MovieRow(
        string Title, string Year, string Rating, string Genre, bool OnDisk, string File,
        [property: Browsable(false)] LocalMovie Movie)
    {
        public static MovieRow From(LocalMovie m) =>
            new(m.Title, m.Item?.Year ?? "", m.Item?.ImdbRating ?? "", m.Item?.Genre ?? "", m.FileExists, m.FilePath, m);
    }

    internal sealed record FileRow(
        string File, string Folder, bool Multipart,
        [property: Browsable(false)] UnmatchedFile Source);
}
