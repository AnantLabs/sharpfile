using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FullyQualifiedMethod {
        private FullyQualifiedType fullyQualifiedType;
        private string method;
        private List<string> arguments;

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

        [XmlArray("Arguments")]
        [XmlArrayItem("Argument")]
        public List<string> Arguments {
            get {
                return arguments;
            }
            set {
                arguments = value;
            }
        }
    }
}