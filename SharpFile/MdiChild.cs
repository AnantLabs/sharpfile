using System.Windows.Forms;

namespace SharpFile {
	public partial class MdiChild : Form {
		/// <summary>
		/// Child ctor.
		/// </summary>
		public MdiChild() {
			InitializeComponent();
		}

		public Child Child {
			get {
				return child;
			}
		}
	}
}