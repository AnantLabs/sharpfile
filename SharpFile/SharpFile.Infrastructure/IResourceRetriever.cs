using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public interface IResourceRetriever {
		IEnumerable<IResource> Get(IResource resource);
		IEnumerable<IResource> Get(IResource resource, string filter);
	}
}