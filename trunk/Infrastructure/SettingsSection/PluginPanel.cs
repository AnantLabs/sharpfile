using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public class PluginPanel {
        string name;
        FullyQualifiedType type;
        FullyQualifiedType settingsType;

        public PluginPanel() {
        }

        public PluginPanel(string name, FullyQualifiedType type, FullyQualifiedType settingsType) {
            this.name = name;
            this.type = type;
            this.settingsType = settingsType;
        }

        [XmlAttribute]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public FullyQualifiedType Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }

        public FullyQualifiedType SettingsType {
            get {
                return settingsType;
            }
            set {
                settingsType = value;
            }
        }
    }
}