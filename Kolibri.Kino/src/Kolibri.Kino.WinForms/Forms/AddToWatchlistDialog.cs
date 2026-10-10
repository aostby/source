using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Pick (or type) a list and add one title to it: to the Kino watchlist, the Plex playlist of that name, or both.
/// The dialog says where a list of that name already is (SilverScreen joined lists of the same name without saying),
/// and the boxes start ticked where it is. DialogResult.OK when added; see <see cref="ResultMessage"/>.
/// </summary>
public partial class AddToWatchlistDialog : AsyncForm
{
    public override string HelpTopic => "watchlists";

    private readonly WatchlistController _watchlists;
    private readonly string _imdbId;
    private IReadOnlyList<WatchlistChoice> _choices = [];
    private string? _plexError;

    public AddToWatchlistDialog(WatchlistController watchlists, string imdbId, string title)
    {
        _watchlists = watchlists;
        _imdbId = imdbId;
        InitializeComponent();
        this.Describe(
            (chkKino, "Kino's own watchlist of this name (Library → Watchlists)."),
            (chkPlex, "The Plex playlist of this name on your Plex server. Only titles that are on Plex can be added."));
        lblMovie.Text = $"Add {title} to:";
        chkPlex.Visible = watchlists.PlexConfigured;
    }

    public string ResultMessage { get; private set; } = string.Empty;

    private async void AddToWatchlistDialog_Load(object sender, EventArgs e) =>
        await RunAsync("Loading lists…", async ct =>
        {
            var lists = await _watchlists.GetListChoicesAsync(ct);
            _choices = lists.Choices;
            _plexError = lists.PlexError;
            cboList.Items.AddRange([.. _choices]);
            cboList.Text = _watchlists.FavoriteList;
            ShowWhere();
        });

    private void cboList_TextChanged(object sender, EventArgs e) => ShowWhere();

    /// <summary>Where a list of the typed name is, and the boxes ticked to match (both, one, or a new Kino watchlist).</summary>
    private void ShowWhere()
    {
        var name = cboList.Text.Trim();
        var choice = _choices.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        var plex = chkPlex.Visible;

        lblWhere.Text = name.Length == 0 ? "Choose or type a list name."
            : choice is null ? $"\"{name}\" is a new list." + (plex ? " Tick where to make it." : "")
            : choice is { InKino: true, OnPlex: true } ? $"\"{name}\" is a Kino watchlist and a Plex playlist."
            : choice.InKino ? $"\"{name}\" is a Kino watchlist only." + (plex ? " Tick Plex playlist to make it on Plex too." : "")
            : $"\"{name}\" is a Plex playlist only. Tick Kino watchlist to make it in Kino too.";
        if (_plexError is not null) lblWhere.Text += $" (Plex's playlists couldn't be read: {_plexError})";

        chkKino.Checked = choice is null || choice.InKino;
        chkPlex.Checked = plex && choice is { OnPlex: true };
        chkKino.Text = choice is { InKino: false } ? "Kino watchlist (new)" : "Kino watchlist";
        chkPlex.Text = choice is { OnPlex: true } ? "Plex playlist" : "Plex playlist (new)";
        UpdateAddButton();
    }

    private void Target_CheckedChanged(object sender, EventArgs e) => UpdateAddButton();

    private void UpdateAddButton() =>
        btnAdd.Enabled = cboList.Text.Trim().Length > 0 && (chkKino.Checked || (chkPlex.Visible && chkPlex.Checked));

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        var list = cboList.Text.Trim();
        var toKino = chkKino.Checked;
        var toPlex = chkPlex.Visible && chkPlex.Checked;
        if (list.Length == 0 || (!toKino && !toPlex)) return;

        await RunAsync("Adding…", async ct =>
        {
            var result = await _watchlists.AddToListsAsync(_imdbId, list, toKino, toPlex, ct);
            ResultMessage = Describe(result, list);
            if (result.PlexError is not null || result.Plex is { NotOnServer.Count: > 0 })
                MessageBox.Show(this, ResultMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        });
    }

    private static string Describe(AddToListsResult r, string list)
    {
        var parts = new List<string>();
        if (r.Kino is { } kino)
            parts.Add(kino.MovedFrom is null
                ? $"Added {r.Title} to the Kino watchlist {list}"
                : $"Moved {r.Title} from {kino.MovedFrom} to the Kino watchlist {list} (a movie is on one watchlist at a time)");
        if (r.PlexError is not null)
            parts.Add($"Plex: {r.PlexError}");
        else if (r.Plex is { } plex)
        {
            var playlist = $"the Plex playlist {list}" + (plex.Created ? " (new)" : "");
            if (plex.NotOnServer.Count > 0) parts.Add($"{r.Title} isn't on Plex, so it wasn't added to {playlist}");
            else if (plex.AlreadyThere > 0) parts.Add($"{r.Title} was already in {playlist}");
            else if (parts.Count > 0) parts[^1] += $" and to {playlist}";
            else parts.Add($"Added {r.Title} to {playlist}");
        }
        return string.Join("; ", parts) + ".";
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        cboList.Enabled = chkKino.Enabled = chkPlex.Enabled = !busy;
        if (busy) btnAdd.Enabled = false;
        else UpdateAddButton();
    }
}
