using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
        private Nodes keyCodes;
        private bool directoriesSortedFirst = true;
        private bool showParentDirectory = true;
        private bool showRootDirectory = true;        

        // Constructed from settings.
        private LoggerService loggerService;
        private List<IParentResourceRetriever> parentResourceRetrievers;

        // Sub-settings.
        private DualParent dualParentSettings;
        private Icons iconSettings;
        private PreviewPanel previewPanelSettings;
        private Logger loggerSettings;
        private List<Tool> toolSettings;
        private List<SettingsSection.View> viewSettings;
        private List<ParentResourceRetriever> parentResourceRetrieverSettings;
        private List<SettingsSection.ChildResourceRetriever> childResourceRetrieverSettings;

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
            previewPanelSettings = new PreviewPanel();
            loggerSettings = new Logger();
            toolSettings = new List<Tool>();
            viewSettings = new List<SettingsSection.View>();
            parentResourceRetrieverSettings = new List<ParentResourceRetriever>();
            childResourceRetrieverSettings = new List<SettingsSection.ChildResourceRetriever>();

            lockObject = new object();
            this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
        }
        #endregion

        #region Static methods.
        /// <summary>
        /// Deep copies the data from an object to a new object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">Object to copy.</param>
        /// <returns>A new version of the object with all of it's values intact.</returns>
        // TODO: Move this to the Common assembly.
        // TODO: Provide a way to deepcopy a whole object including delegates that are attached to the original.
        public static T DeepCopy<T>(T obj) {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return (T)binaryFormatter.Deserialize(memoryStream);
        }

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
                LoggerService loggerService = new LoggerService("log.txt", LogLevelType.Verbose);

                // Null out the reource retrievers in case settings are being reloaded.
                instance.parentResourceRetrievers = null;

                try {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    bool settingsLoaded = false;
                    FileInfo fileInfo = new FileInfo(FilePath);

                    // If there is no settings file, create one from some defaults.
                    if (fileInfo.Exists && fileInfo.Length > 0) {
                        try {
                            // Set our instance properties from the Xml file.
                            deserializeSettings(xmlSerializer);

                            settingsLoaded = true;
                        } catch (Exception ex) {
                            loggerService.Log(LogLevelType.Verbose, ex,
                                "There was an error generating the settings; defaults will be loaded instead.");

                            settingsLoaded = false;

                            // TODO: Show a message saying that default values will 
                            // be loaded because there was an error.
                        }
                    } else {
                        // TODO: Show a message saying that default values will 
                        // be loaded because the file is missing or empty.
                    }

                    // Load up some defaults, since it doesn't look like any settings were found.
                    if (!settingsLoaded) {
                        // Retrieve default xml from the resource embedded in this assembly.
                        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                        xmlWriterSettings.Encoding = Encoding.UTF8;
                        xmlWriterSettings.Indent = true;

                        string settingsXml = Resource.settings;

                        try {
                            // Check to see if the SharpFile assembly has has the 
                            // other assemblies ILMerged into it.
                            Assembly assembly = Assembly.Load("SharpFile");

                            List<Type> types = new List<Type>();
                            types.AddRange(assembly.GetTypes());

                            if (types.Find(delegate(Type t) {
                                if (t != null && t.Namespace != null) {
                                    return t.Namespace.Equals("Common");
                                }

                                return false;
                            }) != null) {
                                settingsXml = Resource.ilmerge_settings;
                            }
                        } catch (Exception ex) {
                            loggerService.Log(LogLevelType.ErrorsOnly, ex, "Generating the correct setting.config file failed.");
                        }

                        using (XmlWriter xmlWriter = XmlWriter.Create(FilePath, xmlWriterSettings)) {
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(settingsXml);
                            xml.WriteTo(xmlWriter);
                        }

                        deserializeSettings(xmlSerializer);
                    }
                } catch (Exception ex) {
                    loggerService.Log(LogLevelType.Verbose, ex,
                        "There was an error generating the settings.");
                }
            }
        }

        /// <summary>
        /// Persists the setttings to the config file.
        /// </summary>
        public static void Save() {
            lock (lockObject) {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

                using (TextWriter tw = new StreamWriter(FilePath)) {
                    xmlSerializer.Serialize(tw, Instance);
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

                foreach (PropertyInfo propertyInfo in settings.GetType().GetProperties()) {
                    // Only set properties which have a setter.
                    if (propertyInfo.CanWrite) {
                        PropertyCaller<Settings, object>.GenGetter getter = PropertyCaller<Settings, object>.CreateGetMethod(propertyInfo);
                        PropertyCaller<Settings, object>.GenSetter setter = PropertyCaller<Settings, object>.CreateSetMethod(propertyInfo);
                        setter(instance, getter(settings));

                        //instance.GetType().GetProperty(propertyInfo.Name).SetValue(
                        //    instance,
                        //    propertyInfo.GetValue(settings, null),
                        //    null);
                    }
                }
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
        /// <summary>
        /// Parent type.
        /// </summary>
        public ParentType ParentType {
            get {
                return parentType;
            }
            set {
                parentType = value;
            }
        }

        /// <summary>
        /// Key codes.
        /// </summary>
        [XmlIgnore]
        public Nodes KeyCodes {
            get {
                return keyCodes;
            }
            set {
                keyCodes = value;
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
        [XmlArray("ParentResourceRetrievers")]
        [XmlArrayItem("ParentResourceRetriever")]
        public List<ParentResourceRetriever> ParentResourceRetrieverSettings {
            get {
                return parentResourceRetrieverSettings;
            }
            set {
                parentResourceRetrieverSettings = value;

                if (parentResourceRetrieverSettings.Count == 0) {
                    FullyQualifiedType retrieverType = new FullyQualifiedType("SharpFile", "SharpFile.IO.Retrievers.DriveRetriever");
                    ParentResourceRetriever retriever = new ParentResourceRetriever(
                        "DriveRetriever", retrieverType, "CompressedRetriever", "DefaultRetriever");
                    parentResourceRetrieverSettings.Add(retriever);
                }
            }
        }

        /// <summary>
        /// Child resource retrievers infos.
        /// </summary>
        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<SettingsSection.ChildResourceRetriever> ChildResourceRetrieverSettings {
            get {
                return childResourceRetrieverSettings;
            }
            set {
                childResourceRetrieverSettings = value;
            }
        }

        /// <summary>
        /// View infos.
        /// </summary>
        [XmlArray("Views")]
        [XmlArrayItem("View")]
        public List<SettingsSection.View> ViewSettings {
            get {
                return viewSettings;
            }
            set {
                viewSettings = value;

                if (viewSettings.Count == 0) {
                    FullyQualifiedType viewType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListView");
                    FullyQualifiedType comparerType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListViewItemComparer");
                    viewSettings.Add(new SettingsSection.View("ListView", viewType, comparerType));
                }
            }
        }

        /// <summary>
        /// Tool infos.
        /// </summary>
        [XmlArray("Tools")]
        [XmlArrayItem("Tool")]
        public List<Tool> ToolSettings {
            get {
                return toolSettings;
            }
            set {
                toolSettings = value;

                if (toolSettings.Count == 0) {
                    toolSettings.Add(new Tool("Command Prompt", "cmd", "/K cd {SelectedPath}"));
                }
            }
        }

        #region Properties for sub-settings.
        /// <summary>
        /// Dual parent settings.
        /// </summary>
        [XmlElement("DualParent")]
        public DualParent DualParent {
            get {
                return dualParentSettings;
            }
            set {
                dualParentSettings = value;                
            }
        }

        /// <summary>
        /// Icon settings.
        /// </summary>
        [XmlElement("Icons")]
        public Icons IconSettings {
            get {
                if (iconSettings.RetrieveIconExtensions.Count == 0) {
                    iconSettings.RetrieveIconExtensions.AddRange(new string[] { ".exe", ".lnk", ".ps", ".scr", ".ico", ".icn" });
                }

                if (iconSettings.IntensiveSearchDriveTypeEnums.Count == 0) {
                    FullyQualifiedType driveType = new FullyQualifiedType("System", "System.IO.DriveType");
                    FullyQualifiedEnum driveTypeEnum = new FullyQualifiedEnum("Fixed", driveType);
                    iconSettings.IntensiveSearchDriveTypeEnums.Add(driveTypeEnum);
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
        public PreviewPanel PreviewPanel {
            get {
                return previewPanelSettings;
            }
            set {
                previewPanelSettings = value;

                if (previewPanelSettings.DetailTextExtensions.Count == 0) {
                    previewPanelSettings.DetailTextExtensions.AddRange(new string[] { 
                        string.Empty, ".txt", ".config", ".xml", ".ini", ".cs", ".log" 
                    });
                }
            }
        }
        #endregion

        #region Properties not derived from settings.config.
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
                    parentResourceRetrievers = new List<IParentResourceRetriever>(parentResourceRetrieverSettings.Count);

                    foreach (ParentResourceRetriever parentResourceRetrieverSetting in parentResourceRetrieverSettings) {
                        try {
                            IParentResourceRetriever parentResourceRetriever = Reflection.InstantiateObject<IParentResourceRetriever>(
                                parentResourceRetrieverSetting.FullyQualifiedType.Assembly,
                                parentResourceRetrieverSetting.FullyQualifiedType.Type);

                            foreach (string childResourceRetrieverName in parentResourceRetrieverSetting.ChildResourceRetrievers) {
                                SettingsSection.ChildResourceRetriever childResourceRetrieverInfo = childResourceRetrieverSettings.Find(delegate(SettingsSection.ChildResourceRetriever c) {
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

                                        SettingsSection.View viewSetting = viewSettings.Find(delegate(SettingsSection.View v) {
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
