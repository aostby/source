#:property TargetFramework=net10.0-windows
#:property UseWindowsForms=true
#:property PublishAot=false
#:property PublishTrimmed=false
// Builds kino.ico from kino.svg:   dotnet run make-icon.cs   (run in this folder)
//
// kino.svg is the "Movie" icon from https://www.iconpacks.net/free-icon/movie-850.html (free for commercial use,
// no attribution required). It is one rectangle and one path, so it is drawn with System.Drawing directly
// (no SVG library) at the sizes Windows uses, and packed as PNG entries into one .ico.
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

int[] sizes = [16, 20, 24, 32, 40, 48, 64, 128, 256];
var folder = Directory.GetCurrentDirectory();
var svg = XDocument.Load(Path.Combine(folder, "kino.svg"));
XNamespace ns = "http://www.w3.org/2000/svg";

// The group transform: translate(tx ty) scale(s).
var group = svg.Descendants(ns + "g").First();
var numbers = Regex.Matches(group.Attribute("transform")!.Value, @"-?\d+(\.\d+)?").Select(m => float.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
var (tx, ty, scale) = (numbers[0], numbers[1], numbers[2]);

var rect = svg.Descendants(ns + "rect").First();
var rectBounds = new RectangleF(F(rect, "x"), F(rect, "y"), F(rect, "width"), F(rect, "height"));
var pathElement = svg.Descendants(ns + "path").First();
var path = ParsePath(pathElement.Attribute("d")!.Value);

var pngs = sizes.Select(size => (Size: size, Png: Render(size))).ToList();
WriteIco(Path.Combine(folder, "kino.ico"), pngs);
Console.WriteLine($"kino.ico: {string.Join(", ", sizes)} px");

byte[] Render(int size)
{
    using var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
    using (var g = Graphics.FromImage(bitmap))
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.Clear(Color.Transparent);
        g.ScaleTransform(size / 256f, size / 256f);
        g.TranslateTransform(tx, ty);
        g.ScaleTransform(scale, scale);
        using (var light = new SolidBrush(Fill(rect))) g.FillRectangle(light, rectBounds);
        using (var red = new SolidBrush(Fill(pathElement))) g.FillPath(red, path);
    }
    using var stream = new MemoryStream();
    bitmap.Save(stream, ImageFormat.Png);
    return stream.ToArray();
}

static float F(XElement e, string name) => float.Parse(e.Attribute(name)!.Value, CultureInfo.InvariantCulture);

static Color Fill(XElement e)
{
    var m = Regex.Match(e.Attribute("style")!.Value, @"fill:\s*rgb\((\d+),(\d+),(\d+)\)");
    return Color.FromArgb(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
}

/// <summary>SVG path data with the commands this icon uses: M L H V C (absolute and relative) and Z.</summary>
static GraphicsPath ParsePath(string d)
{
    var path = new GraphicsPath(FillMode.Winding); // fill-rule: nonzero
    var tokens = Regex.Matches(d, @"[MmLlHhVvCcZz]|-?\d*\.?\d+").Select(m => m.Value).ToList();
    PointF current = default, start = default;
    var i = 0;
    char command = 'M';
    float Next() => float.Parse(tokens[i++], CultureInfo.InvariantCulture);

    while (i < tokens.Count)
    {
        if (char.IsLetter(tokens[i][0])) command = tokens[i++][0];
        var relative = char.IsLower(command);
        var ox = relative ? current.X : 0;
        var oy = relative ? current.Y : 0;
        switch (char.ToUpperInvariant(command))
        {
            case 'M':
                path.StartFigure();
                current = start = new PointF(ox + Next(), oy + Next());
                command = relative ? 'l' : 'L'; // further pairs are line-tos
                break;
            case 'L':
                var to = new PointF(ox + Next(), oy + Next());
                path.AddLine(current, to);
                current = to;
                break;
            case 'H':
                var h = new PointF(ox + Next(), current.Y);
                path.AddLine(current, h);
                current = h;
                break;
            case 'V':
                var v = new PointF(current.X, oy + Next());
                path.AddLine(current, v);
                current = v;
                break;
            case 'C':
                var c1 = new PointF(ox + Next(), oy + Next());
                var c2 = new PointF(ox + Next(), oy + Next());
                var end = new PointF(ox + Next(), oy + Next());
                path.AddBezier(current, c1, c2, end);
                current = end;
                break;
            case 'Z':
                path.CloseFigure();
                current = start;
                break;
        }
    }
    return path;
}

/// <summary>An .ico with PNG-compressed entries (supported by Windows since Vista, and by WinForms).</summary>
static void WriteIco(string file, IReadOnlyList<(int Size, byte[] Png)> images)
{
    using var w = new BinaryWriter(File.Create(file));
    w.Write((short)0);            // reserved
    w.Write((short)1);            // type: icon
    w.Write((short)images.Count);
    var offset = 6 + 16 * images.Count;
    foreach (var (size, png) in images)
    {
        w.Write((byte)(size >= 256 ? 0 : size)); // 0 means 256
        w.Write((byte)(size >= 256 ? 0 : size));
        w.Write((byte)0);         // no palette
        w.Write((byte)0);         // reserved
        w.Write((short)1);        // planes
        w.Write((short)32);       // bits per pixel
        w.Write(png.Length);
        w.Write(offset);
        offset += png.Length;
    }
    foreach (var (_, png) in images) w.Write(png);
}
