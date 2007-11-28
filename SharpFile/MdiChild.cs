using System.Drawing;
using System.Windows.Forms;

namespace SharpFile {
	public class MdiChild : Form {
		private Child child;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public MdiChild() {
			this.child = new Child();
			this.SuspendLayout();

			this.child.Dock = DockStyle.Fill;
			this.child.Location = new Point(0, 0);
			this.child.Size = new Size(497, 379);

			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add(this.child);
			this.ResumeLayout(false);
		}

		public Child Child {
			get {
				return child;
			}
		}
	}
}