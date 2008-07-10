using System.Collections.Generic;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PluginPanes {
        private List<PluginPane> pluginPanes = new List<PluginPane>();
        private DockState visibleState = DockState.DockBottomAutoHide;

        public static List<PluginPane> GenerateDefaultPluginPanels() {
            List<PluginPane> pluginPanes = new List<PluginPane>();

            pluginPanes.Add(new PluginPane("PreviewPane",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.PreviewPane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.PreviewPane")));

            pluginPanes.Add(new PluginPane("CommandLinePane",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.CommandLinePane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.CommandLinePane")));

            return pluginPanes;
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

        [XmlArray("Panes")]
        [XmlArrayItem("Pane")]
        public List<PluginPane> Panes {
            get {
                return pluginPanes;
            }
            set {
                pluginPanes = value;
            }
        }
    }
}