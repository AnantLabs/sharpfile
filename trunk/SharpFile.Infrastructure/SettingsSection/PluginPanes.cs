using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
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
        private bool isHidden = false;

        public static List<PluginPane> GenerateDefaultPluginPanes() {
            List<PluginPane> pluginPanes = new List<PluginPane>();

            pluginPanes.Add(new PluginPane("Previewer", "Previewer",
                new FullyQualifiedType("Previewer", "SharpFile.UI.Previewer"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.SettingsSection.Preview")));

            pluginPanes.Add(new PluginPane("Command Line", "Cmd",
                new FullyQualifiedType("CommandLine", "SharpFile.UI.CommandLine"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.SettingsSection.CommandLine")));

            pluginPanes.Add(new PluginPane("Screen", "Screen",
                new FullyQualifiedType("Screen", "SharpFile.UI.Screen"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.Screen")));

            pluginPanes.Add(new PluginPane("Renamer", "Renamer",
                new FullyQualifiedType("RegexRenamer", "SharpFile.UI.RegexRenamer"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.RegexRenamer")));

            /*
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
        public bool IsHidden {
            get {
                return isHidden;
            }
            set {
                isHidden = value;
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

        /// <summary>
        /// The loaded instances of the plugin panes.
        /// </summary>
        [XmlIgnore]
        public List<IPluginPane> Instances {
            get {
                if (instances == null) {
                    instances = new List<IPluginPane>();
                    PluginRetriever pluginRetriever = new PluginRetriever();
                    List<Assembly> assemblies = pluginRetriever.GetPluginAssemblies();                    

                    foreach (PluginPane pluginPaneSetting in Panes) {
                        Assembly assembly = assemblies.Find(delegate(Assembly a) {
                            return a.ManifestModule.Name.Replace(".dll", string.Empty).Equals(
                                pluginPaneSetting.Type.Assembly);
                        });

                        if (assembly != null) {
                            instantiatePluginPane(assembly, pluginPaneSetting);
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

        /// <summary>
        /// Instantiates the plugin pane.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="pluginPaneSetting">Plugin pane settings.</param>
        private void instantiatePluginPane(Assembly assembly, PluginPane pluginPaneSetting) {
            foreach (Type type in assembly.GetTypes()) {
                if (type.IsPublic && !type.IsAbstract) {
                    Type interfaceType = getInterfaceType<IPluginPane>(assembly, pluginPaneSetting, type);

                    if (interfaceType != null) {
                        try {
                            IPluginPane pluginPane = Reflection.InstantiateObject<IPluginPane>(type);
                            pluginPane.Name = pluginPaneSetting.Name;
                            pluginPane.TabText = pluginPaneSetting.TabText;
                            pluginPane.AutoHidePortion = pluginPaneSetting.AutoHidePortion;
                            pluginPane.DockHandler.IsHidden = pluginPaneSetting.IsHidden;
                            pluginPane.IsActivated = pluginPaneSetting.IsActivated;

                            if (pluginPaneSetting.SettingsType != null) {
                                IPluginPaneSettings settings = instantiatePluginPaneSetting(assembly, pluginPaneSetting);

                                if (settings != null) {
                                    pluginPane.Settings = settings;
                                }
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
        }

        /// <summary>
        /// Instantiates the plugin pane settings.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="pluginPaneSetting">Plugin pane settings.</param>
        /// <returns>Plugin pane settings.</returns>
        private IPluginPaneSettings instantiatePluginPaneSetting(Assembly assembly, PluginPane pluginPaneSetting) {
            foreach (Type settingsType in assembly.GetTypes()) {
                if (settingsType.IsPublic && !settingsType.IsAbstract) {
                    Type settingsInterfaceType = getInterfaceType<IPluginPaneSettings>(assembly,
                        pluginPaneSetting, settingsType);

                    if (settingsInterfaceType != null) {
                        try {
                            IPluginPaneSettings settings = Reflection.InstantiateObject<IPluginPaneSettings>(settingsType);

                            foreach (XmlElement element in Settings.Instance.UnknownConfigurationXmlElements) {
                                string serializedSettingsName = settingsType.Name;

                                if (element.Name.Equals(serializedSettingsName)) {
                                    XmlSerializer xmlSerializer = new XmlSerializer(settingsType);
                                    settings = (IPluginPaneSettings)xmlSerializer.Deserialize(new XmlNodeReader(element));
                                    break;
                                }
                            }

                            settings.GenerateDefaultSettings();
                            return settings;
                        } catch (TypeLoadException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Plugin pane settings, {0}, could not be instantiated because the type is incorrect.",
                                pluginPaneSetting.Name);
                        } catch (FileNotFoundException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Plugin pane settings, {0}, could not be instantiated because the file could not be found.",
                                pluginPaneSetting.Name);
                        } catch (Exception ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Plugin pane settings, {0}, could not be instantiated.",
                                pluginPaneSetting.Name);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the correct interface from the setting.
        /// </summary>
        /// <typeparam name="T">Type of the interface.</typeparam>
        /// <param name="assembly">Assembly.</param>
        /// <param name="pluginPaneSetting">Plugin pane setting.</param>
        /// <param name="type">Type.</param>
        /// <returns>The interface type.</returns>
        private Type getInterfaceType<T>(Assembly assembly, PluginPane pluginPaneSetting, Type type) {
            Type interfaceType = null;

            try {
                interfaceType = type.GetInterface(typeof(T).FullName);
            } catch (ReflectionTypeLoadException ex) {
                logReflectionTypeLoadException(assembly, pluginPaneSetting, ex);
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                        "Retreiving the {0} interface for {1} produced an error.",
                        typeof(T).Name,
                        pluginPaneSetting.Name);
            }

            return interfaceType;
        }

        /// <summary>
        /// Logs the reflection type load exception.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="pluginPaneSetting">Plugin pane settings.</param>
        /// <param name="ex">ReflectionTypeLoad exception.</param>
        private void logReflectionTypeLoadException(Assembly assembly, PluginPane pluginPaneSetting, ReflectionTypeLoadException ex) {
            string versionBuiltAgainst = string.Empty;

            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies()) {
                if (assemblyName.Equals("SharpFile.Infrastructure")) {
                    versionBuiltAgainst = assemblyName.Version.ToString();
                    break;
                }
            }

            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                @"Plugin pane, {0}, could not be instantiated. Plugin could be built aginst the incorrect version of 
                                            SharpFile.Infrastructure. Current version of SharpFile.Infrastructure is {1}. Version of SharpFile.Infrastructure 
                                            against is {2}.",
                pluginPaneSetting.Name,
                Assembly.GetAssembly(typeof(IPluginPane)).GetName().Version.ToString(),
                versionBuiltAgainst);

            foreach (Exception exception in ex.LoaderExceptions) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, exception,
                    "Plugin pane, {0}, loader exception.",
                    pluginPaneSetting.Name);
            }
        }
    }
}