using System;
using System.Collections.Generic;
using Common;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.ParentResources {
    public class DriveInfo : IParentResource {
        private System.IO.DriveInfo driveInfo;
        private string name;

        public DriveInfo(string name) {
            driveInfo = new System.IO.DriveInfo(name);
        }

        #region IResource Members
        public string FullName {
            get { return driveInfo.Name; }
        }

        public string Name {
            get {
                if (string.IsNullOrEmpty(name)) {
                    name = driveInfo.Name;
                }

                return name;
            }
        }

        public long Size {
            get {
                return driveInfo.TotalSize;
            }
        }

        public bool IsReady {
            get {
                return driveInfo.IsReady;
            }
        }

        public string DisplayName {
            get {
                return this.Name;
            }
        }

        public System.IO.DriveType DriveType {
            get {
                return driveInfo.DriveType;
            }
        }

        public IParentResource Root {
            get {
                return this;
            }
        }

        public string Path {
            get {
                return this.Name;
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
                    return r.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase);
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

        #endregion
    }
}