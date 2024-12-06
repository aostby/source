namespace Kolibri.SilverScreen.Forms
{
    partial class MultiMediaForm
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
            splitContainer1 = new SplitContainer();
            textBoxSource = new TextBox();
            buttonSearch = new Button();
            labelNumItems = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(labelNumItems);
            splitContainer1.Panel1.Controls.Add(textBoxSource);
            splitContainer1.Panel1.Controls.Add(buttonSearch);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 49;
            splitContainer1.TabIndex = 0;
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 10);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(704, 23);
            textBoxSource.TabIndex = 1;
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(722, 9);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(75, 23);
            buttonSearch.TabIndex = 0;
            buttonSearch.Text = "Search";
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // labelNumItems
            // 
            labelNumItems.AutoSize = true;
            labelNumItems.Location = new Point(12, 36);
            labelNumItems.Name = "labelNumItems";
            labelNumItems.Size = new Size(38, 15);
            labelNumItems.TabIndex = 2;
            labelNumItems.Text = "Antall";
            // 
            // MultiMediaForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "MultiMediaForm";
            Text = "MultiMediaForm";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TextBox textBoxSource;
        private Button buttonSearch;
        private Label labelNumItems;
    }
}