using System;
using System.Windows.Forms;

namespace SharpFile {
	public partial class TabChild : TabPage {
		private Child child;

		public TabChild() {
			initializeComponent();
		}

		private void initializeComponent() {
			this.SuspendLayout();
			this.Name = "TabChild";
			this.Size = new System.Drawing.Size(448, 340);
			this.ResumeLayout(false);

			child = new Child();
			child.Dock = DockStyle.Fill;
			this.Controls.Add(child);
		}
	}
}