using System.Collections;
using System.Windows.Forms;

namespace SharpFile.Infrastructure {
    public interface IViewComparer : IComparer {
        SortOrder SortOrder { get; set; }
        int ColumnIndex { get; set; }
    }
}