using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.WinForms.Forms;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>A window with its own "Write to Excel" (its own columns and file name) instead of the grid as shown.</summary>
internal interface IWritesToExcel
{
    void WriteToExcel();
}

/// <summary>
/// "Write to Excel…" for every window with a grid: a button on the right of its status bar (<see cref="AddButton"/>)
/// and Ctrl+S (<see cref="Shortcut"/>). Saves CSV (see Controllers.Csv) where the user chooses, then opens it in Excel.
/// </summary>
internal static partial class ExcelExport
{
    private const string ButtonName = "btnExcel";

    /// <summary>
    /// Asks where to save, suggesting "&lt;parts joined by ' - '&gt; - &lt;date&gt;.csv" in Documents, writes the file and opens it.
    /// The first part is the filter shown, so the file name says what's in it. Returns the saved file, or null when
    /// cancelled or it failed (the user has been told).
    /// </summary>
    public static string? Save(IWin32Window owner, string csv, params string[] nameParts)
    {
        var parts = nameParts.Where(p => !string.IsNullOrWhiteSpace(p)).Append($"{DateTime.Now:yyyy-MM-dd}");
        var name = string.Join("_", $"{string.Join(" - ", parts)}.csv".Split(Path.GetInvalidFileNameChars()));
        using var dialog = new SaveFileDialog
        {
            Title = "Write to Excel",
            Filter = "CSV (Excel)|*.csv",
            FileName = name,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (dialog.ShowDialog(owner) != DialogResult.OK) return null;

        try
        {
            File.WriteAllText(dialog.FileName, csv, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
            return dialog.FileName;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Win32Exception)
        {
            MessageBox.Show(owner, ex.Message, "Write to Excel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
    }

    /// <summary>
    /// The last folder name of a path, e.g. "Movies" for \\GREENLANTERN\Movies\. Not Path.GetFileName: a share root
    /// (\\server\share) is all root to it, so it gives "".
    /// </summary>
    public static string FolderName(string path) =>
        path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.TrimEnd(':') ?? path;

    /// <summary>
    /// Adds "Write to Excel…" on the right of <paramref name="form"/>'s status bar, when it has a grid and a status bar
    /// and no such button yet (windows with their own export put it in the designer). Either way the button ends up
    /// bottom right: last on the bar, after a status label that takes the free space (a status bar ignores
    /// right alignment unless some label springs).
    /// </summary>
    public static void AddButton(Form form)
    {
        if (form.Controls.OfType<StatusStrip>().FirstOrDefault() is not { } strip) return;
        if (strip.Items[ButtonName] is { } existing)
        {
            PlaceBottomRight(strip, existing);
            return;
        }
        if (!Grids(form).Any()) return;

        var button = new ToolStripButton("Write to Excel…")
        {
            Name = ButtonName,
            Alignment = ToolStripItemAlignment.Right,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            ToolTipText = "Saves the rows shown, as shown, as a file that opens in Excel, named after the filter, and opens it (Ctrl+S).",
        };
        button.Click += (_, _) => Write(form);
        strip.Items.Add(button);
        PlaceBottomRight(strip, button);
    }

    private static void PlaceBottomRight(StatusStrip strip, ToolStripItem button)
    {
        if (strip.Items.IndexOf(button) != strip.Items.Count - 1)
        {
            strip.Items.Remove(button);
            strip.Items.Add(button);
        }
        if (!strip.Items.OfType<ToolStripStatusLabel>().Any(l => l.Spring)
            && strip.Items.OfType<ToolStripStatusLabel>().FirstOrDefault() is { } label)
        {
            label.Spring = true;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }
    }

    /// <summary>
    /// Writes <paramref name="form"/>'s list: its own export if it has one, else the grid with focus (or the largest
    /// one), with the columns and rows as shown. The file name starts with the checked filter buttons, then the window.
    /// False when the window has nothing to write.
    /// </summary>
    public static bool Write(Form form)
    {
        if (form is IWritesToExcel own)
        {
            own.WriteToExcel();
            return true;
        }

        if (PickGrid(form) is not { } grid) return false;
        if (grid.Rows.Count == 0)
        {
            Status(form, "Nothing to write; the list is empty.");
            return true;
        }

        var parts = Filters(form).Append(WindowName(form.Text)).ToArray();
        if (Save(form, ToCsv(grid), parts) is { } file)
            Status(form, $"Saved {Path.GetFileName(file)}.");
        return true;
    }

    /// <summary>The grid as shown: visible columns in display order, their headers, and the cells' displayed text.</summary>
    public static string ToCsv(DataGridView grid)
    {
        var columns = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
        var csv = new StringBuilder();
        csv.AppendLine(Csv.Line(columns.Select(c => c.HeaderText).ToArray()));
        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row.IsNewRow || !row.Visible) continue;
            csv.AppendLine(Csv.Line(columns.Select(c => Text(row.Cells[c.Index].FormattedValue)).ToArray()));
        }
        return csv.ToString();

        static string? Text(object? value) => value switch
        {
            bool b => b ? "Yes" : "No",
            CheckState s => s == CheckState.Checked ? "Yes" : "No",
            Image => null,
            _ => value?.ToString(),
        };
    }

    private static IEnumerable<DataGridView> Grids(Control parent) =>
        parent.Controls.Cast<Control>().SelectMany(c => c is DataGridView grid ? [grid] : Grids(c));

    /// <summary>The grid with focus, else the largest visible one.</summary>
    private static DataGridView? PickGrid(Form form)
    {
        for (Control? c = form.ActiveControl; c is not null; c = (c as ContainerControl)?.ActiveControl)
            if (c is DataGridView focused) return focused;

        return Grids(form).Where(g => g.Visible).MaxBy(g => g.Width * g.Height);
    }

    /// <summary>The checked, visible radio buttons, top to bottom and left to right, without counts: "All (1,234)" → "All".</summary>
    private static IEnumerable<string> Filters(Form form) =>
        RadioButtons(form)
            .Where(r => r is { Checked: true, Visible: true })
            .OrderBy(r => form.PointToClient(r.PointToScreen(Point.Empty)).Y)
            .ThenBy(r => form.PointToClient(r.PointToScreen(Point.Empty)).X)
            .Select(r => Count().Replace(r.Text.Replace("&", ""), "").Trim());

    private static IEnumerable<RadioButton> RadioButtons(Control parent) =>
        parent.Controls.Cast<Control>().SelectMany(c => c is RadioButton r ? [r] : RadioButtons(c));

    /// <summary>"Local series – \\GREENLANTERN\Series" → "Local series - Series".</summary>
    private static string WindowName(string title)
    {
        var parts = title.Split(" – ", 2);
        if (parts.Length < 2) return title;
        var rest = Path.IsPathRooted(parts[1]) ? FolderName(parts[1]) : parts[1];
        return $"{parts[0]} - {rest}";
    }

    private static void Status(Form form, string text)
    {
        if (form is AsyncForm window) window.DisplayCopyStatus(text);
    }

    [GeneratedRegex(@"\s*\([\d\s,. ]+\)\s*$")]
    private static partial Regex Count();

    /// <summary>Ctrl+S in any Kino window writes its list to Excel (see <see cref="Write"/>). Installed once in Program.</summary>
    public sealed class Shortcut : IMessageFilter
    {
        private const int WmKeyDown = 0x0100;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WmKeyDown || (Keys)(int)m.WParam != Keys.S || Control.ModifierKeys != Keys.Control) return false;

            var form = Control.FromHandle(m.HWnd)?.FindForm() ?? Form.ActiveForm;
            if (form?.IsMdiContainer == true) form = form.ActiveMdiChild;
            return form is not null && Write(form);
        }
    }
}
