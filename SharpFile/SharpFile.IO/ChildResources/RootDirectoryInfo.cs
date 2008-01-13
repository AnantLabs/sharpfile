using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DriveInfo {
		public new const string DisplayName = ".";

        public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo, ChildResourceRetrievers childResourceRetrievers) :
            base(new System.IO.DriveInfo(directoryInfo.FullName), childResourceRetrievers) {
			this.displayName = DisplayName;
		}
	}
}