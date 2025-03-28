﻿namespace Kolibri.net.SilverScreen.Forms
{
    partial class IMDbDataFilesForm
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
            linkLabelOnline = new LinkLabel();
            flowLayoutPanelGZ = new FlowLayoutPanel();
            flowLayoutPanelDataFiles = new FlowLayoutPanel();
            toolStrip1 = new ToolStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripLabel1 = new ToolStripLabel();
            linkLabelLocal = new LinkLabel();
            flowLayoutPanelUpdate = new FlowLayoutPanel();
            linkLabelIMDBdb = new LinkLabel();
            groupBoxGZ = new GroupBox();
            buttonClearAll = new Button();
            buttonClearDB = new Button();
            buttonCreateSchemas = new Button();
            groupBoxDataFiles = new GroupBox();
            groupBoxUpdate = new GroupBox();
            checkBoxSilent = new CheckBox();
            radioButtonYear = new RadioButton();
            radioButtonList = new RadioButton();
            buttonCancel = new Button();
            panelInfo = new Panel();
            richTextBox1 = new RichTextBox();
            groupBox1 = new GroupBox();
            buttonCreateDapperClasses = new Button();
            buttonTestConnection = new Button();
            button1 = new Button();
            toolStrip1.SuspendLayout();
            groupBoxGZ.SuspendLayout();
            groupBoxDataFiles.SuspendLayout();
            groupBoxUpdate.SuspendLayout();
            panelInfo.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // linkLabelOnline
            // 
            linkLabelOnline.AutoSize = true;
            linkLabelOnline.Location = new Point(23, 7);
            linkLabelOnline.Name = "linkLabelOnline";
            linkLabelOnline.Size = new Size(89, 15);
            linkLabelOnline.TabIndex = 0;
            linkLabelOnline.TabStop = true;
            linkLabelOnline.Text = "linkLabelOnline";
            linkLabelOnline.LinkClicked += linkLabel1_LinkClicked;
            // 
            // flowLayoutPanelGZ
            // 
            flowLayoutPanelGZ.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelGZ.Location = new Point(6, 22);
            flowLayoutPanelGZ.Name = "flowLayoutPanelGZ";
            flowLayoutPanelGZ.Size = new Size(205, 261);
            flowLayoutPanelGZ.TabIndex = 1;
            // 
            // flowLayoutPanelDataFiles
            // 
            flowLayoutPanelDataFiles.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelDataFiles.Location = new Point(10, 22);
            flowLayoutPanelDataFiles.Name = "flowLayoutPanelDataFiles";
            flowLayoutPanelDataFiles.Size = new Size(205, 261);
            flowLayoutPanelDataFiles.TabIndex = 2;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripLabel1 });
            toolStrip1.Location = new Point(0, 542);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1451, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 22);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(86, 22);
            toolStripLabel1.Text = "toolStripLabel1";
            // 
            // linkLabelLocal
            // 
            linkLabelLocal.AutoSize = true;
            linkLabelLocal.Location = new Point(23, 30);
            linkLabelLocal.Name = "linkLabelLocal";
            linkLabelLocal.Size = new Size(82, 15);
            linkLabelLocal.TabIndex = 4;
            linkLabelLocal.TabStop = true;
            linkLabelLocal.Text = "linkLabelLocal";
            linkLabelLocal.LinkClicked += linkLabel1_LinkClicked;
            // 
            // flowLayoutPanelUpdate
            // 
            flowLayoutPanelUpdate.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelUpdate.Location = new Point(10, 22);
            flowLayoutPanelUpdate.Name = "flowLayoutPanelUpdate";
            flowLayoutPanelUpdate.Size = new Size(205, 261);
            flowLayoutPanelUpdate.TabIndex = 3;
            // 
            // linkLabelIMDBdb
            // 
            linkLabelIMDBdb.AutoSize = true;
            linkLabelIMDBdb.Location = new Point(23, 51);
            linkLabelIMDBdb.Name = "linkLabelIMDBdb";
            linkLabelIMDBdb.Size = new Size(53, 15);
            linkLabelIMDBdb.TabIndex = 5;
            linkLabelIMDBdb.TabStop = true;
            linkLabelIMDBdb.Text = "IMDB db";
            linkLabelIMDBdb.LinkClicked += linkLabel1_LinkClicked;
            // 
            // groupBoxGZ
            // 
            groupBoxGZ.Controls.Add(buttonClearAll);
            groupBoxGZ.Controls.Add(buttonClearDB);
            groupBoxGZ.Controls.Add(flowLayoutPanelGZ);
            groupBoxGZ.Location = new Point(23, 71);
            groupBoxGZ.Name = "groupBoxGZ";
            groupBoxGZ.Size = new Size(221, 357);
            groupBoxGZ.TabIndex = 6;
            groupBoxGZ.TabStop = false;
            groupBoxGZ.Text = "IMDB datafiles GZ";
            // 
            // buttonClearAll
            // 
            buttonClearAll.Location = new Point(7, 318);
            buttonClearAll.Name = "buttonClearAll";
            buttonClearAll.Size = new Size(204, 23);
            buttonClearAll.TabIndex = 3;
            buttonClearAll.Text = "Clear ALL";
            buttonClearAll.UseVisualStyleBackColor = true;
            buttonClearAll.Click += buttonClearAll_Click;
            // 
            // buttonClearDB
            // 
            buttonClearDB.Location = new Point(7, 289);
            buttonClearDB.Name = "buttonClearDB";
            buttonClearDB.Size = new Size(204, 23);
            buttonClearDB.TabIndex = 2;
            buttonClearDB.Text = "Clear TEMP DB";
            buttonClearDB.UseVisualStyleBackColor = true;
            buttonClearDB.Click += buttonClearDB_Click;
            // 
            // buttonCreateSchemas
            // 
            buttonCreateSchemas.Location = new Point(6, 22);
            buttonCreateSchemas.Name = "buttonCreateSchemas";
            buttonCreateSchemas.Size = new Size(202, 23);
            buttonCreateSchemas.TabIndex = 4;
            buttonCreateSchemas.Text = "Script MySql Tables (DDL)";
            buttonCreateSchemas.UseVisualStyleBackColor = true;
            buttonCreateSchemas.Click += buttonCreateSchemas_Click;
            // 
            // groupBoxDataFiles
            // 
            groupBoxDataFiles.Controls.Add(flowLayoutPanelDataFiles);
            groupBoxDataFiles.Location = new Point(261, 71);
            groupBoxDataFiles.Name = "groupBoxDataFiles";
            groupBoxDataFiles.Size = new Size(221, 357);
            groupBoxDataFiles.TabIndex = 7;
            groupBoxDataFiles.TabStop = false;
            groupBoxDataFiles.Text = "IMDB Unzipped DataFiles";
            // 
            // groupBoxUpdate
            // 
            groupBoxUpdate.Controls.Add(checkBoxSilent);
            groupBoxUpdate.Controls.Add(radioButtonYear);
            groupBoxUpdate.Controls.Add(radioButtonList);
            groupBoxUpdate.Controls.Add(buttonCancel);
            groupBoxUpdate.Controls.Add(flowLayoutPanelUpdate);
            groupBoxUpdate.Location = new Point(499, 71);
            groupBoxUpdate.Name = "groupBoxUpdate";
            groupBoxUpdate.Size = new Size(221, 468);
            groupBoxUpdate.TabIndex = 8;
            groupBoxUpdate.TabStop = false;
            groupBoxUpdate.Text = "Update Local Temp Database";
            // 
            // checkBoxSilent
            // 
            checkBoxSilent.AutoSize = true;
            checkBoxSilent.Location = new Point(9, 441);
            checkBoxSilent.Name = "checkBoxSilent";
            checkBoxSilent.Size = new Size(108, 19);
            checkBoxSilent.TabIndex = 8;
            checkBoxSilent.Text = "Stille, bakgrunn";
            checkBoxSilent.UseVisualStyleBackColor = true;
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(10, 419);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(37, 19);
            radioButtonYear.TabIndex = 7;
            radioButtonYear.Text = "År";
            radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonList
            // 
            radioButtonList.AutoSize = true;
            radioButtonList.Checked = true;
            radioButtonList.Location = new Point(10, 400);
            radioButtonList.Name = "radioButtonList";
            radioButtonList.Size = new Size(77, 19);
            radioButtonList.TabIndex = 6;
            radioButtonList.TabStop = true;
            radioButtonList.Text = "Lokal liste";
            radioButtonList.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(157, 416);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(58, 21);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // panelInfo
            // 
            panelInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelInfo.AutoSize = true;
            panelInfo.Controls.Add(richTextBox1);
            panelInfo.Location = new Point(740, 5);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(699, 521);
            panelInfo.TabIndex = 9;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(699, 521);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(buttonCreateDapperClasses);
            groupBox1.Controls.Add(buttonTestConnection);
            groupBox1.Controls.Add(buttonCreateSchemas);
            groupBox1.Location = new Point(23, 434);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(459, 97);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "MySQL database";
            // 
            // buttonCreateDapperClasses
            // 
            buttonCreateDapperClasses.Location = new Point(7, 75);
            buttonCreateDapperClasses.Name = "buttonCreateDapperClasses";
            buttonCreateDapperClasses.Size = new Size(202, 23);
            buttonCreateDapperClasses.TabIndex = 6;
            buttonCreateDapperClasses.Text = "Create Dapper Classes";
            buttonCreateDapperClasses.UseVisualStyleBackColor = true;
            buttonCreateDapperClasses.Click += buttonCreateDapperClasses_Click;
            // 
            // buttonTestConnection
            // 
            buttonTestConnection.Location = new Point(7, 48);
            buttonTestConnection.Name = "buttonTestConnection";
            buttonTestConnection.Size = new Size(202, 23);
            buttonTestConnection.TabIndex = 5;
            buttonTestConnection.Text = "Test connection";
            buttonTestConnection.UseVisualStyleBackColor = true;
            buttonTestConnection.Click += buttonTestConnection_Click;
            // 
            // button1
            // 
            button1.Location = new Point(238, 22);
            button1.Name = "button1";
            button1.Size = new Size(215, 23);
            button1.TabIndex = 7;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonInsertFromLiteDBToMySQLDB_Click;
            // 
            // IMDbDataFilesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1451, 567);
            Controls.Add(groupBox1);
            Controls.Add(panelInfo);
            Controls.Add(groupBoxUpdate);
            Controls.Add(groupBoxDataFiles);
            Controls.Add(groupBoxGZ);
            Controls.Add(linkLabelIMDBdb);
            Controls.Add(linkLabelLocal);
            Controls.Add(toolStrip1);
            Controls.Add(linkLabelOnline);
            Name = "IMDbDataFilesForm";
            Text = "IMDbDataFilesForm";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBoxGZ.ResumeLayout(false);
            groupBoxDataFiles.ResumeLayout(false);
            groupBoxUpdate.ResumeLayout(false);
            groupBoxUpdate.PerformLayout();
            panelInfo.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LinkLabel linkLabelOnline;
        private FlowLayoutPanel flowLayoutPanelGZ;
        private FlowLayoutPanel flowLayoutPanelDataFiles;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStrip toolStrip2;
        private LinkLabel linkLabelLocal;
        private FlowLayoutPanel flowLayoutPanelUpdate;
        private LinkLabel linkLabelIMDBdb;
        private GroupBox groupBoxGZ;
        private GroupBox groupBoxDataFiles;
        private GroupBox groupBoxUpdate;
        private Panel panelInfo;
        private Button buttonCancel;
        private Button buttonClearDB;
        private RadioButton radioButtonYear;
        private RadioButton radioButtonList;
        private RichTextBox richTextBox1;
        private Button buttonClearAll;
        private CheckBox checkBoxSilent;
        private Button buttonCreateSchemas;
        private GroupBox groupBox1;
        private Button buttonTestConnection;
        private Button buttonCreateDapperClasses;
        private Button button1;
    }
}