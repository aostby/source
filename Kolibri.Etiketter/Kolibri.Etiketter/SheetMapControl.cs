namespace Kolibri.Etiketter;

/// <summary>
/// Miniatyr av etikettarket. Klikk på en rute for å slå den av eller på.
/// Klikk og dra for å gi flere ruter samme valg.
/// </summary>
public sealed class SheetMapControl : Control
{
    private LabelSheet _sheet = LabelSheet.Default;
    private bool[] _selected = Enumerable.Repeat(true, LabelSheet.Default.LabelsPerSheet).ToArray();
    private bool? _dragValue;

    public SheetMapControl()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Cursor = Cursors.Hand;
    }

    /// <summary>Arket som vises. Et nytt ark starter med alle etikettene valgt.</summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public LabelSheet Sheet
    {
        get => _sheet;
        set
        {
            if (ReferenceEquals(value, _sheet)) return;
            _sheet = value;
            _selected = Enumerable.Repeat(true, value.LabelsPerSheet).ToArray();
            Invalidate();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Valgte posisjoner (fra 0), sortert.</summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<int> SelectedIndices
    {
        get => Enumerable.Range(0, _sheet.LabelsPerSheet).Where(i => _selected[i]).ToArray();
        set
        {
            Array.Clear(_selected);
            foreach (int i in value)
            {
                if (i >= 0 && i < _sheet.LabelsPerSheet)
                    _selected[i] = true;
            }
            Invalidate();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int SelectedCount => _selected.Count(s => s);

    public event EventHandler? SelectionChanged;

    public void SelectAll() => SetAll(true);

    public void ClearAll() => SetAll(false);

    private void SetAll(bool value)
    {
        Array.Fill(_selected, value);
        Invalidate();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private RectangleF SheetBounds()
    {
        float scale = Math.Min((Width - 2f) / LabelSheet.PageWidthMm, (Height - 2f) / LabelSheet.PageHeightMm);
        float w = LabelSheet.PageWidthMm * scale;
        float h = LabelSheet.PageHeightMm * scale;
        return new RectangleF((Width - w) / 2f, (Height - h) / 2f, w, h);
    }

    private RectangleF CellBounds(int index)
    {
        RectangleF sheet = SheetBounds();
        float scale = sheet.Width / LabelSheet.PageWidthMm;
        RectangleF mm = _sheet.GetLabelRect(index);
        return new RectangleF(sheet.X + mm.X * scale, sheet.Y + mm.Y * scale, mm.Width * scale, mm.Height * scale);
    }

    private int HitTest(Point location)
    {
        for (int i = 0; i < _sheet.LabelsPerSheet; i++)
        {
            if (CellBounds(i).Contains(location))
                return i;
        }
        return -1;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        RectangleF sheet = SheetBounds();
        g.FillRectangle(Brushes.White, sheet);

        using var font = new Font(Font.FontFamily, 7f);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        using var printBrush = new SolidBrush(Color.FromArgb(0, 120, 215));
        using var skipBrush = new SolidBrush(Color.FromArgb(225, 225, 225));

        for (int i = 0; i < _sheet.LabelsPerSheet; i++)
        {
            RectangleF cell = CellBounds(i);
            Brush fill = _selected[i] ? printBrush : skipBrush;
            if (_sheet.IsRound)
            {
                g.FillEllipse(fill, cell);
                g.DrawEllipse(Pens.DarkGray, cell);
            }
            else
            {
                g.FillRectangle(fill, cell);
                g.DrawRectangle(Pens.DarkGray, cell.X, cell.Y, cell.Width, cell.Height);
            }
            // Numrene vises bare når de får plass.
            string number = (i + 1).ToString();
            SizeF size = g.MeasureString(number, font);
            if (size.Width <= cell.Width && size.Height <= cell.Height + 2f)
                g.DrawString(number, font, _selected[i] ? Brushes.White : Brushes.DimGray, cell, format);
        }
        g.DrawRectangle(Pens.Black, sheet.X, sheet.Y, sheet.Width, sheet.Height);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left) return;
        int index = HitTest(e.Location);
        if (index < 0) return;

        // Første rute bestemmer om draget slår på eller av.
        _dragValue = !_selected[index];
        Apply(index);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_dragValue is null) return;
        int index = HitTest(e.Location);
        if (index >= 0) Apply(index);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragValue = null;
    }

    protected override void OnMouseCaptureChanged(EventArgs e)
    {
        base.OnMouseCaptureChanged(e);
        _dragValue = null;
    }

    private void Apply(int index)
    {
        if (_dragValue is not bool value || _selected[index] == value) return;
        _selected[index] = value;
        Invalidate();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
}
