using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kolibri.Etiketter;

/// <summary>
/// Skrifter som er bygget inn i programmet (C64 Pro og C64 Pro Mono fra Style, style64.org), slik at etiketter med
/// Commodore 64-skriften ser riktige ut også på PC-er der skriften ikke er installert.
/// Skriftfilene ligger ikke i git: de bygges inn fra den installerte skriften på PC-en som bygger programmet
/// (se Kolibri.Etiketter.csproj), siden lisensen ikke tillater at de legges ut til nedlasting.
/// </summary>
public static class EmbeddedFonts
{
    /// <summary>Commodore 64-skriften, standard på nye etiketter.</summary>
    public const string C64 = "C64 Pro";

    private const string ResourcePrefix = "Kolibri.Etiketter.Fonts.";
    private static readonly Lazy<PrivateFontCollection> Collection = new(Load);

    /// <summary>Den innebygde skriftfamilien med dette navnet, eller null.</summary>
    public static FontFamily? Find(string familyName) =>
        Collection.Value.Families.FirstOrDefault(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));

    /// <summary>Navnene på de innebygde skriftene.</summary>
    public static IEnumerable<string> Names => Collection.Value.Families.Select(f => f.Name);

    /// <summary>True dersom skriften er installert i Windows (ikke bare bygget inn).</summary>
    public static bool IsInstalled(string familyName)
    {
        using var installed = new InstalledFontCollection();
        return installed.Families.Any(f => f.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));
    }

    private static PrivateFontCollection Load()
    {
        var collection = new PrivateFontCollection();
        Assembly assembly = typeof(EmbeddedFonts).Assembly;
        foreach (string name in assembly.GetManifestResourceNames().Where(n => n.StartsWith(ResourcePrefix, StringComparison.Ordinal)))
        {
            try
            {
                using Stream? stream = assembly.GetManifestResourceStream(name);
                if (stream is null) continue;
                byte[] data = new byte[stream.Length];
                stream.ReadExactly(data);
                // GDI+ leser skriften fra minnet så lenge programmet kjører, så minnet frigjøres aldri.
                IntPtr memory = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, memory, data.Length);
                collection.AddMemoryFont(memory, data.Length);
            }
            catch (Exception)
            {
                // En skrift som ikke kan leses, hoppes over; da brukes systemets reserveskrift.
            }
        }
        return collection;
    }
}
