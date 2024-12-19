namespace Kolibri.net.SilverScreen.IMDBForms
{
    partial class Top100IMDbForm
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
            this.components = new System.ComponentModel.Container();
            this.gridTop100 = new System.Windows.Forms.DataGridView();
            this.cmsOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miMovieDetails = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridTop100)).BeginInit();
            this.cmsOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridTop100
            // 
            this.gridTop100.AllowUserToAddRows = false;
            this.gridTop100.AllowUserToDeleteRows = false;
            this.gridTop100.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridTop100.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTop100.ContextMenuStrip = this.cmsOptions;
            this.gridTop100.Location = new System.Drawing.Point(1, 1);
            this.gridTop100.MultiSelect = false;
            this.gridTop100.Name = "gridTop100";
            this.gridTop100.ReadOnly = true;
            this.gridTop100.RowHeadersVisible = false;
            this.gridTop100.Size = new System.Drawing.Size(773, 539);
            this.gridTop100.TabIndex = 0;
            this.gridTop100.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridTop100_CellMouseDown);
            this.gridTop100.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.miMovieDetails_Click);
            // 
            // cmsOptions
            // 
            this.cmsOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMovieDetails});
            this.cmsOptions.Name = "cmsOptions";
            this.cmsOptions.Size = new System.Drawing.Size(177, 26);
            // 
            // miMovieDetails
            // 
            this.miMovieDetails.Name = "miMovieDetails";
            this.miMovieDetails.Size = new System.Drawing.Size(176, 22);
            this.miMovieDetails.Text = "Show movie details";
            this.miMovieDetails.Click += new System.EventHandler(this.miMovieDetails_Click);
            // 
            // Top100IMDbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 544);
            this.Controls.Add(this.gridTop100);
            this.KeyPreview = true;
            this.Name = "Top100IMDbForm";
            this.Text = "Top 100 IMDb movies";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Top100IMDbForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.gridTop100)).EndInit();
            this.cmsOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridTop100;
        private System.Windows.Forms.ContextMenuStrip cmsOptions;
        private System.Windows.Forms.ToolStripMenuItem miMovieDetails;
    }
}