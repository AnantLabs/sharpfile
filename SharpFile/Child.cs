using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpFile.IO;
using SharpFile.UI;

namespace SharpFile {
	public partial class Child :UserControl {
		public event SharpFile.IO.View.OnUpdateStatusDelegate OnUpdateStatus;
		public event SharpFile.IO.View.OnUpdateProgressDelegate OnUpdateProgress;
		public event SharpFile.IO.View.OnGetImageIndexDelegate OnGetImageIndex;
		public event SharpFile.IO.View.OnUpdatePathDelegate OnUpdatePath;

		private TabControl tabControl;

		public Child() {
			this.tabControl = new SharpFile.TabControl();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.IsVisible = true;
			// 
			// Child
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.ResumeLayout(false);

			AddTab();
		}

		#region Events.
		private void view_OnUpdateStatus(string status) {
			if (OnUpdateStatus != null) {
				OnUpdateStatus(status);
			}
		}

		private void view_OnUpdateProgress(int value) {
			if (OnUpdateProgress != null) {
				OnUpdateProgress(value);
			}
		}

		private int fileBrowser_OnGetImageIndex(IResource fsi) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(fsi);
			}

			return -1;
		}

		private void tabControl_Selected(object sender, TabControlEventArgs e) {
			string path = Common.Forms.GetPropertyInChild<string>(this.TabControl.SelectedTab, "Path");
			
			this.Text = path;
			fileBrowser_OnUpdatePath(path);
		}

		private void fileBrowser_OnUpdatePath(string path) {
			if (OnUpdatePath != null) {
				OnUpdatePath(path);
			}
		}
		#endregion

		public void AddTab() {
			FileBrowser fileBrowser = new FileBrowser();
			fileBrowser.OnGetImageIndex += fileBrowser_OnGetImageIndex;
			fileBrowser.OnUpdatePath += fileBrowser_OnUpdatePath;
			fileBrowser.View.OnGetImageIndex += fileBrowser_OnGetImageIndex;
			fileBrowser.View.OnUpdateProgress += view_OnUpdateProgress;
			fileBrowser.View.OnUpdateStatus += view_OnUpdateStatus;
			fileBrowser.View.OnUpdatePath += fileBrowser_OnUpdatePath;

			this.TabControl.Controls.Add(fileBrowser);
			this.TabControl.SelectedTab = fileBrowser;
			this.TabControl.Selected += tabControl_Selected;
		}

		public TabControl TabControl {
			get {
				return tabControl;
			}
		}
	}
}