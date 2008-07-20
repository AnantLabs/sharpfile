using System.Drawing;
namespace SharpFile.UI {
    partial class CommandLineTextBox {
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
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.chkUseCommandLine = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.BackColor = System.Drawing.Color.White;
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPath.Location = new System.Drawing.Point(0, 0);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(100, 13);
            this.txtPath.TabIndex = 0;
            // 
            // txtFile
            // 
            this.txtFile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFile.Location = new System.Drawing.Point(0, 19);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(100, 13);
            this.txtFile.TabIndex = 1;
            // 
            // chkUseCommandLine
            // 
            this.chkUseCommandLine.AutoSize = true;
            this.chkUseCommandLine.Location = new System.Drawing.Point(234, 4);
            this.chkUseCommandLine.Name = "chkUseCommandLine";
            this.chkUseCommandLine.Size = new System.Drawing.Size(70, 14);
            this.chkUseCommandLine.TabIndex = 2;
            this.chkUseCommandLine.Text = "Cmd Line";
            this.chkUseCommandLine.UseVisualStyleBackColor = true;
            this.chkUseCommandLine.BackColor = Color.White;
            // 
            // CommandLineTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUseCommandLine);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.txtFile);
            this.Name = "CommandLineTextBox";
            this.Size = new System.Drawing.Size(349, 75);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.CheckBox chkUseCommandLine;
    }
}
