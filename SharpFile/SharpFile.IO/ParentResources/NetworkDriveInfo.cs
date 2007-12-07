using SharpFile.IO.Retrievers;

namespace SharpFile.IO.ParentResources {
    public class NetworkDriveInfo : DriveInfo {
        private string uncPath;
        private bool isFileSystemWatchSupported = false;

        public NetworkDriveInfo(System.IO.DriveInfo driveInfo, string uncPath, bool isFileSystemWatchSupported) 
            : base(driveInfo) {
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
