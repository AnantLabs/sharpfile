using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class FullyQualifiedMethod {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private List<string> arguments = new List<string>();

        public delegate string AlterMethod(string value);

        public FullyQualifiedMethod() {
        }

        public FullyQualifiedMethod(string name, FullyQualifiedType fullyQualifiedType)
            : this(name, fullyQualifiedType, new List<string>()) {
        }

        public FullyQualifiedMethod(string name, FullyQualifiedType fullyQualifiedType, List<string> arguments) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.arguments = arguments;
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
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

        // TODO: Generate the delegate for the method.

        public override string ToString() {
            return string.Format("{0}.{1}({2})",
                FullyQualifiedType,
                Name,
                string.Join(",", Arguments.ToArray()));
        }

        public override bool Equals(object obj) {
            return this.ToString().Equals(obj.ToString());
        }
    }
}