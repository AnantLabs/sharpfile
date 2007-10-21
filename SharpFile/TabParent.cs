using System;
using System.Windows.Forms;

namespace SharpFile {
	public partial class TabParent : Form {
		private static object lockObject = new object();
		private ImageList imageList = new ImageList();

		public TabParent() {
			InitializeComponent();
			AddTab();

			imageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		public void AddTab() {
			TabChild tabChild = new TabChild();
			this.tabControl.Controls.Add(tabChild);
			this.tabControl.SelectedTab = tabChild;

			//FileBrowser fileBrowser = new FileBrowser();
			//fileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			//fileBrowser.OnUpdatePath += fileBrowser_OnUpdatePath;
			//fileBrowser.ListView.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			//fileBrowser.ListView.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
			//fileBrowser.ListView.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;

			//this.tabControl.Controls.Add(fileBrowser);
			//this.tabControl.SelectedTab = fileBrowser;
			//this.tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
		}

		public ImageList ImageList {
			get {
				lock (lockObject) {
					return imageList;
				}
			}
		}
	}
}