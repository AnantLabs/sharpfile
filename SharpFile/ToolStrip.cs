namespace SharpFile {
	public class ToolStrip : System.Windows.Forms.ToolStrip {
		public ToolStrip() {
			// This prevents flicker in the ToolStrip. 
			// It is a protected property, so it is only available if we derive from ToolStrip.
			this.DoubleBuffered = true;
		}
	}
}
