using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public interface IParentResourceRetriever {
		IEnumerable<IParentResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}