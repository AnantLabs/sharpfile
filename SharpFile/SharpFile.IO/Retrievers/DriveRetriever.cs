using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public class DriveRetriever : IParentResourceRetriever {
		public IEnumerable<IParentResource> Get() {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
				yield return new DriveInfo(driveInfo);
			}
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return new FileRetriever();
			}
		}
	}
}