using System;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
    // This class is an implementation of the 'IComparer' interface.
    public class ListViewItemComparer : IViewComparer {
        private int columnIndex;
        private Order order;
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
            int compareResult;

            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;
            IChildResource resourceX = (IChildResource)itemX.Tag;
            IChildResource resourceY = (IChildResource)itemY.Tag;
            string valueX = (string)itemX.SubItems[columnIndex].Tag;
            string valueY = (string)itemY.SubItems[columnIndex].Tag;

            if (directoriesSortedFirst) {
                if (resourceX is DirectoryInfo && resourceY is DirectoryInfo) {
                    compareResult = comparer.Compare(
                        valueX,
                        valueY);
                } else if (resourceX is DirectoryInfo || resourceY is DirectoryInfo) {
                    compareResult = 0;
                } else {
                    compareResult = comparer.Compare(
                        valueX,
                        valueY);
                }
            } else {
                compareResult = comparer.Compare(
                        valueX,
                        valueY);
            }

            // Calculate correct return value based on object comparison.
            if (order == Order.Ascending) {
                // Ascending sort is selected, return normal result of compare operation.
                return compareResult;
            } else if (order == Order.Descending) {
                // Descending sort is selected, return negative result of compare operation.
                return (-compareResult);
            } else {
                return 0;
            }
        }

        // Gets or sets the number of the column to which to
        // apply the sorting operation (Defaults to '0').
        public int ColumnIndex {
            get {
                return columnIndex;
            }
            set {
                columnIndex = value;
            }
        }

        // Gets or sets the order of sorting to apply
        // (for example, 'Ascending' or 'Descending').
        public Order Order {
            get {
                return order;
            }
            set {
                order = value;
            }
        }
    }
}