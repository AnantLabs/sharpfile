using System.Drawing;

namespace SharpFile.Infrastructure.SettingsSection {
    public class FontInfo {
        private string familyName;
        private float size;
        private FontStyle style;

        public FontInfo() {
        }

        public FontInfo(string familyName, float size, FontStyle style) {
            this.familyName = familyName;
            this.size = size;
            this.style = style;
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