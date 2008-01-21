using System;
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

        public FileInfo(System.IO.FileInfo fileInfo, ChildResourceRetrievers childResourceRetrievers) :
            this(fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime, fileInfo.FullName,
            fileInfo.Extension, childResourceRetrievers) {
		}

        public FileInfo(string name, long size, DateTime lastWriteTime, string fullPath, string extension, ChildResourceRetrievers childResourceRetrievers) {
            this.name = name;
            this.size = size;
            this.lastWriteTime = lastWriteTime;
            this.fullPath = fullPath;
            this.extension = extension;
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
            List<IChildResourceRetriever> childResourceRetrievers = 
                (List<IChildResourceRetriever>)ChildResourceRetrievers.Filter(this);

            if (childResourceRetrievers.Count > 0) {
                IChildResourceRetriever childResourceRetriever = childResourceRetrievers[0];

                if (!view.GetType().Equals(childResourceRetriever.View.GetType())) {
                    view = childResourceRetriever.View;
                }

                childResourceRetrievers[0].Execute(view, this);
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