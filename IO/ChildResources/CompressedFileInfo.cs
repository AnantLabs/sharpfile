﻿using System;
using System.IO;

namespace SharpFile.IO.ChildResources {
    public class CompressedFileInfo : FileInfo {
        long compressedSize = 0;

        public CompressedFileInfo(string fullName, string name, long size, long compressedSize, DateTime lastWriteTime) 
            : base(name, fullName, name, FileAttributes.Normal, size, DateTime.MinValue, DateTime.MinValue, lastWriteTime, 
            null) {
            this.compressedSize = compressedSize;
			this.root = new SharpFile.IO.ParentResources.DriveInfo(@"c:\");
			this.name = name;
			this.extension = Common.General.GetExtension(name);
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