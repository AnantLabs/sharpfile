namespace SharpFile.IO {
	public class RootDirectoryInfo : DriveInfo {
		public const string DisplayName = ".";

		public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo) : 
			base(new System.IO.DriveInfo(directoryInfo.FullName)) {
			this.displayName = DisplayName;
		}
	}
}