using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
	public static class IconManager {
        private static readonly object lockObject = new object();
        private static Dictionary<string, int> iconHash = new Dictionary<string, int>();
        private const string folderKey = ":FOLDER_KEY:";

        /// <summary>
        /// Get the image index for the file system object.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="imageList">ImageList.</param>
        /// <returns>Image index.</returns>
        public static int GetImageIndex(IResource resource, ImageList imageList) {
            // Prevent more than one thread from updating the ImageList at a time.
            lock (lockObject) {
                IconReader.IconSize iconSize = IconReader.IconSize.Small;
                int imageIndex = iconHash.Count;
                string fullPath = resource.FullName.ToLower();
                bool showOverlay = false;

                // Specifies whether overlays are turned on for all files, or if they have 
                // been turned on specifically for some paths.
                if (Settings.Instance.Icons.ShowOverlay ||
                    Settings.Instance.Icons.OverlayPaths.Find(delegate(string s) {
                        return (fullPath.Contains(s));
                    }) != null) {
                    showOverlay = true;
                }

                if (resource is FileInfo) {
                    string extension = ((FileInfo)resource).Extension;

                    // Lower-case the extension if it is available.
                    if (!string.IsNullOrEmpty(extension)) {
                        extension = extension.ToLower();
                    }

                    if (showOverlay ||
                        (string.IsNullOrEmpty(extension) ||
                        Settings.Instance.Icons.Extensions.Contains(extension))) {
                        // Add the full name of the file if it is an executable into the ImageList.
                        if (!iconHash.ContainsKey(fullPath)) {
                            Icon icon = IconReader.GetIcon(resource, iconSize, showOverlay);
                            imageList.Images.Add(fullPath, icon);
                            iconHash.Add(fullPath, imageIndex);
                        }

                        imageIndex = iconHash[fullPath];
                    } else {
                        // Add the extension into the ImageList.
                        if (!iconHash.ContainsKey(extension)) {
                            Icon icon = IconReader.GetIcon(resource, iconSize, showOverlay);
                            imageList.Images.Add(extension, icon);
                            iconHash.Add(extension, imageIndex);
                        }

                        imageIndex = iconHash[extension];
                    }
                } else {
                    if (!iconHash.ContainsKey(fullPath)) {
                        Icon icon = IconReader.GetIcon(resource, iconSize, showOverlay);
                        imageList.Images.Add(fullPath, icon);
                        iconHash.Add(fullPath, imageIndex);
                    }

                    imageIndex = iconHash[fullPath];
                }

                return imageIndex;
            }
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