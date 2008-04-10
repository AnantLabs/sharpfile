using System;
using SharpFile.Infrastructure;

namespace SharpFile {
    public class ListViewItem : System.Windows.Forms.ListViewItem {
        public void BeginEdit(IntPtr handle) {
            base.BeginEdit();

            // TODO: Validate the text changes with a BalloonHelp like Explorer does.            
            IntPtr editWnd = IntPtr.Zero;
            editWnd = Shell32.SendMessage(handle,
                                  Shell32.LVM_GETEDITCONTROL, 0, IntPtr.Zero);

            // Only select the name in the textbox, not the extension.
            int selectedLength = this.Text.Length;
            if (this.Text.LastIndexOf('.') > -1) {
                selectedLength = this.Text.LastIndexOf('.');
            }

            Shell32.SendMessage(editWnd, Shell32.EM_SETSEL, 0, selectedLength);
        }
    }
}