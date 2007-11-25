using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace SharpFile {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			
			Application.ApplicationExit += delegate(object sender, EventArgs e) {
				Settings.Save();
			};

			if (Settings.Instance.ParentType == ParentType.Mdi) {
				Application.Run(new MdiParent());
			} else if (Settings.Instance.ParentType == ParentType.Tab) {
				Application.Run(new TabParent());
			} else if (Settings.Instance.ParentType == ParentType.Dual) {
				Application.Run(new DualParent());
			}
        }
    }
}