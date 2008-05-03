using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {    
    public sealed class DualParentSettings {
        private const string defaultDrive = @"c:\";
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;        
        private PanelSettings panel1 = new PanelSettings();
        private PanelSettings panel2 = new PanelSettings();

        [XmlElement("Panel1")]
        public PanelSettings Panel1 {
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
        public PanelSettings Panel2 {
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