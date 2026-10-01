using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using LiteDB;

// Copies an image cache (SilverScreen.imgdb) into a new file with the fixed layout:
//   collection "Image", _id = key string, Data = JPEG/PNG bytes.
// Old "ImageBase" documents (_id = randomized GetHashCode, base64 BMP) are de-duplicated by their ImdbId field.
// The source file is opened read-only and never changed.
//
// Usage: Kolibri.Kino.ImageDbMigrator <source.imgdb> [<target.imgdb>]

if (args.Length is < 1 or > 2)
{
    Console.Error.WriteLine("Usage: Kolibri.Kino.ImageDbMigrator <source.imgdb> [<target.imgdb>]");
    return 2;
}

var source = Path.GetFullPath(args[0]);
var target = Path.GetFullPath(args.Length == 2 ? args[1] : Path.ChangeExtension(source, ".migrated.imgdb"));

if (!File.Exists(source)) { Console.Error.WriteLine($"Source not found: {source}"); return 1; }
if (File.Exists(target)) { Console.Error.WriteLine($"Target already exists, not overwriting: {target}"); return 1; }

Console.WriteLine($"Source: {source} ({Size(new FileInfo(source).Length)})");
Console.WriteLine($"Target: {target}");

var clock = Stopwatch.StartNew();
int read = 0, written = 0, duplicates = 0, unreadable = 0;
var seen = new HashSet<string>(StringComparer.Ordinal);

using (var from = new LiteDatabase(new ConnectionString { Filename = source, ReadOnly = true, Connection = ConnectionType.Shared }))
using (var to = new LiteDatabase(new ConnectionString { Filename = target, Connection = ConnectionType.Direct }))
{
    var images = to.GetCollection("Image");

    // 1. Documents already in the new layout (written by fixed SilverScreen or Kino) are copied as-is.
    foreach (var doc in from.GetCollection("Image").FindAll())
    {
        read++;
        if (seen.Add(doc["_id"].AsString)) { images.Insert(doc); written++; }
    }

    // 2. Old documents: one per key, BMP converted to JPEG (or PNG when it has transparency).
    foreach (var doc in from.GetCollection("ImageBase").FindAll())
    {
        read++;
        var key = doc["ImdbId"].IsString ? doc["ImdbId"].AsString : null;

        if (string.IsNullOrEmpty(key) || seen.Contains(key)) { duplicates++; }
        else if (TryConvert(doc["Base64"], out var data))
        {
            images.Insert(new BsonDocument { ["_id"] = key, ["Data"] = data, ["Added"] = DateTime.UtcNow });
            seen.Add(key);
            written++;
        }
        else { unreadable++; }

        if (read % 500 == 0)
            Console.WriteLine($"  {read} read, {written} kept, {duplicates} duplicates, {unreadable} unreadable ({clock.Elapsed:mm\\:ss})");
    }

    to.Checkpoint();
}

Console.WriteLine();
Console.WriteLine($"Done in {clock.Elapsed:mm\\:ss}: {read} read, {written} kept, {duplicates} duplicates skipped, {unreadable} unreadable.");
Console.WriteLine($"Size: {Size(new FileInfo(source).Length)} -> {Size(new FileInfo(target).Length)}");
Console.WriteLine();
Console.WriteLine("Nothing was changed in the source. To switch over, close SilverScreen and Kino, then rename:");
Console.WriteLine($"  {Path.GetFileName(source)} -> {Path.GetFileName(source)}.bak");
Console.WriteLine($"  {Path.GetFileName(target)} -> {Path.GetFileName(source)}");
return 0;

static bool TryConvert(BsonValue base64, out byte[] data)
{
    data = [];
    if (!base64.IsString) return false;
    try
    {
        // The old writer used MemoryStream.GetBuffer(), so the bytes carry trailing padding; GDI+ ignores it.
        using var input = new MemoryStream(Convert.FromBase64String(base64.AsString));
        using var image = Image.FromStream(input);
        if (image.Width <= 1 || image.Height <= 1) return false;

        using var output = new MemoryStream();
        if (Image.IsAlphaPixelFormat(image.PixelFormat))
        {
            image.Save(output, ImageFormat.Png);
        }
        else
        {
            var jpeg = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            using var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
            image.Save(output, jpeg, parameters);
        }
        data = output.ToArray();
        return true;
    }
    catch (Exception e) when (e is FormatException or ArgumentException or OutOfMemoryException or ExternalException)
    {
        return false;
    }
}

static string Size(long bytes) => bytes >= 1L << 30 ? $"{bytes / (double)(1L << 30):F2} GB" : $"{bytes / (double)(1 << 20):F1} MB";
