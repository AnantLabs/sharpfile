using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public interface IParentResourceRetriever {
		IEnumerable<IParentResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}