using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DirectoryInfo {
		public new const string DisplayName = ".";

        public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo, ChildResourceRetrievers childResourceRetrievers) :
            base(directoryInfo, childResourceRetrievers) {
			this.displayName = DisplayName;
		}
	}
}