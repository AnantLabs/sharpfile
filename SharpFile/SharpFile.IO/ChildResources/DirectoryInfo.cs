using System;
using System.Collections.Generic;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO.Retrievers;
using Common;

namespace SharpFile.IO.ChildResources {
    [Serializable]
    public class DirectoryInfo : FileSystemInfo, IChildResource, IFileContainer {
        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;

        private System.IO.DirectoryInfo directoryInfo;
        private ChildResourceRetrievers childResourceRetrievers;

        public DirectoryInfo(string path, ChildResourceRetrievers childResourceRetrievers)
            : this(new System.IO.DirectoryInfo(path), childResourceRetrievers) {
        }

        public DirectoryInfo(System.IO.DirectoryInfo directoryInfo, ChildResourceRetrievers childResourceRetrievers) :
            this(directoryInfo.Name, directoryInfo.LastWriteTime, directoryInfo.FullName, directoryInfo,
            childResourceRetrievers) {
        }

        public DirectoryInfo(string name, DateTime lastWriteTime, string fullPath, 
            System.IO.DirectoryInfo directoryInfo, ChildResourceRetrievers childResourceRetrievers) {
            this.size = 0;
            this.directoryInfo = directoryInfo;
            this.name = name;
            this.lastWriteTime = lastWriteTime;
            this.fullPath = fullPath;
            this.childResourceRetrievers = childResourceRetrievers;
        }

        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
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

            // Show root directory if specified.
            if (Settings.Instance.ShowRootDirectory) {
                if (!this.Root.Name.Equals(this.FullPath)) {
                    directories.Add(new RootDirectoryInfo(new System.IO.DirectoryInfo(this.Root.FullPath), this.ChildResourceRetrievers));
                }
            }

            // Show parent directory if specified.
            if (Settings.Instance.ShowParentDirectory) {
                if (this.Parent != null) {
                    if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
                        !directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name))) {
                        directories.Add(new ParentDirectoryInfo(this.Parent, this.ChildResourceRetrievers));
                    }
                }
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
            //foreach (IChildResourceRetriever childResourceRetriever in this.ChildResourceRetrievers.Filter(this)) {
            //    childResourceRetriever.Execute(view, this);
            //    break;
            //}

            List<IChildResourceRetriever> childResourceRetrievers = new List<IChildResourceRetriever>(
                ChildResourceRetrievers.Filter(this));

            if (childResourceRetrievers.Count > 0) {
                IChildResourceRetriever childResourceRetriever = childResourceRetrievers[0];
                IView currentView = Forms.GetPropertyInParent<IView>(view.Control.Parent, "View");

                if (childResourceRetriever.View != null) {
                    if (!currentView.GetType().Equals(childResourceRetriever.View.GetType())) {
                        // Set the FileBrowser control (this control's parent) to use this view.
                        childResourceRetriever.View.ColumnInfos = childResourceRetriever.ColumnInfos;

                        Forms.SetPropertyInParent<IView>(view.Control.Parent, "View",
                            childResourceRetriever.View);

                        view = childResourceRetriever.View;
                    }
                }

                childResourceRetrievers[0].Execute(view, this);
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

        public ChildResourceRetrievers ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
        }
    }
}