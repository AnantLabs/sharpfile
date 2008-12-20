using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Win32 {
    public class User32 {
		[DllImport("user32.dll")]
		public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);
		public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("user32", EntryPoint = "GetWindowLongA")]
		public static extern int GetWindowLongPtr(IntPtr hwnd, int nIndex);

		[DllImport("user32")]
		public static extern int GetParent(IntPtr hwnd);

		[DllImport("user32")]
		public static extern IntPtr GetWindow(IntPtr hwnd, int wCmd);

		[DllImport("user32")]
		public static extern bool IsWindowVisible(IntPtr hwnd);

		[DllImport("user32")]
		public static extern int GetDesktopWindow();

		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

		/// <summary>
		/// Gets the text from a window's handle.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="text"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        /// <summary>
        /// Sends the specified message to a window or windows.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="len"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, IntPtr order);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "DestroyIcon", ExactSpelling = true, CharSet = CharSet.Ansi,
            SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);
    }
}