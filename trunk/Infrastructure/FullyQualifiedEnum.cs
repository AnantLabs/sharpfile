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

        public override string ToString() {
            return string.Format("{0}.{1}",
                                FullyQualifiedType.Type,
                                Enum);
        }

        public override bool Equals(object obj) {
            return this.ToString().Equals(obj.ToString());
        }
    }
}