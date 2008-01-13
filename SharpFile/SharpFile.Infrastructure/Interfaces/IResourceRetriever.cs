using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IResourceRetriever {
        IEnumerable<IResource> Get();
        ChildResourceRetrievers ChildResourceRetrievers { get; set; }
	}
}