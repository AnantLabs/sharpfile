using System;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class CompressedDirectoryInfo : DirectoryInfo {
        private long compressedSize;

        public CompressedDirectoryInfo(string name, long size, long compressedSize, DateTime lastWriteTime, string fullPath,
            ChildResourceRetrievers childResourceRetrievers) :
            base(name, lastWriteTime, fullPath, null, childResourceRetrievers) {
            this.size = size;
            this.compressedSize = compressedSize;
        }

        public long CompressedSize {
            get {
                return compressedSize;
            }
            set {
                compressedSize = value;
            }
        }

        /*
         public IEnumerable<IChildResource> GetDirectories() {
            System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            List<IChildResource> directories = new List<IChildResource>(directoryInfos.Length);

            // TODO: Setting that specifies whether to show root directory or not.
            /*
            if (!this.Root.Name.Equals(this.FullName)) {
                directories.Add(new RootDirectoryInfo(this.Root));
            }
            */
        /*

            // TODO: Setting that specifies whether to show parent directory or not.
            if (this.Parent != null) {
                //if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
                directories.Add(new ParentDirectoryInfo(this.Parent, this.ChildResourceRetrievers));
                //}
            }

            directories.AddRange(Array.ConvertAll<System.IO.DirectoryInfo, DirectoryInfo>(directoryInfos,
                delegate(System.IO.DirectoryInfo di) {
                    return new DirectoryInfo(di, this.ChildResourceRetrievers);
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
                    return new FileInfo(fi, this.ChildResourceRetrievers);
                });
        }
        */

        /*
         * public long GetSize() {
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
            foreach (IChildResourceRetriever childResourceRetriever in this.ChildResourceRetrievers.Filter(this)) {
                childResourceRetriever.Execute(view, this);
                break;
            }
        }

        public static DirectoryInfo Create(string path) {
            System.IO.DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(path);
            return new DirectoryInfo(directoryInfo, new ChildResourceRetrievers() { new FileRetriever() });
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
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Directory size could not be determined for {0}.",
                    directoryInfo.FullName);
            }

            return totalSize;
        }
         */
    }
}