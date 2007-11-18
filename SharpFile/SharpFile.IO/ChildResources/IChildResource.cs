using System;
using SharpFile.IO;

namespace SharpFile.ChildResources.IO {
	public interface IChildResource : IResource {
		DateTime LastWriteTime { get; }
		void Copy(string destination);
		void Move(string destination);
	}
}