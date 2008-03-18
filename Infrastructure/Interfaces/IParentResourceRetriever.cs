using System.Collections.Generic;
using System.IO;

namespace SharpFile.Infrastructure {
	public interface IParentResourceRetriever {
        IEnumerable<IParentResource> Get();
        ChildResourceRetrievers ChildResourceRetrievers { get; set; }
        List<IParentResource> ParentResources { get; }
	}
}