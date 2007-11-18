using System;
using SharpFile.ParentResources.IO;

namespace SharpFile.IO {
	public interface IResource {
		string DisplayName { get; }
		string Name { get; }
		string FullPath { get; }
		long Size { get; }
		DriveInfo Root { get; }
		bool Equals(object obj);
	}
}
