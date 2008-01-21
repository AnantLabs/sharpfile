using System.Diagnostics;
using SharpFile.IO;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;
using System.Collections.Generic;
using System;

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

        /*
        public void Copy(string destination) {
            System.IO.File.Copy(this.FullPath, destination, false);
        }

        public void Move(string destination) {
            System.IO.File.Move(this.FullPath, destination);
        }

        public void Execute(IView view) {
            foreach (IChildResourceRetriever childResourceRetriever in childResourceRetrievers.Filter(this)) {
                childResourceRetriever.Execute(view, this);
                break;
            }
        }

        public ChildResourceRetrievers ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
        }

        public static bool Exists(string path) {
            return System.IO.File.Exists(path);
        }
        */
    }
}