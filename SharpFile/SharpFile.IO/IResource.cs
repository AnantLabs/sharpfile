using System;

namespace SharpFile.IO {
	public interface IResource {
        string DisplayName { get; }
        string Name { get; }
        string FullPath { get; }
        long Size { get; }
        DriveInfo Root { get; }
        DateTime LastWriteTime { get; }
        bool Equals(object obj);
	}
}
