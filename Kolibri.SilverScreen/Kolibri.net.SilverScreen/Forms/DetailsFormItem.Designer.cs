
namespace Kolibri.net.SilverScreen.Forms
{
    partial class DetailsFormItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailsFormItem));
            labelPlot = new Label();
            labelMetascore = new Label();
            labelActors = new Label();
            labelGenre = new Label();
            labelRuntime = new Label();
            labelYear = new Label();
            label1 = new Label();
            tbMetascore = new TextBox();
            tbPlot = new RichTextBox();
            tbActors = new TextBox();
            tbGenre = new TextBox();
            tbRuntime = new TextBox();
            tbIMDBRated = new TextBox();
            tbYear = new TextBox();
            tbTitle = new TextBox();
            pbPoster = new PictureBox();
            linkTrailer = new LinkLabel();
            linkLabelOpenFilepath = new LinkLabel();
            buttonDeleteItem = new Button();
            labelQuality = new Label();
            buttonSimilar = new Button();
            toolTipDetail = new ToolTip(components);
            buttonDeleteReference = new Button();
            buttonAddPL = new Button();
            buttonOpenPl = new Button();
            buttonRediger = new Button();
            buttonSubtitleSearch = new Button();
            buttonPosterFix = new Button();
            labelAdded = new Label();
            tbAdded = new TextBox();
            labelRated = new Label();
            tbRated = new TextBox();
            buttonReviews = new Button();
            ((System.ComponentModel.ISupportInitialize)pbPoster).BeginInit();
            SuspendLayout();
            // 
            // labelPlot
            // 
            labelPlot.AutoSize = true;
            labelPlot.Location = new Point(14, 803);
            labelPlot.Margin = new Padding(5, 0, 5, 0);
            labelPlot.Name = "labelPlot";
            labelPlot.Size = new Size(191, 20);
            labelPlot.TabIndex = 36;
            labelPlot.Text = "Plot (double click for more)";
            labelPlot.DoubleClick += tbPlot_DoubleClick;
            // 
            // labelMetascore
            // 
            labelMetascore.AutoSize = true;
            labelMetascore.Location = new Point(217, 749);
            labelMetascore.Margin = new Padding(5, 0, 5, 0);
            labelMetascore.Name = "labelMetascore";
            labelMetascore.Size = new Size(78, 20);
            labelMetascore.TabIndex = 35;
            labelMetascore.Text = "Metascore";
            // 
            // labelActors
            // 
            labelActors.AutoSize = true;
            labelActors.Location = new Point(14, 687);
            labelActors.Margin = new Padding(5, 0, 5, 0);
            labelActors.Name = "labelActors";
            labelActors.Size = new Size(207, 20);
            labelActors.TabIndex = 34;
            labelActors.Text = "Actors (double click for more)";
            // 
            // labelGenre
            // 
            labelGenre.AutoSize = true;
            labelGenre.Location = new Point(56, 631);
            labelGenre.Margin = new Padding(5, 0, 5, 0);
            labelGenre.Name = "labelGenre";
            labelGenre.Size = new Size(48, 20);
            labelGenre.TabIndex = 33;
            labelGenre.Text = "Genre";
            // 
            // labelRuntime
            // 
            labelRuntime.AutoSize = true;
            labelRuntime.Location = new Point(307, 749);
            labelRuntime.Margin = new Padding(5, 0, 5, 0);
            labelRuntime.Name = "labelRuntime";
            labelRuntime.Size = new Size(64, 20);
            labelRuntime.TabIndex = 32;
            labelRuntime.Text = "Runtime";
            // 
            // labelYear
            // 
            labelYear.AutoSize = true;
            labelYear.Location = new Point(14, 631);
            labelYear.Margin = new Padding(5, 0, 5, 0);
            labelYear.Name = "labelYear";
            labelYear.Size = new Size(37, 20);
            labelYear.TabIndex = 30;
            labelYear.Text = "Year";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ImageAlign = ContentAlignment.MiddleLeft;
            label1.Location = new Point(14, 571);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(38, 20);
            label1.TabIndex = 29;
            label1.Text = "Title";
            // 
            // tbMetascore
            // 
            tbMetascore.Location = new Point(217, 773);
            tbMetascore.Margin = new Padding(5, 4, 5, 4);
            tbMetascore.Name = "tbMetascore";
            tbMetascore.ReadOnly = true;
            tbMetascore.Size = new Size(81, 27);
            tbMetascore.TabIndex = 28;
            // 
            // tbPlot
            // 
            tbPlot.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            tbPlot.Location = new Point(14, 831);
            tbPlot.Margin = new Padding(5, 4, 5, 4);
            tbPlot.Name = "tbPlot";
            tbPlot.ReadOnly = true;
            tbPlot.Size = new Size(554, 157);
            tbPlot.TabIndex = 27;
            tbPlot.Text = "";
            tbPlot.DoubleClick += tbPlot_DoubleClick;
            // 
            // tbActors
            // 
            tbActors.Location = new Point(14, 708);
            tbActors.Margin = new Padding(5, 4, 5, 4);
            tbActors.Name = "tbActors";
            tbActors.ReadOnly = true;
            tbActors.Size = new Size(554, 27);
            tbActors.TabIndex = 25;
            tbActors.Click += tbActors_Clicked;
            // 
            // tbGenre
            // 
            tbGenre.Location = new Point(56, 651);
            tbGenre.Margin = new Padding(5, 4, 5, 4);
            tbGenre.Name = "tbGenre";
            tbGenre.ReadOnly = true;
            tbGenre.Size = new Size(407, 27);
            tbGenre.TabIndex = 24;
            // 
            // tbRuntime
            // 
            tbRuntime.Location = new Point(307, 773);
            tbRuntime.Margin = new Padding(5, 4, 5, 4);
            tbRuntime.Name = "tbRuntime";
            tbRuntime.ReadOnly = true;
            tbRuntime.Size = new Size(81, 27);
            tbRuntime.TabIndex = 23;
            // 
            // tbIMDBRated
            // 
            tbIMDBRated.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tbIMDBRated.Location = new Point(225, 532);
            tbIMDBRated.Margin = new Padding(5, 4, 5, 4);
            tbIMDBRated.Name = "tbIMDBRated";
            tbIMDBRated.ReadOnly = true;
            tbIMDBRated.Size = new Size(68, 23);
            tbIMDBRated.TabIndex = 22;
            // 
            // tbYear
            // 
            tbYear.Location = new Point(14, 651);
            tbYear.Margin = new Padding(5, 4, 5, 4);
            tbYear.Name = "tbYear";
            tbYear.ReadOnly = true;
            tbYear.Size = new Size(46, 27);
            tbYear.TabIndex = 21;
            // 
            // tbTitle
            // 
            tbTitle.Location = new Point(14, 596);
            tbTitle.Margin = new Padding(5, 4, 5, 4);
            tbTitle.Name = "tbTitle";
            tbTitle.ReadOnly = true;
            tbTitle.Size = new Size(554, 27);
            tbTitle.TabIndex = 20;
            // 
            // pbPoster
            // 
            pbPoster.Location = new Point(14, 16);
            pbPoster.Margin = new Padding(5, 4, 5, 4);
            pbPoster.Name = "pbPoster";
            pbPoster.Size = new Size(554, 504);
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
            linkTrailer.Location = new Point(16, 536);
            linkTrailer.Margin = new Padding(5, 0, 5, 0);
            linkTrailer.Name = "linkTrailer";
            linkTrailer.Size = new Size(95, 22);
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
            linkLabelOpenFilepath.Location = new Point(117, 536);
            linkLabelOpenFilepath.Margin = new Padding(5, 0, 5, 0);
            linkLabelOpenFilepath.Name = "linkLabelOpenFilepath";
            linkLabelOpenFilepath.Size = new Size(102, 22);
            linkLabelOpenFilepath.TabIndex = 42;
            linkLabelOpenFilepath.TabStop = true;
            linkLabelOpenFilepath.Text = "Open filepath";
            toolTipDetail.SetToolTip(linkLabelOpenFilepath, "Åpne filstien lokalt");
            linkLabelOpenFilepath.LinkClicked += link_LinkClicked;
            // 
            // buttonDeleteItem
            // 
            buttonDeleteItem.Location = new Point(374, 531);
            buttonDeleteItem.Margin = new Padding(5, 4, 5, 4);
            buttonDeleteItem.Name = "buttonDeleteItem";
            buttonDeleteItem.Size = new Size(118, 31);
            buttonDeleteItem.TabIndex = 43;
            buttonDeleteItem.Text = "Slett metadata";
            toolTipDetail.SetToolTip(buttonDeleteItem, "Sletter dette objektet fra DB, men ikke fra disk.");
            buttonDeleteItem.UseVisualStyleBackColor = true;
            buttonDeleteItem.Click += buttonDeleteItem_Click;
            // 
            // labelQuality
            // 
            labelQuality.AutoSize = true;
            labelQuality.Location = new Point(222, 565);
            labelQuality.Margin = new Padding(5, 0, 5, 0);
            labelQuality.Name = "labelQuality";
            labelQuality.Size = new Size(31, 20);
            labelQuality.TabIndex = 45;
            labelQuality.Text = "HQ";
            toolTipDetail.SetToolTip(labelQuality, "Filmkvalitet");
            // 
            // buttonSimilar
            // 
            buttonSimilar.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSimilar.Location = new Point(500, 531);
            buttonSimilar.Margin = new Padding(3, 4, 3, 4);
            buttonSimilar.Name = "buttonSimilar";
            buttonSimilar.Size = new Size(68, 31);
            buttonSimilar.TabIndex = 46;
            buttonSimilar.Text = "Similar";
            buttonSimilar.TextAlign = ContentAlignment.MiddleRight;
            toolTipDetail.SetToolTip(buttonSimilar, "Se etter lignende filmer");
            buttonSimilar.UseVisualStyleBackColor = true;
            buttonSimilar.Click += buttonSimilar_Click;
            // 
            // buttonDeleteReference
            // 
            buttonDeleteReference.Location = new Point(374, 563);
            buttonDeleteReference.Margin = new Padding(5, 4, 5, 4);
            buttonDeleteReference.Name = "buttonDeleteReference";
            buttonDeleteReference.Size = new Size(118, 31);
            buttonDeleteReference.TabIndex = 51;
            buttonDeleteReference.Text = "Slett referanse";
            toolTipDetail.SetToolTip(buttonDeleteReference, "Sletter dette filobjektet fra DB, men ikke fra disk.");
            buttonDeleteReference.UseVisualStyleBackColor = true;
            buttonDeleteReference.Click += buttonDeleteReference_Click;
            // 
            // buttonAddPL
            // 
            buttonAddPL.Location = new Point(58, 565);
            buttonAddPL.Margin = new Padding(3, 4, 3, 4);
            buttonAddPL.Name = "buttonAddPL";
            buttonAddPL.Size = new Size(78, 31);
            buttonAddPL.TabIndex = 56;
            buttonAddPL.Text = "Add to PL";
            toolTipDetail.SetToolTip(buttonAddPL, "Add to PlayList (plex - Spilleliste)");
            buttonAddPL.UseVisualStyleBackColor = true;
            buttonAddPL.Click += buttonPlaylist_Click;
            // 
            // buttonOpenPl
            // 
            buttonOpenPl.Location = new Point(143, 565);
            buttonOpenPl.Margin = new Padding(3, 4, 3, 4);
            buttonOpenPl.Name = "buttonOpenPl";
            buttonOpenPl.Size = new Size(78, 31);
            buttonOpenPl.TabIndex = 57;
            buttonOpenPl.Text = "Open PL";
            toolTipDetail.SetToolTip(buttonOpenPl, "Find a DB playlist this item is in");
            buttonOpenPl.UseVisualStyleBackColor = true;
            buttonOpenPl.Click += buttonPlaylist_Click;
            // 
            // buttonRediger
            // 
            buttonRediger.Location = new Point(301, 531);
            buttonRediger.Margin = new Padding(3, 4, 3, 4);
            buttonRediger.Name = "buttonRediger";
            buttonRediger.Size = new Size(69, 31);
            buttonRediger.TabIndex = 47;
            buttonRediger.Text = "Rediger";
            buttonRediger.UseVisualStyleBackColor = true;
            buttonRediger.Click += buttonRediger_Click;
            // 
            // buttonSubtitleSearch
            // 
            buttonSubtitleSearch.ImageAlign = ContentAlignment.MiddleLeft;
            buttonSubtitleSearch.Location = new Point(500, 563);
            buttonSubtitleSearch.Margin = new Padding(3, 4, 3, 4);
            buttonSubtitleSearch.Name = "buttonSubtitleSearch";
            buttonSubtitleSearch.Size = new Size(68, 31);
            buttonSubtitleSearch.TabIndex = 48;
            buttonSubtitleSearch.Text = "Subtitle search";
            buttonSubtitleSearch.TextAlign = ContentAlignment.MiddleRight;
            buttonSubtitleSearch.UseVisualStyleBackColor = true;
            buttonSubtitleSearch.Click += buttonSubtitleSearch_Click;
            // 
            // buttonPosterFix
            // 
            buttonPosterFix.Location = new Point(301, 561);
            buttonPosterFix.Margin = new Padding(3, 4, 3, 4);
            buttonPosterFix.Name = "buttonPosterFix";
            buttonPosterFix.Size = new Size(69, 31);
            buttonPosterFix.TabIndex = 49;
            buttonPosterFix.Text = "Poster";
            buttonPosterFix.UseVisualStyleBackColor = true;
            buttonPosterFix.Click += buttonPosterFix_Click;
            // 
            // labelAdded
            // 
            labelAdded.AutoSize = true;
            labelAdded.Location = new Point(398, 749);
            labelAdded.Margin = new Padding(5, 0, 5, 0);
            labelAdded.Name = "labelAdded";
            labelAdded.Size = new Size(54, 20);
            labelAdded.TabIndex = 53;
            labelAdded.Text = "Added";
            // 
            // tbAdded
            // 
            tbAdded.Location = new Point(398, 773);
            tbAdded.Margin = new Padding(5, 4, 5, 4);
            tbAdded.Name = "tbAdded";
            tbAdded.ReadOnly = true;
            tbAdded.Size = new Size(81, 27);
            tbAdded.TabIndex = 52;
            // 
            // labelRated
            // 
            labelRated.AutoSize = true;
            labelRated.Location = new Point(487, 749);
            labelRated.Margin = new Padding(5, 0, 5, 0);
            labelRated.Name = "labelRated";
            labelRated.Size = new Size(48, 20);
            labelRated.TabIndex = 54;
            labelRated.Text = "Rated";
            // 
            // tbRated
            // 
            tbRated.Location = new Point(487, 773);
            tbRated.Margin = new Padding(5, 4, 5, 4);
            tbRated.Name = "tbRated";
            tbRated.ReadOnly = true;
            tbRated.Size = new Size(81, 27);
            tbRated.TabIndex = 55;
            // 
            // buttonReviews
            // 
            buttonReviews.Location = new Point(474, 651);
            buttonReviews.Margin = new Padding(3, 4, 3, 4);
            buttonReviews.Name = "buttonReviews";
            buttonReviews.Size = new Size(86, 31);
            buttonReviews.TabIndex = 58;
            buttonReviews.Text = "Reviews";
            buttonReviews.UseVisualStyleBackColor = true;
            buttonReviews.Click += buttonReviews_Click;
            // 
            // DetailsFormItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 1005);
            Controls.Add(buttonReviews);
            Controls.Add(buttonOpenPl);
            Controls.Add(buttonAddPL);
            Controls.Add(tbRated);
            Controls.Add(labelRated);
            Controls.Add(labelAdded);
            Controls.Add(tbAdded);
            Controls.Add(buttonDeleteReference);
            Controls.Add(buttonPosterFix);
            Controls.Add(buttonSubtitleSearch);
            Controls.Add(buttonRediger);
            Controls.Add(buttonSimilar);
            Controls.Add(labelQuality);
            Controls.Add(buttonDeleteItem);
            Controls.Add(linkLabelOpenFilepath);
            Controls.Add(linkTrailer);
            Controls.Add(labelPlot);
            Controls.Add(labelMetascore);
            Controls.Add(labelActors);
            Controls.Add(labelGenre);
            Controls.Add(labelRuntime);
            Controls.Add(labelYear);
            Controls.Add(label1);
            Controls.Add(tbMetascore);
            Controls.Add(tbPlot);
            Controls.Add(pbPoster);
            Controls.Add(tbActors);
            Controls.Add(tbGenre);
            Controls.Add(tbRuntime);
            Controls.Add(tbIMDBRated);
            Controls.Add(tbYear);
            Controls.Add(tbTitle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(5, 4, 5, 4);
            MinimumSize = new Size(18, 47);
            Name = "DetailsFormItem";
            Text = "Movie Detail ";
            KeyDown += MovieDetailsForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pbPoster).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelPlot;
        private System.Windows.Forms.Label labelMetascore;
        private System.Windows.Forms.Label labelActors;
        private System.Windows.Forms.Label labelGenre;
        private System.Windows.Forms.Label labelRuntime;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMetascore;
        internal System.Windows.Forms.RichTextBox tbPlot;
        internal System.Windows.Forms.PictureBox pbPoster;
        private System.Windows.Forms.TextBox tbActors;
        private System.Windows.Forms.TextBox tbGenre;
        private System.Windows.Forms.TextBox tbRuntime;
        private System.Windows.Forms.TextBox tbIMDBRated;
        private System.Windows.Forms.TextBox tbYear;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.LinkLabel linkTrailer;
        private System.Windows.Forms.LinkLabel linkLabelOpenFilepath;
        private System.Windows.Forms.Button buttonDeleteItem;
        private System.Windows.Forms.Label labelQuality;
        private Button buttonSimilar;
        private ToolTip toolTipDetail;
        private Button buttonRediger;
        private Button buttonSubtitleSearch;
        private Button buttonPosterFix;
        private Button buttonPlaylist;
        private Button buttonDeleteReference;
        private Label labelAdded;
        private TextBox tbAdded;
        private Label labelRated;
        private TextBox tbRated;
        private Button buttonAddPL;
        private Button buttonOpenPl;
        private Button buttonReviews;
    }
}