using System;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class DualParentSettings {
        private string panel1Path;
        private string panel2Path;
        private int splitterPercentage = 50;

        public string Panel1Path {
            get {
                return panel1Path;
            }
            set {
                panel1Path = value;
            }
        }

        public string Panel2Path {
            get {
                return panel2Path;
            }
            set {
                panel2Path = value;
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
    }
}