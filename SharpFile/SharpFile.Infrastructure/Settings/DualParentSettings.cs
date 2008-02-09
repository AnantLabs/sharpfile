using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class DualParentSettings {
        private List<string> panel1Paths;
        private List<string> panel2Paths;
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;
        private bool panel1Collapsed = false;
        private bool panel2Collapsed = false;

        [XmlArray("Panel1Paths")]
        [XmlArrayItem("Path")]
        public List<string> Panel1Paths {
            get {
                return panel1Paths;
            }
            set {
                panel1Paths = value;
            }
        }

        [XmlArray("Panel2Paths")]
        [XmlArrayItem("Path")]
        public List<string> Panel2Paths {
            get {
                return panel2Paths;
            }
            set {
                panel2Paths = value;
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

        public bool Panel1Collapsed {
            get {
                return panel1Collapsed;
            }
            set {
                panel1Collapsed = value;
            }
        }

        public bool Panel2Collapsed {
            get {
                return panel2Collapsed;
            }
            set {
                panel2Collapsed = value;
            }
        }
    }
}