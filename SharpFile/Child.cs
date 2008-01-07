using System.Drawing;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using View = SharpFile.Infrastructure.View;

namespace SharpFile {
	public class Child : UserControl {
		public event View.OnUpdateStatusDelegate OnUpdateStatus;
        public event View.OnUpdateProgressDelegate OnUpdateProgress;
        public event View.OnGetImageIndexDelegate OnGetImageIndex;
        public event View.OnUpdatePathDelegate OnUpdatePath;

		private TabControl tabControl;

		public Child(string name) {
            this.Name = name;
			this.tabControl = new TabControl();
			this.SuspendLayout();

			this.tabControl.Dock = DockStyle.Fill;
			this.tabControl.IsVisible = true;

			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.AutoScaleMode = AutoScaleMode.Font;
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
			string path = Forms.GetPropertyInChild<string>(this.TabControl.SelectedTab, "Path");

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
			FileBrowser fileBrowser = new FileBrowser(this.Name);
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