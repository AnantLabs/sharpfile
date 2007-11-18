using System;

namespace SharpFile.IO {
	public interface IChildResource : IResource {
		DateTime LastWriteTime { get; }
		void Copy(string destination);
		void Move(string destination);
	}
}