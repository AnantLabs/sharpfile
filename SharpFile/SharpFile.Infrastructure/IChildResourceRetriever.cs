using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IChildResourceRetriever {
        string Name { get; set; }
		void Execute(IView view, IResource resource);
        IChildResourceRetriever Clone();
        List<ColumnInfo> ColumnInfos { get; set; }

        void OnGetComplete();
        event ChildResourceRetriever.GetCompleteDelegate GetComplete;

        bool OnCustomMethod(IResource resource);
        event ChildResourceRetriever.CustomMethodDelegate CustomMethod;
	}

    public static class ChildResourceRetriever {
        public delegate void GetCompleteDelegate();
        public delegate bool CustomMethodDelegate(IResource resource);
    }
}