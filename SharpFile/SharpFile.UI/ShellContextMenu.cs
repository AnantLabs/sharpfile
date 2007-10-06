using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
			public internalShellContextMenu()
				: base() {
			}

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
	}
}