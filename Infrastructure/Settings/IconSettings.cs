using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class IconSettings {
        private bool showIcons = true;
        private bool showOverlay = false;
        private List<string> overlayPaths = new List<string>();
        private List<string> extensions = new List<string>();

        /// <summary>
        /// Whether or not to show overlays.
        /// </summary>
        public bool ShowOverlay {
            get {
                return showOverlay;
            }
            set {
                showOverlay = value;
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
        [XmlArray("OverlayPaths")]
        [XmlArrayItem("Path")]
        public List<string> OverlayPaths {
            get {
                return overlayPaths;
            }
            set {
                overlayPaths = value;

                // Make sure the overlay paths are always lower-cased.
                overlayPaths.ForEach(delegate(string s) {
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
    }
}