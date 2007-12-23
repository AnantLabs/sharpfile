using System;
using System.Collections.Generic;
using SharpFile.IO.Retrievers;
using SharpFile.IO.ChildResources;
using SharpFile.IO;
using SharpFile.Infrastructure;
using System.Runtime.Serialization;

namespace SharpFile.IO.ParentResources {
    [Serializable]
    public class DriveInfo : FileSystemInfo, IResource, IFileContainer {
        private System.IO.DriveInfo driveInfo;
        private string label;
        private string format;
        private long availableFreeSpace;
        private DriveType driveType;
        private bool? isReady;
        private DirectoryInfo directoryInfo;
        private IChildResourceRetriever childResourceRetriever;

        public DriveInfo(System.IO.DriveInfo driveInfo, IChildResourceRetriever childResourceRetriever) {
            this.driveInfo = driveInfo;
            this.name = driveInfo.Name;
            this.driveType = (DriveType)Enum.Parse(typeof(DriveType), driveInfo.DriveType.ToString());
            this.fullPath = name;
            this.childResourceRetriever = childResourceRetriever;

            if (driveInfo.IsReady) {
                this.label = driveInfo.VolumeLabel;
                this.format = driveInfo.DriveFormat;
                this.size = driveInfo.TotalSize;
                this.availableFreeSpace = driveInfo.AvailableFreeSpace;
            }

            this.displayName = string.Format("{0} <{1}>",
                fullPath,
                Common.General.GetHumanReadableSize(availableFreeSpace.ToString()));
        }

        public bool IsReady {
            get {
                if (!isReady.HasValue) {
                    isReady = driveInfo.IsReady;
                }

                return isReady.Value;
            }
        }

        public string Label {
            get {
                return label;
            }
        }

        public DriveType DriveType {
            get {
                return driveType;
            }
        }

        public long AvailableFreeSpace {
            get {
                return availableFreeSpace;
            }
        }

        public string Format {
            get {
                return format;
            }
        }

        public void Execute(IView view) {
            this.ChildResourceRetriever.Get(view, this);
        }

        public IEnumerable<IChildResource> GetDirectories() {
            if (directoryInfo == null) {
                directoryInfo = new DirectoryInfo(this.FullPath, childResourceRetriever);
            }

            return directoryInfo.GetDirectories();
        }

        public IEnumerable<IChildResource> GetFiles() {
            return GetFiles(string.Empty);
        }

        public IEnumerable<IChildResource> GetFiles(string filter) {
            if (directoryInfo == null) {
                directoryInfo = new DirectoryInfo(this.FullPath, childResourceRetriever);
            }

            return directoryInfo.GetFiles(filter);
        }

        public IChildResourceRetriever ChildResourceRetriever {
            get {
                return childResourceRetriever;
            }
        }
    }
}