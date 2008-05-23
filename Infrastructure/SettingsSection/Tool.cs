using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    [Serializable]
    public sealed class Tool {
        public const string SeperatorName = "{Separator}";

        private string name;
        private string path;
        private string arguments;
        private List<Keys> keys;

        public Tool() {
        }

        public Tool(string name, string path, string arguments, params Keys[] keys) {
            this.name = name;
            this.path = path;
            this.arguments = arguments;
            this.keys = new List<Keys>(keys);
        }

        public void Execute() {
            try {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.ErrorDialog = true;
                processStartInfo.UseShellExecute = true;

                Templater templater = new Templater(Settings.Instance.DualParent);
                processStartInfo.FileName = templater.Generate(path);

                if (!string.IsNullOrEmpty(arguments)) {
                    processStartInfo.Arguments = templater.Generate(arguments);
                }

                Process.Start(processStartInfo);
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Process {0} can not be performed on {1}.",
                    name,
                    path);
            }
        }

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        [XmlAttribute("Path")]
        public string Path {
            get {
                return path;
            }
            set {
                path = value;
            }
        }

        [XmlAttribute("Arguments")]
        public string Arguments {
            get {
                return arguments;
            }
            set {
                arguments = value;
            }
        }

        /// <summary>
        /// Keys.
        /// </summary>
        [XmlArray("Keys")]
        [XmlArrayItem("Key")]
        public List<Keys> Keys {
            get {
                return keys;
            }
            set {
                keys = value;
            }
        }
    }
}