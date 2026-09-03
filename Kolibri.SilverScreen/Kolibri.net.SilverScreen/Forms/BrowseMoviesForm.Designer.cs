
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
            buttonWatchList = new Button();
            groupBoxPlayList = new GroupBox();
            buttonSync = new Button();
            buttonPlaylist = new Button();
            comboBoxWatchLists = new ComboBox();
            button1 = new Button();
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
            groupBoxPlayList.SuspendLayout();
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
            splitContainer1.Margin = new Padding(5, 4, 5, 4);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBoxPlex);
            splitContainer1.Panel1.Controls.Add(groupBoxPlayList);
            splitContainer1.Panel1.Controls.Add(groupBoxSearcByFolder);
            splitContainer1.Panel1.Controls.Add(groupBoxVisualize);
            splitContainer1.Panel1.Controls.Add(groupBoxSearch);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(statusStrip1);
            splitContainer1.Size = new Size(1501, 899);
            splitContainer1.SplitterDistance = 214;
            splitContainer1.SplitterWidth = 7;
            splitContainer1.TabIndex = 0;
            // 
            // groupBoxPlex
            // 
            groupBoxPlex.Controls.Add(buttonWatchList);
            groupBoxPlex.Location = new Point(989, 24);
            groupBoxPlex.Name = "groupBoxPlex";
            groupBoxPlex.Size = new Size(500, 173);
            groupBoxPlex.TabIndex = 16;
            groupBoxPlex.TabStop = false;
            groupBoxPlex.Text = "Plex";
            // 
            // buttonWatchList
            // 
            buttonWatchList.Location = new Point(6, 21);
            buttonWatchList.Name = "buttonWatchList";
            buttonWatchList.Size = new Size(94, 29);
            buttonWatchList.TabIndex = 0;
            buttonWatchList.Text = "Watchlist";
            toolTip1.SetToolTip(buttonWatchList, "Vis Watchlist fra Plex");
            buttonWatchList.UseVisualStyleBackColor = true;
            buttonWatchList.Click += buttonWatchList_Click;
            // 
            // groupBoxPlayList
            // 
            groupBoxPlayList.Controls.Add(buttonSync);
            groupBoxPlayList.Controls.Add(buttonPlaylist);
            groupBoxPlayList.Controls.Add(comboBoxWatchLists);
            groupBoxPlayList.Controls.Add(button1);
            groupBoxPlayList.Location = new Point(757, 11);
            groupBoxPlayList.Margin = new Padding(3, 4, 3, 4);
            groupBoxPlayList.Name = "groupBoxPlayList";
            groupBoxPlayList.Padding = new Padding(3, 4, 3, 4);
            groupBoxPlayList.Size = new Size(217, 189);
            groupBoxPlayList.TabIndex = 15;
            groupBoxPlayList.TabStop = false;
            groupBoxPlayList.Text = "Add to Playlist";
            // 
            // buttonSync
            // 
            buttonSync.Location = new Point(115, 131);
            buttonSync.Margin = new Padding(3, 4, 3, 4);
            buttonSync.Name = "buttonSync";
            buttonSync.Size = new Size(95, 31);
            buttonSync.TabIndex = 3;
            buttonSync.Text = "Plex Sync";
            toolTip1.SetToolTip(buttonSync, "Sync fra lokal DB til Plex");
            buttonSync.UseVisualStyleBackColor = true;
            buttonSync.Click += buttonSync_Click;
            // 
            // buttonPlaylist
            // 
            buttonPlaylist.Location = new Point(7, 131);
            buttonPlaylist.Margin = new Padding(3, 4, 3, 4);
            buttonPlaylist.Name = "buttonPlaylist";
            buttonPlaylist.Size = new Size(95, 31);
            buttonPlaylist.TabIndex = 2;
            buttonPlaylist.Text = "DB Playlist";
            toolTip1.SetToolTip(buttonPlaylist, "Vis denne spillelisten som den er lagret i lokal database.");
            buttonPlaylist.UseVisualStyleBackColor = true;
            buttonPlaylist.Click += buttonPlaylist_Click;
            // 
            // comboBoxWatchLists
            // 
            comboBoxWatchLists.FormattingEnabled = true;
            comboBoxWatchLists.Location = new Point(7, 96);
            comboBoxWatchLists.Margin = new Padding(3, 4, 3, 4);
            comboBoxWatchLists.Name = "comboBoxWatchLists";
            comboBoxWatchLists.Size = new Size(203, 28);
            comboBoxWatchLists.TabIndex = 1;
            comboBoxWatchLists.SelectedIndexChanged += comboBoxWatchLists_SelectedIndexChanged;
            // 
            // button1
            // 
            button1.Location = new Point(7, 24);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(203, 64);
            button1.TabIndex = 0;
            button1.Text = "Add To Playlist (Plex)";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBoxSearcByFolder
            // 
            groupBoxSearcByFolder.Controls.Add(checkBoxPrintable);
            groupBoxSearcByFolder.Controls.Add(buttonOpenFolder);
            groupBoxSearcByFolder.Location = new Point(481, 11);
            groupBoxSearcByFolder.Margin = new Padding(3, 4, 3, 4);
            groupBoxSearcByFolder.Name = "groupBoxSearcByFolder";
            groupBoxSearcByFolder.Padding = new Padding(3, 4, 3, 4);
            groupBoxSearcByFolder.Size = new Size(266, 88);
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
            checkBoxPrintable.Location = new Point(49, 37);
            checkBoxPrintable.Margin = new Padding(3, 4, 3, 4);
            checkBoxPrintable.Name = "checkBoxPrintable";
            checkBoxPrintable.Size = new Size(90, 24);
            checkBoxPrintable.TabIndex = 2;
            checkBoxPrintable.Text = "Printable";
            checkBoxPrintable.UseVisualStyleBackColor = false;
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonOpenFolder.Location = new Point(9, 29);
            buttonOpenFolder.Margin = new Padding(3, 4, 3, 4);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(249, 39);
            buttonOpenFolder.TabIndex = 1;
            buttonOpenFolder.Text = "Let opp mappe";
            buttonOpenFolder.TextAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.Click += buttonOpenFolder_Click;
            // 
            // groupBoxVisualize
            // 
            groupBoxVisualize.Controls.Add(buttonVisualize);
            groupBoxVisualize.Location = new Point(481, 111);
            groupBoxVisualize.Margin = new Padding(5, 4, 5, 4);
            groupBoxVisualize.Name = "groupBoxVisualize";
            groupBoxVisualize.Padding = new Padding(5, 4, 5, 4);
            groupBoxVisualize.Size = new Size(266, 87);
            groupBoxVisualize.TabIndex = 12;
            groupBoxVisualize.TabStop = false;
            groupBoxVisualize.Text = "Visualize";
            // 
            // buttonVisualize
            // 
            buttonVisualize.Location = new Point(9, 31);
            buttonVisualize.Margin = new Padding(5, 4, 5, 4);
            buttonVisualize.Name = "buttonVisualize";
            buttonVisualize.Size = new Size(249, 36);
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
            groupBoxSearch.Location = new Point(9, 11);
            groupBoxSearch.Margin = new Padding(5, 4, 5, 4);
            groupBoxSearch.Name = "groupBoxSearch";
            groupBoxSearch.Padding = new Padding(5, 4, 5, 4);
            groupBoxSearch.Size = new Size(464, 189);
            groupBoxSearch.TabIndex = 11;
            groupBoxSearch.TabStop = false;
            groupBoxSearch.Text = "Search";
            // 
            // groupBoxSort
            // 
            groupBoxSort.Controls.Add(radioButtonRating);
            groupBoxSort.Controls.Add(radioButtonYear);
            groupBoxSort.Location = new Point(337, 60);
            groupBoxSort.Margin = new Padding(3, 4, 3, 4);
            groupBoxSort.Name = "groupBoxSort";
            groupBoxSort.Padding = new Padding(3, 4, 3, 4);
            groupBoxSort.Size = new Size(75, 67);
            groupBoxSort.TabIndex = 14;
            groupBoxSort.TabStop = false;
            groupBoxSort.Text = "Sort";
            // 
            // radioButtonRating
            // 
            radioButtonRating.AutoSize = true;
            radioButtonRating.Checked = true;
            radioButtonRating.Location = new Point(8, 16);
            radioButtonRating.Margin = new Padding(5, 4, 5, 4);
            radioButtonRating.Name = "radioButtonRating";
            radioButtonRating.Size = new Size(73, 24);
            radioButtonRating.TabIndex = 7;
            radioButtonRating.TabStop = true;
            radioButtonRating.Text = "Rating";
            radioButtonRating.UseVisualStyleBackColor = true;
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(8, 39);
            radioButtonYear.Margin = new Padding(5, 4, 5, 4);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(58, 24);
            radioButtonYear.TabIndex = 8;
            radioButtonYear.Text = "Year";
            radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonActor
            // 
            radioButtonActor.AutoSize = true;
            radioButtonActor.Location = new Point(151, 64);
            radioButtonActor.Margin = new Padding(3, 4, 3, 4);
            radioButtonActor.Name = "radioButtonActor";
            radioButtonActor.Size = new Size(66, 24);
            radioButtonActor.TabIndex = 12;
            radioButtonActor.Text = "Actor";
            radioButtonActor.UseVisualStyleBackColor = true;
            radioButtonActor.CheckedChanged += radioButton_CheckedChanged;
            // 
            // radioButtonMovieTitle
            // 
            radioButtonMovieTitle.AutoSize = true;
            radioButtonMovieTitle.Checked = true;
            radioButtonMovieTitle.Location = new Point(37, 64);
            radioButtonMovieTitle.Margin = new Padding(3, 4, 3, 4);
            radioButtonMovieTitle.Name = "radioButtonMovieTitle";
            radioButtonMovieTitle.Size = new Size(104, 24);
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
            checkBoxPoster.Location = new Point(337, 152);
            checkBoxPoster.Margin = new Padding(3, 4, 3, 4);
            checkBoxPoster.Name = "checkBoxPoster";
            checkBoxPoster.Size = new Size(71, 24);
            checkBoxPoster.TabIndex = 10;
            checkBoxPoster.Text = "Poster";
            checkBoxPoster.UseVisualStyleBackColor = true;
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(337, 20);
            buttonSearch.Margin = new Padding(5, 4, 5, 4);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(101, 36);
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
            checkBoxDecending.Location = new Point(337, 125);
            checkBoxDecending.Margin = new Padding(5, 4, 5, 4);
            checkBoxDecending.Name = "checkBoxDecending";
            checkBoxDecending.Size = new Size(103, 24);
            checkBoxDecending.TabIndex = 6;
            checkBoxDecending.Text = "Decending";
            checkBoxDecending.UseVisualStyleBackColor = true;
            // 
            // linkLabelOpenInBrowser
            // 
            linkLabelOpenInBrowser.AutoSize = true;
            linkLabelOpenInBrowser.Location = new Point(38, 166);
            linkLabelOpenInBrowser.Margin = new Padding(5, 0, 5, 0);
            linkLabelOpenInBrowser.Name = "linkLabelOpenInBrowser";
            linkLabelOpenInBrowser.Size = new Size(118, 20);
            linkLabelOpenInBrowser.TabIndex = 9;
            linkLabelOpenInBrowser.TabStop = true;
            linkLabelOpenInBrowser.Text = "Open In Browser";
            linkLabelOpenInBrowser.LinkClicked += linkLabelOpenInBrowser_LinkClicked;
            // 
            // linkLabelYear
            // 
            linkLabelYear.AutoSize = true;
            linkLabelYear.Location = new Point(38, 141);
            linkLabelYear.Margin = new Padding(5, 0, 5, 0);
            linkLabelYear.Name = "linkLabelYear";
            linkLabelYear.Size = new Size(37, 20);
            linkLabelYear.TabIndex = 5;
            linkLabelYear.TabStop = true;
            linkLabelYear.Text = "Year";
            // 
            // linkLabelGenre
            // 
            linkLabelGenre.AutoSize = true;
            linkLabelGenre.Location = new Point(38, 100);
            linkLabelGenre.Margin = new Padding(5, 0, 5, 0);
            linkLabelGenre.Name = "linkLabelGenre";
            linkLabelGenre.Size = new Size(48, 20);
            linkLabelGenre.TabIndex = 4;
            linkLabelGenre.TabStop = true;
            linkLabelGenre.Text = "Genre";
            // 
            // tbSearch
            // 
            tbSearch.Location = new Point(35, 27);
            tbSearch.Margin = new Padding(5, 4, 5, 4);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new Size(282, 27);
            tbSearch.TabIndex = 1;
            // 
            // comboBoxYear
            // 
            comboBoxYear.FormattingEnabled = true;
            comboBoxYear.Location = new Point(88, 136);
            comboBoxYear.Margin = new Padding(5, 4, 5, 4);
            comboBoxYear.Name = "comboBoxYear";
            comboBoxYear.Size = new Size(159, 28);
            comboBoxYear.TabIndex = 3;
            // 
            // comboBoxGenre
            // 
            comboBoxGenre.FormattingEnabled = true;
            comboBoxGenre.Location = new Point(88, 95);
            comboBoxGenre.Margin = new Padding(5, 4, 5, 4);
            comboBoxGenre.Name = "comboBoxGenre";
            comboBoxGenre.Size = new Size(159, 28);
            comboBoxGenre.TabIndex = 2;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 652);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1501, 26);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(1486, 20);
            toolStripStatusLabel1.Spring = true;
            toolStripStatusLabel1.Text = "Search for a database item.";
            // 
            // BrowseMoviesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1501, 899);
            Controls.Add(splitContainer1);
            Margin = new Padding(5, 4, 5, 4);
            Name = "BrowseMoviesForm";
            Text = "Browse for Movies";
            FormClosing += BrowseMoviesForm_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxPlex.ResumeLayout(false);
            groupBoxPlayList.ResumeLayout(false);
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
        private GroupBox groupBoxPlayList;
        private Button button1;
        private GroupBox groupBoxSort;
        private ComboBox comboBoxWatchLists;
        private Button buttonPlaylist;
        private Button buttonSync;
        private ToolTip toolTip1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private GroupBox groupBoxPlex;
        private Button buttonWatchList;
    }
}