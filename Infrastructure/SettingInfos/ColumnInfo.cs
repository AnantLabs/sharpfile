﻿using System;
using System.Xml.Serialization;
using Common.Logger;
using System.Collections.Generic;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class ColumnInfo {
        private string text;
        private string property;
        private bool primaryColumn = false;
        private List<FullyQualifiedType> excludeForTypes;
        private CustomMethod customMethod;
        private FullyQualifiedMethod methodDelegateType;

        public delegate string CustomMethod(string val);

        public ColumnInfo() {
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