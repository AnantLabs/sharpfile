using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FullyQualifiedMethod {
        private FullyQualifiedType fullyQualifiedType;
        private string method;
        private List<string> arguments;

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

        public override string ToString() {
            return string.Format("{0}.{1}({2})",
                FullyQualifiedType,
                Method,
                string.Join(",", Arguments.ToArray()));
        }

        public override bool Equals(object obj) {
            return this.ToString().Equals(obj.ToString());
        }
    }
}