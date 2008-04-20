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
        private FullyQualifiedMethod fullyQualifiedMethod;
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

        public FullyQualifiedMethod FullyQualifiedMethod {
            get {
                return fullyQualifiedMethod;
            }
            set {
                fullyQualifiedMethod = value;
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
                if (customMethod == null && fullyQualifiedMethod != null) {
                    try {
                        // Create the appropriate method delegate based on whether there were arguments passed in.
                        if (fullyQualifiedMethod.Arguments != null && fullyQualifiedMethod.Arguments.Count > 0) {
                            customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodWithArgumentsDelegate>(
                               fullyQualifiedMethod.FullyQualifiedType.Assembly,
                               fullyQualifiedMethod.FullyQualifiedType.Type,
                               fullyQualifiedMethod.Method);

                            customMethodArguments = fullyQualifiedMethod.Arguments;
                        } else {
                            customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodDelegate>(
                                fullyQualifiedMethod.FullyQualifiedType.Assembly,
                                fullyQualifiedMethod.FullyQualifiedType.Type,
                                fullyQualifiedMethod.Method);
                        }
                    } catch (Exception ex) {
                        string message = "Creating the CustomMethod, {0}, for the {1} ChildResourceRetriever failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                fullyQualifiedMethod.FullyQualifiedType.Type, name);
                    }
                }

                // If the customMethod is still null, then there was an error creating the delegate, 
                // or the method delegate type was null. Either way, set a default.
                if (customMethod == null) {
                    customMethod = Common.Reflection.CreateDelegate<ChildResourceRetriever.CustomMethodDelegate>(
                                "SharpFile",
                                "SharpFile.Infrastructure.ChildResourceRetrievers",
								"FalseCustomMethod");
                }

                return customMethod;
            }
        }
    }
}