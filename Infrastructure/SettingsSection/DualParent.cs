using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {    
    public sealed class DualParent {
        private const string defaultDrive = @"c:\";
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;        
        private Panel panel1 = new Panel();
        private Panel panel2 = new Panel();

        [XmlElement("Panel1")]
        public Panel Panel1 {
            get {
                if (panel1.Paths.Count == 0) {
                    panel1.Paths.Add(defaultDrive);
                }

                return panel1;
            }
            set {
                panel1 = value;
            }
        }

        [XmlElement("Panel2")]
        public Panel Panel2 {
            get {
                if (panel2.Paths.Count == 0) {
                    panel2.Paths.Add(defaultDrive);
                }

                return panel2;
            }
            set {
                panel2 = value;
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

        public Orientation Orientation {
            get {
                return orientation;
            }
            set {
                orientation = value;
            }
        }
    }
}