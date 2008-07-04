using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

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
        private Panel panel1 = new Panel();
        private Panel panel2 = new Panel();
        private List<Tool> tools = new List<Tool>();
        private List<PluginPanel> pluginPanels = new List<PluginPanel>();

        public static List<Tool> GenerateDefaultTools() {
            List<Tool> tools = new List<Tool>();
            Key key = new Key(new List<Keys>(), Keys.F4);
            tools.Add(new Tool("Command Prompt", "cmd", "/K cd \"{SelectedPath}\"", key));
            return tools;
        }

        public static List<PluginPanel> GenerateDefaultPluginPanels() {
            List<PluginPanel> pluginPanels = new List<PluginPanel>();

            pluginPanels.Add(new PluginPanel("PreviewPanel",
                new FullyQualifiedType("SharpFile", "SharpFile.UI.PreviewPanel"),
                new FullyQualifiedType("SharpFile", "SharpFile.Infrastructure.SettingsSection.PreviewPanel")));

            return pluginPanels;
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

        [XmlElement("Panel1")]
        public Panel Panel1 {
            get {
                if (panel1.Paths.Count == 0) {
                    panel1.Paths.Add(DefaultDrive);
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
                    panel2.Paths.Add(DefaultDrive);
                }

                return panel2;
            }
            set {
                panel2 = value;
            }
        }

        [XmlArray("PluginPanels")]
        [XmlArrayItem("PluginPanel")]
        public List<PluginPanel> PluginPanels {
            get {
                return pluginPanels;
            }
            set {
                pluginPanels = value;
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