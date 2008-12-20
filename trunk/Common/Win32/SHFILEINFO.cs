using System;
using System.Runtime.InteropServices;

namespace Common.Win32 {
    // Contains information about a file object
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHFILEINFO {
        private const int MAX_PATH = 260;

        public IntPtr hIcon;
        public int iIcon;
        public SFGAO dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
}