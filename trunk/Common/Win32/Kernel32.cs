using System;
using System.Runtime.InteropServices;

namespace Common.Win32 {
    public class Kernel32 {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern SafeFindHandle FindFirstFile(String fileName, [In, Out] WIN32_FIND_DATA findFileData);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		public static extern SafeFindHandle FindFirstFileEx(String fileName, [In, Out] WIN32_FIND_DATA findFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextFile(SafeFindHandle hFindFile, [In, Out] WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll")]
        internal static extern int SetErrorMode(int newMode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetFileAttributesEx(string name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);
    }
}