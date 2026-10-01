using System.Reflection;
using System.Text.RegularExpressions;

namespace Kolibri.Etiketter;

/// <summary>Hjelp > Om: viser README.md, som er lagt inn i programmet som ressurs.</summary>
public sealed partial class AboutForm : Form
{
    private const string ReadmeResource = "Kolibri.Etiketter.README.md";

    private readonly RichTextBox _text = new();

    public AboutForm()
    {
        Text = "Om Kolibri.Etiketter";
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(760, 640);
        MinimumSize = new Size(500, 400);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimizeBox = false;
        KeyPreview = true;

        _text.Dock = DockStyle.Fill;
        _text.ReadOnly = true;
        _text.BorderStyle = BorderStyle.None;
        _text.BackColor = SystemColors.Window;
        _text.DetectUrls = false;
        _text.ScrollBars = RichTextBoxScrollBars.Vertical;

        var close = new Button { Text = "Lukk", AutoSize = true, DialogResult = DialogResult.OK };
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Padding = new Padding(8),
        };
        buttons.Controls.Add(close);

        var textHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16, 12, 8, 0), BackColor = SystemColors.Window };
        textHost.Controls.Add(_text);

        Controls.Add(textHost);
        Controls.Add(buttons);
        AcceptButton = close;
        CancelButton = close;

        RenderMarkdown(LoadReadme());
        _text.Select(0, 0);
    }

    private static string LoadReadme()
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ReadmeResource);
        if (stream is null)
            return "# Kolibri.Etiketter\n\nFant ikke beskrivelsen (README.md).";
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    // ---------------------------------------------------------------- enkel visning av Markdown

    private void RenderMarkdown(string markdown)
    {
        string family = Font.FontFamily.Name;
        using var body = new Font(family, 10f);
        using var bold = new Font(family, 10f, FontStyle.Bold);
        using var h1 = new Font(family, 18f, FontStyle.Bold);
        using var h2 = new Font(family, 13f, FontStyle.Bold);
        using var code = new Font("Consolas", 9.5f);

        string[] lines = markdown.Replace("\r\n", "\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.StartsWith("# "))
            {
                AppendIndent(0);
                Append(line[2..] + "\n", h1, SystemColors.WindowText);
                continue;
            }
            if (line.StartsWith("## "))
            {
                AppendIndent(0);
                Append("\n" + line[3..] + "\n", h2, Color.FromArgb(0, 90, 170));
                continue;
            }
            if (line.StartsWith('|'))
            {
                // Tabell: vis hver rad som "celle – celle", og hopp over skillelinjen og overskriften.
                string[] cells = line.Trim('|').Split('|').Select(c => c.Trim()).ToArray();
                bool separator = cells.All(c => c.Trim('-', ':').Length == 0);
                bool header = i + 1 < lines.Length && lines[i + 1].StartsWith("|---");
                if (separator || header) continue;
                AppendIndent(BulletIndent);
                AppendInline("•  " + string.Join("  –  ", cells) + "\n", body, bold, code);
                continue;
            }

            Match item = Regex.Match(line, @"^(\s*)(- |\d+\. )(.*)$");
            if (item.Success)
            {
                string marker = item.Groups[2].Value == "- " ? "•" : item.Groups[2].Value.Trim();
                string text = item.Groups[3].Value;
                // Slå sammen med fortsettelseslinjer (innrykket tekst på neste linje).
                while (i + 1 < lines.Length && lines[i + 1].StartsWith("  ") && lines[i + 1].Trim().Length > 0
                       && !Regex.IsMatch(lines[i + 1], @"^\s*(- |\d+\. )"))
                {
                    text += " " + lines[++i].Trim();
                }
                AppendIndent(BulletIndent);
                AppendInline(marker + "  " + text + "\n", body, bold, code);
                continue;
            }

            if (line.Trim().Length == 0)
            {
                continue;
            }

            // Vanlig avsnitt: slå sammen linjer til ett avsnitt.
            string paragraph = line.Trim();
            while (i + 1 < lines.Length && lines[i + 1].Trim().Length > 0 && !IsBlockStart(lines[i + 1]))
                paragraph += " " + lines[++i].Trim();
            AppendIndent(0);
            AppendInline("\n" + paragraph + "\n", body, bold, code);
        }
    }

    private const int BulletIndent = 12;

    private static bool IsBlockStart(string line) =>
        line.StartsWith('#') || line.StartsWith('|') || Regex.IsMatch(line, @"^\s*(- |\d+\. )");

    private void AppendIndent(int indent)
    {
        _text.SelectionStart = _text.TextLength;
        _text.SelectionIndent = indent;
        _text.SelectionHangingIndent = indent > 0 ? 14 : 0;
    }

    /// <summary>Legger til tekst med **fet** og `kode` markert.</summary>
    private void AppendInline(string text, Font body, Font bold, Font code)
    {
        foreach (Match part in InlineRegex().Matches(text))
        {
            string value = part.Value;
            if (value.Length > 4 && value.StartsWith("**") && value.EndsWith("**"))
                Append(value[2..^2], bold, SystemColors.WindowText);
            else if (value.Length > 2 && value.StartsWith('`') && value.EndsWith('`'))
                Append(value[1..^1], code, Color.FromArgb(150, 40, 40));
            else
                Append(value, body, SystemColors.WindowText);
        }
    }

    private void Append(string text, Font font, Color color)
    {
        _text.SelectionStart = _text.TextLength;
        _text.SelectionLength = 0;
        _text.SelectionFont = font;
        _text.SelectionColor = color;
        _text.AppendText(text);
    }

    [GeneratedRegex(@"\*\*.+?\*\*|`[^`]+`|[^*`]+|[*`]")]
    private static partial Regex InlineRegex();
}
