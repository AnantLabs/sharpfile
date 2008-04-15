using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SharpFile.UI {
	/// <summary>
	/// Displays the shell context menu.
	/// </summary>
	/// <remarks>
	/// Andrew Vos - http://www.andrewvos.com/?p=190
	/// </remarks>
	public class ShellContextMenu {
		private internalShellContextMenu iscm;

		public ShellContextMenu() {
			this.iscm = new internalShellContextMenu();
		}

		public ContextMenuResult PopupMenu(Environment.SpecialFolder specialFolder, IntPtr parent) {
			return this.iscm.PopupMenu(specialFolder, parent);
		}

		public ContextMenuResult PopupMenu(string path, IntPtr parent) {
			return this.iscm.PopupMenu(path, parent);
		}

		public ContextMenuResult PopupMenu(List<string> paths, IntPtr parent) {
			return this.iscm.PopupMenu(paths, parent);
		}

		public enum ContextMenuResult {
			NoUserFeedback,
			Cut,
			Copy,
			Paste,
			CreateShortcut,
			Delete,
			Rename,
			Properties,
			ContextMenuError,
			SharingAndSecurity
		}

		private class internalShellContextMenu : NativeWindow {
			private Win32.IContextMenu2 contextMenu2;

			public ContextMenuResult PopupMenu(Environment.SpecialFolder specialFolder, IntPtr parent) {
				this.AssignHandle(parent);
				ContextMenuResult result = this.PopUpContextMenu(new IntPtr[] { this.GetPIDLFromFolderID(parent, specialFolder) }, parent);
				this.ReleaseHandle();
				return result;
			}

			public ContextMenuResult PopupMenu(string path, IntPtr parent) {
				this.AssignHandle(parent);
				ContextMenuResult result = this.PopUpContextMenu(new IntPtr[] { Win32.ILCreateFromPath(path) }, parent);
				this.ReleaseHandle();
				return result;
			}

			public ContextMenuResult PopupMenu(List<string> paths, IntPtr parent) {
				this.AssignHandle(parent);
				List<IntPtr> p = new List<IntPtr>();
				foreach (string path in paths) {
					p.Add(Win32.ILCreateFromPath(path));
				}

				ContextMenuResult result = PopUpContextMenu(p.ToArray(), parent);
				this.ReleaseHandle();
				return result;
			}

			private ContextMenuResult PopUpContextMenu(IntPtr pidl, IntPtr parent) {
				return this.PopUpContextMenu(new IntPtr[] { pidl }, parent);
			}

			private ContextMenuResult PopUpContextMenu(IntPtr[] pidls, IntPtr parent) {
				ContextMenuResult result = ContextMenuResult.NoUserFeedback;

				ContextMenu menu = new ContextMenu();
				List<IntPtr> pidlList = new List<IntPtr>();
				Win32.IShellFolder iShellFolder = null;
				try {
					foreach (IntPtr pidl in pidls) {
						IntPtr ptr = new IntPtr();
						if ((((pidl != IntPtr.Zero) && (Win32.SHBindToParent(pidl, Win32.IID_IShellFolder, ref iShellFolder, ref ptr) == 0)) && ((iShellFolder != null)))) {
							pidlList.Add(ptr);
						}
					}

					IntPtr[] apidl = pidlList.ToArray();
					UInt32 rgfReserved = 0;
					Guid riid = Win32.IID_IContextMenu;
					object obj2 = null;
					iShellFolder.GetUIObjectOf(IntPtr.Zero, System.Convert.ToUInt32(apidl.Length), apidl, ref riid, ref rgfReserved, ref obj2);

					if (((this.contextMenu2 != null))) {
						Marshal.ReleaseComObject(this.contextMenu2);
						this.contextMenu2 = null;
					}

					this.contextMenu2 = obj2 as Win32.IContextMenu2;
					if (((this.contextMenu2 != null))) {
						Point mousePosition = Control.MousePosition;

						UInt32 uFlags;
						if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) {
							uFlags = 256;
						} else {
							uFlags = 0;
						}

						this.contextMenu2.QueryContextMenu(menu.Handle, 0, 1, 65535, uFlags);
						UInt32 menuResult = Win32.TrackPopupMenu(menu.Handle, 256, mousePosition.X, mousePosition.Y, 0, parent, IntPtr.Zero);
						if ((menuResult != 0)) {
							Win32.CMINVOKECOMMANDINFO cmici = new Win32.CMINVOKECOMMANDINFO();
							cmici.cbSize = Marshal.SizeOf(cmici);
							cmici.fMask = 0;
							cmici.hwnd = this.Handle;
							cmici.lpVerb = (IntPtr)((menuResult - 1) & 65535);
							cmici.lpParameters = IntPtr.Zero;
							cmici.lpDirectory = IntPtr.Zero;
							cmici.nShow = 1;
							cmici.dwHotKey = 0;
							cmici.hIcon = IntPtr.Zero;
							this.contextMenu2.InvokeCommand(ref cmici);
						}

						switch (menuResult) {
							case 0:
								result = ContextMenuResult.NoUserFeedback;
								break;
							case 25:
								result = ContextMenuResult.Cut;
								break;
							case 26:
								result = ContextMenuResult.Copy;
								break;
							case 27:
								result = ContextMenuResult.Paste;
								break;
							case 17:
								result = ContextMenuResult.CreateShortcut;
								break;
							case 18:
								result = ContextMenuResult.Delete;
								break;
							case 19:
								result = ContextMenuResult.Rename;
								break;
							case 20:
								result = ContextMenuResult.Properties;
								break;
							case 94:
								result = ContextMenuResult.SharingAndSecurity;
								break;
							default:
								break;
						}
					}
				} catch (Exception exception) {
					result = ContextMenuResult.ContextMenuError;
				} finally {
					if (((iShellFolder != null))) {
						Marshal.ReleaseComObject(iShellFolder);
						iShellFolder = null;
					}
					if (((menu != null))) {
						menu.Dispose();
					}
				}

				return result;
			}

			protected override void WndProc(ref System.Windows.Forms.Message m) {
				switch (m.Msg) {
					case Win32.WM.INITMENUPOPUP:
					case Win32.WM.DRAWITEM:
					case Win32.WM.MEASUREITEM:
						if (((this.contextMenu2 != null))) {
							try {
								this.contextMenu2.HandleMenuMsg(m.Msg, m.WParam, m.LParam);
								return;
							} catch {
							}
						}

						break;
					case Win32.WM.CONTEXTMENU:
						return;
					default:
						base.WndProc(ref m);
						break;
				}
			}

			public IntPtr GetPIDLFromFolderID(IntPtr owner, Environment.SpecialFolder specialFolder) {
				IntPtr pidl = new IntPtr();

				if (Win32.SHGetSpecialFolderLocation(owner, (int)specialFolder, ref pidl) == Win32.ERROR_SUCCESS) {
					return pidl;
				}

				return new IntPtr();
			}
		}

		public class Win32 {
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
}