namespace SharpFile {
	public class TabPage : System.Windows.Forms.TabPage {
		FileBrowser fileBrowser;

		public TabPage() {
			fileBrowser = new FileBrowser();
			fileBrowser.UpdateDriveListing();

			this.Controls.Add(fileBrowser);
		}

		public FileBrowser FileBrowser {
			get {
				return this.fileBrowser;
			}
		}
	}
}