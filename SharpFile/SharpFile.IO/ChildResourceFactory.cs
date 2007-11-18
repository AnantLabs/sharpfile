using System;
using SharpFile.ChildResources.IO;

namespace SharpFile.IO {
	public static class ChildResourceFactory {
		/// <summary>
		/// Retrieve the correct IChildResource from the path passed in.
		/// </summary>
		/// <param name="path">Path to retrieve the object for.</param>
		/// <returns>A IChildResource object, or null if it is not valid.</returns>
		public static IChildResource GetChildResource(string path) {
			if (System.IO.Directory.Exists(path)) {
				return new DirectoryInfo(path);
			} else if (System.IO.File.Exists(path)) {
				return new FileInfo(path);
			}

			return null;
		}
	}
}