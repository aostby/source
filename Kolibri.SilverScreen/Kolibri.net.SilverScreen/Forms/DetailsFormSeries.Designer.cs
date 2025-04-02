
namespace Kolibri.net.SilverScreen.Forms
{
    partial class DetailsFormSeries
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailsFormSeries));
            label8 = new Label();
            labelMetascore = new Label();
            labelActors = new Label();
            labelGenre = new Label();
            labelRuntime = new Label();
            labelYear = new Label();
            labelTitle = new Label();
            tbMetascore = new TextBox();
            tbPlot = new RichTextBox();
            tbActors = new TextBox();
            tbGenre = new TextBox();
            tbRuntime = new TextBox();
            tbRated = new TextBox();
            tbYear = new TextBox();
            tbTitle = new TextBox();
            pbPoster = new PictureBox();
            linkTrailer = new LinkLabel();
            linkLabelOpenFilepath = new LinkLabel();
            buttonDeleteItem = new Button();
            labelFileExists = new Label();
            labelQuality = new Label();
            buttonSearch = new Button();
            toolTipDetail = new ToolTip(components);
            buttonRediger = new Button();
            buttonSubtitleSearch = new Button();
            labelTotalSeasons = new Label();
            textBoxTotalSeasons = new TextBox();
            tabControlSeasons = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            splitContainer1 = new SplitContainer();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)pbPoster).BeginInit();
            tabControlSeasons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 605);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(152, 15);
            label8.TabIndex = 36;
            label8.Text = "Plot (double click for more)";
            // 
            // labelMetascore
            // 
            labelMetascore.AutoSize = true;
            labelMetascore.Location = new Point(12, 562);
            labelMetascore.Margin = new Padding(4, 0, 4, 0);
            labelMetascore.Name = "labelMetascore";
            labelMetascore.Size = new Size(62, 15);
            labelMetascore.TabIndex = 35;
            labelMetascore.Text = "Metascore";
            // 
            // labelActors
            // 
            labelActors.AutoSize = true;
            labelActors.Location = new Point(12, 515);
            labelActors.Margin = new Padding(4, 0, 4, 0);
            labelActors.Name = "labelActors";
            labelActors.Size = new Size(41, 15);
            labelActors.TabIndex = 34;
            labelActors.Text = "Actors";
            // 
            // labelGenre
            // 
            labelGenre.AutoSize = true;
            labelGenre.Location = new Point(49, 473);
            labelGenre.Margin = new Padding(4, 0, 4, 0);
            labelGenre.Name = "labelGenre";
            labelGenre.Size = new Size(38, 15);
            labelGenre.TabIndex = 33;
            labelGenre.Text = "Genre";
            // 
            // labelRuntime
            // 
            labelRuntime.AutoSize = true;
            labelRuntime.Location = new Point(181, 562);
            labelRuntime.Margin = new Padding(4, 0, 4, 0);
            labelRuntime.Name = "labelRuntime";
            labelRuntime.Size = new Size(52, 15);
            labelRuntime.TabIndex = 32;
            labelRuntime.Text = "Runtime";
            // 
            // labelYear
            // 
            labelYear.AutoSize = true;
            labelYear.Location = new Point(12, 473);
            labelYear.Margin = new Padding(4, 0, 4, 0);
            labelYear.Name = "labelYear";
            labelYear.Size = new Size(29, 15);
            labelYear.TabIndex = 30;
            labelYear.Text = "Year";
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.ImageAlign = ContentAlignment.MiddleLeft;
            labelTitle.Location = new Point(12, 428);
            labelTitle.Margin = new Padding(4, 0, 4, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(29, 15);
            labelTitle.TabIndex = 29;
            labelTitle.Text = "Title";
            // 
            // tbMetascore
            // 
            tbMetascore.Location = new Point(12, 578);
            tbMetascore.Margin = new Padding(4, 3, 4, 3);
            tbMetascore.Name = "tbMetascore";
            tbMetascore.ReadOnly = true;
            tbMetascore.Size = new Size(158, 23);
            tbMetascore.TabIndex = 28;
            // 
            // tbPlot
            // 
            tbPlot.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbPlot.Location = new Point(12, 623);
            tbPlot.Margin = new Padding(4, 3, 4, 3);
            tbPlot.Name = "tbPlot";
            tbPlot.ReadOnly = true;
            tbPlot.Size = new Size(485, 91);
            tbPlot.TabIndex = 27;
            tbPlot.Text = "";
            tbPlot.DoubleClick += tbPlot_DoubleClick;
            // 
            // tbActors
            // 
            tbActors.Location = new Point(12, 531);
            tbActors.Margin = new Padding(4, 3, 4, 3);
            tbActors.Name = "tbActors";
            tbActors.ReadOnly = true;
            tbActors.Size = new Size(485, 23);
            tbActors.TabIndex = 25;
            tbActors.Click += tbActors_Clicked;
            // 
            // tbGenre
            // 
            tbGenre.Location = new Point(49, 488);
            tbGenre.Margin = new Padding(4, 3, 4, 3);
            tbGenre.Name = "tbGenre";
            tbGenre.ReadOnly = true;
            tbGenre.Size = new Size(447, 23);
            tbGenre.TabIndex = 24;
            // 
            // tbRuntime
            // 
            tbRuntime.Location = new Point(181, 578);
            tbRuntime.Margin = new Padding(4, 3, 4, 3);
            tbRuntime.Name = "tbRuntime";
            tbRuntime.ReadOnly = true;
            tbRuntime.Size = new Size(158, 23);
            tbRuntime.TabIndex = 23;
            // 
            // tbRated
            // 
            tbRated.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbRated.Location = new Point(192, 399);
            tbRated.Margin = new Padding(4, 3, 4, 3);
            tbRated.Name = "tbRated";
            tbRated.ReadOnly = true;
            tbRated.Size = new Size(60, 20);
            tbRated.TabIndex = 22;
            // 
            // tbYear
            // 
            tbYear.Location = new Point(12, 488);
            tbYear.Margin = new Padding(4, 3, 4, 3);
            tbYear.Name = "tbYear";
            tbYear.ReadOnly = true;
            tbYear.Size = new Size(41, 23);
            tbYear.TabIndex = 21;
            // 
            // tbTitle
            // 
            tbTitle.Location = new Point(12, 447);
            tbTitle.Margin = new Padding(4, 3, 4, 3);
            tbTitle.Name = "tbTitle";
            tbTitle.ReadOnly = true;
            tbTitle.Size = new Size(485, 23);
            tbTitle.TabIndex = 20;
            // 
            // pbPoster
            // 
            pbPoster.Location = new Point(14, 12);
            pbPoster.Margin = new Padding(4, 3, 4, 3);
            pbPoster.Name = "pbPoster";
            pbPoster.Size = new Size(483, 378);
            pbPoster.SizeMode = PictureBoxSizeMode.Zoom;
            pbPoster.TabIndex = 26;
            pbPoster.TabStop = false;
            pbPoster.Click += pbPoster_Click;
            pbPoster.MouseHover += pbPoster_MouseHover;
            // 
            // linkTrailer
            // 
            linkTrailer.AutoSize = true;
            linkTrailer.BorderStyle = BorderStyle.FixedSingle;
            linkTrailer.Location = new Point(14, 402);
            linkTrailer.Margin = new Padding(4, 0, 4, 0);
            linkTrailer.Name = "linkTrailer";
            linkTrailer.Size = new Size(76, 17);
            linkTrailer.TabIndex = 40;
            linkTrailer.TabStop = true;
            linkTrailer.Text = "Watch trailer";
            toolTipDetail.SetToolTip(linkTrailer, "Åpne media i IMDB hvis mulig");
            linkTrailer.LinkClicked += link_LinkClicked;
            // 
            // linkLabelOpenFilepath
            // 
            linkLabelOpenFilepath.AutoSize = true;
            linkLabelOpenFilepath.BorderStyle = BorderStyle.FixedSingle;
            linkLabelOpenFilepath.Location = new Point(102, 402);
            linkLabelOpenFilepath.Margin = new Padding(4, 0, 4, 0);
            linkLabelOpenFilepath.Name = "linkLabelOpenFilepath";
            linkLabelOpenFilepath.Size = new Size(81, 17);
            linkLabelOpenFilepath.TabIndex = 42;
            linkLabelOpenFilepath.TabStop = true;
            linkLabelOpenFilepath.Text = "Open filepath";
            toolTipDetail.SetToolTip(linkLabelOpenFilepath, "Åpne filstien lokalt");
            linkLabelOpenFilepath.LinkClicked += link_LinkClicked;
            // 
            // buttonDeleteItem
            // 
            buttonDeleteItem.Location = new Point(337, 398);
            buttonDeleteItem.Margin = new Padding(4, 3, 4, 3);
            buttonDeleteItem.Name = "buttonDeleteItem";
            buttonDeleteItem.Size = new Size(75, 23);
            buttonDeleteItem.TabIndex = 43;
            buttonDeleteItem.Text = "Slett fra DB (Ikke fra disk)";
            toolTipDetail.SetToolTip(buttonDeleteItem, "Sletter dette objektet fra DB, men ikke fra disk.");
            buttonDeleteItem.UseVisualStyleBackColor = true;
            buttonDeleteItem.Click += buttonDeleteItem_Click;
            // 
            // labelFileExists
            // 
            labelFileExists.AutoSize = true;
            labelFileExists.ForeColor = Color.ForestGreen;
            labelFileExists.Location = new Point(100, 424);
            labelFileExists.Margin = new Padding(4, 0, 4, 0);
            labelFileExists.Name = "labelFileExists";
            labelFileExists.Size = new Size(36, 15);
            labelFileExists.TabIndex = 44;
            labelFileExists.Text = "Exists";
            toolTipDetail.SetToolTip(labelFileExists, "Finnes filen på disk");
            // 
            // labelQuality
            // 
            labelQuality.AutoSize = true;
            labelQuality.Location = new Point(158, 424);
            labelQuality.Margin = new Padding(4, 0, 4, 0);
            labelQuality.Name = "labelQuality";
            labelQuality.Size = new Size(25, 15);
            labelQuality.TabIndex = 45;
            labelQuality.Text = "HQ";
            toolTipDetail.SetToolTip(labelQuality, "Filmkvalitet");
            // 
            // buttonSearch
            // 
            buttonSearch.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSearch.Location = new Point(422, 396);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(75, 23);
            buttonSearch.TabIndex = 46;
            buttonSearch.Text = "Search";
            buttonSearch.TextAlign = ContentAlignment.MiddleRight;
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // buttonRediger
            // 
            buttonRediger.Location = new Point(259, 398);
            buttonRediger.Name = "buttonRediger";
            buttonRediger.Size = new Size(75, 23);
            buttonRediger.TabIndex = 47;
            buttonRediger.Text = "Rediger";
            buttonRediger.UseVisualStyleBackColor = true;
            buttonRediger.Click += buttonRediger_Click;
            // 
            // buttonSubtitleSearch
            // 
            buttonSubtitleSearch.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSubtitleSearch.Location = new Point(422, 420);
            buttonSubtitleSearch.Name = "buttonSubtitleSearch";
            buttonSubtitleSearch.Size = new Size(75, 23);
            buttonSubtitleSearch.TabIndex = 48;
            buttonSubtitleSearch.Text = "Subtitle search";
            buttonSubtitleSearch.TextAlign = ContentAlignment.MiddleRight;
            buttonSubtitleSearch.UseVisualStyleBackColor = true;
            buttonSubtitleSearch.Click += buttonSubtitleSearch_Click;
            // 
            // labelTotalSeasons
            // 
            labelTotalSeasons.AutoSize = true;
            labelTotalSeasons.Location = new Point(360, 562);
            labelTotalSeasons.Margin = new Padding(4, 0, 4, 0);
            labelTotalSeasons.Name = "labelTotalSeasons";
            labelTotalSeasons.Size = new Size(74, 15);
            labelTotalSeasons.TabIndex = 51;
            labelTotalSeasons.Text = "TotalSeasons";
            // 
            // textBoxTotalSeasons
            // 
            textBoxTotalSeasons.Location = new Point(360, 578);
            textBoxTotalSeasons.Margin = new Padding(4, 3, 4, 3);
            textBoxTotalSeasons.Name = "textBoxTotalSeasons";
            textBoxTotalSeasons.ReadOnly = true;
            textBoxTotalSeasons.Size = new Size(137, 23);
            textBoxTotalSeasons.TabIndex = 50;
            // 
            // tabControlSeasons
            // 
            tabControlSeasons.Controls.Add(tabPage1);
            tabControlSeasons.Controls.Add(tabPage2);
            tabControlSeasons.Dock = DockStyle.Fill;
            tabControlSeasons.Location = new Point(0, 0);
            tabControlSeasons.Name = "tabControlSeasons";
            tabControlSeasons.SelectedIndex = 0;
            tabControlSeasons.Size = new Size(284, 702);
            tabControlSeasons.TabIndex = 52;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(276, 674);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(276, 674);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(504, 12);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControlSeasons);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(webView21);
            splitContainer1.Size = new Size(854, 702);
            splitContainer1.SplitterDistance = 284;
            splitContainer1.TabIndex = 53;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new Size(566, 702);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // DetailsFormSeries
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1347, 754);
            Controls.Add(splitContainer1);
            Controls.Add(labelTotalSeasons);
            Controls.Add(textBoxTotalSeasons);
            Controls.Add(buttonSubtitleSearch);
            Controls.Add(buttonRediger);
            Controls.Add(buttonSearch);
            Controls.Add(labelQuality);
            Controls.Add(labelFileExists);
            Controls.Add(buttonDeleteItem);
            Controls.Add(linkLabelOpenFilepath);
            Controls.Add(linkTrailer);
            Controls.Add(label8);
            Controls.Add(labelMetascore);
            Controls.Add(labelActors);
            Controls.Add(labelGenre);
            Controls.Add(labelRuntime);
            Controls.Add(labelYear);
            Controls.Add(labelTitle);
            Controls.Add(tbMetascore);
            Controls.Add(tbPlot);
            Controls.Add(pbPoster);
            Controls.Add(tbActors);
            Controls.Add(tbGenre);
            Controls.Add(tbRuntime);
            Controls.Add(tbRated);
            Controls.Add(tbYear);
            Controls.Add(tbTitle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(16, 39);
            Name = "DetailsFormSeries";
            Text = "Serie Detail ";
            KeyDown += MovieDetailsForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pbPoster).EndInit();
            tabControlSeasons.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelMetascore;
        private System.Windows.Forms.Label labelActors;
        private System.Windows.Forms.Label labelGenre;
        private System.Windows.Forms.Label labelRuntime;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox tbMetascore;
        internal System.Windows.Forms.RichTextBox tbPlot;
        internal System.Windows.Forms.PictureBox pbPoster;
        private System.Windows.Forms.TextBox tbActors;
        private System.Windows.Forms.TextBox tbGenre;
        private System.Windows.Forms.TextBox tbRuntime;
        private System.Windows.Forms.TextBox tbRated;
        private System.Windows.Forms.TextBox tbYear;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.LinkLabel linkTrailer;
        private System.Windows.Forms.LinkLabel linkLabelOpenFilepath;
        private System.Windows.Forms.Button buttonDeleteItem;
        private System.Windows.Forms.Label labelFileExists;
        private System.Windows.Forms.Label labelQuality;
        private Button buttonSearch;
        private ToolTip toolTipDetail;
        private Button buttonRediger;
        private Button buttonSubtitleSearch;
        private Label labelTotalSeasons;
        private TextBox textBoxTotalSeasons;
        private TabControl tabControlSeasons;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private SplitContainer splitContainer1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
    }
}