using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Icons {
        private bool showIcons = true;
        private bool showOverlaysForAllPaths = false;
        private List<string> showOverlayPaths = new List<string>();
        private List<string> retrieveIconExtensions = new List<string>();
        private List<FullyQualifiedEnum> intensiveSearchDriveTypeEnums = new List<FullyQualifiedEnum>();

        /// <summary>
        /// Whether or not to show overlays.
        /// </summary>
        public bool ShowOverlaysForAllPaths {
            get {
                return showOverlaysForAllPaths;
            }
            set {
                showOverlaysForAllPaths = value;
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
        /// Extensions to get icons for.
        /// </summary>
        [XmlArray("RetrieveIconExtensions")]
        [XmlArrayItem("Extension")]
        public List<string> RetrieveIconExtensions {
            get {
                return retrieveIconExtensions;
            }
            set {
                retrieveIconExtensions = value;

                // Make sure the extensions are always lower-cased.
                retrieveIconExtensions.ForEach(delegate(string s) {
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