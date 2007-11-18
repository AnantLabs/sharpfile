using System;
using System.Collections.Generic;
using SharpFile.IO;
using SharpFile.ChildResources.IO;

namespace SharpFile.Retrievers.IO {
	public class FileRetriever : IChildResourceRetriever {
		public IEnumerable<IChildResource> Get(IChildResource resource, string filter) {
			DirectoryInfo directoryInfo = resource as DirectoryInfo;
			List<IChildResource> dataInfos = new List<IChildResource>();

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