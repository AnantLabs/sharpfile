using System;
using System.Collections.Generic;
using System.IO;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.IO.Retrievers.CompressedRetrievers;

namespace SharpFile.Infrastructure.IO.ChildResources {
    public class CompressedFileInfo : FileInfo {
        long compressedSize = 0;

        public CompressedFileInfo(string fullName, string name, long size, long compressedSize, DateTime lastWriteTime) 
            : base(name, fullName, name, FileAttributes.Normal, size, DateTime.MinValue, DateTime.MinValue, lastWriteTime, 
            null) {
            this.compressedSize = compressedSize;
            this.root = new SharpFile.Infrastructure.IO.ParentResources.DriveInfo(System.IO.Path.GetPathRoot(fullName));
			this.name = name;
			this.extension = Common.General.GetExtension(name);
        }

        public override ChildResourceRetrievers GetChildResourceRetrievers(bool filterByFsi) {
            List<IChildResourceRetriever> compressedRetrievers = base.GetChildResourceRetrievers(false).FindAll(
                delegate(IChildResourceRetriever c) {
                    return (c is ReadOnlyCompressedRetriever || c is ReadWriteCompressedRetriever);
                });

            IChildResourceRetriever compressedRetriever = null;

            foreach (IChildResourceRetriever retriever in compressedRetrievers) {
                if (retriever is ReadWriteCompressedRetriever) {
                    compressedRetriever = retriever;
                    break;
                } else if (retriever is ReadOnlyCompressedRetriever) {
                    compressedRetriever = retriever;
                }
            }

            if (compressedRetriever != null) {
                ChildResourceRetrievers retrievers = new ChildResourceRetrievers(1);
                retrievers.Add(compressedRetriever);
                return retrievers;
            } else {
                throw new Exception("CompressedRetriever could not be found.");
            }
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