using System;
using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
	public class ServerRetriever : IResourceRetriever {
        private IChildResourceRetriever childResourceRetriever = new FileRetriever();

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

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return childResourceRetriever;
			}
		}
	}
}