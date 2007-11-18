using System;
using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public class NullRetriever : IParentResourceRetriever {
		public IEnumerable<IParentResource> Get() {
			List<IParentResource> resources = new List<IParentResource>();
			resources.Add(new NullInfo());
			return resources;
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				throw new Exception("No child resource specified.");
			}
		}
	}
}
