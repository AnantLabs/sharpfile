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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Child));
			this.listView = new SharpFile.ListView();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.tlsDrives = new System.Windows.Forms.ToolStripDropDownButton();
			this.tlsPath = new System.Windows.Forms.ToolStripTextBox();
			this.tlsFilter = new System.Windows.Forms.ToolStripTextBox();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.AllowDrop = true;
			this.listView.LabelEdit = true;
			this.listView.Location = new System.Drawing.Point(3, 31);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(492, 346);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsDrives,
            this.tlsPath,
            this.tlsFilter});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(497, 25);
			this.toolStrip.TabIndex = 4;
			this.toolStrip.Text = "toolStrip1";
			// 
			// tlsDrives
			// 
			this.tlsDrives.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsDrives.Image = ((System.Drawing.Image)(resources.GetObject("tlsDrives.Image")));
			this.tlsDrives.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsDrives.Name = "tlsDrives";
			this.tlsDrives.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.tlsDrives.Size = new System.Drawing.Size(34, 22);
			this.tlsDrives.Text = "toolStripDropDownButton1";
			// 
			// tlsPath
			// 
			this.tlsPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tlsPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.tlsPath.Name = "tlsPath";
			this.tlsPath.Size = new System.Drawing.Size(300, 25);
			// 
			// tlsFilter
			// 
			this.tlsFilter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.tlsFilter.Name = "tlsFilter";
			this.tlsFilter.Size = new System.Drawing.Size(55, 25);
			// 
			// Child
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(497, 379);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.listView);
			this.Name = "Child";
			this.Text = "Child";
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private SharpFile.ListView listView;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripDropDownButton tlsDrives;
		private System.Windows.Forms.ToolStripTextBox tlsPath;
		private System.Windows.Forms.ToolStripTextBox tlsFilter;
    }
}