using System.Collections;

namespace SharpFile.Infrastructure {
    public class ColumnInfo {
        private string text;
        private string property;
        private IComparer comparer;
        private bool primaryColumn;
        private CustomMethod customMethod;

        public delegate string CustomMethod(string val);

        public ColumnInfo(string text, string property, IComparer comparer)
            : this(text, property, null, comparer, false) {
        }

        public ColumnInfo(string text, string property, IComparer comparer, bool primaryColumn)
            : this(text, property, null, comparer, primaryColumn) {
        }

        public ColumnInfo(string text, string property, CustomMethod customMethod, IComparer comparer)
            : this(text, property, customMethod, comparer, false) {
        }

        public ColumnInfo(string text, string property, CustomMethod customMethod, IComparer comparer, bool primaryColumn) {
            this.text = text;
            this.property = property;
            this.comparer = comparer;
            this.primaryColumn = primaryColumn;
            this.customMethod = customMethod;
        }

        public string Text {
            get {
                return text;
            }
        }

        public string Property {
            get {
                return property;
            }
        }

        public IComparer Comparer {
            get {
                return comparer;
            }
        }

        public bool PrimaryColumn {
            get {
                return primaryColumn;
            }
        }

        public CustomMethod MethodDelegate {
            get {
                return customMethod;
            }
        }
    }
}