using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace SharpFile.Infrastructure.Win32 {
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct FILETIME {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }

    /// <summary>
    /// Structure that maps to WIN32_FIND_DATA.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class WIN32_FIND_DATA {
        public FileAttributes Attributes;
        public FILETIME CreationTime;
        public FILETIME LastAccessTime;
        public FILETIME LastWriteTime;
        public int SizeHigh;
        public int SizeLow;
        public int Reserved0;
        public int Reserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string Name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string AlternateName;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct WIN32_FILE_ATTRIBUTE_DATA {
        public FileAttributes Attributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public int SizeHigh;
        public int SizeLow;
    }

    /// <summary>
    /// SafeHandle class for holding find handles
    /// </summary>
    internal sealed class SafeFindHandle : Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid {
        /// <summary>
        /// Constructor
        /// </summary>
        public SafeFindHandle()
            : base(true) {
        }

        /// <summary>
        /// Release the find handle
        /// </summary>
        /// <returns>true if the handle was released</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            return NativeMethods.FindClose(handle);
        }
    }

    /// <summary>
    /// Wrapper for P/Invoke methods used by FileSystemEnumerator
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.Assert, UnmanagedCode = true)]
    internal static class NativeMethods {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern SafeFindHandle FindFirstFile(String fileName, [In, Out] WIN32_FIND_DATA findFileData);

        //[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        //public static extern SafeFindHandle FindFirstFileEx(String fileName, [In, Out] WIN32_FIND_DATA findFileData);

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