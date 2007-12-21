using SharpFile.IO.ChildResources;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
	public class ParentDirectoryInfo : DirectoryInfo {
		public new const string DisplayName = "..";

		public ParentDirectoryInfo(System.IO.DirectoryInfo directoryInfo, IChildResourceRetriever childResourceRetriever)
            : base(directoryInfo, childResourceRetriever) {
			this.displayName = DisplayName;
		}
	}
}