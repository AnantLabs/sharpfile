using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace SharpFile.Infrastructure {
	public enum ParentType {
		Mdi,
		Tab,
		Dual
	}

	/// <summary>
	/// Settings singleton.
	/// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html
	/// </summary>
	[Serializable()]
	public sealed class Settings {
		public const string FilePath = "settings.config";

		private static readonly Settings instance = new Settings();
		private static object lockObject = new object();

		private ParentType parentType;
		private ImageList imageList = new ImageList();
		private int width;
		private int height;
		private string leftPath;
		private string rightPath;
		private int splitterPercentage;
		private Nodes keyCodes;

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit.
		static Settings() {
			Load();
		}

		private Settings() {
			lockObject = new object();
			this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
		}

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
							instance.Height = settings.Height;
							instance.Width = settings.Width;
							instance.LeftPath = settings.LeftPath;
							instance.RightPath = settings.RightPath;
							instance.ParentType = settings.ParentType;
							instance.SplitterPercentage = settings.SplitterPercentage;
							instance.KeyCodes = settings.KeyCodes;
						}

						settingsLoaded = true;
					} catch {
						settingsLoaded = false;

						// TODO: Show a message saying that default values have been loaded.
					}
				} 

				if (!settingsLoaded) {
					instance.Height = 500;
					instance.Width = 500;
					instance.SplitterPercentage = 250;
					instance.ParentType = ParentType.Dual;

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

		public static Settings Instance {
			get {
				return instance;
			}
		}

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

		[XmlIgnore]
		public ImageList ImageList {
			get {
				lock (lockObject) {
					return imageList;
				}
			}
		}
	}
}