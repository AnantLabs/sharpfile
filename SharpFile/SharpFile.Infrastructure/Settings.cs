using System;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace SharpFile.Infrastructure {
	public enum ParentType {
		Mdi,
		Tab,
		Dual
	}

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

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static Settings() {
			loadXml();
		}

		Settings() {
			lockObject = new object();
			this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		private static void loadXml() {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

			// If there is no settings file, create one from some defaults.
			if (!File.Exists(FilePath)) {
				instance.Height = 500;
				instance.Width = 500;
				instance.ParentType = ParentType.Dual;

				using (TextWriter tw = new StreamWriter(FilePath)) {
					xmlSerializer.Serialize(tw, instance);
				}
			} else {
				using (TextReader tr = new StreamReader(FilePath)) {
					Settings settings = (Settings)xmlSerializer.Deserialize(tr);
					instance.Height = settings.Height;
					instance.Width = settings.Width;
					instance.LeftPath = settings.LeftPath;
					instance.RightPath = settings.RightPath;
					instance.ParentType = settings.ParentType;
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