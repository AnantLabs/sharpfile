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
    }
}