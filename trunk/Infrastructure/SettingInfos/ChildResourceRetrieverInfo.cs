using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common.Logger;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrieverInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;
        private Delegate customMethod;
        private FullyQualifiedMethod methodDelegateType;
        private string view;
        private List<string> customMethodArguments;

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
        public List<string> CustomMethodArguments {
            get {
                return customMethodArguments;
            }
            set {
                customMethodArguments = value;
            }
        }

        [XmlIgnore]
        public Delegate CustomMethod {
            get {
                if (customMethod == null && methodDelegateType != null) {
                    try {
                        // Create the appropriate method delegate based on whether there were arguments passed in.
                        if (methodDelegateType.Arguments != null && methodDelegateType.Arguments.Count > 0) {
                            customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodWithArgumentsDelegate>(
                               methodDelegateType.FullyQualifiedType.Assembly,
                               methodDelegateType.FullyQualifiedType.Type,
                               methodDelegateType.Method);

                            customMethodArguments = methodDelegateType.Arguments;
                        } else {
                            customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodDelegate>(
                                methodDelegateType.FullyQualifiedType.Assembly,
                                methodDelegateType.FullyQualifiedType.Type,
                                methodDelegateType.Method);
                        }
                    } catch (Exception ex) {
                        string message = "Creating the CustomMethod, {0}, for the {1} ChildResourceRetriever failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                methodDelegateType.FullyQualifiedType.Type, name);
                    }
                }

                // If the customMethod is still null, then there was an error creating the delegate, 
                // or the method delegate type was null. Either way, set a default.
                if (customMethod == null) {
                    customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodDelegate>(
                                "SharpFile",
                                "SharpFile.Infrastructure.ChildResourceRetrievers",
                                "DefaultCustomMethod");

                    //customMethod = ChildResourceRetrievers.DefaultCustomMethod;
                }

                return customMethod;
            }
        }
    }
}