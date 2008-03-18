using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpFile.IO.ChildResources;
using SharpFile.Infrastructure;

namespace SharpFile.ExtensionMethods {
    public static class DirectoryInfoExtensionMethods {
        /// <summary>
        /// Get directories.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get directories for.</param>
        /// <returns>List of FileSystemInfo objects.</returns>
        public static IEnumerable<IChildResource> ExtGetDirectories(this SharpFile.IO.ChildResources.DirectoryInfo directoryInfo) {
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            List<IChildResource> directories = new List<IChildResource>(directoryInfos.Length + 2);

            // Show root directory if specified.
            if (Settings.Instance.ShowRootDirectory) {
                if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
                    directories.Add(new RootDirectoryInfo(directoryInfo.Root.FullName));
                }
            }

            // Show parent directory if specified.
            if (Settings.Instance.ShowParentDirectory) {
                if (directoryInfo.Parent != null) {
                    if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
                        !directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name))) {
                        directories.Add(new ParentDirectoryInfo(directoryInfo.Parent.FullName));
                    }
                }
            }

            directories.AddRange(directoryInfos);

            return directories;
        }

        /// <summary>
        /// Get files.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        /// <returns>List of IChildResource objects.</returns>
        public static IEnumerable<IChildResource> ExtGetFiles(this SharpFile.IO.ChildResources.DirectoryInfo directoryInfo) {
            return ExtGetFiles(directoryInfo, string.Empty);
        }

        /// <summary>
        /// Get files.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        /// <param name="filter">Filter what is retrieved.</param>
        /// <returns>List of IChildResource objects.</returns>
        public static IEnumerable<IChildResource> ExtGetFiles(this SharpFile.IO.ChildResources.DirectoryInfo directoryInfo, string filter) {
            System.IO.FileInfo[] fileInfos = null;

            if (string.IsNullOrEmpty(filter)) {
                fileInfos = directoryInfo.GetFiles();
            } else {
                fileInfos = directoryInfo.GetFiles("*" + filter + "*",
                    System.IO.SearchOption.TopDirectoryOnly);
            }

            return new List<IChildResource>(fileInfos);
        }

        /// <summary>
        /// Copies the directory to the destination.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to copy.</param>
        /// <param name="destination">Destination to copy to.</param>
        public static void ExtCopy(this SharpFile.IO.ChildResources.DirectoryInfo directoryInfo, string destination) {
            // Make sure the destination is correct.
            if (!destination.EndsWith(@"\")) {
                destination += @"\";
            }

            // Create the destination if necessary.
            if (!Directory.Exists(destination)) {
                Directory.CreateDirectory(destination);
            }

            // Copy the files to the destination.
            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
                fileInfo.CopyTo(destination + fileInfo.Name, false);
            }

            // Get the directories and recursively copy them over as well.
            foreach (DirectoryInfo di in directoryInfo.GetDirectories()) {
                di.ExtCopy(destination + @"\" + di.Name);
            }
        }
    }
}