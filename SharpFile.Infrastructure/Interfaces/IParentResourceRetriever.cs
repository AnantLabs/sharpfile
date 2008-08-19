using System.Collections.Generic;

namespace SharpFile.Infrastructure.Interfaces {
	public interface IParentResourceRetriever {
        IEnumerable<IParentResource> Get();
        ChildResourceRetrievers ChildResourceRetrievers { get; set; }
        List<IParentResource> ParentResources { get; }
	}
}