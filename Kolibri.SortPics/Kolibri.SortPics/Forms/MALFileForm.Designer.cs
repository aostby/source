
namespace SortPics.Forms
{
    partial class MALFileForm
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
            this.buttonOpenDirD = new System.Windows.Forms.Button();
            this.buttonOpenDirS = new System.Windows.Forms.Button();
            this.buttonDestination = new System.Windows.Forms.Button();
            this.textBoxDestination = new System.Windows.Forms.TextBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonOpenDirD
            // 
            this.buttonOpenDirD.Location = new System.Drawing.Point(636, 34);
            this.buttonOpenDirD.Name = "buttonOpenDirD";
            this.buttonOpenDirD.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirD.TabIndex = 18;
            this.buttonOpenDirD.Text = "Let opp";
            this.buttonOpenDirD.UseVisualStyleBackColor = true;
            this.buttonOpenDirD.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonOpenDirS
            // 
            this.buttonOpenDirS.Location = new System.Drawing.Point(636, 8);
            this.buttonOpenDirS.Name = "buttonOpenDirS";
            this.buttonOpenDirS.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirS.TabIndex = 17;
            this.buttonOpenDirS.Text = "Let opp";
            this.buttonOpenDirS.UseVisualStyleBackColor = true;
            this.buttonOpenDirS.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonDestination
            // 
            this.buttonDestination.Location = new System.Drawing.Point(663, 34);
            this.buttonDestination.Name = "buttonDestination";
            this.buttonDestination.Size = new System.Drawing.Size(75, 23);
            this.buttonDestination.TabIndex = 16;
            this.buttonDestination.Text = "Let opp";
            this.buttonDestination.UseVisualStyleBackColor = true;
            this.buttonDestination.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.Location = new System.Drawing.Point(12, 38);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(622, 20);
            this.textBoxDestination.TabIndex = 15;
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(663, 8);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 23);
            this.buttonSource.TabIndex = 14;
            this.buttonSource.Text = "Let opp";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxSource
            // 
            this.textBoxSource.Location = new System.Drawing.Point(12, 12);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(622, 20);
            this.textBoxSource.TabIndex = 13;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // MALFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 67);
            this.Controls.Add(this.buttonOpenDirD);
            this.Controls.Add(this.buttonOpenDirS);
            this.Controls.Add(this.buttonDestination);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.textBoxSource);
            this.Name = "MALFileForm";
            this.Text = "MALFileForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenDirD;
        private System.Windows.Forms.Button buttonOpenDirS;
        private System.Windows.Forms.Button buttonDestination;
        private System.Windows.Forms.TextBox textBoxDestination;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.TextBox textBoxSource;
    }
}