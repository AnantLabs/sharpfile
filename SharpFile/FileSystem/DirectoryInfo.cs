using System;
using SharpFile.FileSystem;

namespace SharpFile
{
	public class DirectoryInfo : FileSystemInfo
    {
		private System.IO.DirectoryInfo directoryInfo;

        public DirectoryInfo(System.IO.DirectoryInfo directoryInfo)
        {
			this.size = 0;
			this.directoryInfo = directoryInfo;
            this.name = directoryInfo.Name;
            this.lastWriteTime = directoryInfo.LastWriteTime;
			this.fullPath = directoryInfo.FullName;
        }

		public long GetSize() {
			if (size == 0) {
				size = getSize(directoryInfo);
			}

			return size;
		}

        private long getSize(System.IO.DirectoryInfo directoryInfo)
        {
            long totalSize = 0;

            try
            {
                foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    totalSize += fileInfo.Length;
                }

                System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (System.IO.DirectoryInfo subDirectoryInfo in directoryInfos)
                {
                    totalSize += getSize(subDirectoryInfo);
                }
            }
            catch (Exception ex)
            {
            }

            return totalSize;
        }
    }
}