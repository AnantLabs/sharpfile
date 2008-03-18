using System.Xml.Serialization;
using System;

namespace SharpFile.Infrastructure {
    /// <summary>
    /// Object that stores the assembly and type name for a type. Cannot be a struct 
    /// because a parameter-less ctor is required for Xml serialization.
    /// </summary>
    [Serializable]
    public class FullyQualifiedType {
        private string assembly;
        private string type;

        public FullyQualifiedType() {
        }

        public FullyQualifiedType(string assembly, string type) {
            this.assembly = assembly;
            this.type = type;
        }

        [XmlAttribute("Assembly")]
        public string Assembly {
            get {
                return assembly;
            }
            set {
                assembly = value;
            }
        }

        [XmlAttribute("Type")]
        public string Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }
    }
}