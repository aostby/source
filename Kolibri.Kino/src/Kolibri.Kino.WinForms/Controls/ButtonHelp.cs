namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// What each button does, shown when the mouse rests on it: a sentence or two from the help (F1 has the rest).
/// </summary>
internal static class ButtonHelp
{
    /// <summary>Sets the hover text of each control. One ToolTip per form, disposed with it.</summary>
    public static void Describe(this Form form, params (Control Control, string Text)[] tips)
    {
        var toolTip = new ToolTip { AutoPopDelay = 20000, InitialDelay = 500, ReshowDelay = 100 };
        form.Disposed += (_, _) => toolTip.Dispose();
        foreach (var (control, text) in tips) toolTip.SetToolTip(control, text);
    }
}
