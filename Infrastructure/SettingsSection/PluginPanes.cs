using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using Common.Logger;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PluginPanes {
        private List<IPluginPane> instances;
        private List<PluginPane> pluginPanes = new List<PluginPane>();
        private DockState visibleState = DockState.DockBottomAutoHide;
        private double dockPortion = 185;

        public static List<PluginPane> GenerateDefaultPluginPanels() {
            List<PluginPane> pluginPanes = new List<PluginPane>();

            pluginPanes.Add(new PluginPane("PreviewPane", "Preview",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.PreviewPane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.PreviewPane")));

            pluginPanes.Add(new PluginPane("CommandLinePane", "Cmd",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.CommandLinePane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.CommandLinePane")));

            return pluginPanes;
        }

        [XmlAttribute]
        public DockState VisibleState {
            get {
                if (visibleState == DockState.Unknown) {
                    visibleState = DockState.DockBottomAutoHide;
                }

                return visibleState;
            }
            set {
                visibleState = value;
            }
        }

        [XmlAttribute]
        public double DockPortion {
            get {
                return dockPortion;
            }
            set {
                dockPortion = value;
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