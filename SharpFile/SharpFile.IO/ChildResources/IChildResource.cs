using System;
using SharpFile.IO;

namespace SharpFile.IO.ChildResources {
	public interface IChildResource : IResource {
		DateTime LastWriteTime { get; }
		void Copy(string destination);
		void Move(string destination);
	}
}