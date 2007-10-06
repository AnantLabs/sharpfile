using System;
using System.Runtime.InteropServices;

namespace SharpFile.UI {
	internal class Win32 {
		[DllImport("shell32.dll")]
		public static extern int SHBindToParent(IntPtr pidl,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid,
			ref IShellFolder ppv,
			ref IntPtr ppidlLast);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr ILCreateFromPath([MarshalAs(UnmanagedType.LPTStr)] string pszPath);

		[DllImport("user32.dll")]
		public static extern UInt32 TrackPopupMenu(IntPtr hMenu, UInt32 uFlags, int x, int y,
			int nReserved, IntPtr hWnd, IntPtr prcRect);

		[DllImport("shell32.dll")]
		public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);

		public const int ERROR_SUCCESS = 0;

		public static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
		public static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");

		public class WM {
			public const int CONTEXTMENU = 123;
			public const int INITMENUPOPUP = 279;
			public const int DRAWITEM = 43;
			public const int MEASUREITEM = 44;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CMINVOKECOMMANDINFO {
			public int cbSize;
			public int fMask;
			public IntPtr hwnd;
			public IntPtr lpVerb;
			public IntPtr lpParameters;
			public IntPtr lpDirectory;
			public int nShow;
			public int dwHotKey;
			public IntPtr hIcon;
		}

		[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
		Guid("000214F4-0000-0000-c000-000000000046")]
		public interface IContextMenu2 {
			[PreserveSig()]
			int QueryContextMenu(IntPtr hMenu, UInt32 indexMenu, UInt32 idCmdFirst, UInt32 idCmdLast, UInt32 uFlags);
			[PreserveSig()]
			int InvokeCommand(ref CMINVOKECOMMANDINFO pici);
			void GetCommandString(UInt32 idCmd, UInt32 uFlags, ref int pwReserved, IntPtr commandstring, UInt32 cch);
			[PreserveSig()]
			int HandleMenuMsg(int uMsg, IntPtr wParam, IntPtr lParam);
		}

		[ComImport(), Guid("000214F2-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IEnumIDList {
			[PreserveSig()]
			int Next(UInt32 celt, ref IntPtr rgelt, object pceltFetched);
			void Skip(UInt32 celt);
			void Reset();
			void Clone(ref IEnumIDList ppenum);
		}

		[ComImport(), Guid("000214E6-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IShellFolder {
			[PreserveSig()]
			int ParseDisplayName(IntPtr hwnd, IntPtr pbc,
				[MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, ref UInt32 pchEaten,
				ref IntPtr ppidl, ref UInt32 pdwAttributes);

			[PreserveSig()]
			int EnumObjects(IntPtr hwnd, int grfFlags, ref IEnumIDList ppenumIDList);

			[PreserveSig()]
			int BindToObject(IntPtr pidl, IntPtr pbc, ref Guid riid, ref IShellFolder ppv);

			void BindToStorage(IntPtr pidl, IntPtr pbc, Guid riid, ref IntPtr ppv);

			[PreserveSig()]
			int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);

			void CreateViewObject(IntPtr hwndOwner, Guid riid, ref IntPtr ppv);

			[PreserveSig()]
			int GetAttributesOf([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] UInt32 cidl,
				[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] apidl,
				[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ref UInt32 rgfInOut);

			void GetUIObjectOf(IntPtr hwndOwner, UInt32 cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
				ref Guid riid, ref UInt32 rgfReserved, [MarshalAs(UnmanagedType.Interface)] ref object ppv);

			void GetDisplayNameOf(IntPtr pidl, UInt32 uFlags, ref IntPtr pName);

			void SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName,
				UInt32 uFlags, ref IntPtr ppidlOut);
		}
	}
}