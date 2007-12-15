using System.Collections;
using System.Windows.Forms;

namespace SharpFile.Infrastructure {
    public interface IViewComparer : IComparer {
        Order Order { get; set; }
        int Column { get; set; }
    }

    public enum Order {
        None = 0,
        Ascending = 1,
        Descending = 2
    }
}