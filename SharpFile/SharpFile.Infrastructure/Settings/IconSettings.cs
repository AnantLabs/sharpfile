using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class IconSettings {
        private bool showOverlay = true;
        private List<string> extensions = new List<string>();

        public bool ShowOverlay {
            get {
                return showOverlay;
            }
            set {
                showOverlay = value;
            }
        }

        [XmlArray("Extensions")]
        [XmlArrayItem("Extension")]
        public List<string> Extensions {
            get {
                return extensions;
            }
            set {
                extensions = value;
            }
        }
    }
}