using System;
using System.Collections.Generic;
using SharpFile.IO;

namespace SharpFile.Infrastructure {
	public class FileRetriever : IResourceRetriever {
		public IEnumerable<IResource> Get(IResource resource) {
			return Get(resource, string.Empty);
		}

		public IEnumerable<IResource> Get(IResource resource, string filter) {
			DirectoryInfo directoryInfo = resource as DirectoryInfo;
			List<IResource> dataInfos = new List<IResource>();

			// TODO: Setting that specifies whether to show root directory or not.
			/*
			if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
				dataInfos.Add(new RootDirectoryInfo(directoryInfo.Root));
			}
			*/

			// TODO: Setting that specifies whether to show parent directory or not.
			if (directoryInfo.Parent != null) {
				//if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
				dataInfos.Add(new ParentDirectoryInfo(directoryInfo.Parent));
				//}
			}

			dataInfos.AddRange(directoryInfo.GetDirectories());
			dataInfos.AddRange(directoryInfo.GetFiles(filter));

			return dataInfos;
		}
	}
}