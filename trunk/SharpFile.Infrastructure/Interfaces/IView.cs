using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpFile.Infrastructure.SettingsSection;

namespace SharpFile.Infrastructure.Interfaces {
	public interface IView {
		void AddItemRange(IList<IResource> resources);
        void AddItem(IChildResource resource);
		void RemoveItem(string path);
		void Clear();
		void BeginUpdate();
		void EndUpdate();
        void Invoke(Delegate method);
        void ClearPreviousTopIndexes();
        void UpdateImageIndexes(bool useFileAttributes);
        bool Focus();

		IResource Path { get; }
		string Filter { get; }
        FileSystemWatcher FileSystemWatcher { get; }
		Control Control { get; }
        void ShowMessageBox(string text);
        IViewComparer Comparer { get; set; }
        List<ColumnInfo> ColumnInfos { get; set; }
        string Name { get; set; }
        bool Enabled { get; set; }
        Dictionary<string, ListViewItem> ItemDictionary { get; }
        IResource SelectedResource { get; }
        ResourceContainer SelectedResources { get; }
        long FileCount { get; }
        long FolderCount { get; }

        void OnUpdatePath(IResource path);
        void OnUpdateProgress(int progress);
        int OnGetImageIndex(IResource resource, bool useFileAttributes);
        void OnUpdateStatus(IView view);
        void OnUpdatePluginPanes(IView view);

		event View.GetImageIndexDelegate GetImageIndex;
		event View.UpdateProgressDelegate UpdateProgress;
		event View.UpdateStatusDelegate UpdateStatus;
		event View.UpdatePathDelegate UpdatePath;
        event View.UpdatePluginPanesDelegate UpdatePluginPanes;
        event KeyEventHandler KeyDown;
	}

	public static class View {
        public delegate int GetImageIndexDelegate(IResource fsi, bool useFileAttributes);
		public delegate void UpdateProgressDelegate(int value);
		public delegate void UpdatePathDelegate(IResource path);
		public delegate void UpdateStatusDelegate(IView view);
        public delegate void UpdatePluginPanesDelegate(IView view);
	}
}