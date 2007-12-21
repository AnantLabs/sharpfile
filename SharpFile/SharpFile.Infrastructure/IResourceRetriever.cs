using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IResourceRetriever {
        IEnumerable<IResource> Get();
        IChildResourceRetriever ChildResourceRetriever { get; set; }
	}
}