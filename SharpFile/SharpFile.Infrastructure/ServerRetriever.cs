using System;
using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public class ServerRetriever : IParentResourceRetriever {
		public IEnumerable<IParentResource> Get() {
			ServerEnum serverEnum = new ServerEnum(ResourceScope.RESOURCE_CONNECTED,
									ResourceType.RESOURCETYPE_DISK,
									ResourceUsage.RESOURCEUSAGE_ATTACHED,
									ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);

			foreach (string server in serverEnum) {
				yield return new ServerInfo(server);
			}
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return new FileRetriever();
			}
		}
	}
}