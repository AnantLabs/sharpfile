using System.Collections.Generic;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
	public interface IParentResourceRetriever {
        IEnumerable<IResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}