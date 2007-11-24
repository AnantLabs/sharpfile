using System;
using System.Windows.Forms;
using System.Reflection;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.UI;
using Common;

namespace SharpFile {
	public partial class DualParent :BaseParent {
		protected SplitContainer splitContainer;

		public DualParent(Settings settings)
			: base(settings) {
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
				} else if (value == 100) {
					if (timer.Enabled) {
						progressDisk.Visible = false;
						timer.Enabled = false;
					}
				}
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
				} else if (value == 100) {
					if (timer.Enabled) {
						progressDisk.Visible = false;
						timer.Enabled = false;
					}
				}
			};

			splitContainer.Panel1.Controls.Add(child1);
			splitContainer.Panel2.Controls.Add(child2);
		}

		protected override void addControls() {
			this.splitContainer = new SplitContainer();
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.Size = new System.Drawing.Size(641, 364);
			this.splitContainer.SplitterDistance = 318;
			this.Controls.Add(this.splitContainer);

			base.addControls();
		}

		protected override void onFormClosing() {
			settings.LeftPath = Forms.GetPropertyInChild<string>(this.splitContainer.Panel1, "Path");
			settings.RightPath = Forms.GetPropertyInChild<string>(this.splitContainer.Panel2, "Path");

			base.onFormClosing();
		}

		protected override void onFormLoad() {
			//Forms.SetProperty<string>(this.splitContainer.Panel1, "Path", settings.LeftPath);
			//Forms.SetProperty<string>(this.splitContainer.Panel2, "Path", settings.RightPath);

			base.onFormLoad();
		}
	}
}