using System;
using System.Windows.Forms;
using Common.Logger;
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
                Settings.Instance.Logger.Log(LogLevelType.Verbose, 
                    "ApplicationExit");

				Settings.Save();
			};

            if (Settings.Instance.ParentType == ParentType.Dual) {
                Settings.Instance.Logger.Log(LogLevelType.Verbose,
                    "Start the Dual program");

				Application.Run(new DualParent());
			}
        }
    }
}