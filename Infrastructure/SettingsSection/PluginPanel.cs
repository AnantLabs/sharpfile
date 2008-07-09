using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PluginPanel {
        private string name;
        private FullyQualifiedType type;
        private FullyQualifiedType settingsType;
        private DockState visibleState = DockState.DockBottomAutoHide;
        private double autoHidePortion = 150;

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

        [XmlAttribute]
        public DockState VisibleState {
            get {
                return visibleState;
            }
            set {
                visibleState = value;
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