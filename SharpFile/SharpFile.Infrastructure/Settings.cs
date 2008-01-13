using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Common;

namespace SharpFile.Infrastructure {
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
		private Nodes keyCodes;
        private List<ResourceRetrieverInfo> resourceRetrieverInfos;
        private List<ChildResourceRetrieverInfo> childResourceRetrieverInfos;
        private bool directoriesSortedFirst = true;

        // Sub-settings.
        private DualParentSettings dualParentSettings;
        private MdiParentSettings mdiParentSettings;
        private IconSettings iconSettings;

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
            dualParentSettings = new DualParentSettings();
            mdiParentSettings = new MdiParentSettings();
            iconSettings = new IconSettings();

			lockObject = new object();
			this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
        }
        #endregion

        #region Static methods.
        public static void Load() {
            lock (lockObject) {
                // Null out the reource retrievers in case settings are being reloaded.
                instance.resourceRetrievers = null;

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
                            string blob = ex.Message + ex.StackTrace;
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
                                return t.Namespace.Equals("SharpFile.Infrastructure");
                            }) != null) {
                                settingsXml = Resource.ilmerge_settings;
                            }
                        } catch { }

                        using (XmlWriter xmlWriter = XmlWriter.Create(FilePath, xmlWriterSettings)) {
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(settingsXml);
                            xml.WriteTo(xmlWriter);
                        }

                        deserializeSettings(xmlSerializer);
                    }
                } catch (Exception ex) {
                    Exception insideException = Common.General.GetInnerException(ex);
                    string error = insideException.Message + insideException.StackTrace;

                    // Error: Settings object could not be serialized.
                }
            }
        }

        private static void deserializeSettings(XmlSerializer xmlSerializer) {
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
        }

        public static T DeepCopy<T>(T obj) {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return (T)binaryFormatter.Deserialize(memoryStream);
        }

		public static void Save() {
			lock (lockObject) {
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

				using (TextWriter tw = new StreamWriter(FilePath)) {
					xmlSerializer.Serialize(tw, Instance);
				}
			}
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

        #region Properties for sub-settings.
        [XmlElement("DualParent")]
        public DualParentSettings DualParent {
            get {
                return dualParentSettings;
            }
            set {
                dualParentSettings = value;
            }
        }

        [XmlElement("MdiParent")]
        public MdiParentSettings MdiParent {
            get {
                return mdiParentSettings;
            }
            set {
                mdiParentSettings = value;
            }
        }

        [XmlElement("Icons")]
        public IconSettings Icons {
            get {
                return iconSettings;
            }
            set {
                iconSettings = value;
            }
        }
        #endregion

        #region Properties not derived from settings.config.
        [XmlIgnore]
        public List<IResource> Resources {
            get {
                if (resourceRetrievers == null || resourceRetrievers.Count == 0) {
                    resourceRetrievers = new List<IResourceRetriever>(resourceRetrieverInfos.Count);

                    foreach (ResourceRetrieverInfo resourceRetrieverInfo in resourceRetrieverInfos) {
                        try {
                            object resourceRetrieverObject = Reflection.InstantiateObject(
                                resourceRetrieverInfo.FullyQualifiedType.Assembly,
                                resourceRetrieverInfo.FullyQualifiedType.Type);

                            if (resourceRetrieverObject is IResourceRetriever) {
                                IResourceRetriever resourceRetriever = (IResourceRetriever)resourceRetrieverObject;
                                
                                foreach (string childResourceRetrieverName in resourceRetrieverInfo.ChildResourceRetrievers) {
                                    ChildResourceRetrieverInfo childResourceRetrieverInfo = childResourceRetrieverInfos.Find(delegate(ChildResourceRetrieverInfo c) {
                                        return c.Name == childResourceRetrieverName;
                                    });

                                    if (childResourceRetrieverInfo != null) {
                                        try {
                                            object childResourceRetrieverObject = Reflection.InstantiateObject(
                                                childResourceRetrieverInfo.FullyQualifiedType.Assembly,
                                                childResourceRetrieverInfo.FullyQualifiedType.Type);

                                            if (childResourceRetrieverObject is IChildResourceRetriever) {
                                                IChildResourceRetriever childResourceRetriever = (IChildResourceRetriever)childResourceRetrieverObject;
                                                childResourceRetriever.Name = childResourceRetrieverName;
                                                childResourceRetriever.ColumnInfos = childResourceRetrieverInfo.ColumnInfos;
                                                childResourceRetriever.CustomMethod += childResourceRetrieverInfo.CustomMethod;

                                                if (resourceRetriever.ChildResourceRetrievers == null) {
                                                    resourceRetriever.ChildResourceRetrievers = new ChildResourceRetrievers();
                                                }

                                                resourceRetriever.ChildResourceRetrievers.Add(childResourceRetriever);
                                            } else {
                                                // TODO: Log an error: ChildResourceRetriever is not derived from IChildResourceRetriever.
                                            }
                                        } catch (TypeLoadException ex) {
                                            // TODO: Log an error here: ChildResourceRetriever could not be instantiated.
                                        }
                                    } else {
                                        // TODO: Log an error: ChildResourceRetriever could not be found.
                                    }
                                }

                                resourceRetrievers.Add(resourceRetriever);
                            } else {
                                // TODO: Log an error here: ResourceRetriever not derived from IResourceRetriever.
                            }
                        } catch (TypeLoadException ex) {
                            // TODO: Log an error here: ResourceRetriever could not be instantiated.
                        }
                    }

                    Save();
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