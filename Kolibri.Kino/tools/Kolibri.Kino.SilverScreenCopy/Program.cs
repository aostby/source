using System.Diagnostics;
using Kolibri.Kino.SilverScreenCopy;
using LiteDB;
using MySqlConnector;

// Copies the SilverScreen library (SilverScreen.db, .imgdb, .logdb) to the MariaDB database "silverscreen", one table
// per LiteDB collection with the same name. Reads a copy of the files, never the library itself.
//
//   dotnet run --project tools/Kolibri.Kino.SilverScreenCopy                       # everything
//   dotnet run --project tools/Kolibri.Kino.SilverScreenCopy -- --only Item,Episode
//   dotnet run --project tools/Kolibri.Kino.SilverScreenCopy -- --source D:\Lib --user asoes
//   … -- --replace     also replace tables KMDb has changed since the last copy (they are skipped otherwise)
//
// The image table (Image), after it is in MariaDB:
//   … -- images classify                                  which title and type each image is (from keys and the library)
//   … -- images kodi [--kodi \\HTPC\Shared\Kodi_Cache]    add the art of Kodi library exports for the titles in Item
//
// The database connection is DefaultConnection in the library's settings (the user's, --user), with the database
// changed to --database (default silverscreen); SILVERSCREEN_CONNECTION overrides it. It is never printed.

var source = Arg("--source") ?? @"C:\TEMP\SilverScreen";
var work = Arg("--work") ?? @"E:\DEV\tmp\silverscreen-copy";
var user = Arg("--user") ?? Environment.UserName;
var database = Arg("--database") ?? "silverscreen";
var replace = args.Contains("--replace");
var only = Arg("--only")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
var imagesCommand = args.Length >= 2 && args[0] == "images" ? args[1] : null;

if (args.Contains("--help") || args.Contains("-h") || (args.Length > 0 && args[0] == "images" && imagesCommand is not ("classify" or "kodi")))
{
    Console.WriteLine("""
        Copy:    [--source <folder>] [--work <folder>] [--user <settings user>] [--database <name>] [--only <Collection,...>] [--replace]
        Images:  images classify
                 images kodi [--kodi <folder with Kodi library exports>]
        """);
    return 0;
}

// The copy: all three files, or (for the image commands) only the settings.
var files = imagesCommand is null ? new[] { "SilverScreen.db", "SilverScreen.imgdb", "SilverScreen.logdb" } : ["SilverScreen.db"];
Directory.CreateDirectory(work);
Console.WriteLine($"Copying {string.Join(", ", files)} from {source} to {work} (the tool reads the copy) …");
foreach (var name in files)
{
    var stem = Path.GetFileNameWithoutExtension(name);
    foreach (var old in Directory.EnumerateFiles(work, stem + "*")) File.Delete(old);
    foreach (var path in Directory.EnumerateFiles(source, stem + "*"))
    {
        var fileName = Path.GetFileName(path);
        if (fileName == name || fileName.StartsWith(stem + "-log", StringComparison.OrdinalIgnoreCase))
            File.Copy(path, Path.Combine(work, fileName), overwrite: true);
    }
}

string connectionString;
using (var settings = Open(Path.Combine(work, "SilverScreen.db")))
{
    var configured = Environment.GetEnvironmentVariable("SILVERSCREEN_CONNECTION");
    if (string.IsNullOrWhiteSpace(configured))
    {
        var doc = settings.GetCollection("UserSettings").FindById(user)
            ?? throw new InvalidOperationException($"No settings for user '{user}' in the library (use --user).");
        configured = doc["DefaultConnection"].AsString;
        if (string.IsNullOrWhiteSpace(configured))
            throw new InvalidOperationException($"The settings for '{user}' have no DefaultConnection.");
    }
    var builder = new MySqlConnectionStringBuilder(configured)
    {
        Database = database,
        ConnectionTimeout = 15,
        DefaultCommandTimeout = 600,
    };
    connectionString = builder.ConnectionString;
    Console.WriteLine($"Database: {builder.Server}/{builder.Database} as {builder.UserID}");
}

var total = Stopwatch.StartNew();
await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
long packet;
await using (var cmd = connection.CreateCommand())
{
    cmd.CommandText = "SELECT @@max_allowed_packet";
    packet = Convert.ToInt64(await cmd.ExecuteScalarAsync());
}
// Leave room for the SQL text and escaping.
var maxBatch = (int)Math.Min(16L * 1024 * 1024, packet / 3);

switch (imagesCommand)
{
    case "classify":
        Console.WriteLine("Classifying the images:");
        await new ImageClassifier(connection, Console.Out).RunAsync();
        break;

    case "kodi":
        var kodi = Arg("--kodi") ?? @"\\HTPC\Shared\Kodi_Cache";
        Console.WriteLine($"Kodi library exports in {kodi}:");
        await new KodiImport(connection, Console.Out) { MaxBatchBytes = maxBatch }.RunAsync(kodi);
        break;

    default:
        var copier = new Copier(connection, Console.Out) { MaxBatchBytes = maxBatch, Replace = replace };
        await copier.EnsureStatusTableAsync();
        var copied = 0;
        var imagesCopied = false;
        foreach (var name in files)
        {
            var path = Path.Combine(work, name);
            if (!File.Exists(path)) { Console.WriteLine($"{name}: not found, skipped"); continue; }
            using var db = Open(path);
            Console.WriteLine($"{name}:");
            foreach (var collection in db.GetCollectionNames().Order(StringComparer.OrdinalIgnoreCase))
            {
                if (only is not null && !only.Contains(collection)) continue;
                // Kino's bookkeeping for KMDb (changes waiting to be sent, when KMDb was last read) belongs to each LiteDB file.
                if (collection is "KinoPendingSync" or "KinoSyncState") continue;
                var rows = await copier.CopyAsync(db, name, collection);
                if (collection == ImageTable.Name && rows > 0) imagesCopied = true;
                copied++;
            }
        }
        if (imagesCopied)
        {
            // A fresh Image table has only _id/Added/Data: say again which title and type each image is.
            Console.WriteLine("Classifying the images:");
            await new ImageClassifier(connection, Console.Out).RunAsync();
        }
        Console.WriteLine($"Done: {copied} collections.");
        break;
}
Console.WriteLine($"Time: {total.Elapsed.TotalMinutes:n1} min.");
return 0;

string? Arg(string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

// The copy is ours, so it is opened for writing: LiteDB may need to merge its -log file first.
static LiteDatabase Open(string path) => new(new ConnectionString { Filename = path, Connection = ConnectionType.Direct });
