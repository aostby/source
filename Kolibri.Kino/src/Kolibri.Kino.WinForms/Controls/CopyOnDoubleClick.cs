using System.Runtime.InteropServices;
using Kolibri.Kino.WinForms.Forms;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Double-clicking a value in a details pane (title, IMDb id, director, file path …) copies it to the clipboard,
/// says so in a small popup by the mouse and in the window's status bar (not logged: it's not an event worth
/// keeping).
/// </summary>
internal static class CopyOnDoubleClick
{
    private static readonly ToolTip Popup = new() { UseAnimation = false, UseFading = false };
    private static readonly ToolTip Hint = new();

    /// <summary>
    /// Copies the label's text when it's double-clicked, or what <paramref name="value"/> gives (e.g. just the number
    /// of "IMDb 7.3"); <paramref name="name"/> (e.g. "IMDb id") is used in the message.
    /// </summary>
    public static void Enable(Label label, string? name = null, Func<string?>? value = null)
    {
        Hint.SetToolTip(label, "Double-click to copy");
        label.DoubleClick += (_, _) => Copy(label, value?.Invoke() ?? label.Text, name);
    }

    private static void Copy(Control control, string text, string? name)
    {
        var value = text.Trim();
        if (value.Length == 0) return;

        string message;
        try
        {
            Clipboard.SetText(value);
            var shown = value.ReplaceLineEndings(" ");
            if (shown.Length > 60) shown = shown[..57] + "…";
            message = name is null ? $"Copied to the clipboard: {shown}" : $"Copied to the clipboard: {name}: {shown}";
        }
        catch (ExternalException)
        {
            // Another program is holding the clipboard.
            message = "Couldn't copy: the clipboard is busy. Try again.";
        }

        var at = control.PointToClient(Cursor.Position);
        Popup.Show(message.StartsWith("Copied", StringComparison.Ordinal) ? "Copied" : message, control, at.X + 12, at.Y + 12, 1500);
        if (control.FindForm() is AsyncForm form) form.DisplayCopyStatus(message);
    }
}
