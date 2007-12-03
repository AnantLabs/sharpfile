using System;
using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public class ServerRetriever : IParentResourceRetriever {
        private IChildResourceRetriever childResourceRetriever = new FileRetriever();

		public IEnumerable<IParentResource> Get() {
			//ServerEnum serverEnum = new ServerEnum(ResourceScope.RESOURCE_CONNECTED,
			//                        ResourceType.RESOURCETYPE_DISK,
			//                        ResourceUsage.RESOURCEUSAGE_ATTACHED,
			//                        ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);

			//foreach (string server in serverEnum) {
			//    yield return new ServerInfo(server);
			//}

			return new List<IParentResource>();
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return childResourceRetriever;
			}
		}
	}
}