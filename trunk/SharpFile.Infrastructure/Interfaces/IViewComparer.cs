using System.Collections;
using System.Windows.Forms;

namespace SharpFile.Infrastructure.Interfaces {
    public interface IViewComparer : IComparer {
        SortOrder SortOrder { get; set; }
        int ColumnIndex { get; set; }
    }
}