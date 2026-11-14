using System.Text.Json;
using System.Text.Json.Serialization;

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

    /// <summary>Skriveren som sist ble valgt.</summary>
    public string? PrinterName { get; set; }

    /// <summary>Justering per skriver, med skrivernavnet i Windows som nøkkel.</summary>
    public Dictionary<string, PrinterProfile> Printers { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Felles justering fra før hver skriver fikk sin egen. Flyttes til <see cref="Printers"/> ved lasting.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float OffsetXMm { get; set; }

    /// <inheritdoc cref="OffsetXMm"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float OffsetYMm { get; set; }

    public bool PrintGuides { get; set; }

    /// <summary>Etikettarket (<see cref="LabelSheet.Id"/>). Tomt betyr Lyreco 24 per ark, som før arkvalget kom.</summary>
    public string? SheetId { get; set; }

    /// <summary>Posisjoner (fra 1) fra før valget ble husket per ark. Flyttes til <see cref="SelectionsBySheet"/> ved lasting.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<int>? SelectedPositions { get; set; }

    /// <summary>Posisjoner som skal skrives ut på hvert ark (fra 1), per etikettark (<see cref="LabelSheet.Id"/>). Mangler arket, skrives hele arket ut.</summary>
    public Dictionary<string, List<int>> SelectionsBySheet { get; set; } = [];

    /// <summary>Antall ark som skrives ut.</summary>
    public int SheetCount { get; set; } = 1;

    public static AppSettings Load()
    {
        MigrateOldFolder();
        try
        {
            if (File.Exists(SettingsPath))
            {
                AppSettings settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new AppSettings();
                settings.MigrateGlobalOffset();
                settings.SelectionsBySheet ??= [];
                if (settings.SelectedPositions is { } old)
                {
                    settings.SelectionsBySheet.TryAdd(LabelSheet.FindById(settings.SheetId).Id, old);
                    settings.SelectedPositions = null;
                }
                return settings;
            }
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

    /// <summary>Justeringen for <paramref name="printerName"/>. Ukjente skrivere starter på 0.</summary>
    public PrinterProfile GetProfile(string? printerName) =>
        !string.IsNullOrEmpty(printerName) && Printers.TryGetValue(printerName, out PrinterProfile? profile) ? profile : new PrinterProfile();

    /// <summary>Lagrer justeringen for <paramref name="printerName"/>.</summary>
    public void SetOffset(string? printerName, float offsetXMm, float offsetYMm, bool calibrated = false)
    {
        if (string.IsNullOrEmpty(printerName))
            return;
        if (!Printers.TryGetValue(printerName, out PrinterProfile? profile))
        {
            if (offsetXMm == 0f && offsetYMm == 0f && !calibrated)
                return;
            profile = new PrinterProfile();
            Printers[printerName] = profile;
        }
        profile.OffsetXMm = offsetXMm;
        profile.OffsetYMm = offsetYMm;
        if (calibrated)
            profile.CalibratedAt = DateTime.Now;
    }

    /// <summary>
    /// Før hadde programmet én felles justering. Den gjaldt skriveren som sist ble brukt,
    /// så den flyttes dit (eller til standardskriveren om ingen er brukt ennå).
    /// </summary>
    private void MigrateGlobalOffset()
    {
        Printers = new Dictionary<string, PrinterProfile>(Printers ?? [], StringComparer.OrdinalIgnoreCase);
        if (OffsetXMm == 0f && OffsetYMm == 0f)
            return;
        string? printer = string.IsNullOrEmpty(PrinterName) ? PrinterInfo.DefaultPrinterName : PrinterName;
        if (printer is not null && !Printers.ContainsKey(printer))
            Printers[printer] = new PrinterProfile { OffsetXMm = OffsetXMm, OffsetYMm = OffsetYMm };
        OffsetXMm = 0f;
        OffsetYMm = 0f;
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
