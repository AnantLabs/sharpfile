using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    /// <summary>
    /// Specifies what "mode" the view should be.
    /// </summary>
	public enum ParentType {
        /// <summary>
        /// Every view is its own child window.
        /// </summary>
		Mdi,
        /// <summary>
        /// Every view is its own tab.
        /// </summary>
		Tab,
        /// <summary>
        /// Two views that fill up the parent window. The default.
        /// </summary>
		Dual
	}

    /// <summary>
    /// Object that stores the assembly and type name for a type. Cannot be a struct 
    /// because a parameter-less ctor is required for Xml serialization.
    /// </summary>
    public class FullyQualifiedType {
        private string assembly;
        private string type;

        public FullyQualifiedType() {
        }

        public FullyQualifiedType(string assembly, string type) {
            this.assembly = assembly;
            this.type = type;
        }

        [XmlAttribute("Assembly")]
        public string Assembly {
            get {
                return assembly;
            }
            set {
                assembly = value;
            }
        }

        [XmlAttribute("Type")]
        public string Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }
    }

    public class FullyQualifiedMethod {
        private FullyQualifiedType fullyQualifiedType;
        private string method;

        public FullyQualifiedMethod() {
        }

        public FullyQualifiedMethod(FullyQualifiedType fullyQualifiedType, string method) {
            this.fullyQualifiedType = fullyQualifiedType;
            this.method = method;
        }

        public FullyQualifiedType Type {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        [XmlAttribute("Method")]
        public string Method {
            get {
                return method;
            }
            set {
                method = value;
            }
        }
    }

    public class ResourceRetrieverInfo {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private string childResourceRetriever;

        public ResourceRetrieverInfo() {
        }

        public ResourceRetrieverInfo(string name, string childResourceRetriever, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.childResourceRetriever = childResourceRetriever;
        }

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        [XmlAttribute("ChildResourceRetriever")]
        public string ChildResourceRetriever {
            get {
                return childResourceRetriever;
            }
            set {
                childResourceRetriever = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }
    }

    public class ChildResourceRetrieverInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetrieverInfo() {
        }

        public ChildResourceRetrieverInfo(string name, List<ColumnInfo> columnInfos, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.columnInfos = columnInfos;
        }

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        [XmlArray("ColumnInfos")]
        [XmlArrayItem("ColumnInfo")]
        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }
    }

	/// <summary>
	/// Settings singleton.
	/// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html.
	/// </summary>
	[Serializable]
	public sealed class Settings {
		public const string FilePath = "settings.config";

		private static readonly Settings instance = new Settings();
		private static object lockObject = new object();

        private ParentType parentType = ParentType.Dual;
        private int width = 500;
        private int height = 500;
		private string leftPath;
		private string rightPath;
        private int splitterPercentage = 50;
		private Nodes keyCodes;
        private List<ResourceRetrieverInfo> resourceRetrieverInfos;
        private List<ChildResourceRetrieverInfo> childResourceRetrieverInfos;
        private bool directoriesSortedFirst = true;

        private List<IResourceRetriever> resourceRetrievers;
        private ImageList imageList = new ImageList();

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
			lockObject = new object();
			this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
        }
        #endregion

        #region Static methods.
        public static void Load() {
            lock (lockObject) {
                try {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    bool settingsLoaded = false;

                    // If there is no settings file, create one from some defaults.
                    if (File.Exists(FilePath)) {
                        try {
                            // Set our instance properties from the Xml file.
                            using (TextReader tr = new StreamReader(FilePath)) {
                                Settings settings = (Settings)xmlSerializer.Deserialize(tr);

                                foreach (PropertyInfo propertyInfo in settings.GetType().GetProperties()) {
                                    // Only set properties which have a setter.
                                    if (propertyInfo.CanWrite) {
                                        instance.GetType().GetProperty(propertyInfo.Name).SetValue(
                                            instance,
                                            propertyInfo.GetValue(settings, null),
                                            null);
                                    }
                                }
                            }

                            settingsLoaded = true;
                        } catch (Exception ex) {
                            string blob = ex.Message + ex.StackTrace;
                            settingsLoaded = false;

                            // TODO: Show a message saying that default values have been loaded.
                        }
                    }

                    // Set up some defaults, since it doesn't look like any settings were found.
                    if (!settingsLoaded) {
                        List<ResourceRetrieverInfo> resourceRetrieverInfos = new List<ResourceRetrieverInfo>();
                        resourceRetrieverInfos.Add(new ResourceRetrieverInfo("DriveRetriever",
                            "FileRetriever",
                            new FullyQualifiedType("SharpFile.IO", "SharpFile.IO.Retrievers.DriveRetriever")));
                        resourceRetrieverInfos.Add(new ResourceRetrieverInfo("NetworkDriveRetriever",
                            "FileRetriever",
                            new FullyQualifiedType("SharpFile.IO", "SharpFile.IO.Retrievers.NetworkDriveRetriever")));
                        instance.ResourceRetrieverInfos = resourceRetrieverInfos;

                        // TODO: Add FullyQualifiedMethod to the ColumnInfo ctor.
                        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                        columnInfos.Add(
                            new ColumnInfo("Filename", "DisplayName",
                                new StringLogicalComparer(), true));

                        columnInfos.Add(
                            new ColumnInfo("Size", "Size",
                                new ColumnInfo.CustomMethod(Common.General.GetHumanReadableSize),
                                new StringLogicalComparer()));

                        columnInfos.Add(
                            new ColumnInfo("Date", "LastWriteTime",
                                new ColumnInfo.CustomMethod(GetDateTimeShortDateString),
                                new StringLogicalComparer()));

                        columnInfos.Add(
                            new ColumnInfo("Time", "LastWriteTime",
                                new ColumnInfo.CustomMethod(GetDateTimeShortTimeString),
                                new StringLogicalComparer()));

                        List<ChildResourceRetrieverInfo> childResourceRetrieverInfos = new List<ChildResourceRetrieverInfo>();
                        childResourceRetrieverInfos.Add(new ChildResourceRetrieverInfo("FileRetriever",
                            columnInfos,
                            new FullyQualifiedType("SharpFile.IO", "SharpFile.IO.Retrievers.FileRetriever")));

                        instance.ChildResourceRetrieverInfos = childResourceRetrieverInfos;

                        Nodes defaultKeys = new Nodes();
                        defaultKeys.Add("Rename", Keys.F2);
                        instance.KeyCodes = defaultKeys;

                        using (TextWriter tw = new StreamWriter(FilePath)) {
                            xmlSerializer.Serialize(tw, instance);
                        }
                    }
                } catch (Exception ex) {
                    Exception insideException = getInnerException(ex);
                    string error = insideException.Message + insideException.StackTrace;

                    // Error: Settings object could not be serialized. 
                }
            }
        }

        public static Exception getInnerException(Exception ex) {
            if (ex.InnerException == null) {
                return ex;
            }

            return getInnerException(ex.InnerException);
        }

        public static string GetDateTimeShortDateString(string dateTime) {
            string val = dateTime;
            DateTime parsedDateTime;

            if (DateTime.TryParse(dateTime, out parsedDateTime)) {
                val = parsedDateTime.ToShortDateString();
            }

            return val;
        }

        public static string GetDateTimeShortTimeString(string dateTime) {
            string val = dateTime;
            DateTime parsedDateTime;

            if (DateTime.TryParse(dateTime, out parsedDateTime)) {
                val = parsedDateTime.ToShortTimeString();
            }

            return val;
        }

		public static void Save() {
			lock (lockObject) {
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

				using (TextWriter tw = new StreamWriter(FilePath)) {
					xmlSerializer.Serialize(tw, Instance);
				}
			}
		}

        public static object InstantiateObject(string assembly, string type) {
            ObjectHandle objectHandle = Activator.CreateInstance(assembly, type);
            return objectHandle.Unwrap();
        }
        #endregion

        #region Static properties
        public static Settings Instance {
			get {
				return instance;
			}
        }
        #endregion

        #region Instance properties
        public ParentType ParentType {
			get {
				return parentType;
			}
			set {
				parentType = value;
			}
		}

		[XmlIgnore]
		public Nodes KeyCodes
		{
			get {
				return keyCodes;
			}
			set {
				keyCodes = value;
			}
		}

		public int Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}

		public int Height {
			get {
                return height;
			}
			set {
				height = value;
			}
		}

		public string LeftPath {
			get {
				return leftPath;
			}
			set {
				leftPath = value;
			}
		}

		public string RightPath {
			get {
				return rightPath;
			}
			set {
				rightPath = value;
			}
		}

		public int SplitterPercentage {
			get {
                return splitterPercentage;
			}
			set {
				splitterPercentage = value;
			}
		}

        public bool DirectoriesSortedFirst {
            get {
                return directoriesSortedFirst;
            }
            set {
                directoriesSortedFirst = value;
            }
        }

        [XmlArray("ResourceRetrievers")]
        [XmlArrayItem("ResourceRetriever")]
        public List<ResourceRetrieverInfo> ResourceRetrieverInfos {
            get {
                return resourceRetrieverInfos;
            }
            set {
                resourceRetrieverInfos = value;
            }
        }

        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<ChildResourceRetrieverInfo> ChildResourceRetrieverInfos {
            get {
                return childResourceRetrieverInfos;
            }
            set {
                childResourceRetrieverInfos = value;
            }
        }

        #region Properties not derived from settings.config.
        [XmlIgnore]
        public List<IResource> Resources {
            get {
                if (resourceRetrievers == null || resourceRetrievers.Count == 0) {
                    resourceRetrievers = new List<IResourceRetriever>(resourceRetrieverInfos.Count);

                    foreach (ResourceRetrieverInfo resourceRetrieverInfo in resourceRetrieverInfos) {
                        object resourceRetrieverObject = InstantiateObject(
                            resourceRetrieverInfo.FullyQualifiedType.Assembly,
                            resourceRetrieverInfo.FullyQualifiedType.Type);

                        if (resourceRetrieverObject is IResourceRetriever) {
                            IResourceRetriever resourceRetriever = (IResourceRetriever)resourceRetrieverObject;
                            string childResourceRetrieverName = resourceRetrieverInfo.ChildResourceRetriever;

                            ChildResourceRetrieverInfo childResourceRetrieverInfo = childResourceRetrieverInfos.Find(delegate(ChildResourceRetrieverInfo c) {
                                return c.Name == childResourceRetrieverName;
                            });

                            if (childResourceRetrieverInfo != null) {
                                object childResourceRetrieverObject = InstantiateObject(
                                    childResourceRetrieverInfo.FullyQualifiedType.Assembly,
                                    childResourceRetrieverInfo.FullyQualifiedType.Type);

                                if (childResourceRetrieverObject is IChildResourceRetriever) {
                                    IChildResourceRetriever childResourceRetriever = (IChildResourceRetriever)childResourceRetrieverObject;
                                    childResourceRetriever.ColumnInfos = childResourceRetrieverInfo.ColumnInfos;

                                    resourceRetriever.ChildResourceRetriever = childResourceRetriever;
                                } else {
                                    // TODO: Log an error: ChildResourceRetriever is not derived from IChildResourceRetriever.
                                }
                            } else {
                                // TODO: Log an error: ChildResourceRetriever could not be found.
                            }

                            resourceRetrievers.Add(resourceRetriever);
                        } else {
                            // TODO: Log an error here: ResourceRetriever not derived from IResourceRetriever.
                        }
                    }
                }

                List<IResource> resources = new List<IResource>();

                foreach (IResourceRetriever retriever in resourceRetrievers) {
                    resources.AddRange(retriever.Get());
                }

                return resources;
            }
        }

		[XmlIgnore]
		public ImageList ImageList {
			get {
				lock (lockObject) {
					return imageList;
				}
			}
        }
        #endregion
        #endregion
    }
}