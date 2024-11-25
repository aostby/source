using System;
using System.Windows.Forms;

namespace Kolibri.Common.FormUtilities.Forms
{
    partial class PrettyPrintForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrettyPrintForm));
            this.textBoxREQUEST = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxREQUEST)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxREQUEST
            // 
            this.textBoxREQUEST.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.textBoxREQUEST.AutoScrollMinSize = new System.Drawing.Size(1379, 14);
            this.textBoxREQUEST.BackBrush = null;
            this.textBoxREQUEST.CharHeight = 14;
            this.textBoxREQUEST.CharWidth = 8;
            this.textBoxREQUEST.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxREQUEST.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.textBoxREQUEST.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxREQUEST.IsReplaceMode = false;
            this.textBoxREQUEST.Location = new System.Drawing.Point(0, 0);
            this.textBoxREQUEST.Name = "textBoxREQUEST";
            this.textBoxREQUEST.Paddings = new System.Windows.Forms.Padding(0);
            this.textBoxREQUEST.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.textBoxREQUEST.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("textBoxREQUEST.ServiceColors")));
            this.textBoxREQUEST.Size = new System.Drawing.Size(699, 261);
            this.textBoxREQUEST.TabIndex = 12;
            this.textBoxREQUEST.Text = "<Instrucitions><Action>Lim inn tekst</Action><Action>Trykk F5 for å formaterte</A" +
    "ction><Information>Lim inn en JSON for å prettyprinte JSON</Information></Instru" +
    "citions>";
            this.textBoxREQUEST.Zoom = 100;
            // 
            // PrettyPrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 261);
            this.Controls.Add(this.textBoxREQUEST);
            this.KeyPreview = true;
            this.Name = "PrettyPrintForm";
            this.Text = "PrettyPrintForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrettyPrintForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.textBoxREQUEST)).EndInit();
            this.ResumeLayout(false);

        }

   

        #endregion

        internal FastColoredTextBoxNS.FastColoredTextBox textBoxREQUEST;
    }
}