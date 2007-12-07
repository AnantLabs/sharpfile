using SharpFile.IO.ParentResources;
using SharpFile.IO.Retrievers;

namespace SharpFile.IO {
	public interface IResource {
		string DisplayName { get; }
		string Name { get; }
		string FullPath { get; }
		long Size { get; }
		DriveInfo Root { get; }
		bool Equals(object obj);
        string Path { get; }

		void Execute(IView view);
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}