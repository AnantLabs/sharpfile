using System.Xml.Serialization;
using System;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FullyQualifiedMethod {
        private FullyQualifiedType fullyQualifiedType;
        private string method;

        public FullyQualifiedMethod() {
        }

        public FullyQualifiedMethod(FullyQualifiedType fullyQualifiedType, string method) {
            this.fullyQualifiedType = fullyQualifiedType;
            this.method = method;
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        [XmlAttribute("Method")]
        public string Method {
            get {
                return method;
            }
            set {
                method = value;
            }
        }
    }
}