using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.WinForms.Controls;
using OMDbApiNet.Model;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>What the user chose for a folder in <see cref="SeriesLookupDialog"/>.</summary>
public enum SeriesLookupChoice
{
    Linked,
    Skip,
    Ignore,
    Stop,
}

/// <summary>
/// "Find new series" asks here about a folder it couldn't match. The top says what the folder probably is
/// ("We think this is …", the selected candidate) with "Yes, link it"; if that's wrong, enter the exact IMDb id or
/// search by title and year. Only series can be linked; a movie is shown as such. Closing the window counts as Skip.
/// <c>note</c> ("" for none) says why the folder couldn't be matched (ActivatorUtilities can't pass null).
/// </summary>
public partial class SeriesLookupDialog : AsyncForm
{
    public override string HelpTopic => "local-series";

    private readonly NewSeriesController _controller;
    private readonly string _position;
    private readonly string _note;
    private NewSeriesFolder _folder;
    private CancellationTokenSource? _detailsCts;

    public SeriesLookupDialog(NewSeriesController controller, NewSeriesFolder folder, string position, string note)
    {
        _controller = controller;
        _folder = folder;
        _position = position;
        _note = note;
        InitializeComponent();
        // Columns come from the row type; set here because the WinForms designer writes AutoGenerateColumns = false.
        dgvCandidates.AutoGenerateColumns = true;
        ShowFolder();
        ShowGuess();
    }

    public SeriesLookupChoice Choice { get; private set; } = SeriesLookupChoice.Skip;

    /// <summary>The title linked, when <see cref="Choice"/> is Linked.</summary>
    public Item? LinkedItem { get; private set; }

    /// <summary>The folder's path now (it changes when the IMDb id is removed from its name).</summary>
    public string FolderPath => _folder.Path;

    private CandidateRow? Selected => dgvCandidates.CurrentItem() as CandidateRow;

    private void ShowFolder()
    {
        Text = $"Find new series – folder {_position} that couldn't be matched automatically";
        lblFolder.Text = $"Folder:  {_folder.Name}{Environment.NewLine}in {Path.GetDirectoryName(_folder.Path)}" +
                         (_note.Length == 0 ? "" : $"   ({_note})");
        btnRemoveTag.Visible = _folder.Guess.ImdbId is not null;
        btnRemoveTag.Text = $"Remove {_folder.Guess.ImdbId} from the folder name…";
        txtQuery.Text = _folder.Guess.ImdbId ?? _folder.Guess.Title;
        txtYear.Text = _folder.Guess.ImdbId is null ? _folder.Guess.Year?.ToString() ?? "" : "";
    }

    /// <summary>The top line: what the selected candidate is, or that we don't know.</summary>
    private void ShowGuess()
    {
        if (Selected is { Candidate: var c })
        {
            var isSeries = string.Equals(c.Type, "series", StringComparison.OrdinalIgnoreCase);
            lblGuess.Text = isSeries
                ? $"We think this is:  {c.Title} ({c.Year}), {c.ImdbId}"
                : $"{c.ImdbId} is {c.Title} ({c.Year}), a {c.Type}, not a series. Only series are linked here; " +
                  "move it to the movies folder, or choose Don't ask again.";
            btnLink.Enabled = isSeries;
        }
        else
        {
            lblGuess.Text = "We don't know what this is. Enter the IMDb id, or search by title and year below.";
            btnLink.Enabled = false;
        }
    }

    private async void SeriesLookupDialog_Load(object sender, EventArgs e) => await SearchAsync();

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

