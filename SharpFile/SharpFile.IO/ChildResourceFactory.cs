using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using SharpFile.Infrastructure;

namespace SharpFile.IO {
	public static class ChildResourceFactory {
		/// <summary>
		/// Retrieve the correct IChildResource from the path passed in.
		/// </summary>
		/// <param name="path">Path to retrieve the object for.</param>
		/// <returns>A IChildResource object, or null if it is not valid.</returns>
        public static IChildResource GetChildResource(string path, ChildResourceRetrievers childResourceRetrievers) {
			if (DirectoryInfo.Exists(path)) {
                return new DirectoryInfo(path, childResourceRetrievers);
			} else if (FileInfo.Exists(path)) {
				return new FileInfo(path, childResourceRetrievers);
			}

			return null;
		}
	}
}