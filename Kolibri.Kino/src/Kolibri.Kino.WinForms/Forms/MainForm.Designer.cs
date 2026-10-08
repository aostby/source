namespace Kolibri.Kino.WinForms.Forms;

partial class MainForm
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
        menuStrip = new MenuStrip();
        menuFile = new ToolStripMenuItem();
        menuSettings = new ToolStripMenuItem();
        menuFileSeparator = new ToolStripSeparator();
        menuExit = new ToolStripMenuItem();
        menuLibrary = new ToolStripMenuItem();
        menuSearch = new ToolStripMenuItem();
        menuLocalMovies = new ToolStripMenuItem();
        menuLocalSeries = new ToolStripMenuItem();
        menuWatchlists = new ToolStripMenuItem();
        menuWindow = new ToolStripMenuItem();
        menuCascade = new ToolStripMenuItem();
        menuTileHorizontal = new ToolStripMenuItem();
        menuTileVertical = new ToolStripMenuItem();
        menuArrangeIcons = new ToolStripMenuItem();
        menuCloseAll = new ToolStripMenuItem();
        menuWindowSeparator = new ToolStripSeparator();
        menuHelp = new ToolStripMenuItem();
        menuHelpContents = new ToolStripMenuItem();
        menuHelpSeparator = new ToolStripSeparator();
        menuUsage = new ToolStripMenuItem();
        menuLog = new ToolStripMenuItem();
        menuAbout = new ToolStripMenuItem();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        menuStrip.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip
        //
        menuStrip.Items.AddRange(new ToolStripItem[] { menuFile, menuLibrary, menuWindow, menuHelp });
        menuStrip.Location = new Point(0, 0);
        menuStrip.MdiWindowListItem = menuWindow;
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1264, 24);
        menuStrip.TabIndex = 0;
        //
        // menuFile
        //
        menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuSettings, menuFileSeparator, menuExit });
        menuFile.Name = "menuFile";
        menuFile.Size = new Size(34, 20);
        menuFile.Text = "&File";
        //
        // menuSettings
        //
        menuSettings.Name = "menuSettings";
        menuSettings.Size = new Size(180, 22);
        menuSettings.Text = "&Settings…";
        menuSettings.Click += menuSettings_Click;
        //
        // menuFileSeparator
        //
        menuFileSeparator.Name = "menuFileSeparator";
        menuFileSeparator.Size = new Size(177, 6);
        //
        // menuExit
        //
        menuExit.Name = "menuExit";
        menuExit.ShortcutKeyDisplayString = "Alt+F4";
        menuExit.Size = new Size(180, 22);
        menuExit.Text = "E&xit";
        menuExit.Click += menuExit_Click;
        //
        // menuLibrary
        //
        menuLibrary.DropDownItems.AddRange(new ToolStripItem[] { menuSearch, menuLocalMovies, menuLocalSeries, menuWatchlists });
        menuLibrary.Name = "menuLibrary";
        menuLibrary.Size = new Size(68, 20);
        menuLibrary.Text = "&Library";
        //
        // menuSearch
        //
        menuSearch.Name = "menuSearch";
        menuSearch.Size = new Size(220, 22);
        menuSearch.Text = "&Search library and OMDb";
        menuSearch.Click += menuSearch_Click;
        //
        // menuLocalMovies
        //
        menuLocalMovies.Name = "menuLocalMovies";
        menuLocalMovies.Size = new Size(220, 22);
        menuLocalMovies.Text = "Local &movies";
        menuLocalMovies.Click += menuLocalMovies_Click;
        //
        // menuLocalSeries
        //
        menuLocalSeries.Name = "menuLocalSeries";
        menuLocalSeries.Size = new Size(220, 22);
        menuLocalSeries.Text = "Local s&eries";
        menuLocalSeries.Click += menuLocalSeries_Click;
        //
        // menuWatchlists
        //
        menuWatchlists.Name = "menuWatchlists";
        menuWatchlists.Size = new Size(220, 22);
        menuWatchlists.Text = "&Watchlists";
        menuWatchlists.Click += menuWatchlists_Click;
        //
        // menuWindow
        //
        menuWindow.DropDownItems.AddRange(new ToolStripItem[] { menuCascade, menuTileVertical, menuTileHorizontal, menuArrangeIcons, menuCloseAll, menuWindowSeparator });
        menuWindow.Name = "menuWindow";
        menuWindow.Size = new Size(63, 20);
        menuWindow.Text = "&Window";
        //
        // menuCascade
        //
        menuCascade.Name = "menuCascade";
        menuCascade.Size = new Size(200, 22);
        menuCascade.Text = "&Cascade windows";
        menuCascade.Click += menuCascade_Click;
        //
        // menuTileHorizontal
        //
        menuTileHorizontal.Name = "menuTileHorizontal";
        menuTileHorizontal.Size = new Size(200, 22);
        menuTileHorizontal.Text = "Tile &Horizontal";
        menuTileHorizontal.Click += menuTileHorizontal_Click;
        //
        // menuTileVertical
        //
        menuTileVertical.Name = "menuTileVertical";
        menuTileVertical.Size = new Size(200, 22);
        menuTileVertical.Text = "Tile &Vertical";
        menuTileVertical.Click += menuTileVertical_Click;
        //
        // menuArrangeIcons
        //
        menuArrangeIcons.Name = "menuArrangeIcons";
        menuArrangeIcons.Size = new Size(200, 22);
        menuArrangeIcons.Text = "&Arrange Icons";
        menuArrangeIcons.Click += menuArrangeIcons_Click;
        //
        // menuCloseAll
        //
        menuCloseAll.Name = "menuCloseAll";
        menuCloseAll.Size = new Size(200, 22);
        menuCloseAll.Text = "Close &All";
        menuCloseAll.Click += menuCloseAll_Click;
        //
        // menuWindowSeparator
        //
        menuWindowSeparator.Name = "menuWindowSeparator";
        menuWindowSeparator.Size = new Size(197, 6);
        //
        // menuHelp
        //
        menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuHelpContents, menuUsage, menuLog, menuHelpSeparator, menuAbout });
        menuHelp.Name = "menuHelp";
        menuHelp.Size = new Size(44, 20);
        menuHelp.Text = "&Help";
        //
        // menuHelpContents
        //
        menuHelpContents.Name = "menuHelpContents";
        menuHelpContents.ShortcutKeys = Keys.F1;
        menuHelpContents.Size = new Size(200, 22);
        menuHelpContents.Text = "&Help";
        menuHelpContents.Click += menuHelpContents_Click;
        //
        // menuUsage
        //
        menuUsage.Name = "menuUsage";
        menuUsage.Size = new Size(200, 22);
        menuUsage.Text = "API &usage report";
        menuUsage.Click += menuUsage_Click;
        //
        // menuLog
        //
        menuLog.Name = "menuLog";
        menuLog.Size = new Size(200, 22);
        menuLog.Text = "&Log";
        menuLog.Click += menuLog_Click;
        //
        // menuHelpSeparator
        //
        menuHelpSeparator.Name = "menuHelpSeparator";
        menuHelpSeparator.Size = new Size(197, 6);
        //
        // menuAbout
        //
        menuAbout.Name = "menuAbout";
        menuAbout.Size = new Size(200, 22);
        menuAbout.Text = "&About Kolibri.Kino…";
        menuAbout.Click += menuAbout_Click;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Location = new Point(0, 779);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1264, 22);
        statusStrip.TabIndex = 1;
        //
        // lblStatus
        //
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1249, 17);
        lblStatus.Spring = true;
        lblStatus.Text = "Ready";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1264, 801);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        IsMdiContainer = true;
        MainMenuStrip = menuStrip;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Kolibri.Kino";
        WindowState = FormWindowState.Maximized;
        Shown += MainForm_Shown;
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip;
    private ToolStripMenuItem menuFile;
    private ToolStripMenuItem menuSettings;
    private ToolStripSeparator menuFileSeparator;
    private ToolStripMenuItem menuExit;
    private ToolStripMenuItem menuLibrary;
    private ToolStripMenuItem menuSearch;
    private ToolStripMenuItem menuLocalMovies;
    private ToolStripMenuItem menuLocalSeries;
    private ToolStripMenuItem menuWatchlists;
    private ToolStripMenuItem menuWindow;
    private ToolStripMenuItem menuCascade;
    private ToolStripMenuItem menuTileHorizontal;
    private ToolStripMenuItem menuTileVertical;
    private ToolStripMenuItem menuArrangeIcons;
    private ToolStripMenuItem menuCloseAll;
    private ToolStripSeparator menuWindowSeparator;
    private ToolStripMenuItem menuHelp;
    private ToolStripMenuItem menuHelpContents;
    private ToolStripSeparator menuHelpSeparator;
    private ToolStripMenuItem menuUsage;
    private ToolStripMenuItem menuLog;
    private ToolStripMenuItem menuAbout;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
