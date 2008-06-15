using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public class ViewInfo {
        private string type = "Details";

        public ViewInfo() {
        }

        [XmlAttribute]
        public string Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }
    }
}