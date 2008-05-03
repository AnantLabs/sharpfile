using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ParentResourceRetrieverSetting {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private List<string> childResourceRetrievers;

        public ParentResourceRetrieverSetting() {
        }

        public ParentResourceRetrieverSetting(string name, FullyQualifiedType fullyQualifiedType, params string[] childResourceRetrievers) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.childResourceRetrievers = new List<string>(childResourceRetrievers);
        }

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<string> ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }
    }
}