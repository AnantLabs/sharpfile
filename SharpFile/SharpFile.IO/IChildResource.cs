using System;

namespace SharpFile.IO {
	public interface IChildResource : IResource {
		DateTime LastWriteTime { get; }
	}
}