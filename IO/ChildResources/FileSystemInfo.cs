using System;
using System.Collections.Generic;
using System.IO;
using Common;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.WindowsApi;

namespace SharpFile.IO.ChildResources {
    public abstract class FileSystemInfo : IChildResource {
        public static string DirectorySeparator = System.IO.Path.DirectorySeparatorChar.ToString();

        protected IParentResource root;
        protected FileAttributes attributes;
        protected DateTime creationTime;
        protected DateTime lastAccessTime;
        protected DateTime lastWriteTime;
        protected long size;
        protected string name;
        protected string alternateName;
        protected string displayName;
        protected string path;
		protected IResource parent;

        protected readonly WIN32_FIND_DATA findData;
        protected readonly string fullName;        

        protected FileSystemInfo(string fullName) {
            this.findData = new WIN32_FIND_DATA();
            this.fullName = fullName;

            // Required because the handle is invalid if there is an "\" on the end of directories.
            string validFullName = fullName.EndsWith(DirectorySeparator) ? fullName.Remove(fullName.Length - 1, 1) : fullName;

            using (SafeFindHandle handle = Kernel32.FindFirstFile(validFullName, findData)) {
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

        protected FileSystemInfo(string displayName, string fullName, string alternateName, 
            FileAttributes attributes, long size, DateTime creationTime, DateTime lastAccessTime, 
            DateTime lastWriteTime, IParentResource root) {
            this.displayName = displayName;
            this.fullName = fullName;
            this.alternateName = alternateName;
            this.attributes = attributes;
            this.size = size;
            this.creationTime = creationTime;
            this.lastAccessTime = lastAccessTime;
            this.lastWriteTime = lastWriteTime;
            this.root = root;
        }

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
        public virtual ChildResourceRetrievers GetChildResourceRetrievers(bool filterByFsi) {
            ChildResourceRetrievers childResourceRetrievers = null;

            // Determine the appropriate resource for the particular 
            // file system object and retrieve its child resource retrievers.
            foreach (IParentResourceRetriever resourceRetriever in Settings.Instance.ParentResourceRetrievers) {
                if (resourceRetriever.ParentResources.Find(delegate(IParentResource r) {
					return r.Name.Equals(this.Root.Name, StringComparison.OrdinalIgnoreCase);
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
        public virtual void Execute(IView view) {
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

        /// <summary>
        /// Gets details from the populated WIN32_FIND_DATA.
        /// </summary>
        protected virtual void getDetails() {
            if (findData != null) {
                this.name = findData.Name;
                this.displayName = this.name;
                this.alternateName = findData.AlternateName;
                this.attributes = findData.Attributes;

                // Get LastWriteTime.
                try {
                    lastWriteTime = ConvertFileTimeToDateTime(findData.LastWriteTime);
                } catch { }

                // Get LastAccessTime.
                try {
                    lastAccessTime = ConvertFileTimeToDateTime(findData.LastAccessTime);
                } catch { }

                // Get CreationTime.
                try {
                    creationTime = ConvertFileTimeToDateTime(findData.CreationTime);
                } catch { }
            }
        }

        /// <summary>
        /// Refreshes the WIN32_FIND_DATA.
        /// </summary>
        public void Refresh() {
            using (SafeFindHandle handle = Kernel32.FindFirstFile(
                fullName, findData)) {
                getDetails();
            }
        }

        public abstract void Copy(string destination, bool overwrite);

        public abstract void Move(string destination);

        protected abstract void getSize();

        /// <summary>
        /// Converts the FILETIME structure to the correct DateTime.
        /// </summary>
        /// <param name="fileTime">FILETIME to convert.</param>
        /// <returns>DateTime.</returns>
        private static DateTime ConvertFileTimeToDateTime(FILETIME fileTime) {
            long ticks = (((long)fileTime.dwHighDateTime) << 32) + (long)fileTime.dwLowDateTime;
            DateTime dateTime = DateTime.FromFileTime(ticks);

            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dateTime) == false) {
                dateTime = dateTime.AddHours(1);
            }

            return dateTime;
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
                // TODO: This should actually determine the name instead of just using the fullName.
                if (string.IsNullOrEmpty(name)) {
                    if (fullName.LastIndexOf(DirectorySeparator, 0) > 0) {
                        name = fullName.Remove(0, fullName.LastIndexOf(DirectorySeparator, 0));
                    } else {
                        name = fullName;
                    }
                }

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
                    string rootPath = this.fullName.Substring(0, this.fullName.IndexOf(DirectorySeparator) + 1);
                    //System.IO.Path.GetPathRoot?
                    root = new SharpFile.IO.ParentResources.DriveInfo(rootPath);
                }

                return root;
            }
        }

        public string DisplayName {
            get {
                return displayName;
            }
        }

        public IResource Parent {
            get {
                if (parent == null) {
                    parent = new DirectoryInfo(this.Path);
                }

                return parent;
            }
        }

        public string Path {
            get {
                if (string.IsNullOrEmpty(path) && 
                    !this.FullName.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase)) {
                    path = this.FullName.Replace(this.Name, string.Empty);
                }

                // Get rid of any extra slashes.
                path = path.Replace(DirectorySeparator + DirectorySeparator, DirectorySeparator);

                return path;
            }
        }
    }
}