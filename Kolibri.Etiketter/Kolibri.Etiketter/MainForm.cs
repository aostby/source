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
    private readonly RadioButton _alignCenter = new();
    private readonly RadioButton _alignLeft = new();
    private readonly CheckBox _autoShrink = new();
    private readonly Button _removeImage = new();
    private readonly NumericUpDown _imageWidth = new();
    private readonly ComboBox _imagePosition = new();
    private readonly SheetMapControl _sheetMap = new();
    private readonly NumericUpDown _sheetCount = new();
    private readonly Label _sheetInfo = new();
    private readonly Button _previewButton = new();
    private readonly Button _printButton = new();
    private readonly CheckBox _printGuides = new();
    private readonly NumericUpDown _offsetX = new();
    private readonly NumericUpDown _offsetY = new();

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
            _design.Text = _textBox.Text;
            DesignChanged();
        };

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
        _imagePosition.Items.AddRange(["Venstre", "Høyre"]);
        _imagePosition.Width = 90;
        _imagePosition.SelectedIndexChanged += (_, _) =>
        {
            if (_loading) return;
            _design.ImagePosition = _imagePosition.SelectedIndex == 1 ? LabelImagePosition.Right : LabelImagePosition.Left;
            DesignChanged();
        };

        return Group("Bilde",
            Row(addImage, _removeImage),
            Row(Caption("Bildets bredde (% av etiketten):"), _imageWidth),
            Row(Caption("Plassering:"), _imagePosition));
    }

    private GroupBox BuildPrintGroup()
    {
        _sheetMap.Size = new Size(120, 170);
        _sheetMap.Margin = new Padding(3, 3, 12, 3);
        _sheetMap.SelectionChanged += (_, _) => UpdateSheetInfo();

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
        }

        _previewButton.Text = "Forhåndsvis ark...";
        _previewButton.AutoSize = true;
        _previewButton.Click += (_, _) => ShowSheetPreview();
        _printButton.Text = "Skriv ut...";
        _printButton.AutoSize = true;
        _printButton.Font = new Font(Font, FontStyle.Bold);
        _printButton.Click += (_, _) => PrintLabels();

        var offsetHint = Caption("Justering flytter alle etiketter dersom skriveren treffer litt skjevt.");
        offsetHint.MaximumSize = new Size(290, 0);
        offsetHint.ForeColor = SystemColors.GrayText;

        return Group("Utskrift",
            hint,
            Row(_sheetMap, positionColumn),
            _printGuides,
            Row(Caption("Justering mm, høyre:"), _offsetX, Caption("ned:"), _offsetY),
            offsetHint,
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
            _imagePosition.SelectedIndex = _design.ImagePosition == LabelImagePosition.Right ? 1 : 0;
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
            IEnumerable<int> positions = _settings.SelectedPositions ?? Enumerable.Range(1, LabelSheet.LabelsPerSheet);
            _sheetMap.SelectedIndices = positions.Select(p => p - 1).ToArray();
            _sheetCount.Value = Math.Clamp(_settings.SheetCount, 1, (int)_sheetCount.Maximum);
            _printGuides.Checked = _settings.PrintGuides;
            _offsetX.Value = (decimal)Math.Clamp(_settings.OffsetXMm, -10f, 10f);
            _offsetY.Value = (decimal)Math.Clamp(_settings.OffsetYMm, -10f, 10f);
        }
        finally
        {
            _loading = false;
        }
        UpdateSheetInfo();
    }

    private void SaveUiToSettings()
    {
        _settings.SelectedPositions = _sheetMap.SelectedIndices.Select(i => i + 1).ToList();
        _settings.SheetCount = (int)_sheetCount.Value;
        _settings.PrintGuides = _printGuides.Checked;
        _settings.OffsetXMm = (float)_offsetX.Value;
        _settings.OffsetYMm = (float)_offsetY.Value;
        _settings.Save();
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
        Text = $"{name}{(_dirty && _currentFile is not null ? " *" : "")} – Kolibri.Etiketter (Lyreco 24 per A4, 70 x 37 mm)";
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

    private void UpdateImageControls()
    {
        bool hasImage = _design.HasImage;
        _removeImage.Enabled = hasImage;
        _imageWidth.Enabled = hasImage;
        _imagePosition.Enabled = hasImage;
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
        ReplaceDesign(new LabelDesign { Text = "Skriv teksten din her" });
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
        ReplaceDesign(new LabelDesign());
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
            _sheetMap.SelectedIndices,
            (int)_sheetCount.Value,
            (float)_offsetX.Value,
            (float)_offsetY.Value,
            _printGuides.Checked);
        job.TrySelectPrinter(_settings.PrinterName);
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
        _settings.PrinterName = job.Document.PrinterSettings.PrinterName;
        SaveUiToSettings();

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
