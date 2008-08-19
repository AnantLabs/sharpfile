using System.Collections.Generic;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure.IO.Retrievers {
	public class DriveRetriever : IParentResourceRetriever {
        private ChildResourceRetrievers childResourceRetrievers;
        private List<IParentResource> driveInfos;

        /// <summary>
        /// Get a list of drives.
        /// </summary>
        /// <returns>List of drives.</returns>
        public IEnumerable<IParentResource> Get() {
            driveInfos = new List<IParentResource>();

            foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
                driveInfos.Add(new DriveInfo(driveInfo.Name));

                yield return new DriveInfo(driveInfo.Name);
            }            
		}

        /// <summary>
        /// Returns a list of drive infos.
        /// </summary>
        public List<IParentResource> ParentResources {
            get {
                if (driveInfos == null) {
                    Get();
                }

                return driveInfos;
            }
        }

        /// <summary>
        /// The child resource retrievers for the DriveRetriever.
        /// </summary>
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