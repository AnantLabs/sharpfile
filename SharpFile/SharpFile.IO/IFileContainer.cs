using System.Collections.Generic;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO {
	public interface IFileContainer {
		IEnumerable<IChildResource> GetFiles();
		IEnumerable<IChildResource> GetFiles(string filter);
		IEnumerable<IChildResource> GetDirectories();
	}
}