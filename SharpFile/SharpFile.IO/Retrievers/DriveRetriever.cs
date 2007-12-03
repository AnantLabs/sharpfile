using System.Collections.Generic;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
	public class DriveRetriever : IParentResourceRetriever {
        private IChildResourceRetriever childResourceRetriever = new FileRetriever();

		public IEnumerable<IParentResource> Get() {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
				yield return new DriveInfo(driveInfo);
			}
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return childResourceRetriever;
			}
		}
	}
}