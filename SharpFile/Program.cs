using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			
			Application.ApplicationExit += delegate {
                Settings.Instance.Logger.Log("ApplicationExit", Common.LogLevelType.Verbose);
				Settings.Save();
			};

			if (Settings.Instance.ParentType == ParentType.Mdi) {
                Settings.Instance.Logger.Log("Start the Mdi program", Common.LogLevelType.Verbose);
				Application.Run(new MdiParent());
			} else if (Settings.Instance.ParentType == ParentType.Dual) {
                Settings.Instance.Logger.Log("Start the Dual program", Common.LogLevelType.Verbose);
				Application.Run(new DualParent());
			}
        }
    }
}