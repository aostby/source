namespace Kolibri.Kino.WinForms.Controls;

partial class ItemDetailsControl
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            picPoster.Image?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        posterPanel = new Panel();
        picPoster = new PictureBox();
        tableInfo = new TableLayoutPanel();
        lblTitle = new Label();
        lblFacts = new Label();
        lblPeople = new Label();
        txtPlot = new TextBox();
        lblFile = new Label();
        buttons = new FlowLayoutPanel();
        btnOpenFolder = new Button();
        btnPlay = new Button();
        posterPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picPoster).BeginInit();
        tableInfo.SuspendLayout();
        buttons.SuspendLayout();
        SuspendLayout();
        //
        // posterPanel (width set in code from the control's height; see LayoutPoster)
        //
        posterPanel.Controls.Add(picPoster);
        posterPanel.Dock = DockStyle.Right;
        posterPanel.Name = "posterPanel";
        posterPanel.Padding = new Padding(0, 8, 8, 8);
        posterPanel.Size = new Size(280, 420);
        posterPanel.TabIndex = 0;
        //
        // picPoster (top of the panel, so the poster sits in the upper right corner)
        //
        picPoster.Dock = DockStyle.Top;
        picPoster.Name = "picPoster";
        picPoster.Size = new Size(272, 408);
        picPoster.SizeMode = PictureBoxSizeMode.Zoom;
        picPoster.TabIndex = 0;
        picPoster.TabStop = false;
        //
        // tableInfo
        //
        tableInfo.ColumnCount = 1;
        tableInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableInfo.Controls.Add(lblTitle, 0, 0);
        tableInfo.Controls.Add(lblFacts, 0, 1);
        tableInfo.Controls.Add(lblPeople, 0, 2);
        tableInfo.Controls.Add(txtPlot, 0, 3);
        tableInfo.Controls.Add(lblFile, 0, 4);
        tableInfo.Controls.Add(buttons, 0, 5);
        tableInfo.Dock = DockStyle.Fill;
        tableInfo.Location = new Point(0, 0);
        tableInfo.Name = "tableInfo";
        tableInfo.Padding = new Padding(8, 8, 8, 8);
        tableInfo.RowCount = 7;
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle());
        tableInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // empty filler below the buttons
        tableInfo.Size = new Size(380, 420);
        tableInfo.TabIndex = 1;
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.Margin = new Padding(3, 0, 3, 6);
        lblTitle.Name = "lblTitle";
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Title";
        //
        // lblFacts
        //
        lblFacts.AutoSize = true;
        lblFacts.Dock = DockStyle.Fill;
        lblFacts.ForeColor = SystemColors.GrayText;
        lblFacts.Margin = new Padding(3, 0, 3, 6);
        lblFacts.Name = "lblFacts";
        lblFacts.TabIndex = 1;
        lblFacts.Text = "Year · Runtime · Genre";
        //
        // lblPeople
        //
        lblPeople.AutoSize = true;
        lblPeople.Dock = DockStyle.Fill;
        lblPeople.Margin = new Padding(3, 0, 3, 6);
        lblPeople.Name = "lblPeople";
        lblPeople.TabIndex = 2;
        lblPeople.Text = "Director / Actors";
        //
        // txtPlot
        //
        // Height follows the text (see SizePlot); scroll bar only for unusually long plots.
        txtPlot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtPlot.BackColor = SystemColors.Control;
        txtPlot.BorderStyle = BorderStyle.None;
        txtPlot.Multiline = true;
        txtPlot.Name = "txtPlot";
        txtPlot.ReadOnly = true;
        txtPlot.ScrollBars = ScrollBars.None;
        txtPlot.Size = new Size(300, 40);
        txtPlot.TabIndex = 3;
        //
        // lblFile
        //
        lblFile.AutoSize = true;
        lblFile.Dock = DockStyle.Fill;
        lblFile.ForeColor = SystemColors.GrayText;
        lblFile.Margin = new Padding(3, 6, 3, 3);
        lblFile.Name = "lblFile";
        lblFile.TabIndex = 4;
        lblFile.Text = "File";
        //
        // buttons
        //
        buttons.AutoSize = true;
        buttons.Controls.Add(btnOpenFolder);
        buttons.Controls.Add(btnPlay);
        buttons.Dock = DockStyle.Fill;
        buttons.Name = "buttons";
        buttons.TabIndex = 5;
        //
        // btnOpenFolder
        //
        btnOpenFolder.AutoSize = true;
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new Size(90, 25);
        btnOpenFolder.TabIndex = 0;
        btnOpenFolder.Text = "Open folder";
        btnOpenFolder.Click += btnOpenFolder_Click;
        //
        // btnPlay
        //
        btnPlay.AutoSize = true;
        btnPlay.Name = "btnPlay";
        btnPlay.Size = new Size(75, 25);
        btnPlay.TabIndex = 1;
        btnPlay.Text = "Play";
        btnPlay.Click += btnPlay_Click;
        //
        // ItemDetailsControl
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(tableInfo);
        Controls.Add(posterPanel);
        Name = "ItemDetailsControl";
        Size = new Size(600, 420);
        posterPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picPoster).EndInit();
        tableInfo.ResumeLayout(false);
        tableInfo.PerformLayout();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel posterPanel;
    private PictureBox picPoster;
    private TableLayoutPanel tableInfo;
    private Label lblTitle;
    private Label lblFacts;
    private Label lblPeople;
    private TextBox txtPlot;
    private Label lblFile;
    private FlowLayoutPanel buttons;
    private Button btnOpenFolder;
    private Button btnPlay;
}
