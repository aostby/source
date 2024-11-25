namespace Kolibri.Common.FormUtilities.Forms
{
    partial class TransformFHIRForm
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
            this.btnTest = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rtfResult = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDownFile = new System.Windows.Forms.Button();
            this.btnUpFile = new System.Windows.Forms.Button();
            this.linkLabelFHIRType = new System.Windows.Forms.LinkLabel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.linkLabelPath = new System.Windows.Forms.LinkLabel();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(687, 64);
            this.btnTest.Margin = new System.Windows.Forms.Padding(2);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 21);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(15, 65);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(668, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "C:\\Users\\OnScreenDisplay\\Documents\\Jobb\\Direktoratet for eHelse\\Nasjonalt løft ti" +
    "meavtaler\\sample_NotificationReferralRequest-ContainedReferencedObjects.xml";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // rtfResult
            // 
            this.rtfResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfResult.Location = new System.Drawing.Point(15, 122);
            this.rtfResult.Margin = new System.Windows.Forms.Padding(2);
            this.rtfResult.Name = "rtfResult";
            this.rtfResult.Size = new System.Drawing.Size(747, 245);
            this.rtfResult.TabIndex = 3;
            this.rtfResult.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "XML du vil validere mot profilen:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(539, 90);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Last run:";
            // 
            // btnDownFile
            // 
            this.btnDownFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownFile.Location = new System.Drawing.Point(633, 38);
            this.btnDownFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnDownFile.Name = "btnDownFile";
            this.btnDownFile.Size = new System.Drawing.Size(50, 21);
            this.btnDownFile.TabIndex = 6;
            this.btnDownFile.Text = "Down";
            this.btnDownFile.UseVisualStyleBackColor = true;
            this.btnDownFile.Click += new System.EventHandler(this.btnDownFile_Click);
            // 
            // btnUpFile
            // 
            this.btnUpFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpFile.Location = new System.Drawing.Point(579, 38);
            this.btnUpFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpFile.Name = "btnUpFile";
            this.btnUpFile.Size = new System.Drawing.Size(50, 21);
            this.btnUpFile.TabIndex = 6;
            this.btnUpFile.Text = "Up";
            this.btnUpFile.UseVisualStyleBackColor = true;
            this.btnUpFile.Click += new System.EventHandler(this.btnUpFile_Click);
            // 
            // linkLabelFHIRType
            // 
            this.linkLabelFHIRType.AutoSize = true;
            this.linkLabelFHIRType.Location = new System.Drawing.Point(177, 46);
            this.linkLabelFHIRType.Name = "linkLabelFHIRType";
            this.linkLabelFHIRType.Size = new System.Drawing.Size(49, 13);
            this.linkLabelFHIRType.TabIndex = 7;
            this.linkLabelFHIRType.TabStop = true;
            this.linkLabelFHIRType.Text = "linkLabel";
            this.linkLabelFHIRType.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(132, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(207, 21);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // linkLabelPath
            // 
            this.linkLabelPath.AutoSize = true;
            this.linkLabelPath.Location = new System.Drawing.Point(15, 15);
            this.linkLabelPath.Name = "linkLabelPath";
            this.linkLabelPath.Size = new System.Drawing.Size(71, 13);
            this.linkLabelPath.TabIndex = 9;
            this.linkLabelPath.TabStop = true;
            this.linkLabelPath.Text = "linkLabelPath";
            this.linkLabelPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(346, 7);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(21, 21);
            this.buttonOpen.TabIndex = 10;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenFile.Location = new System.Drawing.Point(687, 38);
            this.buttonOpenFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(75, 21);
            this.buttonOpenFile.TabIndex = 11;
            this.buttonOpenFile.Text = "Last mappe";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // TransformFHIRForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 390);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.linkLabelFHIRType);
            this.Controls.Add(this.btnUpFile);
            this.Controls.Add(this.btnDownFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtfResult);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.linkLabelPath);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TransformFHIRForm";
            this.Text = "Validere samples mot FHIR-profilene";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox rtfResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDownFile;
        private System.Windows.Forms.Button btnUpFile;
        private System.Windows.Forms.LinkLabel linkLabelFHIRType;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.LinkLabel linkLabelPath;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonOpenFile;
    }
}

