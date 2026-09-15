
using Kolibri.net.Common.Formutilities.Controls;

namespace Kolibri.net.Common.MovieAPI.Forms
{
    partial class BrowseMoviesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitContainer1 = new SplitContainer();
            groupBoxPlex = new GroupBox();
            buttonVisualizePlex = new Button();
            buttonWatchList = new Button();
            groupBoxAddToPlayList = new GroupBox();
            buttonVisualizeDBPlaylist = new Button();
            buttonSync = new Button();
            comboBoxWatchLists = new ComboBox();
            buttonPlaylist = new Button();
            buttonAddToPlexPlaylist = new Button();
            groupBoxSearcByFolder = new GroupBox();
            checkBoxPrintable = new CheckBox();
            buttonOpenFolder = new Button();
            groupBoxVisualize = new GroupBox();
            buttonVisualize = new Button();
            groupBoxSearch = new GroupBox();
            groupBoxSort = new GroupBox();
            radioButtonRating = new RadioButton();
            radioButtonYear = new RadioButton();
            radioButtonActor = new RadioButton();
            radioButtonMovieTitle = new RadioButton();
            checkBoxPoster = new CheckBox();
            buttonSearch = new Button();
            checkBoxDecending = new CheckBox();
            linkLabelOpenInBrowser = new LinkLabel();
            linkLabelYear = new LinkLabel();
            linkLabelGenre = new LinkLabel();
            tbSearch = new ContainsTextBox();
            comboBoxYear = new ComboBox();
            comboBoxGenre = new ComboBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxPlex.SuspendLayout();
            groupBoxAddToPlayList.SuspendLayout();
            groupBoxSearcByFolder.SuspendLayout();
            groupBoxVisualize.SuspendLayout();
            groupBoxSearch.SuspendLayout();
            groupBoxSort.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBoxPlex);
            splitContainer1.Panel1.Controls.Add(groupBoxAddToPlayList);
            splitContainer1.Panel1.Controls.Add(groupBoxSearcByFolder);
            splitContainer1.Panel1.Controls.Add(groupBoxVisualize);
            splitContainer1.Panel1.Controls.Add(groupBoxSearch);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(statusStrip1);
            splitContainer1.Size = new Size(1313, 674);
            splitContainer1.SplitterDistance = 160;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // groupBoxPlex
            // 
            groupBoxPlex.Controls.Add(buttonVisualizePlex);
            groupBoxPlex.Controls.Add(buttonWatchList);
            groupBoxPlex.Location = new Point(1080, 8);
            groupBoxPlex.Margin = new Padding(3, 2, 3, 2);
            groupBoxPlex.Name = "groupBoxPlex";
            groupBoxPlex.Padding = new Padding(3, 2, 3, 2);
            groupBoxPlex.Size = new Size(187, 142);
            groupBoxPlex.TabIndex = 16;
            groupBoxPlex.TabStop = false;
            groupBoxPlex.Text = "Plex";
            // 
            // buttonVisualizePlex
            // 
            buttonVisualizePlex.Location = new Point(5, 49);
            buttonVisualizePlex.Margin = new Padding(4, 3, 4, 3);
            buttonVisualizePlex.Name = "buttonVisualizePlex";
            buttonVisualizePlex.Size = new Size(155, 23);
            buttonVisualizePlex.TabIndex = 1;
            buttonVisualizePlex.Text = "Visualize Watchlist";
            buttonVisualizePlex.UseVisualStyleBackColor = true;
            buttonVisualizePlex.Click += buttonWatchList_Click;
            // 
            // buttonWatchList
            // 
            buttonWatchList.Location = new Point(5, 17);
            buttonWatchList.Margin = new Padding(3, 2, 3, 2);
            buttonWatchList.Name = "buttonWatchList";
            buttonWatchList.Size = new Size(155, 23);
            buttonWatchList.TabIndex = 0;
            buttonWatchList.Text = "Plex Watchlist";
            toolTip1.SetToolTip(buttonWatchList, "Vis Watchlist fra Plex");
            buttonWatchList.UseVisualStyleBackColor = true;
            buttonWatchList.Click += buttonWatchList_Click;
            // 
            // groupBoxAddToPlayList
            // 
            groupBoxAddToPlayList.Controls.Add(buttonVisualizeDBPlaylist);
            groupBoxAddToPlayList.Controls.Add(buttonSync);
            groupBoxAddToPlayList.Controls.Add(comboBoxWatchLists);
            groupBoxAddToPlayList.Controls.Add(buttonPlaylist);
            groupBoxAddToPlayList.Controls.Add(buttonAddToPlexPlaylist);
            groupBoxAddToPlayList.Location = new Point(662, 8);
            groupBoxAddToPlayList.Name = "groupBoxAddToPlayList";
            groupBoxAddToPlayList.Size = new Size(397, 142);
            groupBoxAddToPlayList.TabIndex = 15;
            groupBoxAddToPlayList.TabStop = false;
            groupBoxAddToPlayList.Text = "Add to Playlist";
            // 
            // buttonVisualizeDBPlaylist
            // 
            buttonVisualizeDBPlaylist.Location = new Point(203, 49);
            buttonVisualizeDBPlaylist.Name = "buttonVisualizeDBPlaylist";
            buttonVisualizeDBPlaylist.Size = new Size(178, 23);
            buttonVisualizeDBPlaylist.TabIndex = 4;
            buttonVisualizeDBPlaylist.Text = "Visualize DB Playlist";
            toolTip1.SetToolTip(buttonVisualizeDBPlaylist, "Vis denne spillelisten som den er lagret i lokal database.");
            buttonVisualizeDBPlaylist.UseVisualStyleBackColor = true;
            buttonVisualizeDBPlaylist.Click += buttonVisualizeDBPlaylist_Click;
            // 
            // buttonSync
            // 
            buttonSync.Location = new Point(203, 91);
            buttonSync.Name = "buttonSync";
            buttonSync.Size = new Size(178, 30);
            buttonSync.TabIndex = 3;
            buttonSync.Text = "Sync DB and Plex Playlists";
            toolTip1.SetToolTip(buttonSync, "Sync fra lokal DB til Plex");
            buttonSync.UseVisualStyleBackColor = true;
            buttonSync.Click += buttonSync_Click;
            // 
            // comboBoxWatchLists
            // 
            comboBoxWatchLists.FormattingEnabled = true;
            comboBoxWatchLists.Location = new Point(6, 18);
            comboBoxWatchLists.Name = "comboBoxWatchLists";
            comboBoxWatchLists.Size = new Size(178, 23);
            comboBoxWatchLists.TabIndex = 1;
            comboBoxWatchLists.SelectedIndexChanged += comboBoxWatchLists_SelectedIndexChanged;
            // 
            // buttonPlaylist
            // 
            buttonPlaylist.Location = new Point(203, 17);
            buttonPlaylist.Name = "buttonPlaylist";
            buttonPlaylist.Size = new Size(178, 23);
            buttonPlaylist.TabIndex = 3;
            buttonPlaylist.Text = "DB Playlist";
            toolTip1.SetToolTip(buttonPlaylist, "Vis denne spillelisten som den er lagret i lokal database.");
            buttonPlaylist.UseVisualStyleBackColor = true;
            buttonPlaylist.Click += buttonDBPlaylist_Click;
            // 
            // buttonAddToPlexPlaylist
            // 
            buttonAddToPlexPlaylist.Location = new Point(6, 49);
            buttonAddToPlexPlaylist.Name = "buttonAddToPlexPlaylist";
            buttonAddToPlexPlaylist.Size = new Size(178, 30);
            buttonAddToPlexPlaylist.TabIndex = 0;
            buttonAddToPlexPlaylist.Text = "Add To Playlist (Plex)";
            toolTip1.SetToolTip(buttonAddToPlexPlaylist, "Add current search result to selected playlist in Plex");
            buttonAddToPlexPlaylist.UseVisualStyleBackColor = true;
            buttonAddToPlexPlaylist.Click += buttonAddToPlexPlaylist_Click;
            // 
            // groupBoxSearcByFolder
            // 
            groupBoxSearcByFolder.Controls.Add(checkBoxPrintable);
            groupBoxSearcByFolder.Controls.Add(buttonOpenFolder);
            groupBoxSearcByFolder.Location = new Point(421, 8);
            groupBoxSearcByFolder.Name = "groupBoxSearcByFolder";
            groupBoxSearcByFolder.Size = new Size(233, 66);
            groupBoxSearcByFolder.TabIndex = 14;
            groupBoxSearcByFolder.TabStop = false;
            groupBoxSearcByFolder.Text = "Search by folder";
            // 
            // checkBoxPrintable
            // 
            checkBoxPrintable.AutoSize = true;
            checkBoxPrintable.BackColor = SystemColors.ButtonHighlight;
            checkBoxPrintable.Checked = true;
            checkBoxPrintable.CheckState = CheckState.Checked;
            checkBoxPrintable.Location = new Point(43, 28);
            checkBoxPrintable.Name = "checkBoxPrintable";
            checkBoxPrintable.Size = new Size(73, 19);
            checkBoxPrintable.TabIndex = 2;
            checkBoxPrintable.Text = "Printable";
            checkBoxPrintable.UseVisualStyleBackColor = false;
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonOpenFolder.Location = new Point(8, 22);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(218, 29);
            buttonOpenFolder.TabIndex = 1;
            buttonOpenFolder.Text = "Let opp mappe";
            buttonOpenFolder.TextAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.Click += buttonOpenFolder_Click;
            // 
            // groupBoxVisualize
            // 
            groupBoxVisualize.Controls.Add(buttonVisualize);
            groupBoxVisualize.Location = new Point(421, 83);
            groupBoxVisualize.Margin = new Padding(4, 3, 4, 3);
            groupBoxVisualize.Name = "groupBoxVisualize";
            groupBoxVisualize.Padding = new Padding(4, 3, 4, 3);
            groupBoxVisualize.Size = new Size(233, 65);
            groupBoxVisualize.TabIndex = 12;
            groupBoxVisualize.TabStop = false;
            groupBoxVisualize.Text = "Visualize";
            // 
            // buttonVisualize
            // 
            buttonVisualize.Location = new Point(8, 23);
            buttonVisualize.Margin = new Padding(4, 3, 4, 3);
            buttonVisualize.Name = "buttonVisualize";
            buttonVisualize.Size = new Size(218, 27);
            buttonVisualize.TabIndex = 0;
            buttonVisualize.Text = "Visualize";
            buttonVisualize.UseVisualStyleBackColor = true;
            buttonVisualize.Click += buttonVisualize_Click;
            // 
            // groupBoxSearch
            // 
            groupBoxSearch.Controls.Add(groupBoxSort);
            groupBoxSearch.Controls.Add(radioButtonActor);
            groupBoxSearch.Controls.Add(radioButtonMovieTitle);
            groupBoxSearch.Controls.Add(checkBoxPoster);
            groupBoxSearch.Controls.Add(buttonSearch);
            groupBoxSearch.Controls.Add(checkBoxDecending);
            groupBoxSearch.Controls.Add(linkLabelOpenInBrowser);
            groupBoxSearch.Controls.Add(linkLabelYear);
            groupBoxSearch.Controls.Add(linkLabelGenre);
            groupBoxSearch.Controls.Add(tbSearch);
            groupBoxSearch.Controls.Add(comboBoxYear);
            groupBoxSearch.Controls.Add(comboBoxGenre);
            groupBoxSearch.Location = new Point(8, 8);
            groupBoxSearch.Margin = new Padding(4, 3, 4, 3);
            groupBoxSearch.Name = "groupBoxSearch";
            groupBoxSearch.Padding = new Padding(4, 3, 4, 3);
            groupBoxSearch.Size = new Size(406, 142);
            groupBoxSearch.TabIndex = 11;
            groupBoxSearch.TabStop = false;
            groupBoxSearch.Text = "Search";
            // 
            // groupBoxSort
            // 
            groupBoxSort.Controls.Add(radioButtonRating);
            groupBoxSort.Controls.Add(radioButtonYear);
            groupBoxSort.Location = new Point(295, 45);
            groupBoxSort.Name = "groupBoxSort";
            groupBoxSort.Size = new Size(66, 50);
            groupBoxSort.TabIndex = 14;
            groupBoxSort.TabStop = false;
            groupBoxSort.Text = "Sort";
            // 
            // radioButtonRating
            // 
            radioButtonRating.AutoSize = true;
            radioButtonRating.Checked = true;
            radioButtonRating.Location = new Point(7, 12);
            radioButtonRating.Margin = new Padding(4, 3, 4, 3);
            radioButtonRating.Name = "radioButtonRating";
            radioButtonRating.Size = new Size(59, 19);
            radioButtonRating.TabIndex = 7;
            radioButtonRating.TabStop = true;
            radioButtonRating.Text = "Rating";
            radioButtonRating.UseVisualStyleBackColor = true;
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(7, 29);
            radioButtonYear.Margin = new Padding(4, 3, 4, 3);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(47, 19);
            radioButtonYear.TabIndex = 8;
            radioButtonYear.Text = "Year";
            radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonActor
            // 
            radioButtonActor.AutoSize = true;
            radioButtonActor.Location = new Point(132, 48);
            radioButtonActor.Name = "radioButtonActor";
            radioButtonActor.Size = new Size(54, 19);
            radioButtonActor.TabIndex = 12;
            radioButtonActor.Text = "Actor";
            radioButtonActor.UseVisualStyleBackColor = true;
            radioButtonActor.CheckedChanged += radioButton_CheckedChanged;
            // 
            // radioButtonMovieTitle
            // 
            radioButtonMovieTitle.AutoSize = true;
            radioButtonMovieTitle.Checked = true;
            radioButtonMovieTitle.Location = new Point(32, 48);
            radioButtonMovieTitle.Name = "radioButtonMovieTitle";
            radioButtonMovieTitle.Size = new Size(84, 19);
            radioButtonMovieTitle.TabIndex = 11;
            radioButtonMovieTitle.TabStop = true;
            radioButtonMovieTitle.Text = "Movie Title";
            radioButtonMovieTitle.UseVisualStyleBackColor = true;
            radioButtonMovieTitle.CheckedChanged += radioButton_CheckedChanged;
            // 
            // checkBoxPoster
            // 
            checkBoxPoster.AutoSize = true;
            checkBoxPoster.Checked = true;
            checkBoxPoster.CheckState = CheckState.Checked;
            checkBoxPoster.Location = new Point(295, 114);
            checkBoxPoster.Name = "checkBoxPoster";
            checkBoxPoster.Size = new Size(59, 19);
            checkBoxPoster.TabIndex = 10;
            checkBoxPoster.Text = "Poster";
            checkBoxPoster.UseVisualStyleBackColor = true;
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(295, 15);
            buttonSearch.Margin = new Padding(4, 3, 4, 3);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(88, 27);
            buttonSearch.TabIndex = 0;
            buttonSearch.Text = "Search";
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // checkBoxDecending
            // 
            checkBoxDecending.AutoSize = true;
            checkBoxDecending.Checked = true;
            checkBoxDecending.CheckState = CheckState.Checked;
            checkBoxDecending.Location = new Point(295, 94);
            checkBoxDecending.Margin = new Padding(4, 3, 4, 3);
            checkBoxDecending.Name = "checkBoxDecending";
            checkBoxDecending.Size = new Size(83, 19);
            checkBoxDecending.TabIndex = 6;
            checkBoxDecending.Text = "Decending";
            checkBoxDecending.UseVisualStyleBackColor = true;
            // 
            // linkLabelOpenInBrowser
            // 
            linkLabelOpenInBrowser.AutoSize = true;
            linkLabelOpenInBrowser.Location = new Point(33, 124);
            linkLabelOpenInBrowser.Margin = new Padding(4, 0, 4, 0);
            linkLabelOpenInBrowser.Name = "linkLabelOpenInBrowser";
            linkLabelOpenInBrowser.Size = new Size(94, 15);
            linkLabelOpenInBrowser.TabIndex = 9;
            linkLabelOpenInBrowser.TabStop = true;
            linkLabelOpenInBrowser.Text = "Open In Browser";
            linkLabelOpenInBrowser.LinkClicked += linkLabelOpenInBrowser_LinkClicked;
            // 
            // linkLabelYear
            // 
            linkLabelYear.AutoSize = true;
            linkLabelYear.Location = new Point(33, 106);
            linkLabelYear.Margin = new Padding(4, 0, 4, 0);
            linkLabelYear.Name = "linkLabelYear";
            linkLabelYear.Size = new Size(29, 15);
            linkLabelYear.TabIndex = 5;
            linkLabelYear.TabStop = true;
            linkLabelYear.Text = "Year";
            // 
            // linkLabelGenre
            // 
            linkLabelGenre.AutoSize = true;
            linkLabelGenre.Location = new Point(33, 75);
            linkLabelGenre.Margin = new Padding(4, 0, 4, 0);
            linkLabelGenre.Name = "linkLabelGenre";
            linkLabelGenre.Size = new Size(38, 15);
            linkLabelGenre.TabIndex = 4;
            linkLabelGenre.TabStop = true;
            linkLabelGenre.Text = "Genre";
            // 
            // tbSearch
            // 
            tbSearch.Location = new Point(31, 20);
            tbSearch.Margin = new Padding(4, 3, 4, 3);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new Size(247, 23);
            tbSearch.TabIndex = 1;
            // 
            // comboBoxYear
            // 
            comboBoxYear.FormattingEnabled = true;
            comboBoxYear.Location = new Point(77, 102);
            comboBoxYear.Margin = new Padding(4, 3, 4, 3);
            comboBoxYear.Name = "comboBoxYear";
            comboBoxYear.Size = new Size(140, 23);
            comboBoxYear.TabIndex = 3;
            // 
            // comboBoxGenre
            // 
            comboBoxGenre.FormattingEnabled = true;
            comboBoxGenre.Location = new Point(77, 71);
            comboBoxGenre.Margin = new Padding(4, 3, 4, 3);
            comboBoxGenre.Name = "comboBoxGenre";
            comboBoxGenre.Size = new Size(140, 23);
            comboBoxGenre.TabIndex = 2;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 487);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 12, 0);
            statusStrip1.Size = new Size(1313, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(1300, 17);
            toolStripStatusLabel1.Spring = true;
            toolStripStatusLabel1.Text = "Search for a database item.";
            // 
            // BrowseMoviesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1313, 674);
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "BrowseMoviesForm";
            Text = "Browse for Movies";
            FormClosing += BrowseMoviesForm_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxPlex.ResumeLayout(false);
            groupBoxAddToPlayList.ResumeLayout(false);
            groupBoxSearcByFolder.ResumeLayout(false);
            groupBoxSearcByFolder.PerformLayout();
            groupBoxVisualize.ResumeLayout(false);
            groupBoxSearch.ResumeLayout(false);
            groupBoxSearch.PerformLayout();
            groupBoxSort.ResumeLayout(false);
            groupBoxSort.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private ContainsTextBox tbSearch;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.RadioButton radioButtonYear;
        private System.Windows.Forms.RadioButton radioButtonRating;
        private System.Windows.Forms.CheckBox checkBoxDecending;
        private System.Windows.Forms.LinkLabel linkLabelYear;
        private System.Windows.Forms.LinkLabel linkLabelGenre;
        private System.Windows.Forms.ComboBox comboBoxYear;
        private System.Windows.Forms.LinkLabel linkLabelOpenInBrowser;
        private System.Windows.Forms.GroupBox groupBoxVisualize;
        private System.Windows.Forms.Button buttonVisualize;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private GroupBox groupBoxSearcByFolder;
        private Button buttonOpenFolder;
        private CheckBox checkBoxPrintable;
        private CheckBox checkBoxPoster;
        private RadioButton radioButtonActor;
        private RadioButton radioButtonMovieTitle;
        private GroupBox groupBoxAddToPlayList;
        private Button buttonAddToPlexPlaylist;
        private GroupBox groupBoxSort;
        private ComboBox comboBoxWatchLists;
        private Button buttonSync;
        private ToolTip toolTip1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private GroupBox groupBoxPlex;
        private Button buttonWatchList;
        private Button buttonPlaylist;
        private Button buttonVisualizePlex;
        private Button buttonVisualizeDBPlaylist;
    }
}