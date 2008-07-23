using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Common.Comparers {
    public class StringLogicalComparer : IComparer {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);

        /// <summary>
        /// Compares two objects using the StrCmpLogicalW method in the Win32 API.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>An int that determines which object should be sorted first.</returns>
        public int Compare(object x, object y) {
            if (x != null && y != null) {
                string stringX = (string)x;
                string stringY = (string)y;

                DateTime dateTimeX;
                DateTime dateTimeY;

                if (DateTime.TryParse(stringX, out dateTimeX) &&
                    DateTime.TryParse(stringY, out dateTimeY)) {
                    return DateTime.Compare(dateTimeX, dateTimeY);
                } else {
                    return StrCmpLogicalW(stringX, stringY);
                }
            } else {
                return 0;
            }
        }
    }
}