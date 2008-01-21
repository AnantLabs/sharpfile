using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ViewInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private FullyQualifiedType comparerType;

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

        [XmlElement("ComparerType")]
        public FullyQualifiedType ComparerType {
            get {
                return comparerType;
            }
            set {
                comparerType = value;
            }
        }
    }
}
