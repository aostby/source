﻿namespace Kolibri.net.SilverScreen.IMDBForms
{
    partial class FriendsWatchlist
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
            this.gridFriendsWatchlist = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridFriendsWatchlist)).BeginInit();
            this.SuspendLayout();
            // 
            // gridFriendsWatchlist
            // 
            this.gridFriendsWatchlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridFriendsWatchlist.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridFriendsWatchlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFriendsWatchlist.Location = new System.Drawing.Point(2, 2);
            this.gridFriendsWatchlist.Name = "gridFriendsWatchlist";
            this.gridFriendsWatchlist.RowHeadersVisible = false;
            this.gridFriendsWatchlist.Size = new System.Drawing.Size(794, 403);
            this.gridFriendsWatchlist.TabIndex = 0;
            // 
            // FriendsWatchlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 409);
            this.Controls.Add(this.gridFriendsWatchlist);
            this.Name = "FriendsWatchlist";
            this.Text = "Friends Watchlist";
            ((System.ComponentModel.ISupportInitialize)(this.gridFriendsWatchlist)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridFriendsWatchlist;
    }
}