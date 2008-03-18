using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpFile.Infrastructure;

namespace SharpFile.ExtensionMethods {
    public static class DriveInfoExtensionMethods {
        ///// <summary>
        ///// Execute the appropriate child resource retriever for the file 
        ///// system object and populate the correct view accordingly.
        ///// </summary>
        ///// <param name="driveInfo">DriveInfo.</param>
        ///// <param name="view">View to use.</param>
        //public static void ExtExecute(this DriveInfo driveInfo, IView view) {
        //    SharpFile.IO.ChildResources.DirectoryInfo directoryInfo = new SharpFile.IO.ChildResources.DirectoryInfo(driveInfo.RootDirectory.FullName);
        //    directoryInfo.Execute(view);
        //}

        ///// <summary>
        ///// Get directories.
        ///// </summary>
        ///// <param name="directoryInfo">DirectoryInfo to get directories for.</param>
        ///// <returns>List of IChildResource objects.</returns>
        //public static IEnumerable<IChildResource> ExtGetDirectories(this DriveInfo driveInfo) {
        //    SharpFile.IO.ChildResources.DirectoryInfo directoryInfo = new SharpFile.IO.ChildResources.DirectoryInfo(driveInfo.RootDirectory.FullName);
        //    return directoryInfo.GetDirectories();
        //}

        ///// <summary>
        ///// Get files.
        ///// </summary>
        ///// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        ///// <returns>List of IChildResource objects.</returns>
        //public static IEnumerable<IChildResource> ExtGetFiles(this DriveInfo driveInfo) {
        //    return ExtGetFiles(driveInfo, string.Empty);
        //}

        ///// <summary>
        ///// Get files.
        ///// </summary>
        ///// <param name="directoryInfo">DirectoryInfo to get files for.</param>
        ///// <param name="filter">Filter what is retrieved.</param>
        ///// <returns>List of IChildResource objects.</returns>
        //public static IEnumerable<IChildResource> ExtGetFiles(this DriveInfo driveInfo, string filter) {
        //    SharpFile.IO.ChildResources.DirectoryInfo directoryInfo = new SharpFile.IO.ChildResources.DirectoryInfo(driveInfo.RootDirectory.FullName);
        //    return directoryInfo.GetFiles(filter);
        //}
    }
}