using System;
using System.IO;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Win32;

namespace SharpFile.IO.ChildResources {
    public class FileInfo : FileSystemInfo {
        protected string extension;
        protected DirectoryInfo directory;
        protected string directoryName;

        public FileInfo(string path)
            : base(path) {
        }

        public FileInfo(string path, WIN32_FIND_DATA findData)
            : base(path, findData) {
        }

        public FileInfo(string displayName, string fullName, string alternateName,
            FileAttributes attributes, long size, DateTime creationTime, DateTime lastAccessTime,
            DateTime lastWriteTime, IParentResource root)
            : base(displayName, fullName, alternateName, attributes, size, creationTime, lastAccessTime,
            lastWriteTime, root) {
        }

        /// <summary>
        /// Copies the file system object to the destination.
        /// </summary>
        /// <param name="fsi">File system object to copy.</param>
        /// <param name="destination">Destination to copy the file system object to.</param>
        public override void Copy(string destination, bool overwrite) {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullName);
            fileInfo.CopyTo(destination, false);
        }

        public void Copy(string destination) {
            Copy(destination, false);
        }

        /// <summary>
        /// Moves the file system object to the destination.
        /// </summary>
        /// <param name="destination">Destination to move the file system object to.</param>
        public override void Move(string destination) {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullName);
            fileInfo.MoveTo(destination);
        }

        protected override void getSize() {
            size = findData.SizeLow;

            if (findData.SizeLow < 0) {
                size = size + 4294967296;
            }

            if (findData.SizeHigh > 0) {
                size = size + (findData.SizeHigh * 4294967296);
            }
        }

        private void getDirectoryName() {
            directoryName = this.fullName.Substring(0, this.fullName.LastIndexOf('\\') + 1);
        }

        private void getDirectory() {
            directory = new DirectoryInfo(DirectoryName);
        }

        public DirectoryInfo Directory {
            get {
                if (directory == null) {
                    getDirectory();
                }

                return directory;
            }
        }

        public string DirectoryName {
            get {
                if (directory == null) {
                    getDirectoryName();
                }

                return directoryName;
            }
        }

        public string Extension {
            get {
                if (string.IsNullOrEmpty(extension)) {
                    extension = Common.General.GetExtension(FullName);
                }

                return extension;
            }
        }
    }
}