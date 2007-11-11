using System.Windows.Forms;

namespace SharpFile {
	public partial class MdiChild : Form {
		private Child child;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public MdiChild() {
			this.child = new SharpFile.Child();
			this.SuspendLayout();
			// 
			// child
			// 
			this.child.Dock = System.Windows.Forms.DockStyle.Fill;
			this.child.Location = new System.Drawing.Point(0, 0);
			this.child.Size = new System.Drawing.Size(497, 379);
			// 
			// MdiChild
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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