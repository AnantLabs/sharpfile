using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    public class ChildResourceRetriever {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;
        private Delegate filterMethod;
        private FullyQualifiedMethod fullyQualifiedMethod;
        private string view;
        private List<string> filterMethodArguments;

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetriever() {
        }

        public ChildResourceRetriever(string name, FullyQualifiedType fullyQualifiedType, FullyQualifiedMethod fullyQualifiedMethod,
            string view, List<ColumnInfo> columnInfos) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.fullyQualifiedMethod = fullyQualifiedMethod;
            this.view = view;
            this.columnInfos = columnInfos;
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

        [XmlArray("Columns")]
        [XmlArrayItem("Column")]
        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        [XmlIgnore]
        public List<string> FilterMethodArguments {
            get {
                return filterMethodArguments;
            }
            set {
                filterMethodArguments = value;
            }
        }

        [XmlIgnore]
        public Delegate FilterMethod {
            get {
                if (filterMethod == null && fullyQualifiedMethod != null) {
                    try {
                        // Create the appropriate method delegate based on whether there were arguments passed in.
                        if (fullyQualifiedMethod.Arguments != null && fullyQualifiedMethod.Arguments.Count > 0) {
                            filterMethod = Reflection.CreateDelegate<Infrastructure.ChildResourceRetriever.FilterMethodWithArgumentsDelegate>(
                               fullyQualifiedMethod.FullyQualifiedType.Assembly,
                               fullyQualifiedMethod.FullyQualifiedType.Type,
                               fullyQualifiedMethod.Name);

                            filterMethodArguments = fullyQualifiedMethod.Arguments;
                        } else {
                            filterMethod = Reflection.CreateDelegate<Infrastructure.ChildResourceRetriever.FilterMethodDelegate>(
                                fullyQualifiedMethod.FullyQualifiedType.Assembly,
                                fullyQualifiedMethod.FullyQualifiedType.Type,
                                fullyQualifiedMethod.Name);
                        }
                    } catch (Exception ex) {
                        string message = "Creating the FilterMethod, {0}, for the {1} ChildResourceRetriever failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                fullyQualifiedMethod.FullyQualifiedType.Type, name);
                    }
                }

                // If the filterMethod is still null, then there was an error creating the delegate, 
                // or the method delegate type was null. Either way, set a default.
                if (filterMethod == null) {
                    filterMethod = Reflection.CreateDelegate<Infrastructure.ChildResourceRetriever.FilterMethodDelegate>(
                                "SharpFile",
                                "SharpFile.Infrastructure.ChildResourceRetrievers",
                                "FalseFilterMethod");
                }

                return filterMethod;
            }
        }

        public static List<ChildResourceRetriever> GenerateDefaults() {
            List<ChildResourceRetriever> childResourceRetrieverSettings = new List<ChildResourceRetriever>();

            // Default retriever.
            FullyQualifiedType type = new FullyQualifiedType("SharpFile", "SharpFile.IO.Retrievers.DefaultRetriever");
            FullyQualifiedMethod method = new FullyQualifiedMethod("TrueFilterMethod",
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.ChildResourceRetrievers"));

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("Filename", "DisplayName", true));
            columnInfos.Add(new ColumnInfo("Size", "Size", false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Date", "LastWriteTime", false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.RootDirectoryInfo")));
            columnInfos.Add(new ColumnInfo("Time", "LastWriteTime", false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.RootDirectoryInfo")));

            SettingsSection.ChildResourceRetriever retriever = new SettingsSection.ChildResourceRetriever(
                "DefaultRetriever", type, method, "ListView", columnInfos);
            childResourceRetrieverSettings.Add(retriever);

            // Compressed retriever.
            type = new FullyQualifiedType("SharpFile", "SharpFile.IO.Retrievers.CompressedRetrievers.ReadOnlyCompressedRetriever");
            method = new FullyQualifiedMethod("IsFileWithExtension",
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.ChildResourceRetrievers"),
                ".zip");

            columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("Filename", "DisplayName", true));
            columnInfos.Add(new ColumnInfo("Size", "Size", false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Compressed Size", "CompressedSize", false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Date", "LastWriteTime", false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.RootDirectoryInfo")));
            columnInfos.Add(new ColumnInfo("Time", "LastWriteTime", false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile", "SharpFile.IO.ChildResources.RootDirectoryInfo")));

            retriever = new SettingsSection.ChildResourceRetriever(
                "CompressedRetriever", type, method, "ListView", columnInfos);
            childResourceRetrieverSettings.Add(retriever);

            return childResourceRetrieverSettings;
        }
    }
}