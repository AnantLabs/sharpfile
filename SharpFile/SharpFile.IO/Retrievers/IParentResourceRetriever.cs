using System.Collections.Generic;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
	public interface IParentResourceRetriever {
		IEnumerable<IParentResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}