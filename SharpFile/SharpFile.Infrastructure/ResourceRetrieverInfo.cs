using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ResourceRetrieverInfo {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private List<string> childResourceRetrievers;

        public ResourceRetrieverInfo() {
        }

        public ResourceRetrieverInfo(string name, List<string> childResourceRetrievers, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.childResourceRetrievers = childResourceRetrievers;
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

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }
    }
}