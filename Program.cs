using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using Common.Logger;

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
                Settings.Instance.Logger.Log(LogLevelType.Verbose, 
                    "ApplicationExit");

				Settings.Save();
			};

            //if (Settings.Instance.ParentType == ParentType.Mdi) {
            //    Settings.Instance.Logger.Log(LogLevelType.Verbose,
            //        "Start the Mdi program");

            //    Application.Run(new MdiParent());
            //} else 
            if (Settings.Instance.ParentType == ParentType.Dual) {
                Settings.Instance.Logger.Log(LogLevelType.Verbose,
                    "Start the Dual program");

				Application.Run(new DualParent());
			}
        }
    }
}