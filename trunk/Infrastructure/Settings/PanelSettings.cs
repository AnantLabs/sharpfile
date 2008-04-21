using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class PanelSettings {
        private List<string> paths = new List<string>();
        private bool collapsed = false;
        private bool showFilter = true;
        private FormatTemplate driveFormatTemplate = new FormatTemplate("{Name}");

        [XmlArray("Paths")]
        [XmlArrayItem("Path")]
        public List<string> Paths {
            get {
                return paths;
            }
            set {
                paths = value;
            }
        }

        public bool Collapsed {
            get {
                return collapsed;
            }
            set {
                collapsed = value;
            }
        }

        public bool ShowFilter {
            get {
                return showFilter;
            }
            set {
                showFilter = value;
            }
        }


        public FormatTemplate DriveFormatTemplate {
            get {
                return driveFormatTemplate;
            }
            set {
                driveFormatTemplate = value;
            }
        }
    }
}