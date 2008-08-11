using System;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure.IO {
    public static class FileSystemInfoFactory {
        /// <summary>
        /// Retrieve the correct FileSystemObject from the path passed in.
        /// </summary>
		/// <param name="fullName">Path to retrieve the object for.</param>
        /// <returns>A IChildResource object object, or null if it is not valid.</returns>
        public static IResource GetFileSystemInfo(string fullName) {
            IResource resource = null;

            if (Settings.Instance.ParentResources.Find(delegate(IParentResource r) {
				return r.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase);
            }) != null) {
				resource = new DriveInfo(fullName);
			} else if (System.IO.Directory.Exists(fullName)) {
				resource = new DirectoryInfo(fullName);
			} else if (System.IO.File.Exists(fullName)) {
				resource = new FileInfo(fullName);
            }

			return resource;
        }
    }
}