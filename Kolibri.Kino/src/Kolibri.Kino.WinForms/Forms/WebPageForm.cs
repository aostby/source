using System.Diagnostics;
using System.Text.RegularExpressions;
using Kolibri.Kino.WinForms.Controls;
using Microsoft.Web.WebView2.Core;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// A web page inside Kino (WebView2, Edge's engine, as SilverScreen's browser windows), e.g. IMDb's search.
/// Links that open a new window stay in this one. With <see cref="Show(Form, string, Action{string}?)"/>'s
/// <c>useImdbId</c>, a button takes the IMDb id of the page shown (…/title/tt1234567/) back to the window that asked.
/// </summary>
public partial class WebPageForm : Form
{
    private readonly Action<string>? _useImdbId;
    private readonly string _startUrl;
    private string? _imdbId;

    private WebPageForm(string url, Action<string>? useImdbId)
    {
        _startUrl = url;
        _useImdbId = useImdbId;
        InitializeComponent();
        AppIcon.Apply(this);
        btnUseId.Visible = useImdbId is not null;
        txtAddress.Text = url;
    }

    /// <summary>
    /// Opens <paramref name="url"/>: in the main window, or, from a modal dialog (which keeps the main window disabled),
    /// as a window of its own.
    /// </summary>
    public static WebPageForm Show(Form from, string url, Action<string>? useImdbId = null)
    {
        var page = new WebPageForm(url, useImdbId);
        if (!from.Modal && from.Host() is not null)
        {
            page.ShowFrom(from);
        }
        else
        {
            page.StartPosition = FormStartPosition.CenterParent;
            page.Show(from);
        }
        return page;
    }

    private async void WebPageForm_Load(object sender, EventArgs e)
    {
        try
        {
            // Its data (cookies, cache) under %LOCALAPPDATA%, not next to the program, which may not be writable.
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kolibri.Kino", "WebView2");
            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: folder);
            await browser.EnsureCoreWebView2Async(environment);
            browser.CoreWebView2.NewWindowRequested += (_, args) =>
            {
                args.Handled = true;
                browser.CoreWebView2.Navigate(args.Uri);
            };
            browser.CoreWebView2.DocumentTitleChanged += (_, _) => Text = browser.CoreWebView2.DocumentTitle;
            browser.CoreWebView2.Navigate(_startUrl);
        }
        catch (Exception ex) when (ex is WebView2RuntimeNotFoundException or InvalidOperationException or ArgumentException
                                       or System.Runtime.InteropServices.COMException)
        {
            // No WebView2 runtime (it comes with Windows 11 and Edge): use the default browser instead.
            OpenInBrowser(_startUrl);
            Close();
        }
    }

    private void browser_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        var url = browser.Source?.AbsoluteUri ?? "";
        txtAddress.Text = url;
        var match = ImdbTitle().Match(url);
        _imdbId = match.Success ? match.Groups[1].Value.ToLowerInvariant() : null;
        btnUseId.Enabled = _imdbId is not null;
        btnUseId.Text = _imdbId is null ? "Use this movie" : $"Use {_imdbId}";
        btnBack.Enabled = browser.CanGoBack;
        btnForward.Enabled = browser.CanGoForward;
    }

    private void btnBack_Click(object sender, EventArgs e) => browser.GoBack();

    private void btnForward_Click(object sender, EventArgs e) => browser.GoForward();

    private void btnBrowser_Click(object sender, EventArgs e) => OpenInBrowser(browser.Source?.AbsoluteUri ?? _startUrl);

    /// <summary>Gives the IMDb id back to the window that opened this one, and closes.</summary>
    private void btnUseId_Click(object sender, EventArgs e)
    {
        if (_imdbId is null || _useImdbId is null) return;
        _useImdbId(_imdbId);
        Close();
    }

    private static void OpenInBrowser(string url) => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

    [GeneratedRegex(@"imdb\.com/(?:[a-z]{2}/)?title/(tt\d{7,9})", RegexOptions.IgnoreCase)]
    private static partial Regex ImdbTitle();
}
