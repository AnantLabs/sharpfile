using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IChildResourceRetriever {
		void Get(IView view, IResource resource);
        void GetComplete();
        IChildResourceRetriever Clone();
        List<ColumnInfo> ColumnInfos { get; set; }

        event ChildResourceRetriever.OnGetCompleteDelegate OnGetComplete;
	}

    public static class ChildResourceRetriever {
        public delegate void OnGetCompleteDelegate();
    }
}