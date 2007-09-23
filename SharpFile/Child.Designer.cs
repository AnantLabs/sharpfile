namespace SharpFile
{
    partial class Child
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
			this.listView = new System.Windows.Forms.ListView();
			this.ddlDrives = new System.Windows.Forms.ComboBox();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.txtPattern = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.Location = new System.Drawing.Point(3, 31);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(492, 346);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			// 
			// ddlDrives
			// 
			this.ddlDrives.BackColor = System.Drawing.SystemColors.Window;
			this.ddlDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlDrives.FormattingEnabled = true;
			this.ddlDrives.Location = new System.Drawing.Point(3, 5);
			this.ddlDrives.Name = "ddlDrives";
			this.ddlDrives.Size = new System.Drawing.Size(121, 21);
			this.ddlDrives.TabIndex = 1;
			// 
			// txtPath
			// 
			this.txtPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.txtPath.Location = new System.Drawing.Point(131, 5);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(100, 20);
			this.txtPath.TabIndex = 2;
			// 
			// txtPattern
			// 
			this.txtPattern.Location = new System.Drawing.Point(238, 5);
			this.txtPattern.Name = "txtPattern";
			this.txtPattern.Size = new System.Drawing.Size(100, 20);
			this.txtPattern.TabIndex = 3;
			// 
			// Child
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(497, 379);
			this.Controls.Add(this.txtPattern);
			this.Controls.Add(this.txtPath);
			this.Controls.Add(this.ddlDrives);
			this.Controls.Add(this.listView);
			this.Name = "Child";
			this.Text = "Child";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ComboBox ddlDrives;
		private System.Windows.Forms.TextBox txtPath;
		private System.Windows.Forms.TextBox txtPattern;
    }
}