using System.Collections.Generic;
using SharpFile.IO;
using SharpFile.ParentResources.IO;

namespace SharpFile.IO {
	public interface IParentResourceRetriever {
		IEnumerable<IParentResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}