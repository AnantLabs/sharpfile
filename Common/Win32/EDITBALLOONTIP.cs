using System.Runtime.InteropServices;

namespace Common.Win32 {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct EDITBALLOONTIP {
        public int cbStruct;
        public string pszTitle;
        public string pszText;
        public int ttiIcon;
    }
}