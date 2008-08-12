using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PluginPane {
        private string name;
        private FullyQualifiedType type;
        private FullyQualifiedType settingsType;
        private double autoHidePortion = 150;
        private string tabText;
        private bool isHidden = false;
        private bool isActivated = true;

        public PluginPane() {
        }

        public PluginPane(string name, string tabText, FullyQualifiedType type, FullyQualifiedType settingsType) {
            this.name = name;
			this.tabText = tabText;
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

        [XmlAttribute]
        public double AutoHidePortion {
            get {
                return autoHidePortion;
            }
            set {
                autoHidePortion = value;
            }
        }

        [XmlAttribute]
        public string TabText {
            get {
                if (string.IsNullOrEmpty(tabText)) {
                    tabText = name;
                }

                return tabText;
            }
            set {
                tabText = value;
            }
        }

        [XmlAttribute]
        public bool IsHidden {
            get {
                return isHidden;
            }
            set {
                isHidden = value;
            }
        }

        [XmlAttribute]
        public bool IsActivated {
            get {
                return isActivated;
            }
            set {
                isActivated = value;
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