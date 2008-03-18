using System.Collections.Generic;
using System.IO;

namespace SharpFile.Infrastructure {
	public interface IChildResourceRetriever {
        string Name { get; set; }
		void Execute(IView view, IResource resource);
        IChildResourceRetriever Clone();
        List<ColumnInfo> ColumnInfos { get; set; }
        IView View { get; set; }
        List<string> CustomMethodArguments { get; set; }

        void OnGetComplete();
        event ChildResourceRetriever.GetCompleteDelegate GetComplete;

        bool OnCustomMethod(IResource resource);
        event ChildResourceRetriever.CustomMethodDelegate CustomMethod;

        bool OnCustomMethodWithArguments(IResource resource, List<string> arguments);
        event ChildResourceRetriever.CustomMethodWithArgumentsDelegate CustomMethodWithArguments;
	}

    public static class ChildResourceRetriever {
        public delegate void GetCompleteDelegate();
        public delegate bool CustomMethodDelegate(IResource resource);
        public delegate bool CustomMethodWithArgumentsDelegate(IResource resource, List<string> arguments);
    }
}