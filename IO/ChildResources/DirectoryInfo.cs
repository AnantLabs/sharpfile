using System;
using System.Collections.Generic;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Win32;
using System.Diagnostics;
using Common.Logger;

namespace SharpFile.IO.ChildResources {
    public class DirectoryInfo : FileSystemInfo, IResourceGetter {
        public DirectoryInfo(string path)
            : base(path) {
        }

        public DirectoryInfo(string path, WIN32_FIND_DATA findData)
            : base(path, findData) {
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
            if (!destination.EndsWith(@"\")) {
                destination += @"\";
            }

            // TODO: Do something with the overwrite bool here.

            // Create the destination if necessary.
            if (!System.IO.Directory.Exists(destination)) {
                System.IO.Directory.CreateDirectory(destination);
            }

            // Get the files and directories to the destination.
            FileSystemEnumerator enumerator = new FileSystemEnumerator(this.fullName);
            foreach (IChildResource resource in enumerator.Matches()) {
                resource.Copy(destination + resource.Name, false);
            }

            /*
            // Copy the files to the destination.
            foreach (FileInfo fileInfo in GetFiles()) {
                fileInfo.Copy(destination + fileInfo.Name, false);
            }

            // Get the directories and recursively copy them over as well.
            foreach (DirectoryInfo di in GetDirectories()) {
                di.Copy(destination + @"\" + di.Name);
            }
            */
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

        public IEnumerable<IChildResource> GetChildResources() {
            foreach (IChildResource resource in GetDirectories()) {
                yield return resource;
            }

            foreach (IChildResource resource in GetFiles()) {
                yield return resource;
            }
        }

        public IEnumerable<IChildResource> GetDirectories() {
            // TODO: Encapsulate the decision to show the root/parent director in a static method somewhere.
            // Show root directory if specified.
            if (Settings.Instance.ShowRootDirectory) {
				if (!this.Root.FullName.Equals(this.FullName, StringComparison.OrdinalIgnoreCase)) {
                    yield return new RootDirectoryInfo(this.Root.FullName);
                }
            }

            // Show parent directory if specified.
            if (Settings.Instance.ShowParentDirectory) {
				if (!this.Parent.FullName.Equals(this.FullName, StringComparison.OrdinalIgnoreCase)) {
                    if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
						!this.Parent.FullName.Equals(this.Root.FullName, StringComparison.OrdinalIgnoreCase))) {
                        yield return new ParentDirectoryInfo(this.Parent.FullName);
                    }
                }
            }

            // Grab only the directories from the enumerator.
            FileSystemEnumerator enumerator = new FileSystemEnumerator(this.FullName);
            foreach (IChildResource resource in enumerator.Matches()) {
                if (resource is DirectoryInfo) {
                    yield return resource;
                }
            }
        }

        public IEnumerable<IChildResource> GetFiles() {
            return GetFiles("*");
        }

        public IEnumerable<IChildResource> GetFiles(string filter) {
            // Grab only the directories from the enumerator.
            FileSystemEnumerator enumerator = new FileSystemEnumerator(fullName, filter);
            foreach (IChildResource resource in enumerator.Matches()) {
                if (resource is FileInfo) {
                    yield return resource;
                }
            }
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

            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
                totalSize += fileInfo.Size;
            }

            foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories()) {
                if (!(childDirectoryInfo is ParentDirectoryInfo) 
                    && !(childDirectoryInfo is RootDirectoryInfo) 
                    && (System.IO.FileAttributes.ReparsePoint & childDirectoryInfo.Attributes) != System.IO.FileAttributes.ReparsePoint) {
                    totalSize += getDirectorySize(childDirectoryInfo);
                }
            }

            return totalSize;
        }
    }
}