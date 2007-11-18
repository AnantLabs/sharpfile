using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public interface IChildResourceRetriever {
		IEnumerable<IChildResource> Get(IChildResource resource, string filter);
	}
}