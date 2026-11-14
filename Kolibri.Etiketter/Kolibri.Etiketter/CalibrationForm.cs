namespace Kolibri.Etiketter;

/// <summary>
/// «Kalibrer skriver»: skriver ut kalibreringsarket og regner ut justeringen for skriveren fra målene brukeren tar.
/// </summary>
public sealed class CalibrationForm : Form
{
    private readonly LabelSheet _sheet;
    private readonly string _printerName;
    private readonly NumericUpDown _left = MeasureBox();
    private readonly NumericUpDown _top = MeasureBox();
    private readonly NumericUpDown _right = MeasureBox();
    private readonly NumericUpDown _bottom = MeasureBox();
    private readonly CheckBox _useB = new();
    private readonly Label _result = new();

    public CalibrationForm(string printerName, LabelSheet sheet, PrinterProfile current)
    {
        _printerName = printerName;
        _sheet = sheet;

        Text = "Kalibrer skriver";
        Font = new Font("Segoe UI", 9F);
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(12);

        PrinterModel? model = PrinterModel.Find(printerName);
        string calibrated = current.CalibratedAt is { } at ? $"sist kalibrert {at:d}" : "ikke kalibrert";
        var header = Text_(
            $"Skriver: {printerName}{(model is null ? "" : $" ({model.Name})")}\n" +
            $"Nå: høyre {current.OffsetXMm:0.0} mm, ned {current.OffsetYMm:0.0} mm ({calibrated}).");

        var printButton = new Button { Text = "1. Skriv ut kalibreringsark", AutoSize = true };
        printButton.Click += (_, _) => PrintCalibrationSheet();

        var steps = Text_(
            "Skriv ut på vanlig A4-papir. Arket skrives ut uten justering.\n" +
            $"2. Mål med linjal fra arkets kant til midten av krysset. Er alt riktig, er målene {CalibrationPrintJob.CrossDistanceMm:0} mm.");
        steps.ForeColor = SystemColors.GrayText;

        _useB.Text = "Jeg har også målt kryss B (nederst til høyre)";
        _useB.AutoSize = true;
        _useB.CheckedChanged += (_, _) => UpdateResult();

        var grid = new TableLayoutPanel { AutoSize = true, ColumnCount = 4, Margin = new Padding(0, 6, 0, 6) };
        grid.Controls.Add(Text_("Kryss A, fra venstre kant:"), 0, 0);
        grid.Controls.Add(_left, 1, 0);
        grid.Controls.Add(Text_("fra øvre kant:"), 2, 0);
        grid.Controls.Add(_top, 3, 0);
        grid.Controls.Add(Text_("Kryss B, fra høyre kant:"), 0, 1);
        grid.Controls.Add(_right, 1, 1);
        grid.Controls.Add(Text_("fra nedre kant:"), 2, 1);
        grid.Controls.Add(_bottom, 3, 1);

        foreach (NumericUpDown box in new[] { _left, _top, _right, _bottom })
            box.ValueChanged += (_, _) => UpdateResult();

        _result.AutoSize = true;
        _result.Font = new Font(Font, FontStyle.Bold);
        _result.Margin = new Padding(3, 6, 3, 6);

        var ok = new Button { Text = "3. Bruk justeringen", AutoSize = true, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Avbryt", AutoSize = true, DialogResult = DialogResult.Cancel };
        AcceptButton = ok;
        CancelButton = cancel;
        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0) };
        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);

        var flow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Dock = DockStyle.Fill };
        flow.Controls.AddRange([header, printButton, steps, grid, _useB, _result, buttons]);
        Controls.Add(flow);

        UpdateResult();
    }

    /// <summary>Justeringen mot høyre som målene gir, i mm.</summary>
    public float OffsetXMm { get; private set; }

    /// <summary>Justeringen nedover som målene gir, i mm.</summary>
    public float OffsetYMm { get; private set; }

    private static NumericUpDown MeasureBox() => new()
    {
        Minimum = 5,
        Maximum = 35,
        DecimalPlaces = 1,
        Increment = 0.5M,
        Value = (decimal)CalibrationPrintJob.CrossDistanceMm,
        Width = 60,
    };

    private static Label Text_(string text) =>
        new() { Text = text, AutoSize = true, MaximumSize = new Size(520, 0), Margin = new Padding(3, 7, 3, 3) };

    private void UpdateResult()
    {
        _right.Enabled = _useB.Checked;
        _bottom.Enabled = _useB.Checked;

        // Krysset er tegnet 20 mm fra kanten. Havner det lenger fra venstre kant, må alt flyttes mot venstre.
        float d = CalibrationPrintJob.CrossDistanceMm;
        float x = d - (float)_left.Value;
        float y = d - (float)_top.Value;
        if (_useB.Checked)
        {
            // Snitt av begge kryssene; da teller ikke en liten strekking av arket med.
            x = (x + ((float)_right.Value - d)) / 2f;
            y = (y + ((float)_bottom.Value - d)) / 2f;
        }
        OffsetXMm = Math.Clamp(MathF.Round(x, 1), -25f, 25f);
        OffsetYMm = Math.Clamp(MathF.Round(y, 1), -25f, 25f);
        _result.Text = $"Ny justering: høyre {OffsetXMm:0.0} mm, ned {OffsetYMm:0.0} mm";
    }

    private void PrintCalibrationSheet()
    {
        using var job = new CalibrationPrintJob(_sheet, _printerName);
        try
        {
            job.Document.Print();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Utskriften feilet:\n{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
