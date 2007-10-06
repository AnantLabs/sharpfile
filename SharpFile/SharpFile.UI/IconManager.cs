using System;
using System.Windows.Forms;
using System.Drawing;
using SharpFile.IO;

namespace SharpFile.UI {
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

		private static Icon getDriveIcon() {
			// TODO: Actally get the correct drive icon.
			return IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
		}

		public static int GetImageIndex(string text, Font font, ImageList imageList) {
			lock (lockObject) {
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
			}
		}

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

		public static void DeleteImage(string key, ImageListDrag imageList) {
			lock (lockObject) {
				imageList.Images.RemoveByKey(key);
			}
		}
	}
}