using System;
using System.IO;
using SharpFile.Infrastructure.Interfaces;
using Common.Win32;

namespace SharpFile.Infrastructure.IO.ChildResources {
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

            // Calculate the correct size based on the FindData.
            if (findData.SizeLow < 0) {
                size = size + 4294967296;
            }

            if (findData.SizeHigh > 0) {
                size = size + (findData.SizeHigh * 4294967296);
            }
        }

        override protected void getDetails() {
            base.getDetails();
            getSize();
        }

        public DirectoryInfo Directory {
            get {
                if (directory == null) {
                    directory = new DirectoryInfo(DirectoryName);
                }

                return directory;
            }
        }

        public string DirectoryName {
            get {
                if (directoryName == null) {
                    directoryName = this.fullName.Substring(0,
                        this.fullName.LastIndexOf(Common.Path.DirectorySeparator) + 1);
                }

                return directoryName;
            }
        }

        public string Extension {
            get {
                if (extension == null) {
                    //extension = Common.General.GetExtension(FullName);
                    extension = System.IO.Path.GetExtension(FullName);
                }

                return extension;
            }
        }
    }
}