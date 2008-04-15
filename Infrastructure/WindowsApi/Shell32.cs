using System;
using System.Runtime.InteropServices;

namespace Common.WindowsApi {
    public class Shell32 {
        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, a folder, a directory, or a drive root.
        /// </summary>
        /// <param name="pszPath"></param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="psfi"></param>
        /// <param name="cbFileInfo"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
            uint cbFileInfo, uint uFlags);
    }
}