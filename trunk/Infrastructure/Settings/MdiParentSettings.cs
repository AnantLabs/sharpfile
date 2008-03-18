using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
	// TODO: Use the PanelSettings class currently used by DualParentSettings.
    [Serializable]
    public sealed class ChildInfo {
        private List<string> paths;

        [XmlArray("Paths")]
        [XmlArrayItem("Path")]
        public List<string> Paths {
            get {
                return paths;
            }
            set {
                paths = value;
            }
        }
    }

    [Serializable]
    public sealed class MdiParentSettings {
        private List<ChildInfo> children;

        [XmlArray("Children")]
        [XmlArrayItem("Child")]
        public List<ChildInfo> Children {
            get {
                return children;
            }
            set {
                children = value;
            }
        }
    }
}