using System.Text.Json;
using System.Text.Json.Nodes;
using Kolibri.Kino.Core;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Configuration;

namespace Kolibri.Kino.WinForms;

/// <summary>
/// Where the library database is, looked for as SilverScreen does:
/// <list type="number">
/// <item>the file chosen in Settings, kept per Windows user in <see cref="UserFile"/>, while its folder exists;</item>
/// <item>else Kino:LiteDbPath in appsettings.json, the default for a first start;</item>
/// <item>and if that database's user settings name another existing file (LiteDBFilePath), that one.</item>
/// </list>
/// A folder without a database is fine: a new, empty one is made there.
/// </summary>
internal static class DatabaseLocation
{
    public const string ConfigKey = KinoOptions.SectionName + ":" + nameof(KinoOptions.LiteDbPath);

    /// <summary>%LOCALAPPDATA%\Kolibri.Kino\usersettings.json: outside the program folder, so a new build or install keeps it.</summary>
    public static string UserFile { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kolibri.Kino", "usersettings.json");

    public static string Resolve(IConfiguration configuration)
    {
        if (ReadUserChoice() is { } chosen && Directory.Exists(Path.GetDirectoryName(chosen))) return chosen;

        var configured = configuration[ConfigKey] ?? string.Empty;
        var user = new KinoOptions { SettingsUser = configuration["Kino:SettingsUser"] ?? string.Empty }.ResolveSettingsUser();
        return LiteDbSettingsStore.ReadStoredDbPath(configured, user) is { } stored && File.Exists(stored) ? stored : configured;
    }

    /// <summary>Remembers <paramref name="dbPath"/> for the next start; other values in the file are kept.</summary>
    public static void SaveUserChoice(string dbPath)
    {
        var root = ReadUserFile() ?? new JsonObject();
        root[nameof(KinoOptions.LiteDbPath)] = dbPath;
        Directory.CreateDirectory(Path.GetDirectoryName(UserFile)!);
        File.WriteAllText(UserFile, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static string? ReadUserChoice() =>
        ReadUserFile()?[nameof(KinoOptions.LiteDbPath)] is JsonValue value && value.TryGetValue(out string? path)
            && !string.IsNullOrWhiteSpace(path)
            ? path
            : null;

    /// <summary>The user file, or null when it is missing or unreadable (then appsettings.json applies).</summary>
    private static JsonObject? ReadUserFile()
    {
        try
        {
            return File.Exists(UserFile) ? JsonNode.Parse(File.ReadAllText(UserFile)) as JsonObject : null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return null;
        }
    }
}
