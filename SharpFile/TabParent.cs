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