using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;

namespace SharpFile.ExtensionMethods {
    public static class FileSystemInfoExtensionMethods {
        ///// <summary>
        ///// Retrieves all child resource retrievers associated with the file system object.
        ///// </summary>
        ///// <param name="fsi">IChildResource object.</param>
        ///// <returns>All child resource retrievers.</returns>
        //public static ChildResourceRetrievers ExtGetChildResourceRetrievers(this IChildResource fsi) {
        //    return fsi.ExtGetChildResourceRetrievers(false);
        //}

        ///// <summary>
        ///// Retrieves the appropriate child resource retrievers associated with the file system object.
        ///// </summary>
        ///// <param name="fsi">File system object.</param>
        ///// <param name="filterByFsi">Whether or not to filter the child resource retrievers by the file system object.</param>
        ///// <returns>Appropriate child resource retrievers.</returns>
        //public static ChildResourceRetrievers ExtGetChildResourceRetrievers(this IChildResource fsi, bool filterByFsi) {
        //    ChildResourceRetrievers childResourceRetrievers = null;
        //    int resourceIndex = 0;

        //    // Determine the appropriate resource for the particular 
        //    // file system object and retrieve its child resource retrievers.
        //    foreach (IResourceRetriever resourceRetriever in Settings.Instance.ResourceRetrievers) {
        //        if (resourceRetriever.DirectoryInfos.Find(delegate(DirectoryInfo di) {
        //            return di.Name.ToLower().Equals(fsi.ExtGetRoot().RootDirectory.Name.ToLower());
        //        }) != null) {
        //            childResourceRetrievers = resourceRetriever.ChildResourceRetrievers;
        //        }
        //    }

        //    if (childResourceRetrievers != null) {
        //        // Filter the child resource retrievers if necessary.
        //        if (filterByFsi) {
        //            childResourceRetrievers = new ChildResourceRetrievers(childResourceRetrievers.Filter(fsi));
        //        }

        //        return childResourceRetrievers;
        //    } else {
        //        throw new Exception("ChildResourceRetrievers not found for " + fsi.Name);
        //    }
        //}

        ///// <summary>
        ///// Execute the appropriate child resource retriever for the file 
        ///// system object and populate the correct view accordingly.
        ///// </summary>
        ///// <param name="fsi">File system object.</param>
        ///// <param name="view">View to populate.</param>
        //public static void ExtExecute(this IChildResource fsi, IView view) {
        //    // Retrieve the correct child resource retrievers for this object.
        //    List<IChildResourceRetriever> childResourceRetrievers = new List<IChildResourceRetriever>(
        //        fsi.ExtGetChildResourceRetrievers(true));

        //    if (childResourceRetrievers.Count > 0) {
        //        // Use the first child resource retriever.
        //        IChildResourceRetriever childResourceRetriever = childResourceRetrievers[0];
        //        IView currentView = Forms.GetPropertyInParent<IView>(view.Control.Parent, "View");

        //        // Compare the current view to view passed in to see if we need to switch one out for the other.
        //        if (childResourceRetriever.View != null) {
        //            if (!currentView.GetType().Equals(childResourceRetriever.View.GetType())) {
        //                // Set the FileBrowser control (this control's parent) to use this view.
        //                childResourceRetriever.View.ColumnInfos = childResourceRetriever.ColumnInfos;

        //                Forms.SetPropertyInParent<IView>(view.Control.Parent, "View",
        //                    childResourceRetriever.View);

        //                view = childResourceRetriever.View;
        //            }
        //        }

        //        // Actually execute the child resource retriever.
        //        childResourceRetriever.Execute(view, fsi);
        //    }
        //}

        ///// <summary>
        ///// Gets the size of the passed in file system object.
        ///// </summary>
        ///// <param name="fsi">File system object.</param>
        ///// <returns>Size of the file system object.</returns>
        //public static long ExtGetSize(this IChildResource fsi) {
        //    long size = getSize(fsi);

        //    return size;
        //}

        ///// <summary>
        ///// Get root for the file system object.
        ///// </summary>
        ///// <param name="fsi">File system object.</param>
        ///// <returns>DriveInfo of the root.</returns>
        //public static DriveInfo ExtGetRoot(this IChildResource fsi) {
        //    string root = fsi.FullName.Substring(0, fsi.FullName.IndexOf('\\'));

        //    return new DriveInfo(root);
        //}

        ///// <summary>
        ///// Copies the file system object to the destination.
        ///// </summary>
        ///// <param name="fsi">File system object to copy.</param>
        ///// <param name="destination">Destination to copy the file system object to.</param>
        //public static void ExtCopy(this IChildResource fsi, string destination) {
        //    if (fsi is FileInfo) {
        //        ((FileInfo)fsi).CopyTo(destination, false);
        //    } else if (fsi is DirectoryInfo) {
        //        ((DirectoryInfo)fsi).ExtCopy(destination);
        //    }
        //}

        ///// <summary>
        ///// Moves the file system object to the destination.
        ///// </summary>
        ///// <param name="fsi">File system object to move.</param>
        ///// <param name="destination">Destination to move the file system object to.</param>
        //public static void ExtMove(this IChildResource fsi, string destination) {
        //    if (fsi is FileInfo) {
        //        ((FileInfo)fsi).MoveTo(destination);
        //    } else if (fsi is DirectoryInfo) {
        //        ((DirectoryInfo)fsi).MoveTo(destination);
        //    }
        //}

        ///// <summary>
        ///// Calculates the size of the file system object.
        ///// </summary>
        ///// <param name="fsi">File system object.</param>
        ///// <returns>Size.</returns>
        //private static long getSize(IChildResource fsi) {
        //    long totalSize = 0;

        //    if (fsi is FileInfo) {
        //        totalSize = ((FileInfo)fsi).Length;
        //    } else if (fsi is DirectoryInfo) {
        //        DirectoryInfo directoryInfo = (DirectoryInfo)fsi;

        //        try {
        //            // Add up all of the file sizes in the directory.
        //            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
        //                totalSize += fileInfo.Length;
        //            }

        //            // Calculate sizes for each sub-directory.
        //            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories()) {
        //                totalSize += getSize(subDirectoryInfo);
        //            }
        //        } catch (Exception ex) {
        //            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
        //                "Directory size could not be determined for {0}.",
        //                directoryInfo.FullName);
        //        }
        //    }

        //    return totalSize;
        //}
    }
}