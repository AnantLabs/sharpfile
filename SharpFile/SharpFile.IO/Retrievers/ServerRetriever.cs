using System;
using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
	public class ServerRetriever : IResourceRetriever {
        private ChildResourceRetrievers childResourceRetrievers;

        public IEnumerable<IResource> Get() {
			//ServerEnum serverEnum = new ServerEnum(ResourceScope.RESOURCE_CONNECTED,
			//                        ResourceType.RESOURCETYPE_DISK,
			//                        ResourceUsage.RESOURCEUSAGE_ATTACHED,
			//                        ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);

			//foreach (string server in serverEnum) {
			//    yield return new ServerInfo(server);
			//}

            return new List<IResource>();
		}

        public ChildResourceRetrievers ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }
	}
}