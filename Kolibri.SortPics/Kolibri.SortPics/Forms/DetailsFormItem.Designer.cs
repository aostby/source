
namespace SortPics.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailsFormItem));
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMetascore = new System.Windows.Forms.TextBox();
            this.tbPlot = new System.Windows.Forms.TextBox();
            this.tbActors = new System.Windows.Forms.TextBox();
            this.tbGenre = new System.Windows.Forms.TextBox();
            this.tbRuntime = new System.Windows.Forms.TextBox();
            this.tbRated = new System.Windows.Forms.TextBox();
            this.tbYear = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.pbPoster = new System.Windows.Forms.PictureBox();
            this.linkTrailer = new System.Windows.Forms.LinkLabel();
            this.linkLabelOpenFilepath = new System.Windows.Forms.LinkLabel();
            this.buttonDeleteItem = new System.Windows.Forms.Button();
            this.labelFileExists = new System.Windows.Forms.Label();
            this.labelQuality = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbPoster)).BeginInit();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 524);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Plot";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 487);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Metascore";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 446);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Actors";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 410);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Genre";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 487);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Runtime";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 410);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Year";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 371);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Title";
            // 
            // tbMetascore
            // 
            this.tbMetascore.Location = new System.Drawing.Point(10, 501);
            this.tbMetascore.Name = "tbMetascore";
            this.tbMetascore.ReadOnly = true;
            this.tbMetascore.Size = new System.Drawing.Size(136, 20);
            this.tbMetascore.TabIndex = 28;
            // 
            // tbPlot
            // 
            this.tbPlot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPlot.Location = new System.Drawing.Point(10, 540);
            this.tbPlot.Multiline = true;
            this.tbPlot.Name = "tbPlot";
            this.tbPlot.ReadOnly = true;
            this.tbPlot.Size = new System.Drawing.Size(416, 297);
            this.tbPlot.TabIndex = 27;
            this.tbPlot.DoubleClick += new System.EventHandler(this.tbPlot_DoubleClick);
            // 
            // tbActors
            // 
            this.tbActors.Location = new System.Drawing.Point(11, 460);
            this.tbActors.Name = "tbActors";
            this.tbActors.ReadOnly = true;
            this.tbActors.Size = new System.Drawing.Size(416, 20);
            this.tbActors.TabIndex = 25;
            // 
            // tbGenre
            // 
            this.tbGenre.Location = new System.Drawing.Point(42, 423);
            this.tbGenre.Name = "tbGenre";
            this.tbGenre.ReadOnly = true;
            this.tbGenre.Size = new System.Drawing.Size(384, 20);
            this.tbGenre.TabIndex = 24;
            // 
            // tbRuntime
            // 
            this.tbRuntime.Location = new System.Drawing.Point(155, 501);
            this.tbRuntime.Name = "tbRuntime";
            this.tbRuntime.ReadOnly = true;
            this.tbRuntime.Size = new System.Drawing.Size(136, 20);
            this.tbRuntime.TabIndex = 23;
            // 
            // tbRated
            // 
            this.tbRated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRated.Location = new System.Drawing.Point(165, 346);
            this.tbRated.Name = "tbRated";
            this.tbRated.ReadOnly = true;
            this.tbRated.Size = new System.Drawing.Size(52, 20);
            this.tbRated.TabIndex = 22;
            // 
            // tbYear
            // 
            this.tbYear.Location = new System.Drawing.Point(11, 423);
            this.tbYear.Name = "tbYear";
            this.tbYear.ReadOnly = true;
            this.tbYear.Size = new System.Drawing.Size(61, 20);
            this.tbYear.TabIndex = 21;
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(11, 387);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.ReadOnly = true;
            this.tbTitle.Size = new System.Drawing.Size(416, 20);
            this.tbTitle.TabIndex = 20;
            // 
            // pbPoster
            // 
            this.pbPoster.Location = new System.Drawing.Point(165, 10);
            this.pbPoster.Name = "pbPoster";
            this.pbPoster.Size = new System.Drawing.Size(255, 328);
            this.pbPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPoster.TabIndex = 26;
            this.pbPoster.TabStop = false;
            this.pbPoster.Click += new System.EventHandler(this.pbPoster_Click);
            this.pbPoster.MouseHover += new System.EventHandler(this.pbPoster_MouseHover);
            // 
            // linkTrailer
            // 
            this.linkTrailer.AutoSize = true;
            this.linkTrailer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkTrailer.Location = new System.Drawing.Point(12, 348);
            this.linkTrailer.Name = "linkTrailer";
            this.linkTrailer.Size = new System.Drawing.Size(69, 15);
            this.linkTrailer.TabIndex = 40;
            this.linkTrailer.TabStop = true;
            this.linkTrailer.Text = "Watch trailer";
            this.linkTrailer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // linkLabelOpenFilepath
            // 
            this.linkLabelOpenFilepath.AutoSize = true;
            this.linkLabelOpenFilepath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkLabelOpenFilepath.Location = new System.Drawing.Point(87, 348);
            this.linkLabelOpenFilepath.Name = "linkLabelOpenFilepath";
            this.linkLabelOpenFilepath.Size = new System.Drawing.Size(72, 15);
            this.linkLabelOpenFilepath.TabIndex = 42;
            this.linkLabelOpenFilepath.TabStop = true;
            this.linkLabelOpenFilepath.Text = "Open filepath";
            this.linkLabelOpenFilepath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // buttonDeleteItem
            // 
            this.buttonDeleteItem.Location = new System.Drawing.Point(223, 346);
            this.buttonDeleteItem.Name = "buttonDeleteItem";
            this.buttonDeleteItem.Size = new System.Drawing.Size(150, 23);
            this.buttonDeleteItem.TabIndex = 43;
            this.buttonDeleteItem.Text = "Delete item from db (not from disk)";
            this.buttonDeleteItem.UseVisualStyleBackColor = true;
            this.buttonDeleteItem.Click += new System.EventHandler(this.buttonDeleteItem_Click);
            // 
            // labelFileExists
            // 
            this.labelFileExists.AutoSize = true;
            this.labelFileExists.ForeColor = System.Drawing.Color.ForestGreen;
            this.labelFileExists.Location = new System.Drawing.Point(86, 363);
            this.labelFileExists.Name = "labelFileExists";
            this.labelFileExists.Size = new System.Drawing.Size(34, 13);
            this.labelFileExists.TabIndex = 44;
            this.labelFileExists.Text = "Exists";
            // 
            // labelQuality
            // 
            this.labelQuality.AutoSize = true;
            this.labelQuality.Location = new System.Drawing.Point(312, 507);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(23, 13);
            this.labelQuality.TabIndex = 45;
            this.labelQuality.Text = "HQ";
            // 
            // DetailsFormItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 903);
            this.Controls.Add(this.labelQuality);
            this.Controls.Add(this.labelFileExists);
            this.Controls.Add(this.buttonDeleteItem);
            this.Controls.Add(this.linkLabelOpenFilepath);
            this.Controls.Add(this.linkTrailer);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMetascore);
            this.Controls.Add(this.tbPlot);
            this.Controls.Add(this.pbPoster);
            this.Controls.Add(this.tbActors);
            this.Controls.Add(this.tbGenre);
            this.Controls.Add(this.tbRuntime);
            this.Controls.Add(this.tbRated);
            this.Controls.Add(this.tbYear);
            this.Controls.Add(this.tbTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(16, 39);
            this.Name = "DetailsFormItem";
            this.Text = "Movie Detail ";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MovieDetailsForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbPoster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        internal System.Windows.Forms.TextBox tbPlot;
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
    }
}