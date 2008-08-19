using System;
using System.Diagnostics;
using Common.Logger;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.WindowsApi;

namespace SharpFile.Infrastructure.IO.ChildResources {
    public class DirectoryInfo : FileSystemInfo {
        public DirectoryInfo(string fullName)
            : base(fullName) {

            if (fullName != null && name != null) {
                // When the path is null, try to determine it by using the FullName and the Name.
                // This happens for ParentDirectories when starting SharpFile with a path at least 2 directoies deep.
                if (path == null) {
                    int lengthOfPath = (fullName.Length - name.Length - 1);

                    if (lengthOfPath > -1) {
                        path = fullName.Substring(0, lengthOfPath);
                        path = Common.Path.Cleanup(path);
                    }
                } else if (path.EndsWith(name, StringComparison.InvariantCultureIgnoreCase)) {
                    // When the path ends with the name, try to determine the correct path.
                    int lengthOfPath = (fullName.Length - name.Length - 1);

                    if (lengthOfPath > -1) {
                        path = path.Substring(0, lengthOfPath);
                        path = Common.Path.Cleanup(path);
                    }
                }
            }
        }

        public DirectoryInfo(string fullName, WIN32_FIND_DATA findData)
            : base(fullName, findData) {
        }

        public DirectoryInfo(string displayName, string fullName, string alternateName,
            System.IO.FileAttributes attributes, long size, DateTime creationTime, DateTime lastAccessTime,
            DateTime lastWriteTime, IParentResource root)
            : base(displayName, fullName, alternateName, attributes, size, creationTime, lastAccessTime, 
            lastWriteTime, root) {
        }

        /// <summary>
        /// Copies the directory to the destination.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to copy.</param>
        /// <param name="destination">Destination to copy to.</param>
        public void Copy(string destination) {
            Copy(destination, false);
        }

        /// <summary>
        /// Copies the directory to the destination.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to copy.</param>
        /// <param name="destination">Destination to copy to.</param>
        public override void Copy(string destination, bool overwrite) {
            // Make sure the destination is correct.
            destination = Common.Path.Cleanup(destination);

            // Create the destination if necessary.
            if (!System.IO.Directory.Exists(destination)) {
                System.IO.Directory.CreateDirectory(destination);
            }

            // Get the files and directories to the destination.
            FileSystemEnumerator enumerator = new FileSystemEnumerator(this.fullName);
            foreach (IChildResource resource in enumerator.Matches()) {
                resource.Copy(destination + resource.Name, overwrite);
            }
        }

        /// <summary>
        /// Moves the file system object to the destination.
        /// </summary>
        /// <param name="fsi">File system object to move.</param>
        /// <param name="destination">Destination to move the file system object to.</param>
        public override void Move(string destination) {
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(fullName);
            directoryInfo.MoveTo(destination);
        }

        protected override void getSize() {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            size = getDirectorySize(this);

            Settings.Instance.Logger.Log(LogLevelType.Verbose, "Finished retrieving size for {0} took {1} ms.",
                                    this.FullName,
                                    sw.ElapsedMilliseconds.ToString());
            sw.Reset();
        }

        /// <summary>
        /// Recursive calculation of the total size of the directory.
        /// </summary>
        /// <param name="directoryInfo">Directory to calculate the total size.</param>
        /// <returns>Total size of the directory.</returns>
        private long getDirectorySize(DirectoryInfo directoryInfo) {
            long totalSize = 0;
			FileSystemEnumerator filesystemEnumerator = new FileSystemEnumerator(directoryInfo.FullName);

			foreach (IChildResource childResource in filesystemEnumerator.Matches()) {
				if (childResource is FileInfo) {
					totalSize += ((FileInfo)childResource).Size;
				} else if (childResource is DirectoryInfo) {
					DirectoryInfo childDirectoryInfo = (DirectoryInfo)childResource;

					if (!(childDirectoryInfo is ParentDirectoryInfo)
					&& (System.IO.FileAttributes.ReparsePoint & childDirectoryInfo.Attributes) != System.IO.FileAttributes.ReparsePoint) {
						totalSize += getDirectorySize(childDirectoryInfo);
					}
				}
			}

            return totalSize;
        }
    }
}