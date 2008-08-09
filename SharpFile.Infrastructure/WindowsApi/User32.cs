using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.WindowsApi {
    public class User32 {
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

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

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