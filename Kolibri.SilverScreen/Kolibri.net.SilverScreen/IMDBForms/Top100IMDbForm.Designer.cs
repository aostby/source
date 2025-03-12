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
            components = new System.ComponentModel.Container();
            gridTop100 = new DataGridView();
            cmsOptions = new ContextMenuStrip(components);
            miMovieDetails = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)gridTop100).BeginInit();
            cmsOptions.SuspendLayout();
            SuspendLayout();
            // 
            // gridTop100
            // 
            gridTop100.AllowUserToAddRows = false;
            gridTop100.AllowUserToDeleteRows = false;
            gridTop100.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridTop100.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridTop100.ContextMenuStrip = cmsOptions;
            gridTop100.Location = new Point(1, 1);
            gridTop100.Margin = new Padding(4, 3, 4, 3);
            gridTop100.MultiSelect = false;
            gridTop100.Name = "gridTop100";
            gridTop100.ReadOnly = true;
            gridTop100.RowHeadersVisible = false;
            gridTop100.Size = new Size(902, 622);
            gridTop100.TabIndex = 0;
            gridTop100.CellMouseDown += gridTop100_CellMouseDown;
            gridTop100.RowPrePaint += gridTop100_RowPrePaint;
            gridTop100.MouseDoubleClick += miMovieDetails_Click;
            // 
            // cmsOptions
            // 
            cmsOptions.Items.AddRange(new ToolStripItem[] { miMovieDetails });
            cmsOptions.Name = "cmsOptions";
            cmsOptions.Size = new Size(177, 26);
            // 
            // miMovieDetails
            // 
            miMovieDetails.Name = "miMovieDetails";
            miMovieDetails.Size = new Size(176, 22);
            miMovieDetails.Text = "Show movie details";
            miMovieDetails.Click += miMovieDetails_Click;
            // 
            // Top100IMDbForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(902, 628);
            Controls.Add(gridTop100);
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Top100IMDbForm";
            Text = "Top 100 IMDb movies";
            KeyDown += Top100IMDbForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)gridTop100).EndInit();
            cmsOptions.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridTop100;
        private System.Windows.Forms.ContextMenuStrip cmsOptions;
        private System.Windows.Forms.ToolStripMenuItem miMovieDetails;
    }
}