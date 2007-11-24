using System;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
	public enum ParentType {
		Mdi,
		Tab,
		Dual
	}

	[Serializable()]
	public class Settings {
		private static object lockObject = new object();

		private ParentType parentType;
		private ImageList imageList = new ImageList();
		private int width;
		private int height;
		private string leftPath;
		private string rightPath;

		public Settings() {
			this.parentType = ParentType.Dual;
			this.imageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		public ParentType ParentType {
			get {
				return parentType;
			} set {
				parentType = value;
			}
		}

		public int Width {
			get {
				return width;
			} set {
				width = value;
			}
		}

		public int Height {
			get {
				return height;
			} set {
				height = value;
			}
		}

		public string LeftPath {
			get {
				return leftPath;
			} set {
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
