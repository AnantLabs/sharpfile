using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
                if (instances == null) {
                    PluginRetriever pluginRetriever = new PluginRetriever();
                    List<Assembly> assemblies = pluginRetriever.GetPluginAssemblies();
                    instances = new List<IPluginPane>();

                    foreach (PluginPane pluginPaneSetting in Panes) {
                        // TODO: This should be a private method instead of in here.
                        Assembly assembly = assemblies.Find(delegate(Assembly a) {
                            return a.ManifestModule.Name.Replace(".dll", string.Empty).Equals(pluginPaneSetting.Type.Assembly);
                        });

                        if (assembly != null) {
                            try {
                                foreach (Type type in assembly.GetTypes()) {
                                    if (type.IsPublic && !type.IsAbstract) {
                                        Type interfaceType = type.GetInterface(typeof(IPluginPane).FullName);

                                        if (interfaceType != null) {
                                            try {
                                                IPluginPane pluginPane = Reflection.InstantiateObject<IPluginPane>(type);

                                                //IPluginPane pluginPane = Reflection.InstantiateObject<IPluginPane>(
                                                //    pluginPaneSetting.Type.Assembly,
                                                //    pluginPaneSetting.Type.Type);

                                                pluginPane.Name = pluginPaneSetting.Name;
                                                pluginPane.TabText = pluginPaneSetting.TabText;
                                                pluginPane.AutoHidePortion = pluginPaneSetting.AutoHidePortion;

                                                instances.Add(pluginPane);
                                            } catch (TypeLoadException ex) {
                                                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                    "Plugin pane, {0}, could not be instantiated.",
                                                    pluginPaneSetting.Name);
                                            } catch (FileNotFoundException ex) {
                                                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                    "Plugin pane, {0}, could not be instantiated.",
                                                    pluginPaneSetting.Name);
                                            }
                                        }
                                    }
                                }
                            } catch (ReflectionTypeLoadException ex) {
                                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                    "Plugin pane, {0}, could not be instantiated. Plugin could be built aginst the incorrect version of SharpFile.Infrastructure.",
                                    pluginPaneSetting.Name);

                                foreach (Exception exception in ex.LoaderExceptions) {
                                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, exception,
                                        "Plugin pane, {0}, loader exception.",
                                        pluginPaneSetting.Name);
                                }
                            }
                        }
                    }
                }

                return instances;
            }
        }
    }
}