        var query = txtQuery.Text.Trim();
        if (query.Length == 0) return;
        await RunAsync("Searching…", async ct =>
        {
            var result = await _controller.SearchAsync(query, year, ct);
            bindingSource.SetRows(result.Candidates.Select(CandidateRow.From));
            if (dgvCandidates.Columns[nameof(CandidateRow.Title)] is { } title) title.FillWeight = 300;
            var status = result.Candidates.Count switch
            {
                0 => "No series found. Use \"Wrong? Enter the IMDb id…\", or try another title or no year.",
                1 => "One match; it is shown at the top.",
                var n => $"{n} matches; the first is shown at the top. Click another row if that one is wrong.",
            };
            ShowStatus(result.Note is null ? status : $"{status}  ({result.Note})");
            if (result.Candidates.Count == 0) details.Clear();
            ShowGuess();
        });
    }

    /// <summary>Links the folder to exactly this IMDb id (pasting an IMDb link works too).</summary>
    private async void btnEnterId_Click(object sender, EventArgs e)
    {
        var input = Microsoft.VisualBasic.Interaction.InputBox(
            $"The IMDb id of what is in{Environment.NewLine}{_folder.Name}{Environment.NewLine}{Environment.NewLine}" +
            "e.g. tt17505010, or paste the IMDb page's address.",
            "Enter the IMDb id", "");
        if (input.Trim().Length == 0) return;
        if (Regex.Match(input, @"tt\d{7,9}", RegexOptions.IgnoreCase) is not { Success: true } id)
        {
            MessageBox.Show(this, $"\"{input}\" has no IMDb id (tt followed by 7–9 digits).", "Enter the IMDb id");
            return;
        }

        txtQuery.Text = id.Value.ToLowerInvariant();
        txtYear.Clear();
        await SearchAsync();
    }

    /// <summary>The folder name's IMDb id is wrong: rename the folder without it, then search by its title.</summary>
    private async void btnRemoveTag_Click(object sender, EventArgs e)
    {
        var newName = SeriesFolderName.WithoutImdbTag(_folder.Name);
        if (MessageBox.Show(this,
                $"Rename the folder{Environment.NewLine}{_folder.Name}{Environment.NewLine}to{Environment.NewLine}{newName}?",
                "Remove the IMDb id from the folder name", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
            != DialogResult.Yes)
            return;

        var renamed = false;
        await RunAsync("Renaming folder…", async ct =>
        {
            _folder = await _controller.RemoveImdbTagAsync(_folder, ct);
            renamed = true;
        });
        if (!renamed) return;

        ShowFolder();
        await SearchAsync();
    }

    private void dgvCandidates_SelectionChanged(object sender, EventArgs e)
    {
        ShowGuess();
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
            var item = await _controller.GetDetailsAsync(candidate, ct);
            if (ct.IsCancellationRequested || item is null) return;
            details.ShowItem(item);

            var poster = await _controller.GetPosterAsync(item, ct);
            if (ct.IsCancellationRequested || poster is null) return;
            using var stream = new MemoryStream(poster);
            using var image = Image.FromStream(stream);
            details.SetPoster(new Bitmap(image));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception) when (IsGone)
        {
            // Closed while the details were loading.
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
            var item = await _controller.GetDetailsAsync(row.Candidate, ct)
                       ?? throw new InvalidOperationException($"No details found for {row.ImdbId}.");

            var outcome = await _controller.LinkAsync(_folder, item, replaceExisting: false, ct);
            if (outcome.ConflictFolder is { } other)
            {
                var answer = MessageBox.Show(this,
                    $"{item.Title} is already linked to{Environment.NewLine}{other}{Environment.NewLine}{Environment.NewLine}" +
                    $"Link it to this folder instead?{Environment.NewLine}{_folder.Path}",
                    "Already linked", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes) return;
                outcome = await _controller.LinkAsync(_folder, item, replaceExisting: true, ct);
            }

            if (outcome.Linked)
            {
                LinkedItem = item;
                Finish(SeriesLookupChoice.Linked);
            }
        });
    }

    private void btnImdb_Click(object sender, EventArgs e)
    {
        if (Selected is { } row)
            Process.Start(new ProcessStartInfo($"https://www.imdb.com/title/{row.ImdbId}/") { UseShellExecute = true });
    }

    private void btnOpenFolder_Click(object sender, EventArgs e) => Explorer.Show(_folder.Path);

    private void btnSkip_Click(object sender, EventArgs e) => Finish(SeriesLookupChoice.Skip);

    private async void btnIgnore_Click(object sender, EventArgs e) =>
        await RunAsync("Saving…", async ct =>
        {
            await _controller.IgnoreAsync(_folder, ct);
            Finish(SeriesLookupChoice.Ignore);
        });

    private void btnStop_Click(object sender, EventArgs e) => Finish(SeriesLookupChoice.Stop);

    private void Finish(SeriesLookupChoice choice)
    {
        Choice = choice;
        DialogResult = choice == SeriesLookupChoice.Linked ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }

    private void dgvCandidates_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => btnLink.PerformClick();

    protected override void OnBusyChanged(bool busy)
    {
        if (IsGone) return;
        try
        {
            base.OnBusyChanged(busy);
            guessBar.Enabled = searchBar.Enabled = actions.Enabled = !busy;
            progress.Visible = busy;
        }
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException or NullReferenceException)
        {
            // Closed (Skip, Stop, X) while a search or link was still running.
        }
    }

    protected override void DisplayStatus(string text)
    {
        if (!IsGone) lblStatus.Text = text;
    }

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
