using System;
using System.IO;
using SharpFile.Infrastructure.Win32;

namespace SharpFile.IO.ChildResources {
    public abstract class FileSystemInfo {
        protected WIN32_FIND_DATA findData;
        protected FileAttributes attributes;
        protected DateTime creationTime;
        protected DateTime lastAccessTime;
        protected DateTime lastWriteTime;
        protected long size;
        protected string name;
        protected string alternateName;
        protected string fullName;

        protected FileSystemInfo(string path) {
            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                path, findData)) {
                getDetails();
            }
        }

        protected FileSystemInfo(WIN32_FIND_DATA findData) {
            this.findData = findData;
            getDetails();
        }

        protected abstract void getSize();
        public abstract void Copy(string destination);

        protected void getDetails() {
            this.fullName = findData.Name;
            this.name = fullName.Remove(0, fullName.IndexOf('\\') + 1);
            this.alternateName = findData.AlternateName.Remove(0, findData.AlternateName.IndexOf('\\') + 1); ;
            this.attributes = findData.Attributes;

            // Get LastWriteTime.
            try {
                long highDateTime = findData.LastWriteTime.dwHighDateTime;
                long lowDateTime = findData.LastWriteTime.dwLowDateTime;
                long fileTime = (highDateTime << 32) + lowDateTime;

                lastWriteTime = DateTime.FromFileTime(fileTime);
            } catch { }

            // Get LastAccessTime.
            try {
                long highDateTime = findData.LastAccessTime.dwHighDateTime;
                long lowDateTime = findData.LastAccessTime.dwLowDateTime;
                long fileTime = (highDateTime << 32) + lowDateTime;

                lastAccessTime = DateTime.FromFileTime(fileTime);
            } catch { }

            // Get CreationTime.
            try {
                long highDateTime = findData.CreationTime.dwHighDateTime;
                long lowDateTime = findData.CreationTime.dwLowDateTime;
                long fileTime = (highDateTime << 32) + lowDateTime;

                creationTime = DateTime.FromFileTime(fileTime);
            } catch { }
        }

        public void Refresh() {
            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                fullName, findData)) {
                getDetails();
            }
        }

        public string FullName {
            get {
                return fullName;
            }
        }

        public FileAttributes Attributes {
            get {
                return attributes;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public DateTime LastWriteTime {
            get {
                return lastWriteTime;
            }
        }

        public DateTime LastAccessTime {
            get {
                return lastAccessTime;
            }
        }

        public DateTime CreationTime {
            get {
                return creationTime;
            }
        }

        public long Size {
            get {
                if (size == 0) {
                    getSize();
                }

                return size;
            }
        }
    }
}