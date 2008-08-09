using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {    
    public sealed class DualParent {
        public const string DefaultDrive = @"c:\";

        private string selectedPath;
        private string selectedPath1;
        private string selectedPath2;
        private string selectedFile;
        private string selectedFile1;
        private string selectedFile2;
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;        
        private Pane pane1 = new Pane();
        private Pane pane2 = new Pane();
        private List<Tool> tools = new List<Tool>();

        public static List<Tool> GenerateDefaultTools() {
            List<Tool> tools = new List<Tool>();
            Key key = new Key(new List<Keys>(), Keys.F4);
            tools.Add(new Tool("Command Prompt", "cmd", "/K cd \"{SelectedPath}\"", key));
            return tools;
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

        [XmlElement("Pane1")]
        public Pane Pane1 {
            get {
                if (pane1.Paths.Count == 0) {
                    pane1.Paths.Add(DefaultDrive);
                }

                return pane1;
            }
            set {
                pane1 = value;
            }
        }

        [XmlElement("Pane2")]
        public Pane Pane2 {
            get {
                if (pane2.Paths.Count == 0) {
                    pane2.Paths.Add(DefaultDrive);
                }

                return pane2;
            }
            set {
                pane2 = value;
            }
        }

        [XmlIgnore]
        public string SelectedPath {
            get {
                return selectedPath;
            }
            set {
                selectedPath = value;
            }
        }

        [XmlIgnore]
        public string SelectedPath1 {
            get {
                return selectedPath1;
            }
            set {
                selectedPath1 = value;
            }
        }

        [XmlIgnore]
        public string SelectedPath2 {
            get {
                return selectedPath2;
            }
            set {
                selectedPath2 = value;
            }
        }

        [XmlIgnore]
        public string SelectedFile {
            get {
                return selectedFile;
            }
            set {
                selectedFile = value;
            }
        }

        [XmlIgnore]
        public string SelectedFile1 {
            get {
                return selectedFile1;
            }
            set {
                selectedFile1 = value;
            }
        }

        [XmlIgnore]
        public string SelectedFile2 {
            get {
                return selectedFile2;
            }
            set {
                selectedFile2 = value;
            }
        }
    }
}