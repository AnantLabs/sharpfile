using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
	public class TabParent : Form {
		private TabControl tabControl;

		public TabParent() {
			this.tabControl = new TabControl();
			this.SuspendLayout();

			this.tabControl.Dock = DockStyle.Fill;
			this.tabControl.IsVisible = true;
			this.tabControl.Location = new Point(0, 0);
			this.tabControl.TabIndex = 0;

			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.ResumeLayout(false);

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
                return Settings.Instance.ImageList;
			}
		}
	}
}