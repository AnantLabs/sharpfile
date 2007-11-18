using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.IO {
	public interface IChildResourceRetriever {
		IEnumerable<IChildResource> Get(IChildResource resource, string filter);
	}
}