using System;
using System.Reflection;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.UI;

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

            string settingsVersion = string.Empty;
            string assemblyVersion = string.Empty;

            if (!Settings.CompareSettingsVersion(ref settingsVersion, ref assemblyVersion)) {
                string message = string.Format(@"
The current configuration file version is {0}. SharpFile's current version is {1}. 
Settings might have changed between the two versions, so some settings may be lost. 
Press OK to continue loading the program even though settings may be lost.
Press CANCEL to exit SharpFile.",
                    settingsVersion,
                    assemblyVersion);

                // Show the modal dialog asking about converting the settings.
                DialogResult dialogResult = MessageBox.Show(message, "Convert Settings?",
                    MessageBoxButtons.OKCancel);

                if (dialogResult == DialogResult.OK) {
                    Settings.Instance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                } else if (dialogResult == DialogResult.Cancel) {
                    Application.Exit();
                }

                // TODO: Write XSL to convert old version of settings XML to new version of settings XML.
                /*
                string message = string.Format(@"
The current configuration file version is {0}. SharpFile's current version is {1}. 
Settings might have changed between the two versions, so a conversion might be necessary. 
Press YES to convert the current settings to this version of SharpFile. 
Press NO to ignore any settings changes. Note that this might lose custom settings, or might change behavior.
Press CANCEL to exit SharpFile.",
                    settingsVersion, 
                    assemblyVersion);
                
                // Show the modal dialog asking about converting the settings.
                DialogResult dialogResult = MessageBox.Show(message, "Convert Settings?", 
                    MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes) {
                    // TODO: Convert settings file.
                    Settings.Instance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                } else if (dialogResult == DialogResult.No) {
                    // Start the program normally.
                } else if (dialogResult == DialogResult.Cancel) {
                    Application.Exit();
                }
                */
            }

            startProgram();
        }

        private static void startProgram() {
            if (Settings.Instance.ParentType == ParentType.Dual) {
                Settings.Instance.Logger.Log(LogLevelType.Verbose,
                    "Start the Dual program");

                Application.Run(new DualParent());
            }
        }
    }
}