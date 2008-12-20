using System;
using System.Runtime.InteropServices;

namespace Common.Win32 {
    // Represents the number of 100-nanosecond intervals since January 1, 1601
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct FILETIME {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }
}