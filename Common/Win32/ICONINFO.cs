using System;

namespace Common.Win32 {
	public struct ICONINFO {
		public bool fIcon;
		public int xHotspot;
		public int yHotspot;
		public IntPtr hbmMask;
		public IntPtr hbmColor;
	}
}