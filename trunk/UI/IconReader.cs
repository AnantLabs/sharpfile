using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Common.WindowsApi;

namespace SharpFile.UI {
	/// <summary>
	/// Provides static methods to read system icons for both folders and files.
	/// </summary>
	/// <example>
	/// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
	/// </example>
    public class IconReader {
        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        private enum SHGFI : uint {
            ADDOVERLAYS = 0x20,
            ATTR_SPECIFIED = 0x20000,
            ATTRIBUTES = 0x800,
            DISPLAYNAME = 0x200,
            EXETYPE = 0x2000,
            ICON = 0x100,
            ICONLOCATION = 0x1000,
            LARGEICON = 0,
            LINKOVERLAY = 0x8000,
            OPENICON = 2,
            OVERLAYINDEX = 0x40,
            PIDL = 8,
            SELECTED = 0x10000,
            SHELLICONSIZE = 4,
            SMALLICON = 1,
            SYSICONINDEX = 0x4000,
            TYPENAME = 0x400,
            USEFILEATTRIBUTES = 0x10
        }

        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        private enum FILE_ATTRIBUTE {
            READONLY = 0x00000001,
            HIDDEN = 0x00000002,
            SYSTEM = 0x00000004,
            DIRECTORY = 0x00000010,
            ARCHIVE = 0x00000020,
            DEVICE = 0x00000040,
            NORMAL = 0x00000080,
            TEMPORARY = 0x00000100,
            SPARSE_FILE = 0x00000200,
            REPARSE_POINT = 0x00000400,
            COMPRESSED = 0x00000800,
            OFFLINE = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED = 0x00004000
        }

        /// <summary>
        /// Options to specify the size of icons to return.
        /// </summary>
        public enum IconSize {
            /// <summary>
            /// Specify large icon - 32 pixels by 32 pixels.
            /// </summary>
            Large = 0,
            /// <summary>
            /// Specify small icon - 16 pixels by 16 pixels.
            /// </summary>
            Small = 1
        }

        /// <summary>
        /// Options to specify whether folders should be in the open or closed state.
        /// </summary>
        public enum FolderType {
            /// <summary>
            /// Specify open folder.
            /// </summary>
            Open = 0,
            /// <summary>
            /// Specify closed folder.
            /// </summary>
            Closed = 1
        }

        public static Icon GetIcon(IconSize size, string fullName, bool isFile, bool showOverlay, bool isLink) {
            return GetIcon(size, fullName, isFile, true, showOverlay, isLink);
        }

        /// <summary>
        /// Returns an icon for a given file - indicated by the name parameter.
        /// </summary>
        /// <param name="path">Pathname for file.</param>
        /// <param name="size">Large or small</param>
        /// <param name="showOverlay">Whether to include the link icon</param>
        /// <returns>System.Drawing.Icon</returns>
        public static Icon GetIcon(IconSize size, string fullName, bool isFile, bool isIntensiveSearch, bool showOverlay, bool isLink) {
            uint dwAttrs = 0;
            uint flags = 0;

            if (isLink) {
                flags += (uint)SHGFI.LINKOVERLAY;
            }

            // Show overlay for files.
            if (showOverlay) {
                flags += (uint)SHGFI.ADDOVERLAYS;
            }

            if (!showOverlay && !isIntensiveSearch) {
                flags += (uint)SHGFI.USEFILEATTRIBUTES;
            }

            if (isFile) {
                dwAttrs += (uint)FILE_ATTRIBUTE.NORMAL;                
            } else {
                dwAttrs += (uint)FILE_ATTRIBUTE.DIRECTORY;
            }

            // Get the folder icon.
            return getIcon(fullName, size, dwAttrs, flags);
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="path">Path to retrieve the icon.</param>
        /// <param name="dwAttrs">Attributes of the icon.</param>
        /// <param name="size">Size of the icon.</param>
        /// <param name="flags">Flags used when retrieving the icon.</param>
        /// <returns>Icon.</returns>
        private static Icon getIcon(string path, IconSize iconSize, uint dwAttrs, uint flags) {
            flags += (uint)SHGFI.SYSICONINDEX | (uint)SHGFI.ICON;

            /* Check the size specified for return. */
            if (IconSize.Small == iconSize) {
                flags += (uint)SHGFI.SMALLICON;
            } else {
                flags += (uint)SHGFI.LARGEICON;
            }

            Icon icon = null;
            SHFILEINFO shfi = new SHFILEINFO();

            try {
                Common.WindowsApi.Shell32.SHGetFileInfo(path,
                    dwAttrs,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);

                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly.
                icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            }
            finally {
                // Cleanup.
                Common.WindowsApi.User32.DestroyIcon(shfi.hIcon);
            }

            return icon;
        }
    }
}