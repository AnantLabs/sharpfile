using System;
using System.Runtime.InteropServices;
using SharpFile.Infrastructure;

/*
 * 
 * NOTE : These classes and logic will work only and only if the
 * following key in the registry is set
 * HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\EnableToolTips\
 * 
 * VSNET ver1 : Provide a manifest file in the executable output firectory;
 * VSNET ver2 : CAll Application.EnableVisualStyles(), before InitializeComponent()
*/

namespace SharpFile.UI {
    public enum TooltipIcon : int {
        None,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// This class creates a balloon tooltip in the form of a message.
    /// This becomes useful for showing important information 
    /// quickly to the user.
    /// http://www.codeproject.com/KB/shell/balloontipsarticle.aspx - ramshri
    /// </summary>
    public class EditBalloon {
        private IntPtr m_parentHandle;
        private string m_text = string.Empty;
        private string m_title = string.Empty;
        private TooltipIcon m_titleIcon = TooltipIcon.None;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct EDITBALLOONTIP {
            public int cbStruct;
            public string pszTitle;
            public string pszText;
            public int ttiIcon;
        }

        private EditBalloon() {
        }

        public EditBalloon(IntPtr parentHandle) {
            m_parentHandle = parentHandle;
        }

        /// <summary>
        /// Show a balloon tooltip for edit control.
        /// </summary>
        public void Show() {
            EDITBALLOONTIP ebt = new EDITBALLOONTIP();

            ebt.cbStruct = Marshal.SizeOf(ebt);
            ebt.pszText = m_text;
            ebt.pszTitle = m_title;
            ebt.ttiIcon = (int)m_titleIcon;

            IntPtr ptrStruct = Marshal.AllocHGlobal(Marshal.SizeOf(ebt));
            Marshal.StructureToPtr(ebt, ptrStruct, false);

            System.Diagnostics.Debug.Assert(m_parentHandle != null, "Parent control is null", "Set parent before calling Show");

            Shell32.SendMessage(m_parentHandle,
                Shell32.EM_SHOWBALLOONTIP,
                0, ptrStruct);

            Marshal.FreeHGlobal(ptrStruct);
        }

        /// <summary>
        /// Sets or gets the Title.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }
            set {
                m_title = value;
            }
        }

        /// <summary>
        /// Sets or gets the display icon.
        /// </summary>
        public TooltipIcon TitleIcon {
            get {
                return m_titleIcon;
            }
            set {
                m_titleIcon = value;
            }
        }

        /// <summary>
        /// Sets or gets the display text.
        /// </summary>
        public string Text {
            get {
                return m_text;
            }
            set {
                m_text = value;
            }
        }

        public IntPtr ParentHandle {
            get {
                return m_parentHandle;
            }
            set {
                m_parentHandle = value;
            }
        }
    }
}