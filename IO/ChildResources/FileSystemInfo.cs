using System;
using System.IO;
using SharpFile.Infrastructure.Win32;
using SharpFile.Infrastructure;
using System.Collections.Generic;
using Common;

namespace SharpFile.IO.ChildResources {
    public abstract class FileSystemInfo : IChildResource {
        private IParentResource root;
        protected WIN32_FIND_DATA findData;
        protected FileAttributes attributes;
        protected DateTime creationTime;
        protected DateTime lastAccessTime;
        protected DateTime lastWriteTime;
        protected long size;
        protected string name;
        protected string alternateName;
        protected string displayName;

        protected readonly string fullName;        

        protected FileSystemInfo(string fullName) {
            this.findData = new WIN32_FIND_DATA();
            this.fullName = fullName;

            // Required because the handle is invalid if there is an "\" on the end of directories.
            string validFullName = fullName.EndsWith(@"\") ? fullName.Remove(fullName.Length - 1, 1) : fullName;

            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                validFullName, findData)) {
                if (handle.IsInvalid) {
                    throw new Exception("Filesystem object cannot be found for " + fullName);
                } else {
                    getDetails();
                }
            }
        }

        protected FileSystemInfo(string fullName, WIN32_FIND_DATA findData) {
            this.fullName = fullName;
            this.findData = findData;

            getDetails();
        }

        protected abstract void getSize();

        public abstract void Copy(string destination, bool overwrite);

        public abstract void Move(string destination);

        /// <summary>
        /// Retrieves all child resource retrievers associated with the file system object.
        /// </summary>
        /// <param name="fsi">IChildResource object.</param>
        /// <returns>All child resource retrievers.</returns>
        public ChildResourceRetrievers GetChildResourceRetrievers() {
            return GetChildResourceRetrievers(false);
        }

        /// <summary>
        /// Retrieves the appropriate child resource retrievers associated with the file system object.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="filterByFsi">Whether or not to filter the child resource retrievers by the file system object.</param>
        /// <returns>Appropriate child resource retrievers.</returns>
        public ChildResourceRetrievers GetChildResourceRetrievers(bool filterByFsi) {
            ChildResourceRetrievers childResourceRetrievers = null;

            // Determine the appropriate resource for the particular 
            // file system object and retrieve its child resource retrievers.
            foreach (IParentResourceRetriever resourceRetriever in Settings.Instance.ParentResourceRetrievers) {
                if (resourceRetriever.ParentResources.Find(delegate(IParentResource r) {
                    return r.Name.ToLower().Equals(this.Root.Name.ToLower());
                }) != null) {
                    childResourceRetrievers = resourceRetriever.ChildResourceRetrievers;
                }
            }

            if (childResourceRetrievers != null) {
                // Filter the child resource retrievers if necessary.
                if (filterByFsi) {
                    childResourceRetrievers = new ChildResourceRetrievers(childResourceRetrievers.Filter(this));
                }

                return childResourceRetrievers;
            } else {
                throw new Exception("ChildResourceRetrievers not found for " + this.Name);
            }
        }

        /// <summary>
        /// Execute the appropriate child resource retriever for the file 
        /// system object and populate the correct view accordingly.
        /// </summary>
        /// <param name="view">View to populate.</param>
        public void Execute(IView view) {
            // Retrieve the correct child resource retrievers for this object.
            List<IChildResourceRetriever> childResourceRetrievers = new List<IChildResourceRetriever>(
                GetChildResourceRetrievers(true));

            if (childResourceRetrievers.Count > 0) {
                // Use the first child resource retriever.
                IChildResourceRetriever childResourceRetriever = childResourceRetrievers[0];
                IView currentView = Forms.GetPropertyInParent<IView>(view.Control.Parent, "View");

                // Compare the current view to view passed in to see if we need to switch one out for the other.
                if (childResourceRetriever.View != null) {
                    if (!currentView.GetType().Equals(childResourceRetriever.View.GetType())) {
                        // Set the FileBrowser control (this control's parent) to use this view.
                        childResourceRetriever.View.ColumnInfos = childResourceRetriever.ColumnInfos;

                        Forms.SetPropertyInParent<IView>(view.Control.Parent, "View",
                            childResourceRetriever.View);

                        view = childResourceRetriever.View;
                    }
                }

                // Actually execute the child resource retriever.
                childResourceRetriever.Execute(view, this);
            }
        }

        protected void getDetails() {
            if (findData != null) {
                this.name = findData.Name;
                this.displayName = this.name;
                this.alternateName = findData.AlternateName;
                this.attributes = findData.Attributes;

                // Get LastWriteTime.
                try {
                    long highDateTime = findData.LastWriteTime.dwHighDateTime;
                    long lowDateTime = findData.LastWriteTime.dwLowDateTime;
                    long fileTime = (highDateTime << 32) + lowDateTime;

                    lastWriteTime = DateTime.FromFileTime(fileTime);
                } catch { }

                // Get LastAccessTime.
                try {
                    long highDateTime = findData.LastAccessTime.dwHighDateTime;
                    long lowDateTime = findData.LastAccessTime.dwLowDateTime;
                    long fileTime = (highDateTime << 32) + lowDateTime;

                    lastAccessTime = DateTime.FromFileTime(fileTime);
                } catch { }

                // Get CreationTime.
                try {
                    long highDateTime = findData.CreationTime.dwHighDateTime;
                    long lowDateTime = findData.CreationTime.dwLowDateTime;
                    long fileTime = (highDateTime << 32) + lowDateTime;

                    creationTime = DateTime.FromFileTime(fileTime);
                } catch { }
            }
        }

        public void Refresh() {
            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                fullName, findData)) {
                getDetails();
            }
        }

        private void getRoot() {
            string rootPath = this.fullName.Substring(0, this.fullName.IndexOf('\\') + 1);
            root = new SharpFile.IO.ParentResources.DriveInfo(rootPath);
        }

        public string FullName {
            get {
                return fullName;
            }
        }

        public FileAttributes Attributes {
            get {
                return attributes;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public DateTime LastWriteTime {
            get {
                return lastWriteTime;
            }
        }

        public DateTime LastAccessTime {
            get {
                return lastAccessTime;
            }
        }

        public DateTime CreationTime {
            get {
                return creationTime;
            }
        }

        public long Size {
            get {
                if (size == 0) {
                    getSize();
                }

                return size;
            }
        }

        public IParentResource Root {
            get {
                if (root == null) {
                    getRoot();
                }

                return root;
            }
        }

        public string DisplayName {
            get {
                return displayName;
            }
        }
    }
}