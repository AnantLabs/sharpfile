using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
	public partial class TabParent : Form {
		private TabControl tabControl;

		public TabParent() {
			this.tabControl = new SharpFile.TabControl();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.IsVisible = true;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.TabIndex = 0;
			// 
			// TabParent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.ResumeLayout(false);

			AddTab();
		}

		public void AddTab() {
			TabChild tabChild = new TabChild();
			
			// TODO: Wire tab child to OnGetImageIndex() to actually grab images.
			this.tabControl.Controls.Add(tabChild);
			this.tabControl.SelectedTab = tabChild;
		}

		public ImageList ImageList {
			get {
				return Settings.Instance.ImageList;
			}
		}
	}
}