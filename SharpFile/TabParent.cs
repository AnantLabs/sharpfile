using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
	public partial class TabParent : Form {
		private Settings settings;

		public TabParent(Settings settings) {
			this.settings = settings;
			InitializeComponent();
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
				return settings.ImageList;
			}
		}
	}
}