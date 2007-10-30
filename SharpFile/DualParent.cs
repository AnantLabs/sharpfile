using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.UI;

namespace SharpFile {
	public partial class DualParent : BaseParent {
		public DualParent(Settings settings) : base(settings) {
			InitializeComponent();

			// TODO: Get rid of the designer crap and I wouldn't have to do this.
			base.Resize();

			Child child1 = new Child();
			child1.TabControl.Appearance = TabAppearance.FlatButtons;
			child1.TabControl.IsVisible = true;
			child1.Dock = DockStyle.Fill;
			child1.OnGetImageIndex += delegate(FileSystemInfo fsi, DriveType driveType) {
				return IconManager.GetImageIndex(fsi, ImageList, driveType);
			};

			child1.OnUpdateStatus += delegate(string status) {
				//toolStripStatus.Text = status;
			};

			child1.OnUpdateProgress += delegate(int value) {
				//if (value < 100) {
				//    if (!timer.Enabled) {
				//        progressDisk.Value = 4;
				//        progressDisk.Visible = true;
				//        timer.Enabled = true;
				//    }
				//} else if (value == 100) {
				//    if (timer.Enabled) {
				//        progressDisk.Visible = false;
				//        timer.Enabled = false;
				//    }
				//}
			};

			Child child2 = new Child();
			child2.Dock = DockStyle.Fill;
			child2.TabControl.IsVisible = true;
			child2.TabControl.Appearance = TabAppearance.FlatButtons;
			child2.OnGetImageIndex += delegate(FileSystemInfo fsi, DriveType driveType) {
				return IconManager.GetImageIndex(fsi, ImageList, driveType);
			};

			child2.OnUpdateStatus += delegate(string status) {
				//toolStripStatus.Text = status;
			};

			child2.OnUpdateProgress += delegate(int value) {
				//if (value < 100) {
				//    if (!timer.Enabled) {
				//        progressDisk.Value = 4;
				//        progressDisk.Visible = true;
				//        timer.Enabled = true;
				//    }
				//} else if (value == 100) {
				//    if (timer.Enabled) {
				//        progressDisk.Visible = false;
				//        timer.Enabled = false;
				//    }
				//}
			};

			splitContainer.Panel1.Controls.Add(child1);
			splitContainer.Panel2.Controls.Add(child2);
		}

		public ImageList ImageList {
			get {
				return settings.ImageList;
			}
		}
	}
}