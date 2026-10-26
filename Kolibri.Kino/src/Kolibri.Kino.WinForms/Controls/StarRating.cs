using System.Drawing.Drawing2D;

namespace Kolibri.Kino.WinForms.Controls;

/// <summary>
/// Five stars in halves, as Plex rates (1–10): the left half of a star is half a star. Pointing previews, a click
/// chooses (<see cref="Chosen"/>). Keyboard: arrows move half a star, Enter or Space chooses.
/// </summary>
internal sealed class StarRating : Control
{
    private const int Stars = 5;
    private const int Gap = 3;
    private static readonly Color Filled = Color.FromArgb(232, 180, 60);
    private static readonly Color Empty = Color.FromArgb(215, 215, 215);
    private static readonly Color Outline = Color.FromArgb(150, 150, 150);

    private int? _value;
    private int? _preview;

    public StarRating()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        TabStop = true;
        AccessibleRole = AccessibleRole.Slider;
        AccessibleName = "My rating";
        Size = new Size(StarSize * Stars + Gap * (Stars - 1) + 2, StarSize + 2);
    }

    public int StarSize { get; } = 26;

    /// <summary>The rating shown, 1–10, or null for none. Set in code, not by the designer.</summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int? Value
    {
        get => _value;
        set
        {
            _value = value is null ? null : Math.Clamp(value.Value, 1, Stars * 2);
            Invalidate();
        }
    }

    /// <summary>A rating was clicked (or chosen with the keyboard): 1–10.</summary>
    public event EventHandler<int>? Chosen;

    /// <summary>The rating being pointed at (null when the mouse leaves), to show as text.</summary>
    public event EventHandler<int?>? PreviewChanged;

    private int? Shown => _preview ?? _value;

    private void SetPreview(int? preview)
    {
        if (_preview == preview) return;
        _preview = preview;
        Invalidate();
        PreviewChanged?.Invoke(this, preview);
    }

    /// <summary>The rating under x: which star, and which half of it.</summary>
    private int ValueAt(int x)
    {
        var star = Math.Clamp(x / (StarSize + Gap), 0, Stars - 1);
        var inStar = x - star * (StarSize + Gap);
        return star * 2 + (inStar < StarSize / 2 ? 1 : 2);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (Enabled) SetPreview(ValueAt(e.X));
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        SetPreview(null);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (e.Button != MouseButtons.Left || !Enabled) return;
        Focus();
        Chosen?.Invoke(this, ValueAt(e.X));
    }

    protected override bool IsInputKey(Keys keyData) => keyData is Keys.Left or Keys.Right || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        var current = _preview ?? _value ?? 0;
        switch (e.KeyCode)
        {
            case Keys.Right: SetPreview(Math.Min(current + 1, Stars * 2)); break;
            case Keys.Left: SetPreview(Math.Max(current - 1, 1)); break;
            case Keys.Enter or Keys.Space when _preview is { } chosen:
                SetPreview(null);
                Chosen?.Invoke(this, chosen);
                break;
            default: return;
        }
        e.Handled = true;
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        SetPreview(null);
        Invalidate();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var shown = Shown ?? 0;
        using var empty = new SolidBrush(Enabled ? Empty : SystemColors.Control);
        using var filled = new SolidBrush(Enabled ? Filled : SystemColors.ControlDark);
        using var outline = new Pen(Outline, 1f);

        for (var i = 0; i < Stars; i++)
        {
            var box = new RectangleF(1 + i * (StarSize + Gap), 1, StarSize, StarSize);
            using var star = StarPath(box);
            g.FillPath(empty, star);

            var fill = shown >= (i + 1) * 2 ? 1f : shown == i * 2 + 1 ? 0.5f : 0f;
            if (fill > 0)
            {
                var state = g.Save();
                g.SetClip(new RectangleF(box.X, box.Y, box.Width * fill, box.Height));
                g.FillPath(filled, star);
                g.Restore(state);
            }
            g.DrawPath(outline, star);
        }

        if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(g, ClientRectangle);
    }

    /// <summary>A five-pointed star filling <paramref name="box"/>.</summary>
    private static GraphicsPath StarPath(RectangleF box)
    {
        var center = new PointF(box.X + box.Width / 2, box.Y + box.Height * 0.53f);
        var outer = box.Width / 2;
        var inner = outer * 0.45f;
        var points = new PointF[10];
        for (var p = 0; p < 10; p++)
        {
            var radius = p % 2 == 0 ? outer : inner;
            var angle = Math.PI / 5 * p - Math.PI / 2;
            points[p] = new PointF(center.X + (float)(radius * Math.Cos(angle)), center.Y + (float)(radius * Math.Sin(angle)));
        }
        var path = new GraphicsPath();
        path.AddPolygon(points);
        return path;
    }
}
