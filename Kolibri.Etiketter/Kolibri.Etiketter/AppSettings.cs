using System.Text.Json;

namespace Kolibri.Etiketter;

/// <summary>Innstillinger som huskes mellom hver gang programmet startes.</summary>
public sealed class AppSettings
{
    private static readonly string AppDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    private static readonly string Folder = Path.Combine(AppDataRoot, "Kolibri.Etiketter");

    /// <summary>Mappen som ble brukt før programmet het Kolibri.Etiketter.</summary>
    private static readonly string OldFolder = Path.Combine(AppDataRoot, "Etikettprogram");

    private static readonly string SettingsPath = Path.Combine(Folder, "innstillinger.json");

    /// <summary>Siste etikett lagres automatisk her når programmet lukkes.</summary>
    public static string LastSessionPath => Path.Combine(Folder, "siste.etikett");

    public string? PrinterName { get; set; }
    public float OffsetXMm { get; set; }
    public float OffsetYMm { get; set; }
    public bool PrintGuides { get; set; }

    /// <summary>Posisjoner som skal skrives ut på hvert ark (1–24). Standard er hele arket.</summary>
    public List<int>? SelectedPositions { get; set; } = Enumerable.Range(1, LabelSheet.LabelsPerSheet).ToList();

    /// <summary>Antall ark som skrives ut.</summary>
    public int SheetCount { get; set; } = 1;

    public static AppSettings Load()
    {
        MigrateOldFolder();
        try
        {
            if (File.Exists(SettingsPath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new AppSettings();
        }
        catch (Exception)
        {
            // Ødelagt fil: start med standardverdier.
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Folder);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception)
        {
            // Innstillinger er ikke kritiske.
        }
    }

    public static void EnsureFolder() => Directory.CreateDirectory(Folder);

    /// <summary>Kopierer innstillinger og siste etikett fra den gamle mappen første gang.</summary>
    private static void MigrateOldFolder()
    {
        try
        {
            if (Directory.Exists(Folder) || !Directory.Exists(OldFolder))
                return;
            Directory.CreateDirectory(Folder);
            foreach (string file in Directory.GetFiles(OldFolder))
                File.Copy(file, Path.Combine(Folder, Path.GetFileName(file)));
        }
        catch (Exception)
        {
            // Ikke kritisk: programmet starter da med standardverdier.
        }
    }
}
