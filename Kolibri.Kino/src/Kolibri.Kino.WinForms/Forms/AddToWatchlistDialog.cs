using Kolibri.Kino.Controllers.Watchlists;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Pick (or type) a watchlist and add one movie to it. DialogResult.OK when added; see <see cref="ResultMessage"/>.
/// </summary>
public partial class AddToWatchlistDialog : AsyncForm
{
    public override string HelpTopic => "watchlists";

    private readonly WatchlistController _watchlists;
    private readonly string _imdbId;

    public AddToWatchlistDialog(WatchlistController watchlists, string imdbId, string title)
    {
        _watchlists = watchlists;
        _imdbId = imdbId;
        InitializeComponent();
        lblMovie.Text = $"Add {title} to watchlist:";
    }

    public string ResultMessage { get; private set; } = string.Empty;

    private async void AddToWatchlistDialog_Load(object sender, EventArgs e) =>
        await RunAsync("Loading lists…", async ct =>
        {
            cboList.Items.AddRange([.. await _watchlists.GetListNamesAsync(ct)]);
            cboList.Text = _watchlists.FavoriteList;
        });

    private async void btnAdd_Click(object sender, EventArgs e)
    {
        var list = cboList.Text.Trim();
        if (list.Length == 0) return;

        await RunAsync("Adding…", async ct =>
        {
            var result = await _watchlists.AddAsync(_imdbId, list, ct);
            ResultMessage = result.MovedFrom is null
                ? $"Added {result.Title} to {list}."
                : $"Moved {result.Title} from {result.MovedFrom} to {list} (a movie is on one watchlist at a time).";
            DialogResult = DialogResult.OK;
            Close();
        });
    }

    protected override void OnBusyChanged(bool busy)
    {
        base.OnBusyChanged(busy);
        cboList.Enabled = btnAdd.Enabled = !busy;
    }
}
