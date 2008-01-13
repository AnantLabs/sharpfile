using System.Diagnostics;
using SharpFile.IO;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;
using System.Collections.Generic;

namespace SharpFile.IO.ChildResources {
	public class FileInfo : FileSystemInfo, IChildResource {
        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;

		private string extension;
        private ChildResourceRetrievers childResourceRetrievers;

		public FileInfo(string fileName, ChildResourceRetrievers childResourceRetrievers)
            : this(new System.IO.FileInfo(fileName), childResourceRetrievers) {
		}

        public FileInfo(System.IO.FileInfo fileInfo, ChildResourceRetrievers childResourceRetrievers) {
			this.name = fileInfo.Name;
			this.size = fileInfo.Length;
			this.lastWriteTime = fileInfo.LastWriteTime;
			this.fullPath = fileInfo.FullName;
			this.extension = fileInfo.Extension;
            this.childResourceRetrievers = childResourceRetrievers;
		}

        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
        }

		public string Extension {
			get {
				return extension;
			}
		}

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
	}
}