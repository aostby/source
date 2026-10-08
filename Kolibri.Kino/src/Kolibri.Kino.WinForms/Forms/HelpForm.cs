using System.Diagnostics;
using Kolibri.Kino.WinForms.Controls;

namespace Kolibri.Kino.WinForms.Forms;

/// <summary>
/// Kino's help (Help/help.html, embedded), opened with F1 or Help → Help at the part about the current window.
/// Links to web sites open in the default browser.
/// </summary>
public partial class HelpForm : Form
{
    private static readonly Lazy<string> Html = new(LoadHtml);
    private string _topic = StartTopic;

    /// <summary>The top of the help; also used for an unknown topic.</summary>
    public const string StartTopic = "start";

    public HelpForm()
    {
        InitializeComponent();
        AppIcon.Apply(this);
        browser.DocumentText = Html.Value;
    }

    /// <summary>
    /// Shows the help at <paramref name="topic"/> (an id in help.html): in the main window, or, from a modal dialog,
    /// which keeps the main window disabled, as a window of its own.
    /// </summary>
    public static void Show(Form from, string? topic)
    {
        if (!from.Modal && from.Host() is { } host)
        {
            var open = host.MdiChildren.OfType<HelpForm>().FirstOrDefault();
            if (open is null)
            {
                open = new HelpForm();
                open.ShowFrom(host);
            }
            else
            {
                if (open.WindowState == FormWindowState.Minimized) open.WindowState = FormWindowState.Normal;
                open.Activate();
            }
            open.GoTo(topic);
        }
        else
        {
            var help = new HelpForm { StartPosition = FormStartPosition.CenterParent };
            help.GoTo(topic);
            help.Show(from);
        }
    }

    public void GoTo(string? topic)
    {
        _topic = string.IsNullOrWhiteSpace(topic) ? StartTopic : topic;
        if (browser.ReadyState == WebBrowserReadyState.Complete) ScrollToTopic();
    }

    private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) => ScrollToTopic();

    private void ScrollToTopic() =>
        (browser.Document?.GetElementById(_topic) ?? browser.Document?.GetElementById(StartTopic))?.ScrollIntoView(true);

    /// <summary>Web links go to the default browser; links within the help (#topic) stay here.</summary>
    private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        if (e.Url is not { Scheme: "http" or "https" or "mailto" } url) return;
        e.Cancel = true;
        Process.Start(new ProcessStartInfo(url.AbsoluteUri) { UseShellExecute = true });
    }

    private static string LoadHtml()
    {
        using var stream = typeof(HelpForm).Assembly.GetManifestResourceStream("Kolibri.Kino.WinForms.help.html");
        if (stream is null) return "<p>The help is missing from this build.</p>";
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
