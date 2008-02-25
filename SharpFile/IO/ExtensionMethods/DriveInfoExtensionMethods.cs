using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpFile.Infrastructure;

namespace SharpFile.ExtensionMethods {
    public static class DriveInfoExtensionMethods {
        /// <summary>
        /// Execute the appropriate child resource retriever for the file 
        /// system object and populate the correct view accordingly.
        /// </summary>
        /// <param name="driveInfo">DriveInfo.</param>
        /// <param name="view">View to use.</param>
        public static void ExtExecute(this DriveInfo driveInfo, IView view) {
            driveInfo.RootDirectory.ExtExecute(view);
        }

        /// <summary>
        /// Get directories.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get directories for.</param>
        /// <returns>List of FileSystemInfo objects.</returns>
        public static IEnumerable<FileSystemInfo> ExtGetDirectories(this DriveInfo driveInfo) {
            return driveInfo.RootDirectory.ExtGetDirectories();
        }

        /// <summary>
        /// Get files.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        /// <returns>List of FileSystemInfo objects.</returns>
        public static IEnumerable<FileSystemInfo> ExtGetFiles(this DriveInfo driveInfo) {
            return ExtGetFiles(driveInfo, string.Empty);
        }

        /// <summary>
        /// Get files.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        /// <param name="filter">Filter what is retrieved.</param>
        /// <returns>List of FileSystemInfo objects.</returns>
        public static IEnumerable<FileSystemInfo> ExtGetFiles(this DriveInfo driveInfo, string filter) {
            return driveInfo.RootDirectory.ExtGetFiles(filter);
        }
    }
}