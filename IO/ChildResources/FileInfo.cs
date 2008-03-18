using System;
using System.Runtime.InteropServices;
using SharpFile.Infrastructure.Win32;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class FileInfo : FileSystemInfo, IChildResource {
        protected string extension = string.Empty;
        protected DirectoryInfo directory;
        protected string directoryName;

        public FileInfo(string path)
            : base(path) {
        }

        public FileInfo(WIN32_FIND_DATA findData)
            : base(findData) {

            if (name.IndexOf('.') > 0) {
                this.extension = name.Remove(0, name.LastIndexOf('.'));
            }
        }

        /// <summary>
        /// Copies the directory to the destination.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo to copy.</param>
        /// <param name="destination">Destination to copy to.</param>
        public void Copy(string destination) {
            // Make sure the destination is correct.
            if (!destination.EndsWith(@"\")) {
                destination += @"\";
            }

            // Create the destination if necessary.
            if (!Directory.Exists(destination)) {
                Directory.CreateDirectory(destination);
            }

            // Copy the files to the destination.
            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) {
                fileInfo.CopyTo(destination + fileInfo.Name, false);
            }

            // Get the directories and recursively copy them over as well.
            foreach (DirectoryInfo di in directoryInfo.GetDirectories()) {
                di.ExtCopy(destination + @"\" + di.Name);
            }
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
            WIN32_FIND_DATA directoryFindData = new WIN32_FIND_DATA();

            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                DirectoryName, directoryFindData)) {
                directory = new DirectoryInfo(directoryFindData);
            }
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
                return extension;
            }
        }
    }
}