namespace Kolibri.Desktop.RAPT.Forms
{
    partial class RAPTForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RAPTForm));
            groupBoxTables = new GroupBox();
            flowLayoutPanelTables = new FlowLayoutPanel();
            fastColoredTextBoxResult = new FastColoredTextBoxNS.FastColoredTextBox();
            groupBoxDetails = new GroupBox();
            flowLayoutPanelDetails = new FlowLayoutPanel();
            groupBoxTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBoxResult).BeginInit();
            groupBoxDetails.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxTables
            // 
            groupBoxTables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxTables.Controls.Add(flowLayoutPanelTables);
            groupBoxTables.Location = new Point(20, 19);
            groupBoxTables.Name = "groupBoxTables";
            groupBoxTables.Size = new Size(226, 419);
            groupBoxTables.TabIndex = 0;
            groupBoxTables.TabStop = false;
            groupBoxTables.Text = "Tables";
            // 
            // flowLayoutPanelTables
            // 
            flowLayoutPanelTables.Dock = DockStyle.Fill;
            flowLayoutPanelTables.Location = new Point(3, 19);
            flowLayoutPanelTables.Name = "flowLayoutPanelTables";
            flowLayoutPanelTables.Size = new Size(220, 397);
            flowLayoutPanelTables.TabIndex = 0;
            // 
            // fastColoredTextBoxResult
            // 
            fastColoredTextBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fastColoredTextBoxResult.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            fastColoredTextBoxResult.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            fastColoredTextBoxResult.AutoScrollMinSize = new Size(179, 14);
            fastColoredTextBoxResult.BackBrush = null;
            fastColoredTextBoxResult.CharHeight = 14;
            fastColoredTextBoxResult.CharWidth = 8;
            fastColoredTextBoxResult.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fastColoredTextBoxResult.Hotkeys = resources.GetString("fastColoredTextBoxResult.Hotkeys");
            fastColoredTextBoxResult.IsReplaceMode = false;
            fastColoredTextBoxResult.Location = new Point(502, 1);
            fastColoredTextBoxResult.Name = "fastColoredTextBoxResult";
            fastColoredTextBoxResult.Paddings = new Padding(0);
            fastColoredTextBoxResult.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            fastColoredTextBoxResult.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fastColoredTextBoxResult.ServiceColors");
            fastColoredTextBoxResult.Size = new Size(286, 437);
            fastColoredTextBoxResult.TabIndex = 1;
            fastColoredTextBoxResult.Text = "fastColoredTextBox1";
            fastColoredTextBoxResult.Zoom = 100;
            // 
            // groupBoxDetails
            // 
            groupBoxDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxDetails.Controls.Add(flowLayoutPanelDetails);
            groupBoxDetails.Location = new Point(261, 19);
            groupBoxDetails.Name = "groupBoxDetails";
            groupBoxDetails.Size = new Size(226, 419);
            groupBoxDetails.TabIndex = 1;
            groupBoxDetails.TabStop = false;
            groupBoxDetails.Text = "Tables";
            // 
            // flowLayoutPanelDetails
            // 
            flowLayoutPanelDetails.Dock = DockStyle.Fill;
            flowLayoutPanelDetails.Location = new Point(3, 19);
            flowLayoutPanelDetails.Name = "flowLayoutPanelDetails";
            flowLayoutPanelDetails.Size = new Size(220, 397);
            flowLayoutPanelDetails.TabIndex = 0;
            // 
            // RAPTForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBoxDetails);
            Controls.Add(fastColoredTextBoxResult);
            Controls.Add(groupBoxTables);
            Name = "RAPTForm";
            Text = "RAPTForm";
            groupBoxTables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBoxResult).EndInit();
            groupBoxDetails.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxTables;
        private FlowLayoutPanel flowLayoutPanelTables;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBoxResult;
        private GroupBox groupBoxDetails;
        private FlowLayoutPanel flowLayoutPanelDetails;
    }
}