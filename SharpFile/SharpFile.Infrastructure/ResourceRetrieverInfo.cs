using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ResourceRetrieverInfo {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private string childResourceRetriever;

        public ResourceRetrieverInfo() {
        }

        public ResourceRetrieverInfo(string name, string childResourceRetriever, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.childResourceRetriever = childResourceRetriever;
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

        [XmlAttribute("ChildResourceRetriever")]
        public string ChildResourceRetriever {
            get {
                return childResourceRetriever;
            }
            set {
                childResourceRetriever = value;
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