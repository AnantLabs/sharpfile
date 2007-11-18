using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public static class FileSystem {
		//public static IEnumerable<IResource> GetFiles(System.IO.DirectoryInfo directoryInfo) {
		//    return GetFiles(directoryInfo, string.Empty);
		//}

		//public static IEnumerable<IResource> GetFiles(System.IO.DirectoryInfo directoryInfo, string filter) {
		//    List<IResource> dataInfos = new List<IResource>();

		//    /*
		//    if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
		//        dataInfos.Add(new RootDirectoryInfo(directoryInfo.Root));
		//    }
		//    */

		//    if (directoryInfo.Parent != null) {
		//        //if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
		//            dataInfos.Add(new ParentDirectoryInfo(directoryInfo.Parent));
		//        //}
		//    }

		//    System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
		//    dataInfos.AddRange(
		//        Array.ConvertAll<System.IO.DirectoryInfo, FileSystemInfo>(directoryInfos, delegate(System.IO.DirectoryInfo di) {
		//            return new DirectoryInfo(di);
		//        })
		//    );

		//    System.IO.FileInfo[] fileInfos = getFilteredFiles(directoryInfo, filter);
		//    dataInfos.AddRange(
		//        Array.ConvertAll<System.IO.FileInfo, FileSystemInfo>(fileInfos, delegate(System.IO.FileInfo fi) {
		//            return new FileInfo(fi);
		//        })
		//    );

		//    return dataInfos;
		//}

		//private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, string filter) {
		//    if (string.IsNullOrEmpty(filter)) {
		//        return directoryInfo.GetFiles();
		//    } else {
		//        return directoryInfo.GetFiles("*" + filter + "*", System.IO.SearchOption.TopDirectoryOnly);
		//    }
		//}

		///// <summary>
		///// Get files based on a regex. Not currently used.
		///// </summary>
		//private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, Regex filter) {
		//    return Array.FindAll<System.IO.FileInfo>(directoryInfo.GetFiles(string.Empty, System.IO.SearchOption.TopDirectoryOnly),
		//        delegate(System.IO.FileInfo fi) {
		//            return filter.IsMatch(fi.Name);
		//        });
		//}

		//public static IEnumerable<DriveInfo> GetDrives() {
		//    foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
		//        yield return new DriveInfo(driveInfo);
		//    }
		//}

		//public static IEnumerable<string> GetServers() {
		//    ServerEnum serverEnum = new ServerEnum(ResourceScope.RESOURCE_GLOBALNET,
		//        ResourceType.RESOURCETYPE_DISK,
		//        ResourceUsage.RESOURCEUSAGE_ALL,
		//        ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);

		//    foreach (string server in serverEnum) {
		//        yield return server;
		//    }
		//}
	}
}