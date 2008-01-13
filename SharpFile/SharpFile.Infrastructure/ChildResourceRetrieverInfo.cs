using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrieverInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;
        private ChildResourceRetriever.CustomMethodDelegate customMethod;
        private FullyQualifiedMethod methodDelegateType;

        //public delegate bool CustomMethod(IResource resource);

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetrieverInfo() {
        }

        public ChildResourceRetrieverInfo(string name, List<ColumnInfo> columnInfos, ChildResourceRetriever.CustomMethodDelegate customMethod, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.columnInfos = columnInfos;

            if (customMethod != null) {
                this.methodDelegateType = new FullyQualifiedMethod(new FullyQualifiedType(
                    customMethod.Method.DeclaringType.Namespace,
                    customMethod.Method.DeclaringType.FullName),
                    customMethod.Method.Name);
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

        [XmlArray("ColumnInfos")]
        [XmlArrayItem("ColumnInfo")]
        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
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
        public ChildResourceRetriever.CustomMethodDelegate CustomMethod {
            get {
                if (customMethod == null && methodDelegateType != null) {
                    try {
                        // TODO: This should be in Common.
                        Type type = Assembly.Load(methodDelegateType.FullyQualifiedType.Assembly)
                            .GetType(methodDelegateType.FullyQualifiedType.Type, true);

                        customMethod = (ChildResourceRetriever.CustomMethodDelegate)Delegate.CreateDelegate(
                            typeof(ChildResourceRetriever.CustomMethodDelegate),
                            type.GetMethod(methodDelegateType.Method, 
                            BindingFlags.Public | BindingFlags.Static));
                    } catch (Exception ex) {
                        string blob = ex.Message + ex.StackTrace;
                        // Error: Log the error right here.
                    }
                } else if (customMethod == null && methodDelegateType == null) {
                    customMethod = ChildResourceRetrievers.DefaultCustomMethod;
                }

                return customMethod;
            }
        }
    }
}