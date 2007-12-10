using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using System.Runtime.Serialization;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
	public class DriveRetriever : IResourceRetriever {
        private IChildResourceRetriever childResourceRetriever = new FileRetriever();

        public IEnumerable<IResource> Get() {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
                //if (driveInfo.DriveType == System.IO.DriveType.Network) {
                //    yield return new NetworkDriveInfo(driveInfo);
                //} else {
                //    yield return new DriveInfo(driveInfo);
                //}

                if (driveInfo.DriveType != System.IO.DriveType.Network) {
                    yield return new DriveInfo(driveInfo);
                }
			}
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return childResourceRetriever;
			}
		}
	}
}