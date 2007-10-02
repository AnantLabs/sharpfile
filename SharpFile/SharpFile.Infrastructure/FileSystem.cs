using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public static class FileSystem {
		private const string allFilesFilter = "*.*";

		public static IEnumerable<FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo) {
			return GetFiles(directoryInfo, allFilesFilter);
		}

		public static IEnumerable<FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo, string filter) {
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

			System.IO.FileInfo[] fileInfos = getFilteredFiles(directoryInfo, filter);
			dataInfos.AddRange(
				Array.ConvertAll<System.IO.FileInfo, FileSystemInfo>(fileInfos, delegate(System.IO.FileInfo fi) {
				return new FileInfo(fi);
			})
			);

			return dataInfos;
		}

		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, string filter) {
			if (filter.Equals(allFilesFilter)) {
				return directoryInfo.GetFiles();
			} else {
				return directoryInfo.GetFiles(filter, System.IO.SearchOption.TopDirectoryOnly);
			}
		}

		/// <summary>
		/// Get files based on a regex. Not currently used.
		/// </summary>
		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, Regex filter) {
			return Array.FindAll<System.IO.FileInfo>(directoryInfo.GetFiles("*.*", System.IO.SearchOption.TopDirectoryOnly),
				delegate(System.IO.FileInfo fi) {
					return filter.IsMatch(fi.Name);
				});
		}

		public static IEnumerable<DriveInfo> GetDrives() {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
				yield return new DriveInfo(driveInfo);
			}
		}
	}
}