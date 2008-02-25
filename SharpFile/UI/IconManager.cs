using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
	public static class IconManager {
		private const string folderKey = ":FOLDER:";

        /// <summary>
        /// Get the image index for the file system object.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="imageList">ImageList.</param>
        /// <returns>Image index.</returns>
        public static int GetImageIndex(FileSystemInfo fsi, ImageList imageList) {
            int imageIndex = imageList.Images.Count;
            string fullPath = fsi.FullName;

            if (fsi is FileInfo) {
                string extension = ((FileInfo)fsi).Extension;

                if (Settings.Instance.Icons.ShowOverlay ||
                    (Settings.Instance.Icons.Extensions.Contains(extension) ||
                    string.IsNullOrEmpty(extension))) {
                    // Add the full name of the file if it is an executable into the the ImageList.
                    if (!imageList.Images.ContainsKey(fullPath)) {
                        Icon icon = getFileIcon(fullPath, Settings.Instance.Icons.ShowOverlay);
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
            } else if (fsi is DirectoryInfo || fsi is ParentDirectoryInfo || fsi is RootDirectoryInfo) {
                DirectoryInfo directoryInfo = Settings.Instance.Resources.Find(delegate(DirectoryInfo di) {
                    return (di.FullName.ToLower().Equals(fsi.FullName.ToLower()));
                });

                if (fsi is DirectoryInfo && directoryInfo != null) {
                    // Resource is really a drive, so grab its icon.
                    if (!imageList.Images.ContainsKey(fullPath)) {
                        Icon icon = getDriveIcon(fullPath);
                        imageList.Images.Add(fullPath, icon);
                    }

                    imageIndex = imageList.Images.IndexOfKey(fullPath);
                } else {
                    // Add the directory information into the ImageList.
                    if (!imageList.Images.ContainsKey(folderKey)) {
                        Icon icon = getFolderIcon(null, false);
                        imageList.Images.Add(folderKey, icon);
                    }

                    imageIndex = imageList.Images.IndexOfKey(folderKey);
                }
            } else {
                throw new ArgumentException("The object, " + fsi.GetType() + ", is not supported.");
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

        /// <summary>
        /// Get file icon.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Icon derived from the path.</returns>
        private static Icon getFileIcon(string path) {
            return getFileIcon(path, false);
        }

        /// <summary>
        /// Get file icon.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="showOverlay">Whether to show overlay for the file.</param>
        /// <returns>Icon derived from the path.</returns>
        private static Icon getFileIcon(string path, bool showOverlay) {
            return IconReader.GetFileIcon(path, IconReader.IconSize.Small, showOverlay);
        }

        /// <summary>
        /// Get folder icon.
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Icon derived from the path.</returns>
        private static Icon getFolderIcon(string path) {
            return getFolderIcon(path, false);
        }

        /// <summary>
        /// Get folder icon.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="showOverlay">Whether or not to show overlay for the folder.</param>
        /// <returns>Icon derived from the folder.</returns>
        private static Icon getFolderIcon(string path, bool showOverlay) {
            return IconReader.GetFolderIcon(path, IconReader.IconSize.Small, IconReader.FolderType.Closed, showOverlay);
        }

        /// <summary>
        /// Get drive icon.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Icon derived from the drive.</returns>
        private static Icon getDriveIcon(string path) {
            return IconReader.GetDriveIcon(path, IconReader.IconSize.Small, IconReader.FolderType.Closed);
        }
	}
}