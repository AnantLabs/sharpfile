using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Common.Logger;
using SharpFile.Infrastructure.SettingsSection;

namespace SharpFile.Infrastructure {
    /// <summary>
    /// Settings singleton.
    /// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html.
    /// </summary>
    [Serializable]
    public sealed class Settings {
        // Constants.
        public const string FilePath = "settings.config";

        // Statics.
        private static readonly Settings instance = new Settings();
        private static object lockObject = new object();

        // Privates.
        private ImageList imageList = new ImageList();
        private ParentType parentType = ParentType.Dual;
        private int width = 500;
        private int height = 500;
        private bool minimizeToSystray = false;
        private bool directoriesSortedFirst = true;
        private bool showParentDirectory = true;
        private bool showRootDirectory = true;
        private FormWindowState windowState = FormWindowState.Normal;
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private Point location = new Point(0, 0);

        // Sub-settings.
        private DualParent dualParentSettings;
        private Icons iconSettings;
        private PreviewPane previewPanelSettings;
        private Logger loggerSettings;
        private Retrievers retrieverSettings;
        private FontInfo fontInfo;
        private ViewInfo viewInfo;
        private PluginPanes pluginPaneSettings;

        // Constructed from settings.
        private LoggerService loggerService;
        private List<IParentResourceRetriever> parentResourceRetrievers;
        private Font font;
        private System.Windows.Forms.View view;
        

        #region Ctors.
        /// <summary>
        /// Explicit static ctor to load settings and to 
        /// tell C# compiler not to mark type as beforefieldinit.
        /// </summary>
        static Settings() {
            Load();
        }

        /// <summary>
        /// Internal instance ctor.
        /// </summary>
        private Settings() {
            dualParentSettings = new DualParent();
            iconSettings = new Icons();
            previewPanelSettings = new PreviewPane();
            loggerSettings = new Logger();
            retrieverSettings = new Retrievers();
            fontInfo = new FontInfo();
            viewInfo = new ViewInfo();
            //pluginPanelSettings = new List<PluginPane>();
            pluginPaneSettings = new PluginPanes();

            lockObject = new object();
            this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
        }
        #endregion

        #region Static methods.
        /// <summary>
        /// Clears the current resources.
        /// </summary>
        public static void ClearResources() {
            instance.parentResourceRetrievers = null;
        }

        /// <summary>
        /// Loads the settings config file into the singleton instance.
        /// </summary>
        public static void Load() {
            lock (lockObject) {
                // Null out the reource retrievers in case settings are being reloaded.
                instance.parentResourceRetrievers = null;

                try {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    FileInfo fileInfo = new FileInfo(FilePath);

                    // If there is no settings file, create one from some defaults.
                    if (fileInfo.Exists && fileInfo.Length > 0) {
                        // Set our instance properties from the Xml file.
                        deserializeSettings(xmlSerializer);
                    }
                } catch (Exception ex) {
                    LoggerService loggerService = new LoggerService("log.txt", LogLevelType.Verbose);
                    loggerService.Log(LogLevelType.Verbose, ex,
                        "There was an error generating the settings.");

                    throw;
                }
            }
        }

        /// <summary>
        /// Checks to see whether the current assembly version matches the version specified in the settings.
        /// </summary>
        /// <param name="settingsVersion">Version specified in the settings.</param>
        /// <param name="assemblyVersion">Assembly-specified version.</param>
        /// <returns>Whether the two versions are the same.</returns>
        public static bool CompareSettingsVersion(ref string settingsVersion, ref string assemblyVersion) {
            FileInfo fileInfo = new FileInfo(FilePath);

            // If there is no settings file, create one from some defaults.
            if (fileInfo.Exists && fileInfo.Length > 0) {
                try {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(fileInfo.FullName);

                    assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    settingsVersion = xml.SelectSingleNode("/Settings/Version").InnerText;

                    if (assemblyVersion != settingsVersion) {
                        return false;
                    }
                } catch {
                    // Ignore any xml exceptions here. They will be caught when the file attempts to deserialize.
                }
            }

            return true;
        }

        /// <summary>
        /// Persists the setttings to the config file.
        /// </summary>
        public static void Save() {
            lock (lockObject) {
                try {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

                    using (TextWriter tw = new StreamWriter(FilePath)) {
                        xmlSerializer.Serialize(tw, Instance);
                    }
                } catch (Exception ex) {
                    instance.loggerService.Log(LogLevelType.ErrorsOnly, ex, "Error when saving settings.");
                }
            }
        }

