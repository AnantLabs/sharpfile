using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class DualParentSettings {
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;        
        private PanelSettings panel1 = new PanelSettings();
        private PanelSettings panel2 = new PanelSettings();

        [XmlElement("Panel1")]
        public PanelSettings Panel1 {
            get {
                return panel1;
            }
            set {
                panel1 = value;
            }
        }

        [XmlElement("Panel2")]
        public PanelSettings Panel2 {
            get {
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