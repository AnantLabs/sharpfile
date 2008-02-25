using System.IO;

namespace SharpFile.IO {
    public static class FileSystemInfoFactory {
        /// <summary>
        /// Retrieve the correct FileSystemObject from the path passed in.
        /// </summary>
        /// <param name="path">Path to retrieve the object for.</param>
        /// <returns>A FileSystemInfo object object, or null if it is not valid.</returns>
        public static FileSystemInfo GetFileSystemInfo(string path) {
            FileSystemInfo fsi = null;

            if (Directory.Exists(path)) {
                fsi = new DirectoryInfo(path);
            } else if (File.Exists(path)) {
                fsi = new FileInfo(path);
            }

            return fsi;
        }
    }
}