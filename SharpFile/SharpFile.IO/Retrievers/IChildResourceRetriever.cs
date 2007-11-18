using System.Collections.Generic;
using SharpFile.IO;
using SharpFile.ChildResources.IO;

namespace SharpFile.IO {
	public interface IChildResourceRetriever {
		IEnumerable<IChildResource> Get(IChildResource resource, string filter);
	}
}