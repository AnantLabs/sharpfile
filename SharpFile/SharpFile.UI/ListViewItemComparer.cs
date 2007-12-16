using System;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
    // This class is an implementation of the 'IComparer' interface.
    public class ListViewItemComparer : IViewComparer {
        private ColumnHeader column;
        private int columnIndex;
        private Order order;
        private IComparer comparer;
        private bool directoriesSortedFirst = true;

        public ListViewItemComparer() {
            if (column == null) {
                columnIndex = 0;
            } else {
                columnIndex = column.Index;
            }

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
            ListViewItem itemX, itemY;
            IChildResource resourceX, resourceY;

            // Cast the objects to be compared to ListViewItem objects.
            itemX = (ListViewItem)x;
            itemY = (ListViewItem)y;

            resourceX = (IChildResource)itemX.Tag;
            resourceY = (IChildResource)itemY.Tag;

            if (directoriesSortedFirst) {
                if (resourceX is DirectoryInfo && resourceY is DirectoryInfo) {
                    compareResult = comparer.Compare(
                        itemX.SubItems[columnIndex].Text,
                        itemY.SubItems[columnIndex].Text);
                } else if (resourceX is DirectoryInfo || resourceY is DirectoryInfo) {
                    compareResult = 0;
                } else {
                    compareResult = comparer.Compare(
                        itemX.SubItems[columnIndex].Text,
                        itemY.SubItems[columnIndex].Text);
                }
            } else {
                compareResult = comparer.Compare(
                        itemX.SubItems[columnIndex].Text,
                        itemY.SubItems[columnIndex].Text);
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
        public ColumnHeader Column {
            get {
                return column;
            }
            set {
                column = value;
                columnIndex = column.Index;
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