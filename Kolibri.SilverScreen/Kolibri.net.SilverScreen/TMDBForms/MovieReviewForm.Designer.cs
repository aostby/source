using System;
using System.Collections.Generic;
using System.Text;
using TMDbLib.Objects.Reviews;
using System;
using System.Windows.Forms;
using TMDbLib.Objects.Reviews;
namespace Kolibri.net.SilverScreen.TMDBForms
{

    public partial class MovieReviewForm : Form
    {
        private Label lblAuthor;
        private TextBox txtAuthor;
        private Label lblRating;
        private TextBox txtRating;
        private Label lblDate;
        private TextBox txtDate;
        private Label lblContent;
        private TextBox txtContent;
        private Label lblUrl;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private LinkLabel lnkUrl;
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
        private void InitializeComponent()
        {
            lblAuthor = new Label();
            txtAuthor = new TextBox();
            lblRating = new Label();
            txtRating = new TextBox();
            lblDate = new Label();
            txtDate = new TextBox();
            lblContent = new Label();
            txtContent = new TextBox();
            lblUrl = new Label();
            lnkUrl = new LinkLabel();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // lblAuthor
            // 
            lblAuthor.AutoSize = true;
            lblAuthor.Location = new Point(13, 13);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(55, 15);
            lblAuthor.TabIndex = 0;
            lblAuthor.Text = "Forfatter:";
            // 
            // txtAuthor
            // 
            txtAuthor.Location = new Point(13, 30);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.ReadOnly = true;
            txtAuthor.Size = new Size(200, 23);
            txtAuthor.TabIndex = 1;
            // 
            // lblRating
            // 
            lblRating.AutoSize = true;
            lblRating.Location = new Point(230, 13);
            lblRating.Name = "lblRating";
            lblRating.Size = new Size(96, 15);
            lblRating.TabIndex = 2;
            lblRating.Text = "Vurdering (0-10):";
            // 
            // txtRating
            // 
            txtRating.Location = new Point(230, 30);
            txtRating.Name = "txtRating";
            txtRating.ReadOnly = true;
            txtRating.Size = new Size(100, 23);
            txtRating.TabIndex = 3;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(345, 13);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(61, 15);
            lblDate.TabIndex = 4;
            lblDate.Text = "Opprettet:";
            // 
            // txtDate
            // 
            txtDate.Location = new Point(345, 30);
            txtDate.Name = "txtDate";
            txtDate.ReadOnly = true;
            txtDate.Size = new Size(120, 23);
            txtDate.TabIndex = 5;
            // 
            // lblContent
            // 
            lblContent.AutoSize = true;
            lblContent.Location = new Point(13, 65);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(51, 15);
            lblContent.TabIndex = 6;
            lblContent.Text = "Innhold:";
            // 
            // txtContent
            // 
            txtContent.Location = new Point(13, 82);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ReadOnly = true;
            txtContent.ScrollBars = ScrollBars.Vertical;
            txtContent.Size = new Size(450, 200);
            txtContent.TabIndex = 7;
            // 
            // lblUrl
            // 
            lblUrl.AutoSize = true;
            lblUrl.Location = new Point(13, 295);
            lblUrl.Name = "lblUrl";
            lblUrl.Size = new Size(78, 15);
            lblUrl.TabIndex = 8;
            lblUrl.Text = "Original-URL:";
            // 
            // lnkUrl
            // 
            lnkUrl.AutoSize = true;
            lnkUrl.Location = new Point(13, 312);
            lnkUrl.Name = "lnkUrl";
            lnkUrl.Size = new Size(0, 15);
            lnkUrl.TabIndex = 9;
            lnkUrl.TabStop = true;
            lnkUrl.LinkClicked += LnkUrl_LinkClicked;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(489, 12);
            webView21.Name = "webView21";
            webView21.Size = new Size(762, 331);
            webView21.TabIndex = 10;
            webView21.ZoomFactor = 1D;
            // 
            // MovieReviewForm
            // 
            ClientSize = new Size(1263, 355);
            Controls.Add(webView21);
            Controls.Add(lblAuthor);
            Controls.Add(txtAuthor);
            Controls.Add(lblRating);
            Controls.Add(txtRating);
            Controls.Add(lblDate);
            Controls.Add(txtDate);
            Controls.Add(lblContent);
            Controls.Add(txtContent);
            Controls.Add(lblUrl);
            Controls.Add(lnkUrl);
            Name = "MovieReviewForm";
            Text = "Filmanmeldelse";
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

    }
}