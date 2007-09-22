using System;

namespace SharpFile
{
    public class DirectoryInfo : DataInfo
    {
        public DirectoryInfo(System.IO.DirectoryInfo directoryInfo)
        {
            this.name = directoryInfo.Name;
            this.size = 0;
            this.lastWriteTime = directoryInfo.LastWriteTime;
			this.fullPath = directoryInfo.FullName;
        }

        public long GetSize(System.IO.DirectoryInfo directoryInfo)
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
                    totalSize += GetSize(subDirectoryInfo);
                }
            }
            catch (Exception ex)
            {
            }

            return totalSize;
        }
    }
}