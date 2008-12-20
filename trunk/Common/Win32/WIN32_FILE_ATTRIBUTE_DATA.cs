using System;
using System.Runtime.InteropServices;

namespace Common.Win32 {
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct WIN32_FILE_ATTRIBUTE_DATA {
        public System.IO.FileAttributes Attributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public int SizeHigh;
        public int SizeLow;
    }
}