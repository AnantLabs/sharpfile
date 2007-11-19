using SharpFile.IO.ParentResources;

namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DriveInfo {
		public new const string DisplayName = ".";

		public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo) : 
			base(new System.IO.DriveInfo(directoryInfo.FullName)) {
			this.displayName = DisplayName;
		}
	}
}