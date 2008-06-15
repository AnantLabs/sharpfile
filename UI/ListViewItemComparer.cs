using System.Collections;
using System.Windows.Forms;
using Common.Comparers;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;

namespace SharpFile.UI {
    public class ListViewItemComparer : IViewComparer {
        private int columnIndex;
        private SortOrder order;
        private IComparer comparer;
        private bool directoriesSortedFirst = true;

        public ListViewItemComparer() {
            columnIndex = 0;
            directoriesSortedFirst = Settings.Instance.DirectoriesSortedFirst;

            // Initialize the comparer.
            comparer = new StringLogicalComparer();
        }

        /// <summary>
        /// Compares two ListViewItems.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Comparison of the first object versus the second.</returns>
        public int Compare(object x, object y) {
            int compareResult = 0;
            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;
            IResource resourceX = (IResource)itemX.Tag;
            IResource resourceY = (IResource)itemY.Tag;
            string valueX = (string)itemX.SubItems[columnIndex].Tag;
            string valueY = (string)itemY.SubItems[columnIndex].Tag;

            if (directoriesSortedFirst) {
                if (resourceX is RootDirectoryInfo || resourceX is DriveInfo) {
                    compareResult = 0;
                } else if (resourceX is ParentDirectoryInfo) {
                    compareResult = 0;
                } else
                    if (resourceX is DirectoryInfo && resourceY is DirectoryInfo) {
                        compareResult = comparer.Compare(
                            valueX,
                            valueY);
                    } else if (!(resourceX is DirectoryInfo) && !(resourceY is DirectoryInfo)) {
                        compareResult = comparer.Compare(
                            valueX,
                            valueY);
                    } else if (resourceX is DirectoryInfo && resourceY is FileInfo) {
                        compareResult = 0;
                    } else if (resourceY is DirectoryInfo && resourceX is FileInfo) {
                        compareResult = 1;
                    } else {
                        compareResult = 0;
                    }
            } else {
                if (resourceX is RootDirectoryInfo || resourceX is DriveInfo) {
                    compareResult = 0;
                } else if (resourceX is ParentDirectoryInfo) {
                    compareResult = 0;
                } else {
                    compareResult = comparer.Compare(
                            valueX,
                            valueY);
                }
            }

            // Calculate correct return value based on object comparison.
            if (order == SortOrder.Ascending) {
                // Ascending sort is selected, return normal result of compare operation.
                return compareResult;
            } else if (order == SortOrder.Descending) {
                // Descending sort is selected, return negative result of compare operation.
                return (-compareResult);
            } else {
                return 0;
            }
        }

        /// <summary>
        /// Index of the column.
        /// </summary>
        public int ColumnIndex {
            get {
                return columnIndex;
            }
            set {
                columnIndex = value;
            }
        }

        /// <summary>
        /// Order of sorting.
        /// </summary>
        public SortOrder SortOrder {
            get {
                return order;
            }
            set {
                order = value;
            }
        }
    }
}