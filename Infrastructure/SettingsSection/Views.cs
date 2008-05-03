using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Views {
        private List<View> list = new List<View>();

        public void Add(View item) {
            list.Add(item);
        }

        public View Find(System.Predicate<View> match) {
            return list.Find(match);
        }

        [XmlArray("Views")]
        [XmlArrayItem("View")]
        public List<View> List {
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
