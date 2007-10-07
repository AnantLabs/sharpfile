using System;
using System.Windows.Forms;
using SharpFile.IO;
using SharpFile.Infrastructure;
using SharpFile.UI;
using Common;

namespace SharpFile {
	public partial class Child : Form {
		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo fsi);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public Child() {
			InitializeComponent();
			AddTab();
		}

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

		int FileBrowser_OnGetImageIndex(FileSystemInfo fsi) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(fsi);
			}

			return -1;
		}

		public void AddTab() {
			TabPage tabPage = new TabPage();
			tabPage.FileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			tabPage.FileBrowser.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
			tabPage.FileBrowser.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;
			tabPage.FileBrowser.UpdateDriveListing();

			this.tabControl.Controls.Add(tabPage);
			this.tabControl.SelectedTab = tabPage;
		}

		public ImageList ImageList {
			get {
				return ((Parent)this.MdiParent).ImageList;
			}
		}
	}
}