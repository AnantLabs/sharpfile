using System;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class CompressedDirectoryInfo : DirectoryInfo {
        public CompressedDirectoryInfo(string name, DateTime lastWriteTime, string fullPath,
            ChildResourceRetrievers childResourceRetrievers) :
            base(name, lastWriteTime, fullPath, null, childResourceRetrievers) {
        }
    }
}