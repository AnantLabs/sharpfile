using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class MdiParentSettings {
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
}