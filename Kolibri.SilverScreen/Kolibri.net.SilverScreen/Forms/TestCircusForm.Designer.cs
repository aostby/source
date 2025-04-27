namespace Kolibri.net.SilverScreen.Forms
{
    partial class TestCircusForm
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
            button1 = new Button();
            button2 = new Button();
            buttonExecuteChange = new Button();
            groupBoxChangePath = new GroupBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            labelFrom = new Label();
            labelTo = new Label();
            groupBoxChangePath.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(29, 12);
            button1.Name = "button1";
            button1.Size = new Size(490, 87);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(569, 32);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // buttonExecuteChange
            // 
            buttonExecuteChange.Location = new Point(150, 84);
            buttonExecuteChange.Name = "buttonExecuteChange";
            buttonExecuteChange.Size = new Size(75, 23);
            buttonExecuteChange.TabIndex = 2;
            buttonExecuteChange.Text = "Execute";
            buttonExecuteChange.UseVisualStyleBackColor = true;
            buttonExecuteChange.Click += buttonExecuteChange_Click;
            // 
            // groupBoxChangePath
            // 
            groupBoxChangePath.Controls.Add(labelTo);
            groupBoxChangePath.Controls.Add(labelFrom);
            groupBoxChangePath.Controls.Add(textBox2);
            groupBoxChangePath.Controls.Add(buttonExecuteChange);
            groupBoxChangePath.Controls.Add(textBox1);
            groupBoxChangePath.Location = new Point(97, 186);
            groupBoxChangePath.Name = "groupBoxChangePath";
            groupBoxChangePath.Size = new Size(231, 116);
            groupBoxChangePath.TabIndex = 3;
            groupBoxChangePath.TabStop = false;
            groupBoxChangePath.Text = "ChangePath";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(56, 26);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(169, 23);
            textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(56, 55);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(169, 23);
            textBox2.TabIndex = 1;
            // 
            // labelFrom
            // 
            labelFrom.AutoSize = true;
            labelFrom.Location = new Point(15, 29);
            labelFrom.Name = "labelFrom";
            labelFrom.Size = new Size(35, 15);
            labelFrom.TabIndex = 3;
            labelFrom.Text = "From";
            // 
            // labelTo
            // 
            labelTo.AutoSize = true;
            labelTo.Location = new Point(15, 58);
            labelTo.Name = "labelTo";
            labelTo.Size = new Size(19, 15);
            labelTo.TabIndex = 4;
            labelTo.Text = "To";
            // 
            // TestCircusForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBoxChangePath);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "TestCircusForm";
            Text = "TestCircusForm";
            groupBoxChangePath.ResumeLayout(false);
            groupBoxChangePath.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button buttonExecuteChange;
        private GroupBox groupBoxChangePath;
        private Label labelTo;
        private Label labelFrom;
        private TextBox textBox2;
        private TextBox textBox1;
    }
}