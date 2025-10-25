
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
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label2 = new Label();
            label1 = new Label();
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
            ((System.ComponentModel.ISupportInitialize)pbPoster).BeginInit();
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
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 562);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(62, 15);
            label7.TabIndex = 35;
            label7.Text = "Metascore";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 515);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(41, 15);
            label6.TabIndex = 34;
            label6.Text = "Actors";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(49, 473);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 33;
            label5.Text = "Genre";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(181, 562);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(52, 15);
            label4.TabIndex = 32;
            label4.Text = "Runtime";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 473);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(29, 15);
            label2.TabIndex = 30;
            label2.Text = "Year";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ImageAlign = ContentAlignment.MiddleLeft;
            label1.Location = new Point(12, 428);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(30, 15);
            label1.TabIndex = 29;
            label1.Text = "Title";
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
            pbPoster.Size = new Size(476, 378);
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
            labelFileExists.Size = new Size(35, 15);
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
            buttonSearch.Location = new Point(415, 398);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(75, 23);
            buttonSearch.TabIndex = 46;
            buttonSearch.Text = "Search";
            buttonSearch.TextAlign = ContentAlignment.MiddleRight;
            toolTipDetail.SetToolTip(buttonSearch, "Se etter lignende filmer");
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
            buttonSubtitleSearch.Location = new Point(415, 422);
            buttonSubtitleSearch.Name = "buttonSubtitleSearch";
            buttonSubtitleSearch.Size = new Size(75, 23);
            buttonSubtitleSearch.TabIndex = 48;
            buttonSubtitleSearch.Text = "Subtitle search";
            buttonSubtitleSearch.TextAlign = ContentAlignment.MiddleRight;
            buttonSubtitleSearch.UseVisualStyleBackColor = true;
            buttonSubtitleSearch.Click += buttonSubtitleSearch_Click;
            // 
            // DetailsFormItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 754);
            Controls.Add(buttonSubtitleSearch);
            Controls.Add(buttonRediger);
            Controls.Add(buttonSearch);
            Controls.Add(labelQuality);
            Controls.Add(labelFileExists);
            Controls.Add(buttonDeleteItem);
            Controls.Add(linkLabelOpenFilepath);
            Controls.Add(linkTrailer);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
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
            Name = "DetailsFormItem";
            Text = "Movie Detail ";
            KeyDown += MovieDetailsForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pbPoster).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
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
    }
}