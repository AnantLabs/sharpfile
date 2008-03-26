using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class IconSettings {
        private bool showIcons = true;
        private bool showAllOverlays = false;
        private List<string> showOverlayPaths = new List<string>();
        private List<string> extensions = new List<string>();
        private List<FullyQualifiedEnum> intensiveSearchDriveTypeEnums = new List<FullyQualifiedEnum>();

        /// <summary>
        /// Whether or not to show overlays.
        /// </summary>
        public bool ShowAllOverlays {
            get {
                return showAllOverlays;
            }
            set {
                showAllOverlays = value;
            }
        }

        /// <summary>
        /// Whether or not to show icons.
        /// </summary>
        public bool ShowIcons {
            get {
                return showIcons;
            }
            set {
                showIcons = value;
            }
        }

        /// <summary>
        /// Paths to show overlays for.
        /// </summary>
        [XmlArray("ShowOverlayPaths")]
        [XmlArrayItem("Path")]
        public List<string> ShowOverlayPaths {
            get {
                return showOverlayPaths;
            }
            set {
                showOverlayPaths = value;

                // Make sure the overlay paths are always lower-cased.
                showOverlayPaths.ForEach(delegate(string s) {
                    s = s.ToLower();
                });
            }
        }

        /// <summary>
        /// Extensions to get icons for.
        /// </summary>
        [XmlArray("Extensions")]
        [XmlArrayItem("Extension")]
        public List<string> Extensions {
            get {
                return extensions;
            }
            set {
                extensions = value;

                // Make sure the extensions are always lower-cased.
                extensions.ForEach(delegate(string s) {
                    s = s.ToLower();
                });
            }
        }

        [XmlArray("IntensiveSearchDriveTypeEnums")]
        [XmlArrayItem("FullyQualifiedEnum")]
        public List<FullyQualifiedEnum> IntensiveSearchDriveTypeEnums {
            get {
                return intensiveSearchDriveTypeEnums;
            }
            set {
                intensiveSearchDriveTypeEnums = value;
            }
        }
    }
}