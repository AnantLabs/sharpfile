using System.Drawing;

namespace SharpFile.Infrastructure.SettingsSection {
    public class FontInfo {
        private string familyName = "Arial";
        private float size = 8;
        private FontStyle style = FontStyle.Regular;

        public FontInfo() {
        }

        public string FamilyName {
            get {
                return familyName;
            }
            set {
                familyName = value;
            }
        }

        public float Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        public FontStyle Style {
            get {
                return style;
            }
            set {
                style = value;
            }
        }
    }
}