using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
	public class BaseParent : Form {
		protected Settings settings;

		public BaseParent(Settings settings) {
			this.settings = settings;

			this.Width = settings.Width;
			this.Height = settings.Height;

			this.FormClosing += delegate(object sender, FormClosingEventArgs e) {
				settings.Width = this.Width;
				settings.Height = this.Height;
			};
		}

		public void Resize() {
			this.Width = settings.Width;
			this.Height = settings.Height;
		}
	}
}