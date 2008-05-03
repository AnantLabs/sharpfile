using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    [Serializable]
    public class ColumnInfo {
        private string text;
        private string property;
        private bool primaryColumn = false;
        private List<FullyQualifiedType> excludeForTypes;
        private FullyQualifiedMethod.AlterMethod alterMethod;
        private FullyQualifiedMethod fullyQualifiedMethod;

        public ColumnInfo() {
        }

        public ColumnInfo(string text, string property, bool primaryColumn)
            : this(text, property, primaryColumn, null) {
        }

        public ColumnInfo(string text, string property, bool primaryColumn,
            FullyQualifiedMethod fullyQualifiedMethod, params FullyQualifiedType[] excludeForTypes) {
            this.text = text;
            this.property = property;
            this.primaryColumn = primaryColumn;
            this.fullyQualifiedMethod = fullyQualifiedMethod;

            if (excludeForTypes == null) {
                this.excludeForTypes = new List<FullyQualifiedType>();
            } else {
                this.excludeForTypes = new List<FullyQualifiedType>(excludeForTypes);
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

        [XmlArray("ExcludeForTypes")]
        [XmlArrayItem("FullyQualifiedType")]
        public List<FullyQualifiedType> ExcludeForTypes {
            get {
                return excludeForTypes;
            }
            set {
                excludeForTypes = value;
            }
        }

        public FullyQualifiedMethod FullyQualifiedMethod {
            get {
                return fullyQualifiedMethod;
            }
            set {
                fullyQualifiedMethod = value;
            }
        }

        [XmlIgnore]
        public FullyQualifiedMethod.AlterMethod MethodDelegate {
            get {
                if (alterMethod == null && fullyQualifiedMethod != null) {
                    try {
                        alterMethod = Reflection.CreateDelegate<FullyQualifiedMethod.AlterMethod>(
                            fullyQualifiedMethod.FullyQualifiedType.Assembly,
                            fullyQualifiedMethod.FullyQualifiedType.Type,
                            fullyQualifiedMethod.Name);
                    } catch (Exception ex) {
                        string message = "Creating the AlterMethod, {0}, for the {1} Column failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                fullyQualifiedMethod.FullyQualifiedType.Type, text);

                    }
                }

                return alterMethod;
            }
        }
    }
}