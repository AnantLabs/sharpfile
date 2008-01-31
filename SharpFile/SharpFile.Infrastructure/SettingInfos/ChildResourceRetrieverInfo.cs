using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common.Logger;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrieverInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;
        private ChildResourceRetriever.CustomMethodDelegate customMethod;
        private FullyQualifiedMethod methodDelegateType;
        private string view;

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetrieverInfo() {
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

        public string View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        [XmlIgnore]
        public ChildResourceRetriever.CustomMethodDelegate CustomMethod {
            get {
                if (customMethod == null && methodDelegateType != null) {
                    try {
                        customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodDelegate>(
                            methodDelegateType.FullyQualifiedType.Assembly,
                            methodDelegateType.FullyQualifiedType.Type,
                            methodDelegateType.Method);
                    } catch (Exception ex) {
                        string message = "Creating the CustomMethod, {0}, for the {1} ChildResourceRetriever failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message, 
                                methodDelegateType.FullyQualifiedType.Type, name);
                    }
                } 
							
							  // If the customMethod is still null, then there was an error creating the delegate, 
								// or the method delegate type was null. Either way, set a default.
							  if (customMethod == null) {
                    customMethod = ChildResourceRetrievers.DefaultCustomMethod;
                }

                return customMethod;
            }
        }
    }
}