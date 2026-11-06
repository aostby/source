using System.Buffers.Binary;

namespace Kolibri.Kino.Core;

/// <summary>An image's type and size, read from its first bytes (no image library needed).</summary>
public static class ImageInfo
{
    /// <summary>The MIME type from the first bytes: image/jpeg, image/png, image/gif, image/webp, image/bmp, else application/octet-stream.</summary>
    public static string ContentType(ReadOnlySpan<byte> data) => data switch
    {
        [0xFF, 0xD8, ..] => "image/jpeg",
        [0x89, 0x50, 0x4E, 0x47, ..] => "image/png",
        [0x47, 0x49, 0x46, ..] => "image/gif",
        [0x52, 0x49, 0x46, 0x46, _, _, _, _, 0x57, 0x45, 0x42, 0x50, ..] => "image/webp",
        [0x42, 0x4D, ..] => "image/bmp",
        _ => "application/octet-stream",
    };

    /// <summary>Width and height in pixels, or null when the format isn't known or the header is cut off.</summary>
    public static (int Width, int Height)? Size(ReadOnlySpan<byte> d)
    {
        try
        {
            switch (ContentType(d))
            {
                case "image/png" when d.Length >= 24:
                    return (BinaryPrimitives.ReadInt32BigEndian(d[16..]), BinaryPrimitives.ReadInt32BigEndian(d[20..]));
                case "image/gif" when d.Length >= 10:
                    return (BinaryPrimitives.ReadUInt16LittleEndian(d[6..]), BinaryPrimitives.ReadUInt16LittleEndian(d[8..]));
                case "image/bmp" when d.Length >= 26:
                    return (BinaryPrimitives.ReadInt32LittleEndian(d[18..]), Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(d[22..])));
                case "image/webp" when d.Length >= 30:
                    return WebpSize(d);
                case "image/jpeg":
                    return JpegSize(d);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            // A header that ends early: size unknown.
        }
        return null;
    }

    /// <summary>"poster" for portrait images, "backdrop" for landscape ones: the guess when nothing else tells the type.</summary>
    public static string TypeFromShape(int width, int height) => height > width ? "poster" : "backdrop";

    private static (int, int)? WebpSize(ReadOnlySpan<byte> d)
    {
        var chunk = System.Text.Encoding.ASCII.GetString(d[12..16]);
        return chunk switch
        {
            "VP8 " => (BinaryPrimitives.ReadUInt16LittleEndian(d[26..]) & 0x3FFF, BinaryPrimitives.ReadUInt16LittleEndian(d[28..]) & 0x3FFF),
            "VP8L" => (1 + ((d[22] & 0x3F) << 8 | d[21]), 1 + ((d[24] & 0x0F) << 10 | d[23] << 2 | (d[22] & 0xC0) >> 6)),
            "VP8X" => (1 + (d[24] | d[25] << 8 | d[26] << 16), 1 + (d[27] | d[28] << 8 | d[29] << 16)),
            _ => null,
        };
    }

    private static (int, int)? JpegSize(ReadOnlySpan<byte> d)
    {
        var i = 2;
        while (i + 9 < d.Length)
        {
            if (d[i] != 0xFF) { i++; continue; }
            var marker = d[i + 1];
            if (marker is 0xD8 or 0x01 or (>= 0xD0 and <= 0xD7) or 0xFF) { i += marker == 0xFF ? 1 : 2; continue; }
            // Start of frame (baseline, progressive …), not DHT (C4), JPG (C8) or DAC (CC).
            if (marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC)
                return (BinaryPrimitives.ReadUInt16BigEndian(d[(i + 7)..]), BinaryPrimitives.ReadUInt16BigEndian(d[(i + 5)..]));
            i += 2 + BinaryPrimitives.ReadUInt16BigEndian(d[(i + 2)..]);
        }
        return null;
    }
}
