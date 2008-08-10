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
        private bool isVisible = true;

        public static List<PluginPane> GenerateDefaultPluginPanels() {
            List<PluginPane> pluginPanes = new List<PluginPane>();

            pluginPanes.Add(new PluginPane("Previewer", "Previewer",
                new FullyQualifiedType("PreviewPane", "SharpFile.UI.PreviewPane"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.SettingsSection.PreviewPane")));

            pluginPanes.Add(new PluginPane("Command Line", "Cmd",
                new FullyQualifiedType("CommandLinePane", "SharpFile.UI.CommandLinePane"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.SettingsSection.CommandLinePane")));

			/*
            pluginPanes.Add(new PluginPane("Screen", "Screen",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.ScreenPane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.ScreenPane")));

            pluginPanes.Add(new PluginPane("Renamer", "Renamer",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.RegexRenamerPane"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.RegexRenamerPane")));

             pluginPanes.Add(new PluginPane("SharpFileEditor", "Editor",
                new FullyQualifiedType("SharpFileEditor", "SharpFileEditor.SharpFileEditor"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.SharpFileEditor")));
             */

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

        [XmlAttribute]
        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
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
                    instances = new List<IPluginPane>();
                    PluginRetriever pluginRetriever = new PluginRetriever();
                    List<Assembly> assemblies = pluginRetriever.GetPluginAssemblies();                    

                    foreach (PluginPane pluginPaneSetting in Panes) {
                        // TODO: This should be a private method instead of in here.
                        Assembly assembly = assemblies.Find(delegate(Assembly a) {
                            return a.ManifestModule.Name.Replace(".dll", string.Empty).Equals(pluginPaneSetting.Type.Assembly);
                        });

                        if (assembly != null) {
                            foreach (Type type in assembly.GetTypes()) {
                                if (type.IsPublic && !type.IsAbstract) {
                                    Type interfaceType = null;

                                    try {
                                        interfaceType = type.GetInterface(typeof(IPluginPane).FullName);
                                    } catch (ReflectionTypeLoadException ex) {
                                        // TODO: Determine version of SharpFile.Infrastructure this plugin was built against.
                                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                            @"Plugin pane, {0}, could not be instantiated. Plugin could be built aginst the incorrect version of 
                                            SharpFile.Infrastructure. Current version of SharpFile.Infrastructure is {1}.",
                                            pluginPaneSetting.Name,
                                            "0");

                                        foreach (Exception exception in ex.LoaderExceptions) {
                                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, exception,
                                                "Plugin pane, {0}, loader exception.",
                                                pluginPaneSetting.Name);
                                        }
                                    } catch (Exception ex) {
                                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                "Retreiving the IPluginPane interface for {0} produced an error.",
                                                pluginPaneSetting.Name);
                                    }

                                    if (interfaceType != null) {
                                        try {
                                            IPluginPane pluginPane = Reflection.InstantiateObject<IPluginPane>(type);
                                            pluginPane.Name = pluginPaneSetting.Name;
                                            pluginPane.TabText = pluginPaneSetting.TabText;
                                            pluginPane.AutoHidePortion = pluginPaneSetting.AutoHidePortion;

                                            if (!pluginPaneSetting.IsVisible) {
                                                pluginPane.VisibleState = DockState.Hidden;
                                            }

                                            instances.Add(pluginPane);
                                        } catch (TypeLoadException ex) {
                                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                "Plugin pane, {0}, could not be instantiated because the type is incorrect.",
                                                pluginPaneSetting.Name);
                                        } catch (FileNotFoundException ex) {
                                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                "Plugin pane, {0}, could not be instantiated because the file could not be found.",
                                                pluginPaneSetting.Name);
                                        } catch (Exception ex) {
                                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                                "Plugin pane, {0}, could not be instantiated.",
                                                pluginPaneSetting.Name);
                                        }
                                    }
                                }
                            }
                        } else {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly,
                                "Plugin pane, {0}, assembly could not be found. Assembly: {1}; Type: {2}",
                                pluginPaneSetting.Name,
                                pluginPaneSetting.Type.Assembly,
                                pluginPaneSetting.Type.Type);
                        }
                    }
                }

                return instances;
            }
        }
    }
}