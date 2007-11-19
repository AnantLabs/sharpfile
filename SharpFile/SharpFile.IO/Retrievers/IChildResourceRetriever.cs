using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public interface IChildResourceRetriever {
		IEnumerable<IChildResource> Get(IView view, IResource resource);
	}
}