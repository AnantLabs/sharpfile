using System;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure.IO {
    public static class FileSystemInfoFactory {
        /// <summary>
        /// Retrieve the correct FileSystemObject from the path passed in.
        /// </summary>
        /// <param name="path">Path to retrieve the object for.</param>
        /// <returns>A IChildResource object object, or null if it is not valid.</returns>
        public static IResource GetFileSystemInfo(string path) {
            IResource fsi = null;

            if (Settings.Instance.ParentResources.Find(delegate(IParentResource r) {
				return r.Name.Equals(path, StringComparison.OrdinalIgnoreCase);
            }) != null) {
                fsi = new DriveInfo(path);
            } else if (System.IO.Directory.Exists(path)) {
                fsi = new DirectoryInfo(path);
            } else if (System.IO.File.Exists(path)) {
                fsi = new FileInfo(path);
            }

            return fsi;
        }
    }
}