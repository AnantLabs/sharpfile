using System.Collections.Generic;
using SharpFile.Infrastructure.SettingsSection;

namespace SharpFile.Infrastructure.Interfaces {
	public interface IChildResourceRetriever {
        string Name { get; set; }
		void Execute(IView view, IResource resource);
        IChildResourceRetriever Clone();
        List<ColumnInfo> ColumnInfos { get; set; }
        IView View { get; set; }
        List<string> FilterMethodArguments { get; set; }

        void OnGetComplete();
        event ChildResourceRetriever.GetCompleteDelegate GetComplete;

        bool OnFilterMethod(IResource resource);
        event ChildResourceRetriever.FilterMethodDelegate FilterMethod;

        bool OnFilterMethodWithArguments(IResource resource, List<string> arguments);
        event ChildResourceRetriever.FilterMethodWithArgumentsDelegate FilterMethodWithArguments;
	}

    public static class ChildResourceRetriever {
        public delegate void GetCompleteDelegate();
        public delegate bool FilterMethodDelegate(IResource resource);
        public delegate bool FilterMethodWithArgumentsDelegate(IResource resource, List<string> arguments);
    }
}