        /// <summary>
        /// Deserializes the settings config file to an object and sets the singleton's properties appropriately.
        /// </summary>
        /// <param name="xmlSerializer">XmlSerializer to deserialize.</param>
        private static void deserializeSettings(XmlSerializer xmlSerializer) {
            using (TextReader tr = new StreamReader(FilePath)) {
                Settings settings = (Settings)xmlSerializer.Deserialize(tr);
                Reflection.DuplicateObject<Settings>(settings, instance);
            }
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Singleton instance of the settings.
        /// </summary>
        public static Settings Instance {
            get {
                return instance;
            }
        }
        #endregion

        #region Instance properties
        public string Version {
            get {
                return version;
            }
            set {
                version = value;
            }
        }

        /// <summary>
        /// Parent type.
        /// </summary>
        [XmlIgnore]
        public ParentType ParentType {
            get {
                return parentType;
            }
            set {
                parentType = value;
            }
        }

        /// <summary>
        /// Width of the form.
        /// </summary>
        public int Width {
            get {
                return width;
            }
            set {
                width = value;
            }
        }

        /// <summary>
        /// Height of the form.
        /// </summary>
        public int Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }

        /// <summary>
        /// Whether or not to minimize the application to the systray.
        /// </summary>
        public bool MinimizeToSystray {
            get {
                return minimizeToSystray;
            }
            set {
                minimizeToSystray = value;
            }
        }

        /// <summary>
        /// Whether or not the directories are sorted first.
        /// </summary>
        public bool DirectoriesSortedFirst {
            get {
                return directoriesSortedFirst;
            }
            set {
                directoriesSortedFirst = value;
            }
        }

        /// <summary>
        /// Whether or not to show the parent directory.
        /// </summary>
        public bool ShowParentDirectory {
            get {
                return showParentDirectory;
            }
            set {
                showParentDirectory = value;
            }
        }

        /// <summary>
        /// Whether or not to show the root directory.
        /// </summary>
        public bool ShowRootDirectory {
            get {
                return showRootDirectory;
            }
            set {
                showRootDirectory = value;
            }
        }

        /// <summary>
        /// The form window state.
        /// </summary>
        public FormWindowState WindowState {
            get {
                return windowState;
            }
            set {
                windowState = value;
            }
        }

        /// <summary>
        /// The location of the form.
        /// </summary>
        public Point Location {
            get {
                return location;
            }
            set {
                location = value;
            }
        }

        /// <summary>
        /// View information to derive a System.Windows.Forms.View object.
        /// </summary>
        public ViewInfo ViewInfo {
            get {
                return viewInfo;
            }
            set {
                viewInfo = value;
            }
        }

        /// <summary>
        /// Font information to derive a System.Drawing.Font object.
        /// </summary>
        public FontInfo FontInfo {
            get {
                return fontInfo;
            }
            set {
                fontInfo = value;
            }
        }

        /// <summary>
        /// Logger info.
        /// </summary>
        [XmlElement("Logger")]
        public Logger LoggerSettings {
            get {
                return loggerSettings;
            }
            set {
                loggerSettings = value;
            }
        }

        /// <summary>
        /// Resource retriever infos.
        /// </summary>
        [XmlElement("Retrievers")]
        public Retrievers RetrieverSettings {
            get {
                if (retrieverSettings.ParentResourceRetrievers.Count == 0) {
                    retrieverSettings.ParentResourceRetrievers = Retrievers.GenerateDefaultParentResourceRetrievers();
                }

                if (retrieverSettings.ChildResourceRetrievers.Count == 0) {
                    retrieverSettings.ChildResourceRetrievers = Retrievers.GenerateDefaultChildResourceRetrievers();
                }

                if (retrieverSettings.Views.Count == 0) {
                    retrieverSettings.Views = Retrievers.GenerateDefaultViews();
                }

                return retrieverSettings;
            }
            set {
                retrieverSettings = value;
            }
        }

