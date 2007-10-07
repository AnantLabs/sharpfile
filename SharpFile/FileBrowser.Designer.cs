namespace SharpFile {
	partial class FileBrowser {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileBrowser));
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.tlsDrives = new System.Windows.Forms.ToolStripSplitButton();
			this.tlsPath = new System.Windows.Forms.ToolStripTextBox();
			this.tlsFilter = new System.Windows.Forms.ToolStripTextBox();
			this.listView = new SharpFile.ListView();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
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
			this.toolStrip.ShowItemToolTips = false;
			this.toolStrip.Size = new System.Drawing.Size(454, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip";
			// 
			// tlsDrives
			// 
			this.tlsDrives.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsDrives.Image = ((System.Drawing.Image)(resources.GetObject("tlsDrives.Image")));
			this.tlsDrives.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsDrives.Name = "tlsDrives";
			this.tlsDrives.Size = new System.Drawing.Size(32, 22);
			this.tlsDrives.Text = "toolStripSplitButton";
			// 
			// tlsPath
			// 
			this.tlsPath.Name = "tlsPath";
			this.tlsPath.Size = new System.Drawing.Size(100, 25);
			// 
			// tlsFilter
			// 
			this.tlsFilter.Name = "tlsFilter";
			this.tlsFilter.Size = new System.Drawing.Size(50, 25);
			// 
			// listView
			// 
			this.listView.AllowColumnReorder = true;
			this.listView.AllowDrop = true;
			this.listView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listView.LabelEdit = true;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(454, 254);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// FileBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.listView);
			this.Name = "FileBrowser";
			this.Size = new System.Drawing.Size(454, 254);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ListView listView;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripSplitButton tlsDrives;
		private System.Windows.Forms.ToolStripTextBox tlsPath;
		private System.Windows.Forms.ToolStripTextBox tlsFilter;
	}
}
