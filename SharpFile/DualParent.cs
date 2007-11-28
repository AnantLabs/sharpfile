using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.UI;
using Common;

namespace SharpFile {
	public class DualParent : BaseParent {
		protected SplitContainer splitContainer;

		public DualParent() {
			Child child1 = new Child();
			child1.TabControl.Appearance = TabAppearance.FlatButtons;
			child1.TabControl.IsVisible = true;
			child1.Dock = DockStyle.Fill;
			child1.OnGetImageIndex += delegate(IResource fsi) {
			                          	return IconManager.GetImageIndex(fsi, ImageList);
			                          };

			child1.OnUpdateStatus += delegate(string status) {
			                         	toolStripStatus.Text = status;
			                         };

			child1.OnUpdateProgress += delegate(int value) {
			                           	if (value < 100) {
			                           		if (!timer.Enabled) {
			                           			progressDisk.Value = 4;
			                           			progressDisk.Visible = true;
			                           			timer.Enabled = true;
			                           		}
			                           	}
			                           	else if (value == 100) {
			                           		if (timer.Enabled) {
			                           			progressDisk.Visible = false;
			                           			timer.Enabled = false;
			                           		}
			                           	}
			                           };

			child1.OnUpdatePath += delegate(string path) {
			                       	this.Text = string.Format("{0} - {1}",
			                       	                          formName,
			                       	                          path);
			                       };

			Child child2 = new Child();
			child2.Dock = DockStyle.Fill;
			child2.TabControl.IsVisible = true;
			child2.TabControl.Appearance = TabAppearance.FlatButtons;
			child2.OnGetImageIndex += delegate(IResource fsi) {
			                          	return IconManager.GetImageIndex(fsi, ImageList);
			                          };

			child2.OnUpdateStatus += delegate(string status) {
			                         	toolStripStatus.Text = status;
			                         };

			child2.OnUpdateProgress += delegate(int value) {
			                           	if (value < 100) {
			                           		if (!timer.Enabled) {
			                           			progressDisk.Value = 4;
			                           			progressDisk.Visible = true;
			                           			timer.Enabled = true;
			                           		}
			                           	}
			                           	else if (value == 100) {
			                           		if (timer.Enabled) {
			                           			progressDisk.Visible = false;
			                           			timer.Enabled = false;
			                           		}
			                           	}
			                           };

			child2.OnUpdatePath += delegate(string path) {
			                       	this.Text = string.Format("{0} - {1}",
			                       	                          formName,
			                       	                          path);
			                       };

			splitContainer.Panel1.Controls.Add(child1);
			splitContainer.Panel2.Controls.Add(child2);
		}

		protected override void addControls() {
			this.splitContainer = new SplitContainer();
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.Size = new Size(641, 364);
			this.splitContainer.SplitterDistance = 318;

			this.Controls.Add(this.splitContainer);
			base.addControls();
		}

		protected override void onFormClosing() {
			Settings.Instance.LeftPath = Forms.GetPropertyInChild<string>(this.splitContainer.Panel1, "Path");
			Settings.Instance.RightPath = Forms.GetPropertyInChild<string>(this.splitContainer.Panel2, "Path");

			// TODO: Save the splitter position.

			base.onFormClosing();
		}

		protected override void onFormLoad() {
			Forms.SetPropertyInChild<string>(this.splitContainer.Panel1, "Path", Settings.Instance.LeftPath);
			Forms.SetPropertyInChild<string>(this.splitContainer.Panel2, "Path", Settings.Instance.RightPath);

			// TODO: Retrieve the splitter position.

			base.onFormLoad();
		}
	}
}