namespace SharpFile.IO {
	public class ParentDirectoryInfo : DirectoryInfo {
		public new const string DisplayName = "..";

		public ParentDirectoryInfo(System.IO.DirectoryInfo directoryInfo)
			: base(directoryInfo) {
			this.displayName = DisplayName;
		}
	}
}