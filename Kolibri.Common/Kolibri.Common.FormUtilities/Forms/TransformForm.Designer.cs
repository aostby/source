using Utilities.Properties;

namespace Kolibri.Common.FormUtilities.Forms
{
    partial class TransformForm
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
            this.buttonTransform = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonPath = new System.Windows.Forms.Button();
            this.labelExtension = new System.Windows.Forms.Label();
            this.buttonXSLT = new System.Windows.Forms.Button();
            this.buttonPasteText = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonTransform
            // 
            this.buttonTransform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTransform.Location = new System.Drawing.Point(731, 210);
            this.buttonTransform.Name = "buttonTransform";
            this.buttonTransform.Size = new System.Drawing.Size(186, 23);
            this.buttonTransform.TabIndex = 0;
            this.buttonTransform.Text = "Transform";
            this.buttonTransform.UseVisualStyleBackColor = true;
            this.buttonTransform.Click += new System.EventHandler(this.buttonTransform_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 36);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(905, 159);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            this.dataGridView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 198);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(618, 49);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select a file to be transformed. Double clik to view the file, right click to cho" +
    "ose default application to open the file. Click Transform to choose XSLT file to" +
    " be used for the transformation";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(114, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(713, 20);
            this.textBox1.TabIndex = 4;
            // 
            // buttonPath
            // 
            this.buttonPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPath.Location = new System.Drawing.Point(833, 8);
            this.buttonPath.Name = "buttonPath";
            this.buttonPath.Size = new System.Drawing.Size(84, 23);
            this.buttonPath.TabIndex = 5;
            this.buttonPath.Text = "Change path";
            this.buttonPath.UseVisualStyleBackColor = true;
            this.buttonPath.Click += new System.EventHandler(this.buttonPath_Click);
            // 
            // labelExtension
            // 
            this.labelExtension.AutoSize = true;
            this.labelExtension.Location = new System.Drawing.Point(12, 8);
            this.labelExtension.Name = "labelExtension";
            this.labelExtension.Size = new System.Drawing.Size(56, 13);
            this.labelExtension.TabIndex = 6;
            this.labelExtension.Text = "Extension:";
            // 
            // buttonXSLT
            // 
            this.buttonXSLT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonXSLT.BackgroundImage =  Resources.OpenFile;
            this.buttonXSLT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonXSLT.Location = new System.Drawing.Point(699, 210);
            this.buttonXSLT.Name = "buttonXSLT";
            this.buttonXSLT.Size = new System.Drawing.Size(26, 23);
            this.buttonXSLT.TabIndex = 7;
            this.buttonXSLT.UseVisualStyleBackColor = true;
            this.buttonXSLT.Click += new System.EventHandler(this.buttonXSLT_Click);
            // 
            // buttonPasteText
            // 
            this.buttonPasteText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPasteText.BackgroundImage = Resources.dataset;
            this.buttonPasteText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPasteText.Location = new System.Drawing.Point(667, 210);
            this.buttonPasteText.Name = "buttonPasteText";
            this.buttonPasteText.Size = new System.Drawing.Size(26, 23);
            this.buttonPasteText.TabIndex = 8;
            this.buttonPasteText.UseVisualStyleBackColor = true;
            this.buttonPasteText.Click += new System.EventHandler(this.buttonPasteText_Click);
            // 
            // TransformForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 245);
            this.Controls.Add(this.buttonPasteText);
            this.Controls.Add(this.buttonXSLT);
            this.Controls.Add(this.labelExtension);
            this.Controls.Add(this.buttonPath);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonTransform);
            this.Name = "TransformForm";
            this.Text = "TransformForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTransform;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonPath;
        private System.Windows.Forms.Label labelExtension;
        private System.Windows.Forms.Button buttonXSLT;
        private System.Windows.Forms.Button buttonPasteText;
    }
}