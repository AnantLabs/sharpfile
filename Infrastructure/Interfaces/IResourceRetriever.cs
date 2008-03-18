using System.Collections.Generic;

namespace SharpFile.Infrastructure {
	public interface IResourceRetriever {
        IEnumerable<IChildResource> Get();
        ChildResourceRetrievers ChildResourceRetrievers { get; set; }
        List<IChildResource> DirectoryInfos { get; }
	}
}