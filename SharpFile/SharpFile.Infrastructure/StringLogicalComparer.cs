using System.Collections;
using System.Runtime.InteropServices;

namespace SharpFile.Infrastructure {
    public class StringLogicalComparer : IComparer {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y) {
            return StrCmpLogicalW((string)x, (string)y);
        }
    }
}