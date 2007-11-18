using System.Collections.Generic;
using SharpFile.IO;
using SharpFile.ParentResources.IO;

namespace SharpFile.Retrievers.IO {
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