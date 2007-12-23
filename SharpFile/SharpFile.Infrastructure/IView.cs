using System.Windows.Forms;
using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	// TODO: This might be better somewhere else. Maybe there should be an assembly just for interfaces.
	public interface IView {
		void AddItemRange(IEnumerable<IChildResource> childResources);
		void InsertItem(IChildResource childResource);
		void RemoveItem(string path);
		void UpdatePath(string path);
		void UpdateProgress(int progress);
		void ClearView();
		void BeginUpdate();
		void EndUpdate();
		string Path { get; }
		string Filter { get; }
        FileSystemWatcher FileSystemWatcher { get; }
		Control Control { get; }
        void CancelChildRetrieverOperations();
        void ShowMessageBox(string text);
        IViewComparer Comparer { get; }
        IEnumerable<ColumnInfo> ColumnInfos { get; set; }
        string Name { get; set; }

		event View.OnGetImageIndexDelegate OnGetImageIndex;
		event View.OnUpdateProgressDelegate OnUpdateProgress;
		event View.OnUpdateStatusDelegate OnUpdateStatus;
		event View.OnUpdatePathDelegate OnUpdatePath;
        event View.OnCancelOperationsDelegate OnCancelOperations;
	}

	public static class View {
		public delegate int OnGetImageIndexDelegate(IResource fsi);
		public delegate void OnUpdateProgressDelegate(int value);
		public delegate void OnUpdatePathDelegate(string path);
		public delegate void OnUpdateStatusDelegate(string status);
        public delegate void OnCancelOperationsDelegate();
	}
}