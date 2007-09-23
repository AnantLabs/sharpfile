using System;
using System.Drawing;

namespace SharpFile.FileSystem
{
    public abstract class FileSystemInfo
    {
		protected string displayName;
        protected string name;
		protected string fullPath;
        protected long size;
        protected DateTime lastWriteTime;

		public string DisplayName {
			get {
				if (string.IsNullOrEmpty(displayName)) {
					return name;
				} else {
					return displayName;
				}
			}
		}

        public string Name
        {
            get
            {
                return name;
            }
        }

        public long Size
        {
            get
            {
                return size;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return lastWriteTime;
            }
        }

		public string FullPath {
			get {
				return fullPath;
			}
		}
    }
}
