using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Tool {
        private string name;
        private string path;
        private string arguments;

        public Tool() {
        }

        public Tool(string name, string path, string arguments) {
            this.name = name;
            this.path = path;
            this.arguments = arguments;
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
    }
}