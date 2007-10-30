using System;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using System.Xml.Serialization;
using System.IO;

namespace SharpFile
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			string settingsFilePath = "settings.config";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			Settings settings;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

			if (!File.Exists(settingsFilePath)) {
				settings = new Settings();
				settings.Height = 500;
				settings.Width = 500;

				using (TextWriter tw = new StreamWriter(settingsFilePath)) {
					xmlSerializer.Serialize(tw, settings);
				}
			} else {
				using (TextReader tr = new StreamReader(settingsFilePath)) {					
					settings = (Settings)xmlSerializer.Deserialize(tr);
				}
			}

			Application.ApplicationExit += delegate(object sender, EventArgs e) {
				using (TextWriter tw = new StreamWriter(settingsFilePath)) {
					xmlSerializer.Serialize(tw, settings);
				}
			};

			if (settings.ParentType == ParentType.Mdi) {
				Application.Run(new MdiParent(settings));
			} else if (settings.ParentType == ParentType.Tab) {
				Application.Run(new TabParent(settings));
			} else if (settings.ParentType == ParentType.Dual) {
				Application.Run(new DualParent(settings));
			}
        }
    }
}