using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpFile.Infrastructure {
	public interface IView {
		void AddItemRange(IEnumerable<IChildResource> childResources);
		void InsertItem(IChildResource childResource);
		void RemoveItem(string path);
		void OnUpdatePath(string path);
		void OnUpdateProgress(int progress);
		void Clear();
		void BeginUpdate();
		void EndUpdate();
		string Path { get; }
		string Filter { get; }
        FileSystemWatcher FileSystemWatcher { get; }
		Control Control { get; }
        void ShowMessageBox(string text);
        IViewComparer Comparer { get; set; }
        IEnumerable<ColumnInfo> ColumnInfos { get; set; }
        string Name { get; set; }
        bool Enabled { get; set; }

		event View.GetImageIndexDelegate GetImageIndex;
		event View.UpdateProgressDelegate UpdateProgress;
		event View.UpdateStatusDelegate UpdateStatus;
		event View.UpdatePathDelegate UpdatePath;
	}

	public static class View {
		public delegate int GetImageIndexDelegate(IResource fsi);
		public delegate void UpdateProgressDelegate(int value);
		public delegate void UpdatePathDelegate(string path);
		public delegate void UpdateStatusDelegate(string status);
	}
}