        #region Properties for sub-settings.
        /// <summary>
        /// Dual parent settings.
        /// </summary>
        [XmlElement("DualParent")]
        public DualParent DualParent {
            get {
                if (dualParentSettings.Tools.Count == 0) {
                    dualParentSettings.Tools = DualParent.GenerateDefaultTools();
                }

                //if (dualParentSettings.PluginPanels.Count == 0) {
                //    dualParentSettings.PluginPanels = DualParent.GenerateDefaultPluginPanels();
                //}

                return dualParentSettings;
            }
            set {
                dualParentSettings = value;
            }
        }

        /// <summary>
        /// Plugin panes settings.
        /// </summary>
        public PluginPanes PluginPanes {
            get {
                if (pluginPaneSettings.Panes.Count == 0) {
                    pluginPaneSettings.Panes = PluginPanes.GenerateDefaultPluginPanels();
                }

                return pluginPaneSettings;
            }
            set {
                pluginPaneSettings = value;
            }
        }

        /// <summary>
        /// Icon settings.
        /// </summary>
        [XmlElement("Icons")]
        public Icons IconSettings {
            get {
                if (iconSettings.RetrieveIconExtensions.Count == 0) {
                    iconSettings.RetrieveIconExtensions = Icons.GenerateDefaultRetrieveIconExtensions();
                }

                if (iconSettings.IntensiveSearchDriveTypeEnums.Count == 0) {
                    iconSettings.IntensiveSearchDriveTypeEnums = Icons.GenerateDefaultIntensiveSearchDriveTypeEnums();
                }

                return iconSettings;
            }
            set {
                iconSettings = value;
            }
        }

        /// <summary>
        /// Icon settings.
        /// </summary>
        [XmlElement("PreviewPanel")]
        public PreviewPane PreviewPanel {
            get {
                if (previewPanelSettings.DetailTextExtensions.Count == 0) {
                    previewPanelSettings.DetailTextExtensions = 
                        PreviewPane.GenerateDefaultDetailTextExtensions();
                }

                return previewPanelSettings;
            }
            set {
                previewPanelSettings = value;

                
            }
        }
        #endregion

        #region Properties not derived from settings.config.
        [XmlIgnore]
        public Font Font {
            get {
                if (font == null) {
                    font = new Font(fontInfo.FamilyName, fontInfo.Size, fontInfo.Style);
                }

                return font;
            }
        }

        [XmlIgnore]
        public System.Windows.Forms.View View {
            get {
                if (!view.ToString().Equals(viewInfo.Type)) {
                    if (Enum.IsDefined(typeof(System.Windows.Forms.View), viewInfo.Type)) {
                        view = (System.Windows.Forms.View)Enum.Parse(typeof(System.Windows.Forms.View), viewInfo.Type);
                    } else {
                        Logger.Log(LogLevelType.ErrorsOnly, "{0} is not a valid view type. Setting view type to default of Details.",
                            viewInfo.Type);
                        view = System.Windows.Forms.View.Details;
                        viewInfo.Type = view.ToString();
                    }
                }

                return view;
            }
        }

        /// <summary>
        /// Derived Logger service.
        /// </summary>
        [XmlIgnore]
        public LoggerService Logger {
            get {
                if (loggerService == null) {
                    loggerService = new LoggerService(loggerSettings.File, loggerSettings.LogLevel);
                }

                return loggerService;
            }
        }

        /// <summary>
        /// Derived parent resources.
        /// </summary>
        [XmlIgnore]
        public List<IParentResource> ParentResources {
            get {
                List<IParentResource> resources = new List<IParentResource>();

                foreach (IParentResourceRetriever retriever in ParentResourceRetrievers) {
                    resources.AddRange(retriever.Get());
                }

                return resources;
            }
        }

