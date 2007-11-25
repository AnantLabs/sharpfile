using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using System.IO;
using System;

namespace SharpFile.IO {
	// TODO: This might be better somewhere else. Maybe there should be an assembly just for interfaces.
	public interface IView {
		void UpdateView(IEnumerable<IChildResource> childResources);
		void UpdatePath(string path);
		void UpdateProgress(int progress);
		void ClearView();
		void BeginUpdate();
		void EndUpdate();
		string Path { get; }
		string Filter { get; }
		FileSystemWatcher FileSystemWatcher { get; }

		event View.OnGetImageIndexDelegate OnGetImageIndex;
		event View.OnUpdateProgressDelegate OnUpdateProgress;
		event View.OnUpdateStatusDelegate OnUpdateStatus;
		event View.OnUpdatePathDelegate OnUpdatePath;
	}

	public static class View {
		public delegate int OnGetImageIndexDelegate(IResource fsi);
		public delegate void OnUpdateProgressDelegate(int value);
		public delegate void OnUpdatePathDelegate(string path);
		public delegate void OnUpdateStatusDelegate(string status);
	}
}