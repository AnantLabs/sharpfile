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
			this.tabControl = new SharpFile.TabControl();
			this.tabPage = new System.Windows.Forms.TabPage();
			this.listView = new SharpFile.ListView();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.tlsDrives = new System.Windows.Forms.ToolStripDropDownButton();
			this.tlsPath = new System.Windows.Forms.ToolStripTextBox();
			this.tlsFilter = new System.Windows.Forms.ToolStripTextBox();
			this.tabControl.SuspendLayout();
			this.tabPage.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl.Controls.Add(this.tabPage);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.IsVisible = true;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = new System.Drawing.Point(0, 0);
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(497, 379);
			this.tabControl.TabIndex = 5;
			// 
			// tabPage
			// 
			this.tabPage.Controls.Add(this.listView);
			this.tabPage.Controls.Add(this.toolStrip);
			this.tabPage.Location = new System.Drawing.Point(4, 25);
			this.tabPage.Name = "tabPage";
			this.tabPage.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage.Size = new System.Drawing.Size(489, 350);
			this.tabPage.TabIndex = 0;
			this.tabPage.Text = "tabPage";
			this.tabPage.UseVisualStyleBackColor = true;
			// 
			// listView
			// 
			this.listView.AllowColumnReorder = true;
			this.listView.AllowDrop = true;
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.LabelEdit = true;
			this.listView.Location = new System.Drawing.Point(3, 28);
			this.listView.Margin = new System.Windows.Forms.Padding(0);
			this.listView.Name = "listView";
			this.listView.ShowItemToolTips = true;
			this.listView.Size = new System.Drawing.Size(483, 319);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsDrives,
            this.tlsPath,
            this.tlsFilter});
			this.toolStrip.Location = new System.Drawing.Point(3, 3);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(483, 25);
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
			// tabPage
			// 
			this.tabPage.Location = new System.Drawing.Point(4, 25);
			this.tabPage.Name = "tabPage1";
			this.tabPage.Size = new System.Drawing.Size(489, 350);
			this.tabPage.TabIndex = 1;
			this.tabPage.Text = "tabPage";
			this.tabPage.Visible = false;
			this.tabPage.Controls.Add(this.toolStrip);
			this.tabPage.Controls.Add(this.listView);
			// 
			// Child
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(497, 379);
			this.Controls.Add(this.tabControl);
			this.Name = "Child";
			this.Text = "Child";
			this.tabControl.ResumeLayout(false);
			this.tabPage.ResumeLayout(false);
			this.tabPage.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TabPage tabPage;
		private ListView listView;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripDropDownButton tlsDrives;
		private System.Windows.Forms.ToolStripTextBox tlsPath;
		private System.Windows.Forms.ToolStripTextBox tlsFilter;
		private SharpFile.TabControl tabControl;

	}
}