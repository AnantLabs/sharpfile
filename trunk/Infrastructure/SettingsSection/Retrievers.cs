using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Retrievers {
        private List<View> views = new List<View>();
        private List<ParentResourceRetriever> parentResourceRetrievers = new List<ParentResourceRetriever>();
        private List<ChildResourceRetriever> childResourceRetrievers = new List<ChildResourceRetriever>();

        [XmlArray("ParentResourceRetrievers")]
        [XmlArrayItem("ParentResourceRetriever")]
        public List<ParentResourceRetriever> ParentResourceRetrievers {
            get {
                return parentResourceRetrievers;
            }
            set {
                parentResourceRetrievers = value;
            }
        }

        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<ChildResourceRetriever> ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }

        [XmlArray("Views")]
        [XmlArrayItem("View")]
        public List<View> Views {
            get {
                return views;
            }
            set {
                views = value;
            }
        }

        public static List<View> GenerateDefaultViews() {
            List<View> views = new List<View>();
            FullyQualifiedType viewType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListView");
            FullyQualifiedType comparerType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListViewItemComparer");
            views.Add(new SettingsSection.View("ListView", viewType, comparerType));
            return views;
        }

        public static List<ParentResourceRetriever> GenerateDefaultParentResourceRetrievers() {
            List<ParentResourceRetriever> parentResourceRetrievers = new List<ParentResourceRetriever>();
            FullyQualifiedType type = new FullyQualifiedType("SharpFile", "SharpFile.IO.Retrievers.DriveRetriever");
            ParentResourceRetriever retriever = new ParentResourceRetriever(
                "DriveRetriever", type, "CompressedRetriever", "DefaultRetriever");
            parentResourceRetrievers.Add(retriever);
            return parentResourceRetrievers;
        }

        public static List<ChildResourceRetriever> GenerateDefaultChildResourceRetrievers() {
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
                new FullyQualifiedMethod("GetDateTimeShortTimeString",
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
                new FullyQualifiedMethod("GetDateTimeShortTimeString",
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