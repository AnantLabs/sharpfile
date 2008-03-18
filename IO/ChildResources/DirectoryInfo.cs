using System;
using System.Collections.Generic;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Win32;

namespace SharpFile.IO.ChildResources {
    public class DirectoryInfo : FileSystemInfo, IResourceGetter {
        private IResource parent;

        public DirectoryInfo(string path)
            : base(path) {
        }

        public DirectoryInfo(string path, WIN32_FIND_DATA findData)
            : base(path, findData) {
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
            // Show root directory if specified.
            if (Settings.Instance.ShowRootDirectory) {
                if (!this.Root.Name.Equals(this.FullName)) {
                    yield return new RootDirectoryInfo(this.Root.Name);
                }
            }

            // Show parent directory if specified.
            if (Settings.Instance.ShowParentDirectory) {
                if (this.Parent != null) {
                    if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
                        !this.Parent.Name.Equals(this.Root.Name))) {
                        yield return new ParentDirectoryInfo(this.Parent.FullName);
                    }
                }
            }

            // Grab only the directories from the enumerator.
            FileSystemEnumerator enumerator = new FileSystemEnumerator(fullName);
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
            size = 0;
        }

        private void getParent() {
            try {
                // Make sure that the full name has an ending slash.
                string correctFullName = this.fullName;

                if (!correctFullName.EndsWith("\\")) {
                    correctFullName += "\\";
                }

                string[] paths = correctFullName.Split('\\');

                // Resize the array to chop off the last 2 directories 
                // where the last is an empty string because of the test above.
                Array.Resize<string>(ref paths, paths.Length - 2);

                string parentPath = string.Join("\\", paths);

                if (!parentPath.EndsWith("\\")) {
                    parentPath += "\\";
                }

                parent = new DirectoryInfo(parentPath);
            } catch (Exception ex) {
                // TODO: Parent is invalid.
            }
        }

        public IResource Parent {
            get {
                if (parent == null) {
                    getParent();
                }

                return parent;
            }
        }
    }
}