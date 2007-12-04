using System;
using System.Collections.Generic;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
	public class NullRetriever : IParentResourceRetriever {
        public IEnumerable<IResource> Get() {
            List<IResource> resources = new List<IResource>();
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
