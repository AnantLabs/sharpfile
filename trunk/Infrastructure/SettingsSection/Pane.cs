using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Pane {
        private List<string> paths = new List<string>();
        private bool collapsed = false;
        private bool showFilter = true;
        private FormatTemplate driveFormatTemplate;

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
                if (driveFormatTemplate == null) {
                    driveFormatTemplate = new FormatTemplate("{Name} <{Size}>");
                    driveFormatTemplate.FullyQualifiedMethod = new FullyQualifiedMethod("GetHumanReadableSize",
                        new FullyQualifiedType("Common", "Common.General"));
                }

                return driveFormatTemplate;
            }
            set {
                driveFormatTemplate = value;                
            }
        }
    }
}