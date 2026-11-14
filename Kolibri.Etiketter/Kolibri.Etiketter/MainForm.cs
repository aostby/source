using System.Drawing.Printing;

namespace Kolibri.Etiketter;

public sealed class MainForm : Form
{
    private const string FileFilter = "Etikett (*.etikett)|*.etikett|Alle filer (*.*)|*.*";
    private const string ImageFilter = "Bilder|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|Alle filer (*.*)|*.*";

    private readonly AppSettings _settings = AppSettings.Load();
    private LabelDesign _design = new();
    private string? _currentFile;
    private bool _dirty;
    private bool _loading;

    private readonly LabelPreviewControl _preview = new();
    private readonly TextBox _textBox = new();
    private readonly Label _overflowLabel = new();
    private readonly Label _fontInfo = new();
    private readonly Label _lineFontInfo = new();
    private readonly Button _lineDefaultFont = new();
    private readonly Label _imageCenterNote = new();
    private readonly RadioButton _alignCenter = new();
    private readonly RadioButton _alignLeft = new();
    private readonly CheckBox _autoShrink = new();
    private readonly Button _removeImage = new();
    private readonly NumericUpDown _imageWidth = new();
    private readonly ComboBox _imagePosition = new();
    private readonly ComboBox _sheetType = new();
    private readonly SheetMapControl _sheetMap = new();
    private readonly NumericUpDown _sheetCount = new();
    private readonly Label _sheetInfo = new();
    private readonly Button _previewButton = new();
    private readonly Button _printButton = new();
    private readonly CheckBox _printGuides = new();
    private readonly NumericUpDown _offsetX = new();
    private readonly NumericUpDown _offsetY = new();
    private readonly ComboBox _printer = new();
    private readonly Label _printerInfo = new();
    private readonly Label _printerAdvice = new();
    private readonly Label _marginWarning = new();

