using System.Collections;

namespace SharpFile.Infrastructure {
    public class ColumnInfo {
        private string text;
        private string property;
        private IComparer comparer;
        private bool primaryColumn;
        private MyMethod myMethod;

        public delegate string MyMethod(string val);

        public ColumnInfo(string text, string property, IComparer comparer, bool primaryColumn) :
            this(text, property, null, comparer, primaryColumn) {
        }

        public ColumnInfo(string text, string property, MyMethod myMethod, IComparer comparer, bool primaryColumn) {
            this.text = text;
            this.property = property;
            this.comparer = comparer;
            this.primaryColumn = primaryColumn;
            this.myMethod = myMethod;
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

        public MyMethod MethodDelegate {
            get {
                return myMethod;
            }
        }
    }
}