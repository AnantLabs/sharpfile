using System.Collections.Generic;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ParentResources {
    public class NetworkDriveInfo : DriveInfo {
        private string uncPath;
        private bool isFileSystemWatchSupported = false;

        public NetworkDriveInfo(System.IO.DriveInfo driveInfo, ChildResourceRetrievers childResourceRetrievers, string uncPath, bool isFileSystemWatchSupported)
            : base(driveInfo, childResourceRetrievers) {
            this.uncPath = uncPath;
            this.isFileSystemWatchSupported = isFileSystemWatchSupported;
        }

        public string UncPath {
            get {
                return uncPath;
            }
        }

        public bool IsFileSystemWatchSupported {
            get {
                return isFileSystemWatchSupported;
            }
        }
    }
}
