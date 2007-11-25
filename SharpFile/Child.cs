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
	public partial class Child : UserControl {
		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(IResource fsi);
		public event OnGetImageIndexDelegate OnGetImageIndex;

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
		void SelectedFileBrowser_OnUpdateStatus(string status) {
			if (OnUpdateStatus != null) {
				OnUpdateStatus(status);
			}
		}

		void SelectedFileBrowser_OnUpdateProgress(int value) {
			if (OnUpdateProgress != null) {
				OnUpdateProgress(value);
			}
		}

		int FileBrowser_OnGetImageIndex(IResource fsi) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(fsi);
			}

			return -1;
		}

		void tabControl_Selected(object sender, TabControlEventArgs e) {
			this.Text = Common.Forms.GetPropertyInChild<string>(this.TabControl.SelectedTab, "Path");
		}

		void fileBrowser_OnUpdatePath(string path) {
			this.Text = path;
		}
		#endregion

		public void AddTab() {
			FileBrowser fileBrowser = new FileBrowser();
			fileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			fileBrowser.OnUpdatePath += fileBrowser_OnUpdatePath;
			fileBrowser.View.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			fileBrowser.View.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
			fileBrowser.View.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;

			this.TabControl.Controls.Add(fileBrowser);
			this.TabControl.SelectedTab = fileBrowser;
			this.TabControl.Selected += new TabControlEventHandler(tabControl_Selected);
		}

		public TabControl TabControl {
			get {
				return tabControl;
			}
		}
	}
}
