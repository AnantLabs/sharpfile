using System.Windows.Forms;
using SharpFile.IO;
using Common;

namespace SharpFile {
	public partial class Child : Form {
		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo fsi, DriveType driveType);
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

		int FileBrowser_OnGetImageIndex(FileSystemInfo fsi, DriveType driveType) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(fsi, driveType);
			}

			return -1;
		}

		public void AddTab() {
			FileBrowser fileBrowser = new FileBrowser();
			fileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			fileBrowser.ListView.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			fileBrowser.ListView.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
			fileBrowser.ListView.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;

			this.tabControl.Controls.Add(fileBrowser);
			this.tabControl.SelectedTab = fileBrowser;
		}
	}
}