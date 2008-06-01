using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Common.Logger;

namespace SharpFile.Infrastructure {
    public enum HookType {
        WH_KEYBOARD = 2,
        WH_KEYBOARD_LL = 13
    }

    /// <summary>
    /// Creates a global/application-specific keyboard hook.
    /// <remarks>
    /// George Mamaladze: http://www.codeproject.com/KB/cs/globalhook.aspx 
    /// Stephen Toub: http://blogs.msdn.com/toub/archive/2006/05/03/589423.aspx
    /// Eitan Pogrebizsky: http://www.koders.com/csharp/fid6AF179F76C05AF581905FF86477E98F2D4D86622.aspx
    /// Michael Schierl: http://mwinapi.sourceforge.net/
    /// </remarks>
    /// </summary>
    public class KeyboardHook : IDisposable {
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        private static int hKeyboardHook = 0; //Declare keyboard hook handle as int.

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const uint MASK_KEYDOWN = 0x40000000; // for bit 30
        private const uint MASK_KEYUP = 0x80000000; // for bit 31
        private const int HC_ACTION = 0;  //#define HC_ACTION           0
        private const int HC_NOREMOVE = 3; //#define HC_NOREMOVE         3

        private readonly HookProc keyboardHook; //Declare KeyboardHookProcedure as HookProc type.
        private HookType hookType = HookType.WH_KEYBOARD;

        public KeyboardHook(HookType hookType) {
            this.hookType = hookType;
            keyboardHook = new HookProc(keyboardHookProc);
            Start();
        }

        ~KeyboardHook() {
            Stop();
        }

        /// <summary>
        /// Unhooks the hook if necessary.
        /// </summary>
        public void Dispose() {
            Stop();
        }

        public void Start() {
            // install Keyboard hook
            if (hKeyboardHook == 0) {
                IntPtr keyboardHookPointer = Marshal.GetFunctionPointerForDelegate(keyboardHook);

                if (hookType == HookType.WH_KEYBOARD) {
                    hKeyboardHook = SetWindowsHookEx((int)hookType,
                        keyboardHookPointer,
                        IntPtr.Zero,
                        (uint)GetCurrentThreadId());
                } else if (hookType == HookType.WH_KEYBOARD_LL) {
                    hKeyboardHook = SetWindowsHookEx((int)hookType,
                        keyboardHookPointer,
                        Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                        0);
                } else {
                    throw new Exception("Unknown hook type.");
                }

                //If SetWindowsHookEx fails.
                if (hKeyboardHook == 0) {
                    Win32Exception ex = new Win32Exception(Marshal.GetLastWin32Error());
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Installing keyboard hook failed.");
                    Stop();

                    throw ex;

                }
            }
        }

        public void Stop() {
            bool retKeyboard = true;

            if (hKeyboardHook != 0) {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            //If UnhookWindowsHookEx fails.
            if (!(retKeyboard)) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, "UnhookWindowsHookEx failed.");
                throw new Exception("UnhookWindowsHookEx failed.");
            }
        }

        //Declare wrapper managed KeyboardHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct {
            public int vkCode; //Specifies a virtual-key code. The code must be a value in the range 1 to 254.
            public int scanCode; // Specifies a hardware scan code for the key.
            public int flags; // Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            public int time; // Specifies the time stamp for this message.
            public int dwExtraInfo; // Specifies extra information associated with the message.
        }

        //Import for SetWindowsHookEx function.
        //Use this function to install a hook.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, IntPtr callback, IntPtr hInstance, uint threadId);

        //Import for UnhookWindowsHookEx.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //Import for CallNextHookEx.
        //Use this function to pass the hook information to next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        public static extern int GetCurrentThreadId();

        private int keyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
            if ((nCode >= 0) && (nCode != HC_NOREMOVE) && (KeyDown != null || KeyUp != null || KeyPress != null)) {
                if (HookType.WH_KEYBOARD == hookType) {
                    uint flags = (uint)lParam; //Marshal.ReadInt32(lParam);
                    //bit 30 Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
                    bool WasKeyDown = (flags & MASK_KEYDOWN) > 0;
                    //bit 31 Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
                    bool IsKeyReleased = (flags & MASK_KEYUP) > 0;

                    Keys keyData = (Keys)wParam;

                    if (Keys.Control != keyData && Keys.Alt != keyData && Keys.Shift != keyData) {
                        // Add in modifier key.
                        keyData |= Control.ModifierKeys;
                    }

                    KeyEventArgs e = new KeyEventArgs(keyData);

                    if (KeyDown != null && !WasKeyDown && !IsKeyReleased) {
                        KeyDown(this, e);
                    }

                    if (KeyUp != null && WasKeyDown && IsKeyReleased) {
                        KeyUp(this, e);
                    }
                } else if (HookType.WH_KEYBOARD_LL == hookType) {
                    KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(
                        lParam, typeof(KeyboardHookStruct));

                    Keys keyData = (Keys)keyboardHookStruct.vkCode;

                    /*
                    if (Keys.Control != keyData && Keys.Alt != keyData && Keys.Shift != keyData) {
                        // Add in modifier key.
                        keyData |= Control.ModifierKeys;
                    }
                    */

                    KeyEventArgs e = new KeyEventArgs(keyData);

                    // Raise KeyDown.
                    if (KeyDown != null && (wParam.ToInt32() == WM_KEYDOWN || wParam.ToInt32() == WM_SYSKEYDOWN)) {
                        KeyDown(this, e);
                    }

                    // Raise KeyUp.
                    if (KeyUp != null && (wParam.ToInt32() == WM_KEYUP || wParam.ToInt32() == WM_SYSKEYUP)) {
                        KeyUp(this, e);
                    }
                }
            }

            return CallNextHookEx(hKeyboardHook, nCode, wParam, (IntPtr)lParam);
        }
    }
}