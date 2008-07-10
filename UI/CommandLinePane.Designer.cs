using System.Windows.Forms;
using System.Drawing;
namespace SharpFile.UI {
    partial class CommandLinePane {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.txtCommandLine = new CommandLineTextBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtResults
            // 
            this.txtResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResults.Location = new System.Drawing.Point(4, 30);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(143, 117);
            this.txtResults.TabIndex = 1;
            //
            // txtCommandLine
            //
            this.txtCommandLine.Location = new Point(4, 4);
            this.txtCommandLine.Name = "txtCommandLine";
            // 
            // CommandLinePanel
            // 
            this.Dock = DockStyle.Top;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.txtCommandLine);
            this.Name = "CommandLinePanel";
            this.Size = new System.Drawing.Size(754, 586);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResults;
        private CommandLineTextBox txtCommandLine;
    }
}