namespace Kolibri.Etiketter;

/// <summary>Viser én etikett forstørret, med de samme målene som på papiret.</summary>
public sealed class LabelPreviewControl : Control
{
    private LabelDesign? _design;
    private bool _textOverflow;
    private float _usedFontSize;

    public LabelPreviewControl()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Color.FromArgb(220, 222, 226);
    }

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public LabelDesign? Design
    {
        get => _design;
        set { _design = value; Invalidate(); }
    }

    /// <summary>True dersom teksten ikke fikk plass ved siste opptegning.</summary>
    public bool TextOverflow => _textOverflow;

    /// <summary>Skriftstørrelsen (punkt) teksten faktisk fikk ved siste opptegning, etter eventuell krymping.</summary>
    public float UsedFontSize => _usedFontSize;

    /// <summary>Utløses når <see cref="TextOverflow"/> eller <see cref="UsedFontSize"/> endres.</summary>
    public event EventHandler? TextFitChanged;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(BackColor);
        if (_design is null)
            return;

        const float paddingPx = 24f;
        float pxPerMm = Math.Min(
            (ClientSize.Width - 2 * paddingPx) / LabelSheet.LabelWidthMm,
            (ClientSize.Height - 2 * paddingPx) / LabelSheet.LabelHeightMm);
        if (pxPerMm <= 0.1f)
            return;

        // Tegn i millimeter, men forstørret. Da blir også skriftstørrelser (punkt) riktige i forhold til etiketten.
        g.PageUnit = GraphicsUnit.Millimeter;
        g.PageScale = pxPerMm / (g.DpiX / 25.4f);

        float x = (ClientSize.Width - LabelSheet.LabelWidthMm * pxPerMm) / 2f / pxPerMm;
        float y = (ClientSize.Height - LabelSheet.LabelHeightMm * pxPerMm) / 2f / pxPerMm;
        var label = new RectangleF(x, y, LabelSheet.LabelWidthMm, LabelSheet.LabelHeightMm);

        float shadow = 3f / pxPerMm;
        using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            g.FillRectangle(shadowBrush, label.X + shadow, label.Y + shadow, label.Width, label.Height);
        g.FillRectangle(Brushes.White, label);

        LabelDrawResult result = LabelRenderer.Draw(g, _design, label, LabelRenderMode.Screen);
        if (result.TextOverflow != _textOverflow || result.FontSize != _usedFontSize)
        {
            _textOverflow = result.TextOverflow;
            _usedFontSize = result.FontSize;
            TextFitChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
