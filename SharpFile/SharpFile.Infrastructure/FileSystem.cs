using System;
using System.Management;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public static class FileSystem {
		private const string allFilesPattern = "*.*";

		public static IEnumerable<FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo) {
			return GetFiles(directoryInfo, allFilesPattern);
		}

		public static IEnumerable<FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo, string pattern) {
			List<FileSystemInfo> dataInfos = new List<FileSystemInfo>();

			if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
				dataInfos.Add(new RootDirectoryInfo(directoryInfo.Root));
			}

			if (directoryInfo.Parent != null) {
				dataInfos.Add(new ParentDirectoryInfo(directoryInfo.Parent));
			}

			System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
			dataInfos.AddRange(
				Array.ConvertAll<System.IO.DirectoryInfo, FileSystemInfo>(directoryInfos, delegate(System.IO.DirectoryInfo di) {
				return new DirectoryInfo(di);
			})
			);

			System.IO.FileInfo[] fileInfos = getFilteredFiles(directoryInfo, pattern);
			dataInfos.AddRange(
				Array.ConvertAll<System.IO.FileInfo, FileSystemInfo>(fileInfos, delegate(System.IO.FileInfo fi) {
				return new FileInfo(fi);
			})
			);

			return dataInfos;
		}

		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, string pattern) {
			if (pattern.Equals(allFilesPattern)) {
				return directoryInfo.GetFiles();
			} else {
				return directoryInfo.GetFiles(pattern, System.IO.SearchOption.TopDirectoryOnly);
			}
		}

		/// <summary>
		/// Get files based on a regex. Not currently used.
		/// </summary>
		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, Regex pattern) {
			return Array.FindAll<System.IO.FileInfo>(directoryInfo.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly),
				delegate(System.IO.FileInfo fi) {
					return pattern.IsMatch(fi.Name);
				});
		}

		public static IEnumerable<DriveInfo> GetDrives() {
			foreach (ManagementObject managementObject in GetManagementObjects("Win32_LogicalDisk")) {
				int driveType_i = managementObject["DriveType"] != null ?
					int.Parse(managementObject["DriveType"].ToString()) : 0;

				if (driveType_i >= 2 &&
					driveType_i <= 6) {
					string name = managementObject["Name"].ToString();
					DriveType driveType = (DriveType)Enum.Parse(typeof(DriveType), driveType_i.ToString());
					string description = managementObject["Description"] != null ?
						managementObject["Description"].ToString() : string.Empty;
					string providerName = managementObject["ProviderName"] != null ?
						managementObject["ProviderName"].ToString() : "";
					long freeSpace = managementObject["FreeSpace"] != null ?
						long.Parse(managementObject["FreeSpace"].ToString()) : 0;
					long size = managementObject["Size"] != null ?
						long.Parse(managementObject["Size"].ToString()) : 0;

					DriveInfo driveInfo = new DriveInfo(name, providerName, driveType, description, size, freeSpace);
					yield return driveInfo;
				}
			}
		}

		public static ManagementObjectCollection GetManagementObjects(string path) {
			ManagementClass managementClass = new ManagementClass(path);
			ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			return managementObjectCollection;
		}
	}
}