using System;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;

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
        private string assemblyName;
        private string typeName;

        public FullyQualifiedType() {
        }

        public FullyQualifiedType(string assemblyName, string typeName) {
            this.assemblyName = assemblyName;
            this.typeName = typeName;
        }

        [XmlAttribute("Assembly")]
        public string AssemblyName {
            get {
                return assemblyName;
            }
            set {
                assemblyName = value;
            }
        }

        [XmlAttribute("Name")]
        public string TypeName {
            get {
                return typeName;
            }
            set {
                typeName = value;
            }
        }
    }

	/// <summary>
	/// Settings singleton.
	/// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html.
	/// </summary>
	[Serializable()]
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
        private List<FullyQualifiedType> resourceRetrieverTypes;
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
                    List<FullyQualifiedType> resourceRetrieverTypes = new List<FullyQualifiedType>();
                    resourceRetrieverTypes.Add(
                        new FullyQualifiedType("SharpFile.IO", "SharpFile.IO.Retrievers.DriveRetriever"));
                    resourceRetrieverTypes.Add(
                        new FullyQualifiedType("SharpFile.IO", "SharpFile.IO.Retrievers.NetworkDriveRetriever"));
                    instance.ResourceRetrieverTypes = resourceRetrieverTypes;

                    Nodes defaultKeys = new Nodes();
                    defaultKeys.Add("Rename", Keys.F2);
                    instance.KeyCodes = defaultKeys;

                    using (TextWriter tw = new StreamWriter(FilePath)) {
                        xmlSerializer.Serialize(tw, instance);
                    }
                }
            }
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
        [XmlArrayItem("Type")]
        public List<FullyQualifiedType> ResourceRetrieverTypes {
            get {
                return resourceRetrieverTypes;
            }
            set {
                resourceRetrieverTypes = value;
            }
        }

        #region Properties not derived from settings.config.
        [XmlIgnore]
        public List<IResource> Resources {
            get {
                if (resourceRetrievers == null) {
                    resourceRetrievers = new List<IResourceRetriever>(resourceRetrieverTypes.Count);

                    foreach (FullyQualifiedType fullyQualifiedType in resourceRetrieverTypes) {
                        ObjectHandle objectHandle = Activator.CreateInstance(
                                fullyQualifiedType.AssemblyName, fullyQualifiedType.TypeName);
                        object obj = objectHandle.Unwrap();

                        if (obj is IResourceRetriever) {
                            resourceRetrievers.Add(obj as IResourceRetriever);
                        } else {
                            // TODO: Log an error here. "Unhandled resource type: " + fullyQualifiedType.TypeName
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