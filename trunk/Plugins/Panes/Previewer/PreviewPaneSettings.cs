using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class PreviewPaneSettings : IPluginPaneSettings {
        private bool collapsed = false;
        private List<string> detailTextExtensions = new List<string>();
        private FormatTemplate nameFormatTemplate = new FormatTemplate("{Name} {LastWriteTime}");
        private int splitterPercentage = 10;
        private bool thumbnailImages = false;
        private int maximumLinesOfDetailText = 10;
        private bool showDetailTextForAllExtensions = false;
        private bool showAllDetailText = false;

        public void GenerateDefaultSettings() {
            if (detailTextExtensions.Count == 0) {
                detailTextExtensions = GenerateDefaultDetailTextExtensions();
            }
        }

        /// <summary>
        /// Whether or not to show detail text for all extensions.
        /// </summary>
        public bool ShowDetailTextForAllExtensions {
            get {
                return showDetailTextForAllExtensions;
            }
            set {
                showDetailTextForAllExtensions = value;
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

        public bool ShowAllDetailText {
            get {
                return showAllDetailText;
            }
            set {
                showAllDetailText = value;
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

        public static List<string> GenerateDefaultDetailTextExtensions() {
            List<string> extensions = new List<string>(new string[] { 
                        string.Empty, ".txt", ".config", ".xml", ".ini", ".cs", ".log" 
                    });
            return extensions;
        }
    }
}