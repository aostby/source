namespace Kolibri.Common.FormUtilities.Forms
{
    partial class CertificateCircusForm
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
            this.buttonFindBy = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.labelFindBy = new System.Windows.Forms.Label();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonListCertificates = new System.Windows.Forms.Button();
            this.textBoxFindPrivateKeyEXE = new System.Windows.Forms.TextBox();
            this.buttonCreateCACLSbatch = new System.Windows.Forms.Button();
            this.groupBoxCACLS = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxPrivateKey = new System.Windows.Forms.TextBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.dataGridViewCert = new System.Windows.Forms.DataGridView();
            this.buttonDisplayCert = new System.Windows.Forms.Button();
            this.buttonOpenCertMgr = new System.Windows.Forms.Button();
            this.groupBoxCACLS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCert)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonFindBy
            // 
            this.buttonFindBy.Location = new System.Drawing.Point(312, 122);
            this.buttonFindBy.Name = "buttonFindBy";
            this.buttonFindBy.Size = new System.Drawing.Size(97, 23);
            this.buttonFindBy.TabIndex = 0;
            this.buttonFindBy.Text = "Find by";
            this.buttonFindBy.UseVisualStyleBackColor = true;
            this.buttonFindBy.Click += new System.EventHandler(this.buttonFindBy_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(71, 66);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(338, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // labelFindBy
            // 
            this.labelFindBy.AutoSize = true;
            this.labelFindBy.Location = new System.Drawing.Point(26, 67);
            this.labelFindBy.Name = "labelFindBy";
            this.labelFindBy.Size = new System.Drawing.Size(39, 13);
            this.labelFindBy.TabIndex = 2;
            this.labelFindBy.Text = "FindBy";
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(71, 96);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(337, 20);
            this.textBoxValue.TabIndex = 3;
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(26, 103);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 4;
            this.labelValue.Text = "Value";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(29, 211);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(379, 143);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // buttonListCertificates
            // 
            this.buttonListCertificates.Location = new System.Drawing.Point(29, 12);
            this.buttonListCertificates.Name = "buttonListCertificates";
            this.buttonListCertificates.Size = new System.Drawing.Size(379, 23);
            this.buttonListCertificates.TabIndex = 7;
            this.buttonListCertificates.Text = "List Certificates";
            this.buttonListCertificates.UseVisualStyleBackColor = true;
            this.buttonListCertificates.Click += new System.EventHandler(this.buttonListCertificates_Click);
            // 
            // textBoxFindPrivateKeyEXE
            // 
            this.textBoxFindPrivateKeyEXE.Location = new System.Drawing.Point(29, 185);
            this.textBoxFindPrivateKeyEXE.Name = "textBoxFindPrivateKeyEXE";
            this.textBoxFindPrivateKeyEXE.Size = new System.Drawing.Size(927, 20);
            this.textBoxFindPrivateKeyEXE.TabIndex = 8;
            this.textBoxFindPrivateKeyEXE.TextChanged += new System.EventHandler(this.textBoxFindPrivateKeyEXE_TextChanged);
            // 
            // buttonCreateCACLSbatch
            // 
            this.buttonCreateCACLSbatch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonCreateCACLSbatch.Location = new System.Drawing.Point(98, 46);
            this.buttonCreateCACLSbatch.Name = "buttonCreateCACLSbatch";
            this.buttonCreateCACLSbatch.Size = new System.Drawing.Size(315, 23);
            this.buttonCreateCACLSbatch.TabIndex = 9;
            this.buttonCreateCACLSbatch.Text = "Copy script to clipboard, for manually paste";
            this.buttonCreateCACLSbatch.UseVisualStyleBackColor = true;
            this.buttonCreateCACLSbatch.Click += new System.EventHandler(this.buttonCreateCACLSbatch_Click);
            // 
            // groupBoxCACLS
            // 
            this.groupBoxCACLS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCACLS.Controls.Add(this.button1);
            this.groupBoxCACLS.Controls.Add(this.textBoxPrivateKey);
            this.groupBoxCACLS.Controls.Add(this.buttonCreateCACLSbatch);
            this.groupBoxCACLS.Location = new System.Drawing.Point(-1, 374);
            this.groupBoxCACLS.Name = "groupBoxCACLS";
            this.groupBoxCACLS.Size = new System.Drawing.Size(431, 87);
            this.groupBoxCACLS.TabIndex = 10;
            this.groupBoxCACLS.TabStop = false;
            this.groupBoxCACLS.Text = "Create CACLS batch and place it on clipboard, or copy Private Key from text below" +
    "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Open location";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonOpenLocation_Click_1);
            // 
            // textBoxPrivateKey
            // 
            this.textBoxPrivateKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPrivateKey.Location = new System.Drawing.Point(17, 20);
            this.textBoxPrivateKey.Name = "textBoxPrivateKey";
            this.textBoxPrivateKey.Size = new System.Drawing.Size(396, 20);
            this.textBoxPrivateKey.TabIndex = 0;
            this.textBoxPrivateKey.TextChanged += new System.EventHandler(this.textBoxPrivateKey_TextChanged);
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Location = new System.Drawing.Point(29, 122);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.Size = new System.Drawing.Size(196, 61);
            this.groupBoxCredentials.TabIndex = 11;
            this.groupBoxCredentials.TabStop = false;
            this.groupBoxCredentials.Text = "Credentials";
            // 
            // dataGridViewCert
            // 
            this.dataGridViewCert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCert.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCert.Location = new System.Drawing.Point(416, 12);
            this.dataGridViewCert.MultiSelect = false;
            this.dataGridViewCert.Name = "dataGridViewCert";
            this.dataGridViewCert.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCert.Size = new System.Drawing.Size(0, 171);
            this.dataGridViewCert.TabIndex = 12;
            this.dataGridViewCert.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCert_CellContentDoubleClick);
            // 
            // buttonDisplayCert
            // 
            this.buttonDisplayCert.Location = new System.Drawing.Point(231, 122);
            this.buttonDisplayCert.Name = "buttonDisplayCert";
            this.buttonDisplayCert.Size = new System.Drawing.Size(75, 23);
            this.buttonDisplayCert.TabIndex = 13;
            this.buttonDisplayCert.Text = "Display Certificate";
            this.buttonDisplayCert.UseVisualStyleBackColor = true;
            this.buttonDisplayCert.Click += new System.EventHandler(this.buttonDisplayCert_Click);
            // 
            // buttonOpenCertMgr
            // 
            this.buttonOpenCertMgr.Location = new System.Drawing.Point(231, 151);
            this.buttonOpenCertMgr.Name = "buttonOpenCertMgr";
            this.buttonOpenCertMgr.Size = new System.Drawing.Size(177, 23);
            this.buttonOpenCertMgr.TabIndex = 14;
            this.buttonOpenCertMgr.Text = "Open certficate manager";
            this.buttonOpenCertMgr.UseVisualStyleBackColor = true;
            this.buttonOpenCertMgr.Click += new System.EventHandler(this.buttonOpenCertMgr_Click);
            // 
            // CertificateCircusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 477);
            this.Controls.Add(this.buttonOpenCertMgr);
            this.Controls.Add(this.buttonDisplayCert);
            this.Controls.Add(this.groupBoxCredentials);
            this.Controls.Add(this.dataGridViewCert);
            this.Controls.Add(this.groupBoxCACLS);
            this.Controls.Add(this.textBoxFindPrivateKeyEXE);
            this.Controls.Add(this.buttonListCertificates);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.labelFindBy);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttonFindBy);
            this.MinimumSize = new System.Drawing.Size(447, 516);
            this.Name = "CertificateCircusForm";
            this.Text = "CertificateCircusForm";
            this.groupBoxCACLS.ResumeLayout(false);
            this.groupBoxCACLS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCert)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFindBy;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label labelFindBy;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonListCertificates;
        private System.Windows.Forms.TextBox textBoxFindPrivateKeyEXE;
        private System.Windows.Forms.Button buttonCreateCACLSbatch;
        private System.Windows.Forms.GroupBox groupBoxCACLS;
        private System.Windows.Forms.TextBox textBoxPrivateKey;
        private System.Windows.Forms.GroupBox groupBoxCredentials;
        private System.Windows.Forms.DataGridView dataGridViewCert;
        private System.Windows.Forms.Button buttonDisplayCert;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonOpenCertMgr;
    }
}