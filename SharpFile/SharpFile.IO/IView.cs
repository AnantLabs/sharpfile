using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using System.IO;

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
	}
}