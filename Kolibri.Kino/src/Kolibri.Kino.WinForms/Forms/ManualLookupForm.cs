using System.ComponentModel;
using System.Diagnostics;
using Kolibri.Kino.Controllers.Lookup;
using OMDbApiNet.Model;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Find the movie for one file by hand and link it (port of the file part of SilverScreen's MovieForm).
/// Returns DialogResult.OK when the file was linked.
/// </summary>
public partial class ManualLookupForm : AsyncForm
{
    public override string HelpTopic => "local-movies";

    private readonly ManualLookupController _lookup;
    private readonly string _filePath;
    private CancellationTokenSource? _detailsCts;

    public ManualLookupForm(ManualLookupController lookup, string filePath)
    {
        _lookup = lookup;
        _filePath = filePath;
        InitializeComponent();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvCandidates.AutoGenerateColumns = true;

        Text = $"Find movie – {Path.GetFileName(filePath)}";
        lblFile.Text = filePath;
        var (query, year) = lookup.Suggest(filePath);
        txtQuery.Text = query;
        txtYear.Text = year?.ToString() ?? string.Empty;
    }

    private CandidateRow? Selected => dgvCandidates.CurrentItem() as CandidateRow;

    private async void ManualLookupForm_Load(object sender, EventArgs e) => await SearchAsync();

    private async void btnSearch_Click(object sender, EventArgs e) => await SearchAsync();

    private async Task SearchAsync()
    {
        var yearText = txtYear.Text.Trim();
        int? year = null;
        if (yearText.Length > 0)
        {
            if (!int.TryParse(yearText, out var y) || yearText.Length != 4)
            {
                ShowStatus("Year must be four digits, or empty.");
                return;
            }
            year = y;
        }

        var query = txtQuery.Text;
        await RunAsync("Searching…", async ct =>
        {
            var result = await _lookup.SearchAsync(query, year, ct);
            bindingSource.SetRows(result.Candidates.Select(CandidateRow.From));
            if (dgvCandidates.Columns[nameof(CandidateRow.Title)] is { } title) title.FillWeight = 300;

            var status = result.Candidates.Count == 0
                ? "Nothing found. Try another title, leave the year empty, or paste an IMDb id (tt…)."
                : $"{result.Candidates.Count} candidate(s). Pick the right one and click Link to this file.";
            ShowStatus(result.Note is null ? status : $"{status}  ({result.Note})");
            if (result.Candidates.Count == 0) details.Clear();
        });
    }

    private void dgvCandidates_SelectionChanged(object sender, EventArgs e)
    {
        if (Selected is { } row) LoadDetails(row.Candidate);
    }

    /// <summary>Details and poster in the background; OMDb search hits cost one OMDb call here.</summary>
    private async void LoadDetails(LookupCandidate candidate)
    {
        _detailsCts?.Cancel();
        _detailsCts?.Dispose();
        _detailsCts = new CancellationTokenSource();
        var ct = _detailsCts.Token;

        details.ShowItem(candidate.Details ?? new Item { Title = candidate.Title, Year = candidate.Year, Type = candidate.Type });
        try
        {
            var item = await _lookup.GetDetailsAsync(candidate, ct);
            if (ct.IsCancellationRequested || item is null) return;
            details.ShowItem(item);

            var poster = await _lookup.GetPosterAsync(item, ct);
            if (ct.IsCancellationRequested || poster is null) return;
            using var stream = new MemoryStream(poster);
            using var image = Image.FromStream(stream);
            details.SetPoster(new Bitmap(image));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ShowStatus($"Details for {candidate.Title}: {ex.Message}");
        }
    }

    private async void btnLink_Click(object sender, EventArgs e)
    {
        if (Selected is not { } row) return;

        await RunAsync($"Linking to {row.Title}…", async ct =>
        {
            var outcome = await _lookup.LinkAsync(_filePath, row.Candidate, replaceExisting: false, ct);
            if (outcome.ConflictPath is { } other)
            {
                var answer = MessageBox.Show(this,
                    $"{row.Title} is already linked to{Environment.NewLine}{other}{Environment.NewLine}{Environment.NewLine}" +
                    $"Link it to this file instead?{Environment.NewLine}{_filePath}",
                    "Already linked", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes) return;
                outcome = await _lookup.LinkAsync(_filePath, row.Candidate, replaceExisting: true, ct);
            }

            if (outcome.Linked)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        });
    }

    /// <summary>
    /// IMDb in a web window: the selected movie's page, or with none (nothing found) IMDb's own title search for the
    /// search text and year (the file name when the search text is empty), or the page of an IMDb id typed in.
    /// On a movie's page, "Use tt…" puts its id in the search here and searches.
    /// </summary>
    private void btnImdb_Click(object sender, EventArgs e)
    {
        string url;
        if (Selected is { } row)
        {
            url = $"https://www.imdb.com/title/{row.ImdbId}/";
        }
        else
        {
            var query = txtQuery.Text.Trim() is { Length: > 0 } text ? text : Path.GetFileNameWithoutExtension(_filePath);
            if (System.Text.RegularExpressions.Regex.Match(query, @"\btt\d{7,9}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase) is { Success: true } id)
            {
                url = $"https://www.imdb.com/title/{id.Value.ToLowerInvariant()}/";
            }
            else
            {
                var search = txtYear.Text.Trim() is { Length: 4 } year ? $"{query} {year}" : query;
                url = $"https://www.imdb.com/find/?q={Uri.EscapeDataString(search)}&s=tt";
                ShowStatus($"Opened IMDb's search for \"{search}\".");
            }
        }

        WebPageForm.Show(this, url, useImdbId: async imdbId =>
        {
            if (IsGone) return;
            txtQuery.Text = imdbId;
            txtYear.Text = string.Empty;
            Activate();
            await SearchAsync();
        });
    }

    private void dgvCandidates_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => btnLink.PerformClick();

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        searchBar.Enabled = actions.Enabled = !busy;
        progress.Visible = busy;
    }

    protected override void DisplayStatus(string text) => lblStatus.Text = text;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _detailsCts?.Cancel();
        _detailsCts?.Dispose();
        base.OnFormClosed(e);
    }

    internal sealed record CandidateRow(
        string Title, string Year, string Type, string Source, string ImdbId,
        [property: Browsable(false)] LookupCandidate Candidate)
    {
        public static CandidateRow From(LookupCandidate c) => new(c.Title, c.Year, c.Type, c.Source, c.ImdbId, c);
    }
}
