using System.Collections.Generic;
using System.IO;

namespace SharpFile.Infrastructure {
	public interface IResourceRetriever {
        IEnumerable<DirectoryInfo> Get();
        ChildResourceRetrievers ChildResourceRetrievers { get; set; }
        List<DirectoryInfo> DirectoryInfos { get; }
	}
}