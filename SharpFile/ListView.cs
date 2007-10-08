namespace SharpFile {
	public class ListView : System.Windows.Forms.ListView {
		public ListView() {
			// This prevents flicker in the listview. 
			// It is a protected property, so it is only available if we derive from ListView.
			this.DoubleBuffered = true;
		}
	}
}