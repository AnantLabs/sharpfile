using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IResource {
		string DisplayName { get; }
		string Name { get; }
		string FullPath { get; }
		long Size { get; }
		IResource Root { get; }
		bool Equals(object obj);
        string Path { get; }

		void Execute(IView view);
		ChildResourceRetrievers ChildResourceRetrievers { get; }
	}
}