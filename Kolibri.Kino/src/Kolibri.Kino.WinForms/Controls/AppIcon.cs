namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Kino's icon (assets/kino.ico, embedded), for every window's title bar and taskbar button.
/// </summary>
internal static class AppIcon
{
    private static readonly Lazy<Icon?> Loaded = new(Load);

    /// <summary>The icon, or null if it can't be loaded (then the window keeps the default icon).</summary>
    public static Icon? Icon => Loaded.Value;

    /// <summary>Sets Kino's icon on <paramref name="form"/>.</summary>
    public static void Apply(Form form)
    {
        if (Icon is { } icon) form.Icon = icon;
    }

    private static Icon? Load()
    {
        try
        {
            using var stream = typeof(AppIcon).Assembly.GetManifestResourceStream("Kolibri.Kino.WinForms.kino.ico");
            return stream is null ? null : new Icon(stream);
        }
        catch (Exception ex) when (ex is ArgumentException or IOException)
        {
            return null;
        }
    }
}
