using System;
using System.Windows.Forms;
using System.Drawing;
using Etier.IconHelper;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public static class IconManager {
		private const string openFolderKey = ":OPEN_FOLDER:";
		private const string closedFolderKey = ":CLOSED_FOLDER:";

		private readonly static object lockObject = new object();

		private static Icon getFileIcon(FileSystemInfo fileSystemInfo) {
			return IconReader.GetFileIcon(fileSystemInfo.FullPath, IconReader.IconSize.Small, false);
		}

		private static Icon getFolderIcon() {
			return IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
		}

		//private static Icon getDriveIcon() {
			//Icon icon = new Icon();
		//}

		public static int GetImageIndex(FileSystemInfo fileSystemInfo, ImageList imageList) {
			lock (lockObject) {
				int imageIndex = imageList.Images.Count;
				string fullPath = fileSystemInfo.FullPath;

				if (fileSystemInfo is FileInfo) {
					string extension = ((FileInfo)fileSystemInfo).Extension;

					// TODO: Specify the extensions to grab the images from in a config file.
					if (extension.Equals(".exe") ||
						extension.Equals(".lnk") ||
						extension.Equals(".dll") ||
						extension.Equals(".ps") ||
						extension.Equals(".scr") ||
						extension.Equals(".ico") ||
						extension.Equals(".icn") ||
						extension.Equals(string.Empty)) {
						// Add the full name of the file if it is an executable into the the ImageList.
						if (!imageList.Images.ContainsKey(fullPath)) {
							Icon icon = getFileIcon(fileSystemInfo);
							imageList.Images.Add(fullPath, icon);
						}

						imageIndex = imageList.Images.IndexOfKey(fullPath);
					} else {
						// Add the extension into the ImageList.
						if (!imageList.Images.ContainsKey(extension)) {
							Icon icon = getFileIcon(fileSystemInfo);
							imageList.Images.Add(extension, icon);
						}

						imageIndex = imageList.Images.IndexOfKey(extension);
					}
				} else if (fileSystemInfo is DirectoryInfo) {
					// Add the directory information into the ImageList.
					if (!imageList.Images.ContainsKey(closedFolderKey)) {
						Icon icon = getFolderIcon();
						imageList.Images.Add(closedFolderKey, icon);
					}

					imageIndex = imageList.Images.IndexOfKey(closedFolderKey);
				} else if (fileSystemInfo is DriveInfo) {
					// Add the directory information into the ImageList.
					if (!imageList.Images.ContainsKey(closedFolderKey)) {
						Icon icon = getFolderIcon();
						imageList.Images.Add(closedFolderKey, icon);
					}

					imageIndex = imageList.Images.IndexOfKey(closedFolderKey);
				} else {
					throw new ArgumentException("The object, " + fileSystemInfo.GetType() + ", is not supported.");
				}

				return imageIndex;
			}
		}
	}
}