using System.Windows.Forms;
using SharpFile.IO;
using Common;

namespace SharpFile {
	public partial class MdiChild : Form {
		//public delegate void OnUpdateStatusDelegate(string status);
		//public event OnUpdateStatusDelegate OnUpdateStatus;

		//public delegate void OnUpdateProgressDelegate(int value);
		//public event OnUpdateProgressDelegate OnUpdateProgress;

		//public delegate int OnGetImageIndexDelegate(FileSystemInfo fsi, DriveType driveType);
		//public event OnGetImageIndexDelegate OnGetImageIndex;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public MdiChild() {
			InitializeComponent();
			//AddTab();
		}

		//#region Events.
		//void SelectedFileBrowser_OnUpdateStatus(string status) {
		//    if (OnUpdateStatus != null) {
		//        OnUpdateStatus(status);
		//    }
		//}

		//void SelectedFileBrowser_OnUpdateProgress(int value) {
		//    if (OnUpdateProgress != null) {
		//        OnUpdateProgress(value);
		//    }
		//}

		//int FileBrowser_OnGetImageIndex(FileSystemInfo fsi, DriveType driveType) {
		//    if (OnGetImageIndex != null) {
		//        return OnGetImageIndex(fsi, driveType);
		//    }

		//    return -1;
		//}

		//void tabControl_Selected(object sender, TabControlEventArgs e) {
		//    this.Text = ((FileBrowser)this.child.TabControl.SelectedTab).Path;
		//}

		//void fileBrowser_OnUpdatePath(string path) {
		//    this.Text = path;
		//}
		//#endregion

		//public void AddTab() {
		//    FileBrowser fileBrowser = new FileBrowser();
		//    fileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
		//    fileBrowser.OnUpdatePath += fileBrowser_OnUpdatePath;
		//    fileBrowser.ListView.OnGetImageIndex += FileBrowser_OnGetImageIndex;
		//    fileBrowser.ListView.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
		//    fileBrowser.ListView.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;

		//    this.child.TabControl.Controls.Add(fileBrowser);
		//    this.child.TabControl.SelectedTab = fileBrowser;
		//    this.child.TabControl.Selected += new TabControlEventHandler(tabControl_Selected);
		//}

		//public ImageList ImageList {
		//    get {
		//        if (this.MdiParent != null) {
		//            return ((MdiParent)this.MdiParent).ImageList;
		//        } else {
		//            return ((MdiParent)this.Parent.Parent.Parent).ImageList;
		//        }
		//    }
		//}

		public Child Child {
			get {
				return child;
			}
		}
	}
}