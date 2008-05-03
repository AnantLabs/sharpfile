using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SharpFile.Infrastructure.SettingsSection {    
    public sealed class DualParent {
        private const string defaultDrive = @"c:\";
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;        
        private Panel panel1 = new Panel();
        private Panel panel2 = new Panel();
        private List<Tool> tools = new List<Tool>();

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

        [XmlArray("Tools")]
        [XmlArrayItem("Tool")]
        public List<Tool> Tools {
            get {
                return tools;
            }
            set {
                tools = value;
            }
        }

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

        public static List<Tool> GenerateDefaultTools() {
            List<Tool> tools = new List<Tool>();
            tools.Add(new Tool("Command Prompt", "cmd", "/K cd {SelectedPath}"));
            return tools;
        }
    }
}