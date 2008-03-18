using System;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class CompressedFileInfo : FileInfo {
        long compressedSize = 0;

        public CompressedFileInfo(string name, long size, long compressedSize, DateTime lastWriteTime, string fullPath, string extension, ChildResourceRetrievers childResourceRetrievers) :
            base(name, size, lastWriteTime, fullPath, extension, childResourceRetrievers) {
            this.compressedSize = compressedSize;
        }

        public long CompressedSize {
            get {
                return compressedSize;
            }
            set {
                compressedSize = value;
            }
        }
    }
}