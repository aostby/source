using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>A window with a status bar others can write to: Kino's MDI main window.</summary>
internal interface IStatusDisplay
{
    void SetStatus(string text);
}

/// <summary>
/// Opens windows inside Kino's MDI main window, so they stay within its frame.
/// Modal dialogs (ShowDialog) can't be MDI children and stay as they are.
/// </summary>
internal static class Mdi
{
    /// <summary>The MDI main window <paramref name="opener"/> is (or lives in), or null when it stands alone.</summary>
    public static Form? Host(this Form opener) => opener.IsMdiContainer ? opener : opener.MdiParent;

    /// <summary>
    /// Shows <paramref name="window"/> in the MDI main window of <paramref name="opener"/>;
    /// without one, as a window owned by <paramref name="opener"/>.
    /// </summary>
    public static void ShowFrom(this Form window, Form opener)
    {
        if (opener.Host() is { } host)
        {
            window.MdiParent = host;
            window.Show();
        }
        else
        {
            window.Show(opener);
        }
    }

    /// <summary>
    /// Shows <paramref name="text"/> in the status bar of the MDI main window <paramref name="form"/> lives in
    /// (any thread). Does nothing for a window outside it.
    /// </summary>
    public static void ShowMainStatus(this Form form, string text) => (form.Host() as IStatusDisplay)?.SetStatus(text);

    /// <summary>Brings the open <typeparamref name="T"/> to the front, or opens a new one.</summary>
    // ActivatorUtilities: the container doesn't track the form, so a closed window can be collected.
    public static T ShowSingle<T>(this Form opener, IServiceProvider services) where T : Form
    {
        if (opener.Host()?.MdiChildren.OfType<T>().FirstOrDefault() is { } open)
        {
            if (open.WindowState == FormWindowState.Minimized) open.WindowState = FormWindowState.Normal;
            open.Activate();
            return open;
        }

        var window = ActivatorUtilities.CreateInstance<T>(services);
        window.ShowFrom(opener);
        return window;
    }
}
