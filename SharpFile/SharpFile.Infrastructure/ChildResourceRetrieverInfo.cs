using System.Xml.Serialization;
using System.Collections.Generic;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrieverInfo {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetrieverInfo() {
        }

        public ChildResourceRetrieverInfo(string name, List<ColumnInfo> columnInfos, FullyQualifiedType fullyQualifiedType) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
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
    }
}