        /// <summary>
        /// Derived parent resource retrievers.
        /// </summary>
        [XmlIgnore]
        public List<IParentResourceRetriever> ParentResourceRetrievers {
            get {
                if (parentResourceRetrievers == null || parentResourceRetrievers.Count == 0) {
                    parentResourceRetrievers = new List<IParentResourceRetriever>(retrieverSettings.ParentResourceRetrievers.Count);

                    foreach (ParentResourceRetriever parentResourceRetrieverSetting in retrieverSettings.ParentResourceRetrievers) {
                        try {
                            IParentResourceRetriever parentResourceRetriever = Reflection.InstantiateObject<IParentResourceRetriever>(
                                parentResourceRetrieverSetting.FullyQualifiedType.Assembly,
                                parentResourceRetrieverSetting.FullyQualifiedType.Type);

                            foreach (string childResourceRetrieverName in parentResourceRetrieverSetting.ChildResourceRetrievers) {
                                SettingsSection.ChildResourceRetriever childResourceRetrieverInfo = retrieverSettings.ChildResourceRetrievers.Find(delegate(SettingsSection.ChildResourceRetriever c) {
                                    return c.Name == childResourceRetrieverName;
                                });

                                if (childResourceRetrieverInfo != null) {
                                    try {
                                        IChildResourceRetriever childResourceRetriever = Reflection.InstantiateObject<IChildResourceRetriever>(
                                            childResourceRetrieverInfo.FullyQualifiedType.Assembly,
                                            childResourceRetrieverInfo.FullyQualifiedType.Type);

                                        childResourceRetriever.Name = childResourceRetrieverName;
                                        childResourceRetriever.ColumnInfos = childResourceRetrieverInfo.ColumnInfos;

                                        if (childResourceRetrieverInfo.FilterMethod is ChildResourceRetriever.FilterMethodWithArgumentsDelegate) {
                                            childResourceRetriever.FilterMethodWithArguments += (ChildResourceRetriever.FilterMethodWithArgumentsDelegate)childResourceRetrieverInfo.FilterMethod;
                                            childResourceRetriever.FilterMethodArguments = childResourceRetrieverInfo.FilterMethodArguments;
                                        } else if (childResourceRetrieverInfo.FilterMethod is ChildResourceRetriever.FilterMethodDelegate) {
                                            childResourceRetriever.FilterMethod += (ChildResourceRetriever.FilterMethodDelegate)childResourceRetrieverInfo.FilterMethod;
                                        }

                                        SettingsSection.View viewSetting = retrieverSettings.Views.Find(delegate(SettingsSection.View v) {
                                            return v.Name == childResourceRetrieverInfo.View;
                                        });

                                        if (viewSetting != null) {
                                            try {
                                                IView view = Reflection.InstantiateObject<IView>(
                                                    viewSetting.FullyQualifiedType.Assembly,
                                                    viewSetting.FullyQualifiedType.Type);

                                                if (viewSetting.Comparer != null) {
                                                    view.Comparer = viewSetting.Comparer;
                                                }

                                                childResourceRetriever.View = view;
                                            } catch (TypeLoadException ex) {
                                                Logger.Log(LogLevelType.ErrorsOnly,
                                                    "{0} is not derived from IView.",
                                                    viewSetting.Name);
                                            }
                                        } else {
                                            Logger.Log(LogLevelType.ErrorsOnly,
                                                "{0} could not be found.",
                                                childResourceRetrieverInfo.View);
                                        }

                                        if (parentResourceRetriever.ChildResourceRetrievers == null) {
                                            parentResourceRetriever.ChildResourceRetrievers = new ChildResourceRetrievers();
                                        }

                                        parentResourceRetriever.ChildResourceRetrievers.Add(childResourceRetriever);
                                    } catch (MissingMethodException ex) {
                                        Logger.Log(LogLevelType.ErrorsOnly, ex,
                                            "ChildResourceRetriever, {0}, could not be instantiated.",
                                            childResourceRetrieverName);
                                    } catch (TypeLoadException ex) {
                                        Logger.Log(LogLevelType.ErrorsOnly, ex,
                                            "ChildResourceRetriever, {0}, could not be instantiated.",
                                            childResourceRetrieverName);
                                    }
                                } else {
                                    Logger.Log(LogLevelType.ErrorsOnly,
                                        "ChildResourceRetriever, {0}, could not be found.",
                                        childResourceRetrieverName);
                                }
                            }

                            parentResourceRetrievers.Add(parentResourceRetriever);
                        } catch (MissingMethodException ex) {
                            Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "ResourceRetriever, {0}, could not be instantiated (is it an abstract class?).",
                                parentResourceRetrieverSetting.Name);
                        } catch (TypeLoadException ex) {
                            Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "ResourceRetriever, {0}, could not be instantiated.",
                                parentResourceRetrieverSetting.Name);
                        }
                    }

                    Save();
                }

                return parentResourceRetrievers;
            }
        }

        /// <summary>
        /// Shared Imagelist.
        /// </summary>
        [XmlIgnore]
        public ImageList ImageList {
            get {
                return imageList;
            }
        }
        #endregion
        #endregion
    }
}