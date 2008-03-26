using System;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FullyQualifiedEnum {
        private FullyQualifiedType fullyQualifiedType;
        private string enumeration;

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        [XmlAttribute("Enum")]
        public string Enum {
            get {
                return enumeration;
            }
            set {
                enumeration = value;
            }
        }
    }
}