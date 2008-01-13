using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using System.Runtime.Serialization;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
	public class DriveRetriever : IResourceRetriever {
        private ChildResourceRetrievers childResourceRetrievers;

        public IEnumerable<IResource> Get() {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
                //if (driveInfo.DriveType == System.IO.DriveType.Network) {
                //    yield return new NetworkDriveInfo(driveInfo);
                //} else {
                //    yield return new DriveInfo(driveInfo);
                //}

                if (driveInfo.DriveType != System.IO.DriveType.Network) {
                    yield return new DriveInfo(driveInfo, childResourceRetrievers);
                }
			}
		}

		public ChildResourceRetrievers ChildResourceRetrievers {
			get {
				return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
		}
	}
}