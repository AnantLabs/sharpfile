namespace SharpFile.IO {
	public class RootDirectoryInfo : DriveInfo {
		public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo) : 
			base(new System.IO.DriveInfo(directoryInfo.FullName)) {
			this.displayName = ".";
		}
	}
}