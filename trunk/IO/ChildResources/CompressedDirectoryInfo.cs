using System;
using System.IO;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class CompressedDirectoryInfo : DirectoryInfo {
        public CompressedDirectoryInfo(string fullName, string name, DateTime lastWriteTime)
            : base(name, fullName, name, FileAttributes.Normal, 0, DateTime.MinValue, DateTime.MinValue,
            lastWriteTime, null) {
        }
    }
}