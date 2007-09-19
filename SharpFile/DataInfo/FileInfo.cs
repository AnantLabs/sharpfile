using System;

namespace SharpFile
{
    public class FileInfo : DataInfo
    {
        public FileInfo(System.IO.FileInfo fileInfo)
        {
            this.name = fileInfo.Name;
            this.size = fileInfo.Length;
            this.lastWriteTime = fileInfo.LastWriteTime;
        }
    }
}
