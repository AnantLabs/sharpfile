using System;
using System.Xml.Serialization;
using Common.Logger;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class ColumnInfo {
        private string text;
        private string property;
        private bool primaryColumn;
        private CustomMethod customMethod;
        private FullyQualifiedMethod methodDelegateType;

        public delegate string CustomMethod(string val);

        public ColumnInfo() {
        }

        public ColumnInfo(string text, string property, CustomMethod customMethod, bool primaryColumn) {
            this.text = text;
            this.property = property;
            this.primaryColumn = primaryColumn;
            this.customMethod = customMethod;

            if (customMethod != null) {
                this.methodDelegateType = new FullyQualifiedMethod(new FullyQualifiedType(
                    customMethod.Method.DeclaringType.Namespace,
                    customMethod.Method.DeclaringType.FullName),
                    customMethod.Method.Name);
            }
        }

        [XmlAttribute("Text")]
        public string Text {
            get {
                return text;
            }
            set {
                text = value;
            }
        }

        [XmlAttribute("Property")]
        public string Property {
            get {
                return property;
            }
            set {
                property = value;
            }
        }

        [XmlAttribute("PrimaryColumn")]
        public bool PrimaryColumn {
            get {
                return primaryColumn;
            }
            set {
                primaryColumn = value;
            }
        }

        public FullyQualifiedMethod MethodDelegateType {
            get {
                return methodDelegateType;
            }
            set {
                methodDelegateType = value;
            }
        }

        [XmlIgnore]
        public CustomMethod MethodDelegate {
            get {
                if (customMethod == null && methodDelegateType != null) {
                    try {
                        customMethod = Common.Reflection.CreateDelegate<CustomMethod>(
                            methodDelegateType.FullyQualifiedType.Assembly,
                            methodDelegateType.FullyQualifiedType.Type,
                            methodDelegateType.Method);
                    } catch (Exception ex) {
                        string message = "Creating the CustomMethod, {0}, for the {1} ColumnInfo failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                methodDelegateType.FullyQualifiedType.Type, text);

                    }
                }

                return customMethod;
            }
        }
    }
}