    public MainForm()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1100, 760);
        MinimumSize = new Size(900, 640);
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;

        BuildLayout();
        BuildMenu();
        ResumeLayout(true);

        LoadStartupDesign();
        LoadSettingsIntoUi();
    }

    /// <summary>Etikettarket som er valgt.</summary>
    private LabelSheet Sheet => _sheetMap.Sheet;

    /// <summary>
    /// Skriveren som er valgt i listen. «Windows' standardskriver» gir standardskriveren slik den er nå.
    /// Null om ingen skrivere er installert.
    /// </summary>
    private string? SelectedPrinter => _printer.SelectedItem is PrinterChoice choice ? choice.Name ?? PrinterInfo.DefaultPrinterName : null;

    /// <summary>Et valg i skriverlisten. <see cref="Name"/> er null for «Windows' standardskriver».</summary>
    private sealed record PrinterChoice(string? Name)
    {
        public override string ToString() =>
            Name ?? $"Windows' standardskriver ({PrinterInfo.DefaultPrinterName ?? "ingen"})";
    }

    // ---------------------------------------------------------------- oppsett av skjermbildet

    private void BuildMenu()
    {
        var menu = new MenuStrip();

        var file = new ToolStripMenuItem("&Fil");
        file.DropDownItems.Add(new ToolStripMenuItem("&Ny etikett", null, (_, _) => NewDesign()) { ShortcutKeys = Keys.Control | Keys.N });
        file.DropDownItems.Add(new ToolStripMenuItem("&Åpne...", null, (_, _) => OpenDesign()) { ShortcutKeys = Keys.Control | Keys.O });
        file.DropDownItems.Add(new ToolStripMenuItem("&Lagre", null, (_, _) => SaveDesign(false)) { ShortcutKeys = Keys.Control | Keys.S });
        file.DropDownItems.Add(new ToolStripMenuItem("Lagre &som...", null, (_, _) => SaveDesign(true)));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add(new ToolStripMenuItem("&Forhåndsvis ark...", null, (_, _) => ShowSheetPreview()));
        file.DropDownItems.Add(new ToolStripMenuItem("Skriv &ut...", null, (_, _) => PrintLabels()) { ShortcutKeys = Keys.Control | Keys.P });
        file.DropDownItems.Add(new ToolStripMenuItem("&Kalibrer skriver...", null, (_, _) => CalibratePrinter()));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add(new ToolStripMenuItem("&Avslutt", null, (_, _) => Close()));

        var edit = new ToolStripMenuItem("&Rediger");
        edit.DropDownItems.Add(new ToolStripMenuItem("Legg til &bilde...", null, (_, _) => AddImage()));
        edit.DropDownItems.Add(new ToolStripMenuItem("Lim inn bilde fra &utklippstavlen", null, (_, _) => PasteImage()));
        edit.DropDownItems.Add(new ToolStripMenuItem("&Fjern bilde", null, (_, _) => RemoveImage()));
        edit.DropDownItems.Add(new ToolStripSeparator());
        edit.DropDownItems.Add(new ToolStripMenuItem("Velg &skrift...", null, (_, _) => ChooseFont()));

        var help = new ToolStripMenuItem("&Hjelp");
        help.DropDownItems.Add(new ToolStripMenuItem("&Om Kolibri.Etiketter...", null, (_, _) => ShowAbout()) { ShortcutKeys = Keys.F1 });

        menu.Items.Add(file);
        menu.Items.Add(edit);
        menu.Items.Add(help);
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(8) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Venstre side: etiketten og teksten.
        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Margin = new Padding(0, 0, 8, 0) };
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
        left.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _preview.Dock = DockStyle.Fill;
        _preview.Design = _design;
        _preview.AllowDrop = true;
        _preview.Click += (_, _) => _textBox.Focus();
        _preview.DragEnter += (_, e) =>
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        };
        _preview.DragDrop += (_, e) =>
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } files)
                LoadImageFile(files[0]);
        };
        _preview.TextFitChanged += (_, _) =>
        {
            _overflowLabel.Visible = _preview.TextOverflow;
            UpdateFontInfo();
            UpdateMarginWarning();
        };

        var textCaption = new Label
        {
            Text = "Tekst på etiketten (Enter gir ny linje). Dra et bilde inn på etiketten for å legge det til.",
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 4),
        };

        _textBox.Dock = DockStyle.Fill;
        _textBox.Multiline = true;
        _textBox.AcceptsReturn = true;
        _textBox.ScrollBars = ScrollBars.Vertical;
        _textBox.Font = new Font("Segoe UI", 11F);
        _textBox.TextChanged += (_, _) =>
        {
            if (_loading) return;
            _design.SetTextKeepingLineStyles(_textBox.Text);
            DesignChanged();
            UpdateLineFontInfo();
        };
        _textBox.KeyUp += (_, _) => UpdateLineFontInfo();
        _textBox.MouseUp += (_, _) => UpdateLineFontInfo();

        _overflowLabel.Text = "Teksten får ikke plass på etiketten. Velg mindre skrift, kortere tekst eller slå på automatisk tilpasning.";
        _overflowLabel.ForeColor = Color.Firebrick;
        _overflowLabel.AutoSize = true;
        _overflowLabel.Visible = false;
        _overflowLabel.Margin = new Padding(0, 4, 0, 0);

        left.Controls.Add(_preview, 0, 0);
        left.Controls.Add(textCaption, 0, 1);
        left.Controls.Add(_textBox, 0, 2);
        left.Controls.Add(_overflowLabel, 0, 3);

        // Høyre side: innstillinger.
        var right = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Margin = new Padding(0),
        };
        right.Controls.Add(BuildTextGroup());
        right.Controls.Add(BuildImageGroup());
        right.Controls.Add(BuildPrintGroup());

        root.Controls.Add(left, 0, 0);
        root.Controls.Add(right, 1, 0);
        Controls.Add(root);
    }

    private GroupBox BuildTextGroup()
    {
        var chooseFont = new Button { Text = "Velg skrift...", AutoSize = true };
        chooseFont.Click += (_, _) => ChooseFont();

        _fontInfo.AutoSize = true;
        _fontInfo.MaximumSize = new Size(290, 0);
        _fontInfo.Margin = new Padding(3, 0, 3, 6);

        var lineFont = new Button { Text = "Skrift for markerte linjer...", AutoSize = true };
        lineFont.Click += (_, _) => ChooseLineFont();
        _lineDefaultFont.Text = "Standardskrift";
        _lineDefaultFont.AutoSize = true;
        _lineDefaultFont.Click += (_, _) => ResetLineFont();

        var lineHint = Caption("Marker én eller flere linjer i tekstfeltet og velg skrift, størrelse og farge for dem. Velg skrift... over gjelder resten.");
        lineHint.MaximumSize = new Size(290, 0);
        lineHint.ForeColor = SystemColors.GrayText;

        _lineFontInfo.AutoSize = true;
        _lineFontInfo.MaximumSize = new Size(290, 0);
        _lineFontInfo.Margin = new Padding(3, 0, 3, 6);

        _alignCenter.Text = "Sentrert";
        _alignCenter.AutoSize = true;
        _alignLeft.Text = "Venstrestilt";
        _alignLeft.AutoSize = true;
        _alignCenter.CheckedChanged += (_, _) =>
        {
            if (_loading) return;
            _design.Alignment = _alignCenter.Checked ? LabelTextAlignment.Center : LabelTextAlignment.Left;
            DesignChanged();
        };

        _autoShrink.Text = "Krymp skriften automatisk om teksten ikke får plass";
        _autoShrink.AutoSize = true;
        _autoShrink.MaximumSize = new Size(290, 0);
        _autoShrink.CheckedChanged += (_, _) =>
        {
            if (_loading) return;
            _design.AutoShrinkText = _autoShrink.Checked;
            DesignChanged();
        };

        return Group("Tekst",
            chooseFont,
            _fontInfo,
            Row(lineFont, _lineDefaultFont),
            lineHint,
            _lineFontInfo,
            Row(_alignCenter, _alignLeft),
            _autoShrink);
    }

    private GroupBox BuildImageGroup()
    {
        var addImage = new Button { Text = "Legg til bilde...", AutoSize = true };
        addImage.Click += (_, _) => AddImage();
        _removeImage.Text = "Fjern bilde";
        _removeImage.AutoSize = true;
        _removeImage.Click += (_, _) => RemoveImage();

        _imageWidth.Minimum = 10;
        _imageWidth.Maximum = 90;
        _imageWidth.DecimalPlaces = 0;
        _imageWidth.Width = 60;
        _imageWidth.ValueChanged += (_, _) =>
        {
            if (_loading) return;
            _design.ImageWidthPercent = (float)_imageWidth.Value;
            DesignChanged();
        };

        _imagePosition.DropDownStyle = ComboBoxStyle.DropDownList;
        _imagePosition.Items.AddRange(["Venstre", "Høyre", "Midten (uten tekst)"]);
        _imagePosition.Width = 150;
        _imagePosition.SelectedIndexChanged += (_, _) =>
        {
            if (_loading) return;
            _design.ImagePosition = _imagePosition.SelectedIndex switch
            {
                1 => LabelImagePosition.Right,
                2 => LabelImagePosition.Center,
                _ => LabelImagePosition.Left,
            };
            UpdateImageControls();
            DesignChanged();
        };

        _imageCenterNote.Text = "Bildet fyller etiketten, sentrert. Teksten skrives ikke ut (den beholdes til du velger Venstre eller Høyre igjen).";
        _imageCenterNote.AutoSize = true;
        _imageCenterNote.MaximumSize = new Size(290, 0);
        _imageCenterNote.ForeColor = SystemColors.GrayText;
        _imageCenterNote.Visible = false;

        return Group("Bilde",
            Row(addImage, _removeImage),
            Row(Caption("Bildets bredde (% av etiketten):"), _imageWidth),
            Row(Caption("Plassering:"), _imagePosition),
            _imageCenterNote);
    }

    private GroupBox BuildPrintGroup()
    {
        _sheetType.DropDownStyle = ComboBoxStyle.DropDownList;
        _sheetType.Width = 290;
        _sheetType.DropDownWidth = 420;
        _sheetType.Items.AddRange(LabelSheet.All.ToArray<object>());
        _sheetType.SelectedIndexChanged += (_, _) =>
        {
            if (_loading || _sheetType.SelectedItem is not LabelSheet sheet) return;
            RememberSelection();
            ShowSheet(sheet);
        };

        _sheetMap.Size = new Size(120, 170);
        _sheetMap.Margin = new Padding(3, 3, 12, 3);
        _sheetMap.SelectionChanged += (_, _) =>
        {
            UpdateSheetInfo();
            UpdateMarginWarning();
        };

        var selectAll = new Button { Text = "Velg alle", AutoSize = true };
        selectAll.Click += (_, _) => _sheetMap.SelectAll();
        var clearAll = new Button { Text = "Fjern alle", AutoSize = true };
        clearAll.Click += (_, _) => _sheetMap.ClearAll();

        _sheetCount.Minimum = 1;
        _sheetCount.Maximum = 100;
        _sheetCount.Width = 60;
        _sheetCount.ValueChanged += (_, _) => UpdateSheetInfo();

        _sheetInfo.AutoSize = true;
        _sheetInfo.MaximumSize = new Size(160, 0);
        _sheetInfo.ForeColor = SystemColors.GrayText;

        var positionColumn = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, WrapContents = false, Margin = new Padding(0) };
        positionColumn.Controls.Add(selectAll);
        positionColumn.Controls.Add(clearAll);
        positionColumn.Controls.Add(Caption("Antall ark:"));
        positionColumn.Controls.Add(_sheetCount);
        positionColumn.Controls.Add(_sheetInfo);

        var hint = Caption("Klikk på rutene for å velge hvilke etiketter som skrives ut (blå = skrives ut). Dra for å velge flere.");
        hint.MaximumSize = new Size(290, 0);

        _printGuides.Text = "Skriv ut hjelpelinjer (testutskrift på vanlig papir)";
        _printGuides.AutoSize = true;
        _printGuides.MaximumSize = new Size(290, 0);

        foreach (NumericUpDown offset in new[] { _offsetX, _offsetY })
        {
            offset.Minimum = -10;
            offset.Maximum = 10;
            offset.DecimalPlaces = 1;
            offset.Increment = 0.1M;
            offset.Width = 60;
            offset.ValueChanged += (_, _) =>
            {
                if (_loading) return;
                _settings.SetOffset(SelectedPrinter, (float)_offsetX.Value, (float)_offsetY.Value);
                UpdateMarginWarning();
            };
        }

        _printer.DropDownStyle = ComboBoxStyle.DropDownList;
        _printer.Width = 290;
        _printer.DropDownWidth = 420;
        _printer.SelectedIndexChanged += (_, _) =>
        {
            if (_loading) return;
            ShowPrinter();
        };

        foreach (Label info in new[] { _printerInfo, _printerAdvice })
        {
            info.AutoSize = true;
            info.MaximumSize = new Size(290, 0);
            info.ForeColor = SystemColors.GrayText;
            info.Margin = new Padding(3, 0, 3, 3);
        }

        _marginWarning.AutoSize = true;
        _marginWarning.MaximumSize = new Size(290, 0);
        _marginWarning.ForeColor = Color.Firebrick;
        _marginWarning.Visible = false;

        var calibrate = new Button { Text = "Kalibrer skriver...", AutoSize = true };
        calibrate.Click += (_, _) => CalibratePrinter();

        _previewButton.Text = "Forhåndsvis ark...";
        _previewButton.AutoSize = true;
        _previewButton.Click += (_, _) => ShowSheetPreview();
        _printButton.Text = "Skriv ut...";
        _printButton.AutoSize = true;
        _printButton.Font = new Font(Font, FontStyle.Bold);
        _printButton.Click += (_, _) => PrintLabels();

        var offsetHint = Caption("Justeringen huskes for hver skriver og flytter alle etiketter dersom skriveren treffer litt skjevt.");
        offsetHint.MaximumSize = new Size(290, 0);
        offsetHint.ForeColor = SystemColors.GrayText;

        return Group("Utskrift",
            Caption("Etikettark:"),
            _sheetType,
            hint,
            Row(_sheetMap, positionColumn),
            _printGuides,
            Caption("Skriver:"),
            _printer,
            _printerInfo,
            Row(Caption("Justering mm, høyre:"), _offsetX, Caption("ned:"), _offsetY),
            offsetHint,
            calibrate,
            _printerAdvice,
            _marginWarning,
            Row(_previewButton, _printButton));
    }

    private static GroupBox Group(string title, params Control[] controls)
    {
        var flow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Location = new Point(8, 20),
            Margin = new Padding(0),
        };
        flow.Controls.AddRange(controls);
        var group = new GroupBox
        {
            Text = title,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(300, 0),
            Padding = new Padding(8, 4, 8, 8),
            Margin = new Padding(0, 0, 0, 10),
        };
        group.Controls.Add(flow);
        return group;
    }

    private static FlowLayoutPanel Row(params Control[] controls)
    {
        var row = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0),
        };
        row.Controls.AddRange(controls);
        return row;
    }

    private static Label Caption(string text) =>
        new() { Text = text, AutoSize = true, Margin = new Padding(3, 7, 3, 3) };

    // ---------------------------------------------------------------- synkronisering mellom modell og skjerm

    private void LoadDesignIntoUi()
    {
        _loading = true;
        try
        {
            _preview.Design = _design;
            _textBox.Text = _design.Text;
            _alignCenter.Checked = _design.Alignment == LabelTextAlignment.Center;
            _alignLeft.Checked = _design.Alignment == LabelTextAlignment.Left;
            _autoShrink.Checked = _design.AutoShrinkText;
            _imageWidth.Value = (decimal)Math.Clamp(Math.Round(_design.ImageWidthPercent), 10, 90);
            _imagePosition.SelectedIndex = _design.ImagePosition switch
            {
                LabelImagePosition.Right => 1,
                LabelImagePosition.Center => 2,
                _ => 0,
            };
            UpdateLineFontInfo();
            UpdateFontInfo();
            UpdateImageControls();
        }
        finally
        {
            _loading = false;
        }
        _dirty = false;
        UpdateTitle();
        _preview.Invalidate();
    }

    private void LoadSettingsIntoUi()
    {
        _loading = true;
        try
        {
            LabelSheet sheet = LabelSheet.FindById(_settings.SheetId);
            _sheetType.SelectedItem = sheet;
            ShowSheet(sheet);
            _sheetCount.Value = Math.Clamp(_settings.SheetCount, 1, (int)_sheetCount.Maximum);
            _printGuides.Checked = _settings.PrintGuides;
            LoadPrinters();
        }
        finally
        {
            _loading = false;
        }
        ShowPrinter();
        UpdateSheetInfo();
    }

    /// <summary>Fyller skriverlisten og velger skriveren som sist ble brukt, ellers Windows' standardskriver.</summary>
    private void LoadPrinters()
    {
        _printer.Items.Clear();
        if (PrinterSettings.InstalledPrinters.Count == 0)
            return;
        // Første valg: Windows' standardskriver. Da trenger man ikke vite hvilken skriver man har;
        // alt (kant, farger, A4) leses fra driveren, og justeringen er 0 til skriveren er kalibrert.
        _printer.Items.Add(new PrinterChoice(null));
        foreach (string name in PrinterSettings.InstalledPrinters)
            _printer.Items.Add(new PrinterChoice(name));
        if (!SelectPrinterInList(_settings.PrinterName))
            _printer.SelectedIndex = 0;
    }

    private bool SelectPrinterInList(string? name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        for (int i = 0; i < _printer.Items.Count; i++)
        {
            if (_printer.Items[i] is PrinterChoice { Name: { } item } && string.Equals(item, name, StringComparison.OrdinalIgnoreCase))
            {
                _printer.SelectedIndex = i;
                return true;
            }
        }
        return false;
    }

    /// <summary>Viser justeringen, egenskapene og rådene for skriveren som er valgt.</summary>
    private void ShowPrinter()
    {
        PrinterProfile profile = _settings.GetProfile(SelectedPrinter);
        bool wasLoading = _loading;
        _loading = true;
        try
        {
            _offsetX.Value = (decimal)Math.Clamp(profile.OffsetXMm, -10f, 10f);
            _offsetY.Value = (decimal)Math.Clamp(profile.OffsetYMm, -10f, 10f);
        }
        finally
        {
            _loading = wasLoading;
        }
        _settings.PrinterName = (_printer.SelectedItem as PrinterChoice)?.Name;
        UpdatePrinterInfo();
        UpdateMarginWarning();
    }

    private void UpdatePrinterInfo()
    {
        if (SelectedPrinter is not { } name)
        {
            _printerInfo.Text = "Ingen skrivere er installert.";
            _printerAdvice.Text = "";
            return;
        }
        PrinterInfo info = PrinterInfo.Get(name);
        _printerAdvice.Text = "Tips: " + (info.Model?.Advice ?? PrinterModel.GenericAdvice);
        if (!info.IsValid)
        {
            _printerInfo.Text = "Skriveren svarer ikke akkurat nå.";
            return;
        }
        PrinterProfile profile = _settings.GetProfile(name);
        var parts = new List<string>();
        if (info.Model is { } model) parts.Add(model.Name);
        parts.Add(info.SupportsColor ? "skriver i farger" : "svart-hvitt");
        parts.Add(info.Margins.IsZero ? "ingen kant" : $"kant {info.Margins}");
        parts.Add(profile.CalibratedAt is { } at ? $"kalibrert {at:d}" : "ikke kalibrert");
        _printerInfo.Text = string.Join(" · ", parts);
    }

    /// <summary>
    /// Antall valgte etiketter der tekst eller bilde havner i skriverens ikke-utskrivbare kant, og blir kuttet.
    /// </summary>
    private int CountClippedLabels(out PrinterMarginsMm margins)
    {
        margins = default;
        RectangleF ink = _preview.InkBounds;
        if (SelectedPrinter is not { } name || ink.Width <= 0f || ink.Height <= 0f)
            return 0;
        PrinterInfo info = PrinterInfo.Get(name);
        if (!info.IsValid || info.Margins.IsZero)
            return 0;
        margins = info.Margins;
        RectangleF printable = margins.PrintableArea;
        printable.Inflate(0.05f, 0.05f);
        int clipped = 0;
        foreach (int index in _sheetMap.SelectedIndices)
        {
            RectangleF label = Sheet.GetLabelRect(index, (float)_offsetX.Value, (float)_offsetY.Value);
            var content = new RectangleF(label.X + ink.X, label.Y + ink.Y, ink.Width, ink.Height);
            if (!printable.Contains(content))
                clipped++;
        }
        return clipped;
    }

    private void UpdateMarginWarning()
    {
        int clipped = CountClippedLabels(out PrinterMarginsMm margins);
        _marginWarning.Visible = clipped > 0;
        if (clipped > 0)
        {
            string which = clipped == 1 ? "1 etikett har" : $"{clipped} etiketter har";
            _marginWarning.Text = $"{which} tekst eller bilde i skriverens ikke-utskrivbare kant ({margins}), " +
                "og blir kuttet der. Velg bort kantrutene eller gjør teksten eller bildet mindre.";
        }
    }

    private void CalibratePrinter()
    {
        if (SelectedPrinter is not { } name)
        {
            MessageBox.Show(this, "Ingen skrivere er installert.", "Kalibrer skriver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var form = new CalibrationForm(name, Sheet, _settings.GetProfile(name));
        if (form.ShowDialog(this) != DialogResult.OK)
            return;
        _settings.SetOffset(name, form.OffsetXMm, form.OffsetYMm, calibrated: true);
        ShowPrinter();
        _settings.Save();
    }

    private void SaveUiToSettings()
    {
        _settings.SheetId = Sheet.Id;
        RememberSelection();
        _settings.SheetCount = (int)_sheetCount.Value;
        _settings.PrintGuides = _printGuides.Checked;
        _settings.PrinterName = (_printer.SelectedItem as PrinterChoice)?.Name;
        if (SelectedPrinter is { } printer)
            _settings.SetOffset(printer, (float)_offsetX.Value, (float)_offsetY.Value);
        _settings.Save();
    }

    /// <summary>Husker hvilke posisjoner som er valgt på arket som vises nå.</summary>
    private void RememberSelection() =>
        _settings.SelectionsBySheet[Sheet.Id] = _sheetMap.SelectedIndices.Select(i => i + 1).ToList();

    /// <summary>
    /// Bytter ark: miniatyrarket (med posisjonene som sist var valgt på dette arket, ellers alle),
    /// etiketten på skjermen og tittelen.
    /// </summary>
    private void ShowSheet(LabelSheet sheet)
    {
        _sheetMap.Sheet = sheet;
        if (_settings.SelectionsBySheet.TryGetValue(sheet.Id, out List<int>? positions))
            _sheetMap.SelectedIndices = positions.Select(p => p - 1).ToArray();
        _preview.Sheet = sheet;
        UpdateTitle();
        UpdateMarginWarning();
    }

    private void DesignChanged()
    {
        _dirty = true;
        UpdateTitle();
        _preview.Invalidate();
    }

    private void UpdateTitle()
    {
        string name = _currentFile is null ? "Ny etikett" : Path.GetFileNameWithoutExtension(_currentFile);
        Text = $"{name}{(_dirty && _currentFile is not null ? " *" : "")} – Kolibri.Etiketter ({Sheet.Name}, {Sheet.Products})";
    }

    private void UpdateFontInfo()
    {
        var styles = new List<string>();
        if (_design.FontStyle.HasFlag(FontStyle.Bold)) styles.Add("fet");
        if (_design.FontStyle.HasFlag(FontStyle.Italic)) styles.Add("kursiv");
        if (_design.FontStyle.HasFlag(FontStyle.Underline)) styles.Add("understreket");
        string style = styles.Count > 0 ? ", " + string.Join(", ", styles) : "";
        float used = _preview.UsedFontSize;
        string shrunk = used > 0f && used < _design.FontSize - 0.01f ? $" (krympet til {used:0.##} pt for å få plass)" : "";
        _fontInfo.Text = $"{_design.FontFamily}, {_design.FontSize:0.#} pt{style}{shrunk}";
        _fontInfo.ForeColor = _design.TextColor.GetBrightness() > 0.9f ? SystemColors.ControlText : _design.TextColor;
    }

    /// <summary>Linjene (fra 0) som er markert i tekstfeltet, eller linjen markøren står på.</summary>
    private (int First, int Last) SelectedLines()
    {
        string text = _textBox.Text;
        int start = Math.Clamp(_textBox.SelectionStart, 0, text.Length);
        int end = Math.Clamp(start + Math.Max(0, _textBox.SelectionLength - 1), 0, text.Length);
        // Markeringen slutter ofte rett etter linjeskiftet; da hører den neste linjen ikke med.
        if (_textBox.SelectionLength > 0 && end > start && end < text.Length && (text[end] == '\n' || text[end] == '\r'))
            end--;
        int LineOf(int index) => text.AsSpan(0, Math.Min(index, text.Length)).Count('\n');
        return (LineOf(start), Math.Max(LineOf(start), LineOf(end)));
    }

    private static string DescribeFont(string family, float size, FontStyle style)
    {
        var styles = new List<string>();
        if (style.HasFlag(FontStyle.Bold)) styles.Add("fet");
        if (style.HasFlag(FontStyle.Italic)) styles.Add("kursiv");
        if (style.HasFlag(FontStyle.Underline)) styles.Add("understreket");
        return $"{family}, {size:0.#} pt{(styles.Count > 0 ? ", " + string.Join(", ", styles) : "")}";
    }

    private void UpdateLineFontInfo()
    {
        (int first, int last) = SelectedLines();
        string lines = first == last ? $"Linje {first + 1}" : $"Linje {first + 1}–{last + 1}";
        LabelLineStyle? own = _design.GetLineStyle(first);
        static string Key(LabelLineStyle? s) => s is null ? "" : $"{s.FontFamily}|{s.FontSize}|{s.FontStyle}|{s.TextColorArgb}";
        bool mixed = Enumerable.Range(first, last - first + 1).Select(i => Key(_design.GetLineStyle(i))).Distinct().Count() > 1;
        string font = mixed ? "ulik skrift"
            : own is null ? "standardskrift"
            : DescribeFont(own.FontFamily, own.FontSize, own.FontStyle);
        int count = _design.LinesWithOwnFont;
        string total = count == 0 ? "" : count == 1 ? " (1 linje har egen skrift)" : $" ({count} linjer har egen skrift)";
        _lineFontInfo.Text = $"{lines}: {font}{total}";
        _lineFontInfo.ForeColor = own is not null && !mixed && own.TextColor.GetBrightness() <= 0.9f ? own.TextColor : SystemColors.ControlText;
        _lineDefaultFont.Enabled = Enumerable.Range(first, last - first + 1).Any(i => _design.GetLineStyle(i) is not null);
    }

    private void ChooseLineFont()
    {
        (int first, int last) = SelectedLines();
        LabelLineStyle? current = _design.GetLineStyle(first);
        using Font font = current?.CreateFont(current.FontSize) ?? _design.CreateFont();
        using var dialog = new FontDialog
        {
            Font = font,
            ShowColor = true,
            Color = current?.TextColor ?? _design.TextColor,
            ShowEffects = true,
            FontMustExist = true,
            MinSize = 4,
            MaxSize = 72,
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        _design.SetLineStyle(first, last, LabelLineStyle.From(dialog.Font, dialog.Color));
        DesignChanged();
        UpdateLineFontInfo();
        _textBox.Focus();
    }

    private void ResetLineFont()
    {
        (int first, int last) = SelectedLines();
        _design.SetLineStyle(first, last, null);
        DesignChanged();
        UpdateLineFontInfo();
        _textBox.Focus();
    }

    private void UpdateImageControls()
    {
        bool hasImage = _design.HasImage;
        _removeImage.Enabled = hasImage;
        _imageWidth.Enabled = hasImage;
        _imagePosition.Enabled = hasImage;
        bool center = hasImage && _design.ImagePosition == LabelImagePosition.Center;
        _imageWidth.Enabled = hasImage && !center;
        _imageCenterNote.Visible = center;
    }

    private void UpdateSheetInfo()
    {
        int perSheet = _sheetMap.SelectedCount;
        int sheets = (int)_sheetCount.Value;
        if (perSheet == 0)
        {
            _sheetInfo.Text = "Ingen etiketter valgt.";
            _sheetInfo.ForeColor = Color.Firebrick;
        }
        else
        {
            string labels = perSheet == 1 ? "1 etikett" : $"{perSheet} etiketter";
            _sheetInfo.Text = sheets == 1 ? $"{labels} på 1 ark" : $"{labels} på hvert av {sheets} ark ({perSheet * sheets} i alt)";
            _sheetInfo.ForeColor = SystemColors.GrayText;
        }
        _previewButton.Enabled = perSheet > 0;
        _printButton.Enabled = perSheet > 0;
    }

    // ---------------------------------------------------------------- tekst og bilde

    private void ChooseFont()
    {
        using var current = _design.CreateFont();
        using var dialog = new FontDialog
        {
            Font = current,
            ShowColor = true,
            Color = _design.TextColor,
            ShowEffects = true,
            FontMustExist = true,
            MinSize = 4,
            MaxSize = 72,
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        _design.SetFont(dialog.Font);
        _design.TextColor = dialog.Color;
        UpdateFontInfo();
        DesignChanged();
    }

    private void AddImage()
    {
        using var dialog = new OpenFileDialog { Filter = ImageFilter, Title = "Velg bilde" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            LoadImageFile(dialog.FileName);
    }

    private void LoadImageFile(string path)
    {
        try
        {
            _design.LoadImageFile(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Kunne ikke lese bildet:\n{ex.Message}", "Bilde", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        UpdateImageControls();
        DesignChanged();
    }

    private void PasteImage()
    {
        if (!Clipboard.ContainsImage())
        {
            MessageBox.Show(this, "Utklippstavlen inneholder ikke noe bilde.", "Lim inn bilde", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using Image? image = Clipboard.GetImage();
        if (image is null) return;
        _design.SetImage(image);
        UpdateImageControls();
        DesignChanged();
    }

    private void RemoveImage()
    {
        if (!_design.HasImage) return;
        _design.RemoveImage();
        UpdateImageControls();
        DesignChanged();
    }

    // ---------------------------------------------------------------- filer

    private void LoadStartupDesign()
    {
        try
        {
            if (File.Exists(AppSettings.LastSessionPath))
            {
                ReplaceDesign(LabelDesign.Load(AppSettings.LastSessionPath));
                return;
            }
        }
        catch (Exception)
        {
            // Start med en tom etikett.
        }
        ReplaceDesign(LabelDesign.CreateNew("Skriv teksten din her"));
    }

    private void ReplaceDesign(LabelDesign design)
    {
        LabelDesign old = _design;
        _design = design;
        LoadDesignIntoUi();
        if (!ReferenceEquals(old, design)) old.Dispose();
    }

    private bool ConfirmDiscardChanges()
    {
        if (!_dirty || _currentFile is null)
            return true;

        DialogResult answer = MessageBox.Show(this, $"Vil du lagre endringene i {Path.GetFileName(_currentFile)}?",
            "Kolibri.Etiketter", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        return answer switch
        {
            DialogResult.Yes => SaveDesign(false),
            DialogResult.No => true,
            _ => false,
        };
    }

    private void NewDesign()
    {
        if (!ConfirmDiscardChanges()) return;
        _currentFile = null;
        ReplaceDesign(LabelDesign.CreateNew());
        _textBox.Focus();
    }

    private void OpenDesign()
    {
        if (!ConfirmDiscardChanges()) return;
        using var dialog = new OpenFileDialog { Filter = FileFilter, Title = "Åpne etikett" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            LabelDesign design = LabelDesign.Load(dialog.FileName);
            _currentFile = dialog.FileName;
            ReplaceDesign(design);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Kunne ikke åpne filen:\n{ex.Message}", "Åpne", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private bool SaveDesign(bool saveAs)
    {
        string? path = _currentFile;
        if (saveAs || path is null)
        {
            using var dialog = new SaveFileDialog { Filter = FileFilter, Title = "Lagre etikett", DefaultExt = "etikett", FileName = path is null ? "" : Path.GetFileName(path) };
            if (dialog.ShowDialog(this) != DialogResult.OK) return false;
            path = dialog.FileName;
        }
        try
        {
            _design.Save(path);
            _currentFile = path;
            _dirty = false;
            UpdateTitle();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Kunne ikke lagre:\n{ex.Message}", "Lagre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
    }

    // ---------------------------------------------------------------- utskrift

    private LabelPrintJob CreatePrintJob()
    {
        var job = new LabelPrintJob(
            _design,
            Sheet,
            _sheetMap.SelectedIndices,
            (int)_sheetCount.Value,
            (float)_offsetX.Value,
            (float)_offsetY.Value,
            _printGuides.Checked);
        job.TrySelectPrinter(SelectedPrinter);
        job.ApplyPageSettings();
        return job;
    }

    private bool EnsureSelection(string caption)
    {
        if (_sheetMap.SelectedCount > 0)
            return true;
        MessageBox.Show(this, "Ingen etiketter er valgt. Klikk på rutene på miniatyrarket for å velge hvilke etiketter som skal skrives ut.",
            caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private void ShowSheetPreview()
    {
        if (!EnsureSelection("Forhåndsvisning")) return;
        if (PrinterSettings.InstalledPrinters.Count == 0)
        {
            MessageBox.Show(this, "Ingen skrivere er installert.", "Forhåndsvisning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using LabelPrintJob job = CreatePrintJob();
        using var dialog = new PrintPreviewDialog
        {
            Document = job.Document,
            Text = "Forhåndsvisning av etikettark",
            WindowState = FormWindowState.Maximized,
            UseAntiAlias = true,
        };
        dialog.ShowDialog(this);
    }

    private void PrintLabels()
    {
        if (!EnsureSelection("Skriv ut")) return;
        if (PrinterSettings.InstalledPrinters.Count == 0)
        {
            MessageBox.Show(this, "Ingen skrivere er installert.", "Skriv ut", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using LabelPrintJob job = CreatePrintJob();
        using var dialog = new PrintDialog
        {
            Document = job.Document,
            UseEXDialog = true,
            AllowSomePages = false,
            AllowSelection = false,
            AllowCurrentPage = false,
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        // Valg av annen skriver i dialogen nullstiller papirinnstillingene, så de settes på nytt.
        job.ApplyPageSettings();

        // Ble en annen skriver valgt i dialogen, brukes den skriverens justering.
        string chosen = job.Document.PrinterSettings.PrinterName;
        if (!string.Equals(chosen, SelectedPrinter, StringComparison.OrdinalIgnoreCase) && SelectPrinterInList(chosen))
        {
            job.OffsetXMm = (float)_offsetX.Value;
            job.OffsetYMm = (float)_offsetY.Value;
        }
        SaveUiToSettings();

        int clipped = CountClippedLabels(out PrinterMarginsMm margins);
        if (clipped > 0 && MessageBox.Show(this,
                $"{clipped} av {_sheetMap.SelectedCount} etiketter har tekst eller bilde i skriverens ikke-utskrivbare kant ({margins}). " +
                "Det som havner der blir ikke skrevet ut.\n\nVil du skrive ut likevel?",
                "Skriv ut", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        try
        {
            job.Document.Print();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Utskriften feilet:\n{ex.Message}", "Skriv ut", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ---------------------------------------------------------------- om

    private void ShowAbout()
    {
        using var about = new AboutForm();
        about.ShowDialog(this);
    }

    // ---------------------------------------------------------------- avslutning

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!ConfirmDiscardChanges())
        {
            e.Cancel = true;
            return;
        }
        try
        {
            AppSettings.EnsureFolder();
            _design.Save(AppSettings.LastSessionPath);
        }
        catch (Exception)
        {
            // Autolagring er ikke kritisk.
        }
        SaveUiToSettings();
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _design.Dispose();
        base.Dispose(disposing);
    }
}
