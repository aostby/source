namespace Kolibri.FormUtilities.Forms
{
    partial class RenameFoldersForm
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
            this.buttonSource = new System.Windows.Forms.Button();
            this.linkLabelSource = new System.Windows.Forms.LinkLabel();
            this.labelSource = new System.Windows.Forms.Label();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.textBoxOld = new System.Windows.Forms.TextBox();
            this.textBoxNew = new System.Windows.Forms.TextBox();
            this.labelOld = new System.Windows.Forms.Label();
            this.labelNew = new System.Windows.Forms.Label();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(670, 13);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(118, 23);
            this.buttonSource.TabIndex = 1;
            this.buttonSource.Text = "Open Source";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // linkLabelSource
            // 
            this.linkLabelSource.AutoSize = true;
            this.linkLabelSource.Location = new System.Drawing.Point(103, 18);
            this.linkLabelSource.Name = "linkLabelSource";
            this.linkLabelSource.Size = new System.Drawing.Size(41, 13);
            this.linkLabelSource.TabIndex = 2;
            this.linkLabelSource.TabStop = true;
            this.linkLabelSource.Text = "Source";
            this.linkLabelSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSource_LinkClicked);
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(13, 17);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(41, 13);
            this.labelSource.TabIndex = 3;
            this.labelSource.Text = "Source";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(670, 80);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(118, 23);
            this.buttonExecute.TabIndex = 4;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // textBoxOld
            // 
            this.textBoxOld.Location = new System.Drawing.Point(120, 56);
            this.textBoxOld.Name = "textBoxOld";
            this.textBoxOld.Size = new System.Drawing.Size(234, 20);
            this.textBoxOld.TabIndex = 5;
            // 
            // textBoxNew
            // 
            this.textBoxNew.Location = new System.Drawing.Point(430, 56);
            this.textBoxNew.Name = "textBoxNew";
            this.textBoxNew.Size = new System.Drawing.Size(234, 20);
            this.textBoxNew.TabIndex = 6;
            // 
            // labelOld
            // 
            this.labelOld.AutoSize = true;
            this.labelOld.Location = new System.Drawing.Point(13, 59);
            this.labelOld.Name = "labelOld";
            this.labelOld.Size = new System.Drawing.Size(43, 13);
            this.labelOld.TabIndex = 7;
            this.labelOld.Text = "Old text";
            // 
            // labelNew
            // 
            this.labelNew.AutoSize = true;
            this.labelNew.Location = new System.Drawing.Point(360, 59);
            this.labelNew.Name = "labelNew";
            this.labelNew.Size = new System.Drawing.Size(49, 13);
            this.labelNew.TabIndex = 8;
            this.labelNew.Text = "New text";
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(671, 51);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(117, 23);
            this.buttonPreview.TabIndex = 9;
            this.buttonPreview.Text = "Preview";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // RenameFoldersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.labelNew);
            this.Controls.Add(this.labelOld);
            this.Controls.Add(this.textBoxNew);
            this.Controls.Add(this.textBoxOld);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.linkLabelSource);
            this.Controls.Add(this.buttonSource);
            this.Name = "RenameFoldersForm";
            this.Text = "RenameFoldersForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.LinkLabel linkLabelSource;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.TextBox textBoxOld;
        private System.Windows.Forms.TextBox textBoxNew;
        private System.Windows.Forms.Label labelOld;
        private System.Windows.Forms.Label labelNew;
        private System.Windows.Forms.Button buttonPreview;
    }
}