using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SharpFile {
	public class TabControl : System.Windows.Forms.TabControl {
		private bool isVisible = true;

		public TabControl() {
			this.Scroller.ScrollLeft += new EventHandler(Scroller_ScrollLeft);
			this.Scroller.ScrollRight += new EventHandler(Scroller_ScrollRight);
			this.Scroller.TabClose += new EventHandler(Scroller_TabClose);
			this.Scroller.TabOpen += new EventHandler(Scroller_TabOpen);
		}

		public override Rectangle DisplayRectangle {
			get {
				if (!isVisible) {
					return new Rectangle(0, 0, Width, Height);
				} else {
					return base.DisplayRectangle;
				}
			}
		}

		public new TabPage SelectedTab {
			get {
				return ((TabPage)base.SelectedTab);
			} set {
				base.SelectedTab = value;
			}
		}

		//TabControl overrides dispose to clean up the component list.
		[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
		protected override void Dispose(bool disposing) {
			if (disposing && 
				UpDown != null) {
					UpDown.ReleaseHandle();
			}
			
			base.Dispose(disposing);
		}

		#region  PInvoke Declarations
		[DllImport("User32", CallingConvention = CallingConvention.Cdecl)]
		private static extern int RealGetWindowClass(IntPtr hwnd, System.Text.StringBuilder pszType, int cchType);

		[DllImport("user32")]
		private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		private const int WM_Create = 0x1;
		private const int WM_PARENTNOTIFY = 0x210;
		private const int WM_HSCROLL = 0x114;
		#endregion

		private NativeUpDown UpDown = null;
		private TabScroller Scroller = new TabScroller();

		private int ScrollPosition {
			get {
				int multiplier = -1;
				Rectangle tabRect;

				do {
					tabRect = GetTabRect(multiplier + 1);
					multiplier++;
				}
				while (tabRect.Left < 0 && multiplier < this.TabCount);

				return multiplier;
			}
		}

		[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if (m.Msg == WM_PARENTNOTIFY) {
				if ((ushort)(m.WParam.ToInt32() & 0xFFFF) == WM_Create) {
					System.Text.StringBuilder WindowName = new System.Text.StringBuilder(16);
					RealGetWindowClass(m.LParam, WindowName, 16);

					if (WindowName.ToString() == "msctls_updown32") {
						//unhook the existing updown control as it will be recreated if 
						//the tabcontrol is recreated (alignment, visible changed etc..)
						if (UpDown != null) {
							UpDown.ReleaseHandle();
						}

						//and hook it.
						UpDown = new NativeUpDown();
						UpDown.AssignHandle(m.LParam);
					}
				}
			}

			base.WndProc(ref m);
		}

		protected override void OnHandleCreated(System.EventArgs e) {
			base.OnHandleCreated(e);

			if (this.Multiline == false) {
				Scroller.Font = new Font("Marlett", this.Font.Size, FontStyle.Regular, GraphicsUnit.Pixel, this.Font.GdiCharSet);
				SetParent(Scroller.Handle, this.Handle);
			}

			this.OnFontChanged(EventArgs.Empty);
		}

		protected override void OnFontChanged(System.EventArgs e) {
			base.OnFontChanged(e);
			this.Scroller.Font = new Font("Marlett", this.Font.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
			this.Scroller.Height = this.ItemSize.Height;
			this.Scroller.Width = this.ItemSize.Height * 3;
			this.OnResize(EventArgs.Empty);
		}

		protected override void OnResize(System.EventArgs e) {
			UpdateScroller();
			base.OnResize(e);

			
		}

		private void Scroller_ScrollLeft(Object sender, System.EventArgs e) {
			if (this.TabCount == 0) {
				return;
			}

			int scrollPos = Math.Max(0, (ScrollPosition - 1) * 0x10000);
			SendMessage(this.Handle, WM_HSCROLL, (IntPtr)(scrollPos | 0x4), IntPtr.Zero);
			SendMessage(this.Handle, WM_HSCROLL, (IntPtr)(scrollPos | 0x8), IntPtr.Zero);
		}

		private void Scroller_ScrollRight(Object sender, System.EventArgs e) {
			if (this.TabCount == 0) {
				return;
			}

			if (GetTabRect(this.TabCount - 1).Right <= this.Scroller.Left) {
				return;
			}

			int scrollPos = Math.Max(0, (ScrollPosition + 1) * 0x10000);
			SendMessage(this.Handle, WM_HSCROLL, (IntPtr)(scrollPos | 0x4), IntPtr.Zero);
			SendMessage(this.Handle, WM_HSCROLL, (IntPtr)(scrollPos | 0x8), IntPtr.Zero);
		}

		private void Scroller_TabClose(Object sender, System.EventArgs e) {
			if (this.SelectedTab != null) {
				this.TabPages.Remove(this.SelectedTab);
				UpdateScroller();
			}
		}

		void Scroller_TabOpen(object sender, EventArgs e) {
			((Child)this.Parent).AddTab();			
			UpdateScroller();
		}

		private void UpdateScroller() {
			if (TabCount == 1) {
				Scroller.LeftScroller.Visible = false;
				Scroller.RightScroller.Visible = false;
				Scroller.CloseButton.Visible = false;

				Scroller.Location = new Point(this.Width - Scroller.TotalButtonWidth, 2);
			} else {
				Scroller.LeftScroller.Visible = true;
				Scroller.RightScroller.Visible = true;
				Scroller.CloseButton.Visible = true;

				if (this.Multiline) {
					return;
				}

				if (this.Alignment == TabAlignment.Top) {
					Scroller.Location = new Point(this.Width - Scroller.Width, 2);
				} else {
					Scroller.Location = new Point(this.Width - Scroller.Width, this.Height - Scroller.Height - 2);
				}
			}
		}

		public bool IsVisible {
			get {
				return isVisible;
			}
			set {
				if (isVisible != value) {
					isVisible = value;
					this.UpdateStyles();
				}
			}
		}
	}

	#region   Custom Scroller with Close Button
	internal class TabScroller : System.Windows.Forms.Control {
		#region   Windows Form Designer generated code
		public TabScroller()
			: base() {
			//This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		//TabScroller overrides dispose to clean up the component list.
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components = null;

		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		public System.Windows.Forms.Button LeftScroller;
		public System.Windows.Forms.Button RightScroller;
		public System.Windows.Forms.Button CloseButton;
		public System.Windows.Forms.Button OpenButton;

		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent() {
			this.LeftScroller = new System.Windows.Forms.Button();
			this.RightScroller = new System.Windows.Forms.Button();
			this.CloseButton = new System.Windows.Forms.Button();
			this.OpenButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			//LeftScroller
			//
			this.LeftScroller.Dock = System.Windows.Forms.DockStyle.Right;
			this.LeftScroller.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LeftScroller.Location = new System.Drawing.Point(0, 0);
			this.LeftScroller.Name = "LeftScroller";
			this.LeftScroller.Size = new System.Drawing.Size(40, 40);
			this.LeftScroller.TabIndex = 0;
			this.LeftScroller.Text = "3";
			this.LeftScroller.Click += new EventHandler(LeftScroller_Click);
			//
			//RightScroller
			//
			this.RightScroller.Dock = System.Windows.Forms.DockStyle.Right;
			this.RightScroller.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RightScroller.Location = new System.Drawing.Point(40, 0);
			this.RightScroller.Name = "RightScroller";
			this.RightScroller.Size = new System.Drawing.Size(40, 40);
			this.RightScroller.TabIndex = 1;
			this.RightScroller.Text = "4";
			this.RightScroller.Click += new EventHandler(RightScroller_Click);
			//
			//CloseButton
			//
			this.CloseButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CloseButton.Location = new System.Drawing.Point(80, 0);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(40, 40);
			this.CloseButton.TabIndex = 2;
			this.CloseButton.Text = "r";
			this.CloseButton.Click += new EventHandler(CloseButton_Click);
			//
			//OpenButton
			//
			this.OpenButton.Dock = System.Windows.Forms.DockStyle.Left;
			this.OpenButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OpenButton.Location = new System.Drawing.Point(120, 0);
			this.OpenButton.Name = "OpenButton";
			this.OpenButton.Size = new System.Drawing.Size(40, 40);
			this.OpenButton.TabIndex = 2;
			this.OpenButton.Text = "2";
			this.OpenButton.Click += new EventHandler(OpenButton_Click);
			//
			//TabScroller
			//
			this.Controls.Add(this.OpenButton);
			this.Controls.Add(this.LeftScroller);
			this.Controls.Add(this.RightScroller);
			this.Controls.Add(this.CloseButton);
			this.Font = new System.Drawing.Font("Marlett", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)2);
			this.Name = "TabScroller";
			this.Size = new System.Drawing.Size(160, 40);
			this.Resize += new EventHandler(TabScroller_Resize);
			this.ResumeLayout(false);
		}
		#endregion

		public event EventHandler TabOpen;
		public event EventHandler TabClose;
		public event EventHandler ScrollLeft;
		public event EventHandler ScrollRight;

		private void TabScroller_Resize(Object sender, System.EventArgs e) {
			LeftScroller.Width = this.Width / 4;
			RightScroller.Width = this.Width / 4;
			CloseButton.Width = this.Width / 4;
			OpenButton.Width = this.Width / 4;
		}

		private void LeftScroller_Click(Object sender, System.EventArgs e) {
			if (ScrollLeft != null) {
				ScrollLeft(this, EventArgs.Empty);
			}
		}

		private void RightScroller_Click(Object sender, System.EventArgs e) {
			if (ScrollRight != null) {
				ScrollRight(this, EventArgs.Empty);
			}
		}

		private void CloseButton_Click(Object sender, System.EventArgs e) {
			if (TabClose != null) {
				TabClose(this, EventArgs.Empty);
			}
		}

		private void OpenButton_Click(Object sender, System.EventArgs e) {
			if (TabOpen != null) {
				TabOpen(this, EventArgs.Empty);
			}
		}

		public int TotalButtonWidth {
			get {
				return (this.OpenButton.Visible ? this.OpenButton.Width : 0) + 
					(this.CloseButton.Visible ? this.CloseButton.Width : 0) +
					(this.RightScroller.Visible ? this.RightScroller.Width : 0) +
					(this.LeftScroller.Visible ? this.LeftScroller.Width : 0);
			}
		}
	}
	#endregion

	#region  UpDown Control Subclasser
	internal class NativeUpDown : NativeWindow {
		public NativeUpDown() : base() { }

		private const int WM_DESTROY = 0x2;
		private const int WM_NCDESTROY = 0x82;
		private const int WM_WINDOWPOSCHANGING = 0x46;

		[StructLayout(LayoutKind.Sequential)]
		private struct WINDOWPOS {
			public IntPtr hwnd, hwndInsertAfter;
			public int x, y, cx, cy, flags;
		}

		[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if (m.Msg == WM_DESTROY || m.Msg == WM_NCDESTROY)
				this.ReleaseHandle();
			else if (m.Msg == WM_WINDOWPOSCHANGING) {
				//Move the updown control off the edge so it's not visible
				WINDOWPOS wp = (WINDOWPOS)(m.GetLParam(typeof(WINDOWPOS)));
				wp.x += wp.cx;
				Marshal.StructureToPtr(wp, m.LParam, true);
				_bounds = new Rectangle(wp.x, wp.y, wp.cx, wp.cy);
			}
			base.WndProc(ref m);
		}

		private Rectangle _bounds;
		internal Rectangle Bounds {
			get { return _bounds; }
		}
	}
	#endregion
}