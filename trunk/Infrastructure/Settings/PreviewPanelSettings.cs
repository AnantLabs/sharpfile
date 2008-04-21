using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class PreviewPanelSettings {
        private bool collapsed = false;
        private List<string> detailTextExtensions = new List<string>();
        private FormatTemplate nameFormatTemplate = new FormatTemplate("{Name}");
        private int splitterPercentage = 10;
        private bool thumbnailImages = true;
        private int maximumLinesOfDetailText = 10;
        private bool alwaysShowDetailText = false;

        public bool AlwaysShowDetailText {
            get {
                return alwaysShowDetailText;
            }
            set {
                alwaysShowDetailText = value;
            }
        }

        public int MaximumLinesOfDetailText {
            get {
                return maximumLinesOfDetailText;
            }
            set {
                maximumLinesOfDetailText = value;
            }
        }

        public bool ThumbnailImages {
            get {
                return thumbnailImages;
            }
            set {
                thumbnailImages = value;
            }
        }

        public int SplitterPercentage {
            get {
                return splitterPercentage;
            }
            set {
                splitterPercentage = value;
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

        public FormatTemplate NameFormatTemplate {
            get {
                return nameFormatTemplate;
            }
            set {
                nameFormatTemplate = value;
            }
        }

        /// <summary>
        /// Extensions to get text for.
        /// </summary>
        [XmlArray("DetailTextExtensions")]
        [XmlArrayItem("Extension")]
        public List<string> DetailTextExtensions {
            get {
                return detailTextExtensions;
            }
            set {
                detailTextExtensions = value;

                // Make sure the extensions are always lower-cased.
                detailTextExtensions.ForEach(delegate(string s) {
                    s = s.ToLower();
                });
            }
        }
    }
}