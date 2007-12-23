using System;
using System.Collections.Generic;
using SharpFile.IO.Retrievers;
using SharpFile.IO;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    [Serializable]
    public class DirectoryInfo : FileSystemInfo, IChildResource, IFileContainer {
        private System.IO.DirectoryInfo directoryInfo;
        private IChildResourceRetriever childResourceRetriever;

        public DirectoryInfo(string path, IChildResourceRetriever childResourceRetriever)
            : this(new System.IO.DirectoryInfo(path), childResourceRetriever) {
        }

        public DirectoryInfo(System.IO.DirectoryInfo directoryInfo, IChildResourceRetriever childResourceRetriever) {
            this.size = 0;
            this.directoryInfo = directoryInfo;
            this.name = directoryInfo.Name;
            this.lastWriteTime = directoryInfo.LastWriteTime;
            this.fullPath = directoryInfo.FullName;
            this.childResourceRetriever = childResourceRetriever;
        }

        public System.IO.DirectoryInfo Parent {
            get {
                if (directoryInfo.Parent != null) {
                    return directoryInfo.Parent;
                } else {
                    return null;
                }
            }
        }

        public IEnumerable<IChildResource> GetDirectories() {
            System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            List<IChildResource> directories = new List<IChildResource>(directoryInfos.Length);

            // TODO: Setting that specifies whether to show root directory or not.
            /*
            if (!this.Root.Name.Equals(this.FullName)) {
                directories.Add(new RootDirectoryInfo(this.Root));
            }
            */

            // TODO: Setting that specifies whether to show parent directory or not.
            if (this.Parent != null) {
                //if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
                directories.Add(new ParentDirectoryInfo(this.Parent, this.ChildResourceRetriever));
                //}
            }

            directories.AddRange(Array.ConvertAll<System.IO.DirectoryInfo, DirectoryInfo>(directoryInfos,
                delegate(System.IO.DirectoryInfo di) {
                    return new DirectoryInfo(di, this.ChildResourceRetriever);
                }));

            return directories;
        }

        public IEnumerable<IChildResource> GetFiles() {
            return GetFiles(string.Empty);
        }

        public IEnumerable<IChildResource> GetFiles(string filter) {
            System.IO.FileInfo[] fileInfos = null;

            if (string.IsNullOrEmpty(filter)) {
                fileInfos = directoryInfo.GetFiles();
            } else {
                fileInfos = directoryInfo.GetFiles("*" + filter + "*",
                    System.IO.SearchOption.TopDirectoryOnly);
            }

            return Array.ConvertAll<System.IO.FileInfo, FileInfo>(fileInfos,
                delegate(System.IO.FileInfo fi) {
                    return new FileInfo(fi);
                });
        }

        public long GetSize() {
            if (size == 0) {
                size = getSize(directoryInfo);
            }

            return size;
        }

        public void Copy(string destination) {
            if (!destination.EndsWith(@"\")) {
                destination += @"\";
            }

            if (!Exists(destination)) {
                Create(destination);
            }

            foreach (FileInfo fileInfo in GetFiles()) {
                fileInfo.Copy(destination + fileInfo.Name);
            }

            foreach (DirectoryInfo directory in GetDirectories()) {
                string subFolder = directory.Name;
                Create(destination + @"\" + subFolder);
                directory.Copy(destination + @"\" + subFolder);
            }
        }

        public void Move(string destination) {
            System.IO.Directory.Move(this.FullPath, destination);
        }

        public void Execute(IView view) {
            this.ChildResourceRetriever.Get(view, this);
        }

        public static DirectoryInfo Create(string path) {
            System.IO.DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(path);
            return new DirectoryInfo(directoryInfo, new FileRetriever());
        }

        public static bool Exists(string path) {
            return System.IO.Directory.Exists(path);
        }

        private long getSize(System.IO.DirectoryInfo directoryInfo) {
            long totalSize = 0;

            try {
                foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles()) {
                    totalSize += fileInfo.Length;
                }

                System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (System.IO.DirectoryInfo subDirectoryInfo in directoryInfos) {
                    totalSize += getSize(subDirectoryInfo);
                }
            } catch (Exception ex) {
            }

            return totalSize;
        }

        public IChildResourceRetriever ChildResourceRetriever {
            get {
                return childResourceRetriever;
            }
        }
    }
}