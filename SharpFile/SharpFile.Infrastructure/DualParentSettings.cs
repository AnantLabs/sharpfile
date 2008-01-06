using System;

namespace SharpFile.Infrastructure {
    [Serializable]
    public sealed class DualParentSettings {
        private string leftPath;
        private string rightPath;
        private int splitterPercentage = 50;

        public string LeftPath {
            get {
                return leftPath;
            }
            set {
                leftPath = value;
            }
        }

        public string RightPath {
            get {
                return rightPath;
            }
            set {
                rightPath = value;
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