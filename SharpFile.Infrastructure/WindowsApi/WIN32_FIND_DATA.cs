using System.Runtime.InteropServices;

namespace SharpFile.Infrastructure.WindowsApi {
    /// <summary>
    /// Structure that maps to WIN32_FIND_DATA.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public sealed class WIN32_FIND_DATA {
        public System.IO.FileAttributes Attributes;
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
}