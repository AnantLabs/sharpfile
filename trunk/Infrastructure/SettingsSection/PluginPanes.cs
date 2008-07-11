using System.Collections.Generic;
using System.Xml.Serialization;
using WeifenLuo.WinFormsUI.Docking;
using Common;
using System;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PluginPanes {
        private List<IPluginPane> instances;
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

        [XmlIgnore]
        public List<IPluginPane> Instances {
            get {
                if (instances == null || instances.Count == 0) {
                    instances = new List<IPluginPane>();

                    foreach (PluginPane pluginPaneSetting in Panes) {
                        try {
                            IPluginPane pluginPane = Reflection.InstantiateObject<IPluginPane>(
                                pluginPaneSetting.Type.Assembly,
                                pluginPaneSetting.Type.Type);

                            pluginPane.Name = pluginPaneSetting.Name;
                            pluginPane.TabText = pluginPaneSetting.TabText;
                            pluginPane.AutoHidePortion = pluginPaneSetting.AutoHidePortion;

                            instances.Add(pluginPane);
                        } catch (TypeLoadException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "PluginPane, {0}, could not be instantiated.",
                                pluginPaneSetting.Name);
                        }
                    }
                }

                return instances;
            }
        }
    }
}