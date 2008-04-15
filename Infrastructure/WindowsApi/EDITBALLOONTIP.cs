using System.Runtime.InteropServices;

namespace SharpFile.Infrastructure.WindowsApi {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct EDITBALLOONTIP {
        public int cbStruct;
        public string pszTitle;
        public string pszText;
        public int ttiIcon;
    }
}