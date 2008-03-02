using System.Collections.Generic;
using System.IO;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
	public class DriveRetriever : IResourceRetriever {
        private ChildResourceRetrievers childResourceRetrievers;
        private List<IChildResource> directoryInfos;

        /// <summary>
        /// Get a list of drives.
        /// </summary>
        /// <returns>List of drives.</returns>
        public IEnumerable<IChildResource> Get() {
            directoryInfos = new List<IChildResource>();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives()) {
                directoryInfos.Add(driveInfo.RootDirectory);

                yield return driveInfo.RootDirectory;
            }            
		}

        /// <summary>
        /// Returns a list of drive infos.
        /// </summary>
        public List<IChildResource> DirectoryInfos {
            get {
                if (directoryInfos == null) {
                    Get();
                }

                return directoryInfos;
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