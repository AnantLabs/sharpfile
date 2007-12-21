using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IChildResourceRetriever {
		void Get(IView view, IResource resource);
        void Cancel();
        List<ColumnInfo> ColumnInfos { get; set; }
	}
}