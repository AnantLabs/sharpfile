using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Tools {
        private List<Tool> list = new List<Tool>();

        public void Add(Tool item) {
            list.Add(item);
        }

        [XmlArray("Tools")]
        [XmlArrayItem("Tool")]
        public List<Tool> List {
            get {
                return list;
            }
            set {
                list = value;
            }
        }

        [XmlIgnore]
        public int Count {
            get {
                return list.Count;
            }
        }
    }
}