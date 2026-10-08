namespace Kolibri.Kino.WinForms.Forms;

partial class MovieDetailsForm
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
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        picPoster = new PictureBox();
        scroll = new Panel();
        layout = new FlowLayoutPanel();
        lblTitle = new Label();
        ratingRow = new FlowLayoutPanel();
        lblRating = new Label();
        lblVotes = new Label();
        lblHeadline = new Label();
        lblGenre = new Label();
        lblPlot = new Label();
        facts = new TableLayoutPanel();
        buttons = new FlowLayoutPanel();
        btnPlay = new Button();
        btnOpenOnDisk = new Button();
        btnImdb = new Button();
        btnTmdb = new Button();
        btnSubtitles = new Button();
        btnWatchlist = new Button();
        btnClose = new Button();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)picPoster).BeginInit();
        scroll.SuspendLayout();
        layout.SuspendLayout();
        ratingRow.SuspendLayout();
        buttons.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // picPoster
        //
        picPoster.Dock = DockStyle.Right;
        picPoster.Name = "picPoster";
        picPoster.Padding = new Padding(8);
        picPoster.Size = new Size(360, 560);
        picPoster.SizeMode = PictureBoxSizeMode.Zoom;
        picPoster.TabIndex = 0;
        picPoster.TabStop = false;
        //
        // scroll (text column; scrolls when the facts are long)
        //
        scroll.AutoScroll = true;
        scroll.Controls.Add(layout);
        scroll.Dock = DockStyle.Fill;
        scroll.Name = "scroll";
        scroll.Padding = new Padding(16, 12, 12, 12);
        scroll.TabIndex = 1;
        scroll.Resize += scroll_Resize;
        //
        // layout
        //
        layout.AutoSize = true;
        layout.Controls.Add(lblTitle);
        layout.Controls.Add(ratingRow);
        layout.Controls.Add(lblHeadline);
        layout.Controls.Add(lblGenre);
        layout.Controls.Add(lblPlot);
        layout.Controls.Add(facts);
        layout.Dock = DockStyle.Top;
        layout.FlowDirection = FlowDirection.TopDown;
        layout.Name = "layout";
        layout.WrapContents = false;
        layout.TabIndex = 0;
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Margin = new Padding(0, 0, 0, 8);
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "Title";
        //
        // ratingRow
        //
        ratingRow.AutoSize = true;
        ratingRow.Controls.Add(lblRating);
        ratingRow.Controls.Add(lblVotes);
        ratingRow.Margin = new Padding(0, 0, 0, 8);
        ratingRow.Name = "ratingRow";
        ratingRow.WrapContents = false;
        //
        // lblRating (coloured badge: red below 6, orange 6–6.9, green from 7)
        //
        lblRating.AutoSize = true;
        lblRating.BackColor = SystemColors.ControlDark;
        lblRating.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblRating.ForeColor = Color.White;
        lblRating.Margin = new Padding(0, 0, 10, 0);
        lblRating.Name = "lblRating";
        lblRating.Padding = new Padding(6, 3, 6, 3);
        lblRating.Text = "IMDb";
        //
        // lblVotes
        //
        lblVotes.AutoSize = true;
        lblVotes.ForeColor = SystemColors.GrayText;
        lblVotes.Margin = new Padding(0, 10, 0, 0);
        lblVotes.Name = "lblVotes";
        //
        // lblHeadline
        //
        lblHeadline.AutoSize = true;
        lblHeadline.Font = new Font("Segoe UI", 10F);
        lblHeadline.Margin = new Padding(0, 0, 0, 4);
        lblHeadline.Name = "lblHeadline";
        //
        // lblGenre
        //
        lblGenre.AutoSize = true;
        lblGenre.ForeColor = SystemColors.GrayText;
        lblGenre.Margin = new Padding(0, 0, 0, 12);
        lblGenre.Name = "lblGenre";
        //
        // lblPlot
        //
        lblPlot.AutoSize = true;
        lblPlot.Font = new Font("Segoe UI", 10F);
        lblPlot.Margin = new Padding(0, 0, 0, 16);
        lblPlot.Name = "lblPlot";
        //
        // facts (name | value rows, filled in code)
        //
        facts.AutoSize = true;
        facts.ColumnCount = 2;
        facts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        facts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        facts.Margin = new Padding(0);
        facts.Name = "facts";
        //
        // buttons
        //
        buttons.AutoSize = true;
        buttons.Controls.Add(btnPlay);
        buttons.Controls.Add(btnOpenOnDisk);
        buttons.Controls.Add(btnImdb);
        buttons.Controls.Add(btnTmdb);
        buttons.Controls.Add(btnSubtitles);
        buttons.Controls.Add(btnWatchlist);
        buttons.Controls.Add(btnClose);
        buttons.Dock = DockStyle.Bottom;
        buttons.Name = "buttons";
        buttons.Padding = new Padding(10, 6, 10, 6);
        buttons.TabIndex = 2;
        //
        // btnPlay
        //
        btnPlay.AutoSize = true;
        btnPlay.Enabled = false;
        btnPlay.Name = "btnPlay";
        btnPlay.TabIndex = 0;
        btnPlay.Text = "Play";
        btnPlay.Click += btnPlay_Click;
        //
        // btnOpenOnDisk
        //
        btnOpenOnDisk.AutoSize = true;
        btnOpenOnDisk.Enabled = false;
        btnOpenOnDisk.Name = "btnOpenOnDisk";
        btnOpenOnDisk.TabIndex = 1;
        btnOpenOnDisk.Text = "Open on disk";
        btnOpenOnDisk.Click += btnOpenOnDisk_Click;
        //
        // btnImdb
        //
        btnImdb.AutoSize = true;
        btnImdb.Enabled = false;
        btnImdb.Name = "btnImdb";
        btnImdb.TabIndex = 2;
        btnImdb.Text = "Open on IMDb";
        btnImdb.Click += btnImdb_Click;
        //
        // btnTmdb
        //
        btnTmdb.AutoSize = true;
        btnTmdb.Enabled = false;
        btnTmdb.Name = "btnTmdb";
        btnTmdb.TabIndex = 3;
        btnTmdb.Text = "Open on TMDb";
        btnTmdb.Click += btnTmdb_Click;
        //
        // btnSubtitles
        //
        btnSubtitles.AutoSize = true;
        btnSubtitles.Enabled = false;
        btnSubtitles.Name = "btnSubtitles";
        btnSubtitles.TabIndex = 4;
        btnSubtitles.Text = "Subtitles";
        btnSubtitles.Click += btnSubtitles_Click;
        //
        // btnWatchlist
        //
        btnWatchlist.AutoSize = true;
        btnWatchlist.Enabled = false;
        btnWatchlist.Name = "btnWatchlist";
        btnWatchlist.TabIndex = 5;
        btnWatchlist.Text = "Add to watchlist…";
        btnWatchlist.Click += btnWatchlist_Click;
        //
        // btnClose
        //
        btnClose.AutoSize = true;
        btnClose.Margin = new Padding(24, 3, 3, 3);
        btnClose.Name = "btnClose";
        btnClose.TabIndex = 6;
        btnClose.Text = "Close";
        btnClose.Click += btnClose_Click;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Name = "statusStrip";
        statusStrip.TabIndex = 3;
        //
        // lblStatus
        //
        lblStatus.Name = "lblStatus";
        lblStatus.Spring = true;
        lblStatus.Text = "Ready";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MovieDetailsForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnClose;
        ClientSize = new Size(1000, 640);
        Controls.Add(scroll);
        Controls.Add(picPoster);
        Controls.Add(buttons);
        Controls.Add(statusStrip);
        Name = "MovieDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Details";
        Load += MovieDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)picPoster).EndInit();
        layout.ResumeLayout(false);
        layout.PerformLayout();
        ratingRow.ResumeLayout(false);
        ratingRow.PerformLayout();
        scroll.ResumeLayout(false);
        scroll.PerformLayout();
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private PictureBox picPoster;
    private Panel scroll;
    private FlowLayoutPanel layout;
    private Label lblTitle;
    private FlowLayoutPanel ratingRow;
    private Label lblRating;
    private Label lblVotes;
    private Label lblHeadline;
    private Label lblGenre;
    private Label lblPlot;
    private TableLayoutPanel facts;
    private FlowLayoutPanel buttons;
    private Button btnPlay;
    private Button btnOpenOnDisk;
    private Button btnImdb;
    private Button btnTmdb;
    private Button btnSubtitles;
    private Button btnWatchlist;
    private Button btnClose;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
