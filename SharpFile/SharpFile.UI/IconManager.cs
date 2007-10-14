using System;
using System.Windows.Forms;
using System.Drawing;
using SharpFile.IO;

namespace SharpFile.UI {
	public static class IconManager {
		private const string openFolderKey = ":OPEN_FOLDER:";
		private const string closedFolderKey = ":CLOSED_FOLDER:";

		private readonly static object lockObject = new object();

		private static Icon getFileIcon(string path) {
			return getFileIcon(path, false);
		}

		private static Icon getFileIcon(string path, bool showOverlay) {
			return IconReader.GetFileIcon(path, IconReader.IconSize.Small, showOverlay);
		}

		private static Icon getFolderIcon(string path) {
			return getFolderIcon(path, false);
		}

		private static Icon getFolderIcon(string path, bool showOverlay) {
			return IconReader.GetFolderIcon(path, IconReader.IconSize.Small, IconReader.FolderType.Closed, showOverlay);
		}

		private static Icon getDriveIcon(string path) {
			return IconReader.GetDriveIcon(path, IconReader.IconSize.Small, IconReader.FolderType.Closed);
		}

		public static int GetImageIndex(FileSystemInfo fileSystemInfo, ImageList imageList, DriveType driveType) {
			bool showOverlay = true;
			int imageIndex = imageList.Images.Count;
			string fullPath = fileSystemInfo.FullPath;

			if (fileSystemInfo is FileInfo) {
				string extension = ((FileInfo)fileSystemInfo).Extension;

				// TODO: Specify the extensions to grab the images from in a config file.
				if (showOverlay || 
					(extension.Equals(".exe") ||
					extension.Equals(".lnk") ||
					extension.Equals(".dll") ||
					extension.Equals(".ps") ||
					extension.Equals(".scr") ||
					extension.Equals(".ico") ||
					extension.Equals(".icn") ||
					string.IsNullOrEmpty(extension))) {
					// Add the full name of the file if it is an executable into the the ImageList.
					if (!imageList.Images.ContainsKey(fullPath)) {
						Icon icon = getFileIcon(fullPath, showOverlay);
						imageList.Images.Add(fullPath, icon);
					}

					imageIndex = imageList.Images.IndexOfKey(fullPath);
				} else {
					// Add the extension into the ImageList.
					if (!imageList.Images.ContainsKey(extension)) {
						Icon icon = getFileIcon(fullPath);
						imageList.Images.Add(extension, icon);
					}

					imageIndex = imageList.Images.IndexOfKey(extension);
				}
			} else if (fileSystemInfo is DirectoryInfo) {
				// Add the directory information into the ImageList.
				string folderKey = fileSystemInfo.FullPath;

				// Only actively probe directories when they are on a fixed drive.
				// TODO: Specify what drive types should be actively probed.
				if (driveType == DriveType.Fixed) {
					if (!imageList.Images.ContainsKey(folderKey)) {
						Icon icon = getFolderIcon(folderKey, showOverlay);
						imageList.Images.Add(folderKey, icon);
					}
				} else {
					folderKey = closedFolderKey;

					if (!imageList.Images.ContainsKey(closedFolderKey)) {
						Icon icon = getFolderIcon(null, false);
						imageList.Images.Add(folderKey, icon);
					}
				}

				imageIndex = imageList.Images.IndexOfKey(folderKey);
			} else if (fileSystemInfo is DriveInfo) {
				if (!imageList.Images.ContainsKey(fullPath)) {
					Icon icon = getDriveIcon(fullPath);
					imageList.Images.Add(fullPath, icon);
				}

				imageIndex = imageList.Images.IndexOfKey(fullPath);
			} else {
				throw new ArgumentException("The object, " + fileSystemInfo.GetType() + ", is not supported.");
			}

			return imageIndex;
		}

		/*
		public static int GetImageIndex(string text, Font font, ImageList imageList) {
			//lock (lockObject) {
				int imageIndex = imageList.Images.Count;

				// ImageList is buggy, need to ensure we do this:
				IntPtr ilsHandle = imageList.Handle;

				// Create the bitmap to hold the drag image:
				Bitmap bitmap = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height);

				// Get a graphics object from it:
				Graphics gfx = Graphics.FromImage(bitmap);

				// Default fill the bitmap with black:
				gfx.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);

				// Draw text in highlighted form:
				StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit);
				fmt.Alignment = StringAlignment.Center;

				SizeF size = gfx.MeasureString(text, font, bitmap.Width, fmt);

				float left = 0F;
				if (size.Height > bitmap.Height) {
					size.Height = bitmap.Height;
				}
				if (size.Width < bitmap.Width) {
					left = (bitmap.Width - size.Width) / 2F;
				}

				RectangleF textRect = new RectangleF(
					left, 0F, size.Width, size.Height);

				gfx.FillRectangle(SystemBrushes.Highlight, textRect);
				gfx.DrawString(text, font, SystemBrushes.HighlightText,
					textRect, fmt);
				fmt.Dispose();

				// Add the image to the ImageList:
				//int index = GetBitmapIndex(bitmap);
				//ils.Images.Add(bitmap, Color.Black);

				imageList.Images.Add(bitmap, Color.Black);

				// Clear up the graphics object:
				gfx.Dispose();
				// Clear up the bitmap:
				bitmap.Dispose();

				return imageIndex;
			//}
		}
		*/
	}
}