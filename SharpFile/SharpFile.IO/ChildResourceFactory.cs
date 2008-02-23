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
        public static IChildResource GetChildResource(string path) {
            string pathDrive = path.Substring(0, path.IndexOf(':')).ToLower();

            // Try to deduce the correct ChildResourceRetrievers to use.
            IResource resource = 
                Settings.Instance.Resources.Find(delegate(IResource r) {
                string resourceDrive = r.FullPath.Substring(0, r.FullPath.IndexOf(':')).ToLower();

                return (pathDrive.Equals(resourceDrive));
            });

            // If the correct ChildResourceRetrievers were found the path is valid, return the correct object. 
            if (resource != null) {
                if (DirectoryInfo.Exists(path)) {
                    return new DirectoryInfo(path, resource.ChildResourceRetrievers);
                } else if (FileInfo.Exists(path)) {
                    return new FileInfo(path, resource.ChildResourceRetrievers);
                }
            }

            return null;
        }

		/// <summary>
		/// Retrieve the correct IChildResource from the path passed in.
		/// </summary>
		/// <param name="path">Path to retrieve the object for.</param>
		/// <returns>A IChildResource object, or null if it is not valid.</returns>
        //public static IChildResource GetChildResource(string path, ChildResourceRetrievers childResourceRetrievers) {
        //    if (DirectoryInfo.Exists(path)) {
        //        return new DirectoryInfo(path, childResourceRetrievers);
        //    } else if (FileInfo.Exists(path)) {
        //        return new FileInfo(path, childResourceRetrievers);
        //    }

        //    return null;
        //}
	}
}