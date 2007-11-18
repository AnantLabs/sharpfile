using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.IO {
	public interface IParentResourceRetriever {
		IEnumerable<IParentResource> Get();
		IChildResourceRetriever ChildResourceRetriever { get; }
	}
}