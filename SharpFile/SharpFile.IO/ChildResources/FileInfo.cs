using System.Diagnostics;
using SharpFile.IO;
using SharpFile.IO.Retrievers;

namespace SharpFile.IO.ChildResources {
	public class FileInfo : FileSystemInfo, IChildResource {
		private string extension;

		public FileInfo(string fileName)
			: this(new System.IO.FileInfo(fileName)) {
		}

		public FileInfo(System.IO.FileInfo fileInfo) {
			this.name = fileInfo.Name;
			this.size = fileInfo.Length;
			this.lastWriteTime = fileInfo.LastWriteTime;
			this.fullPath = fileInfo.FullName;
			this.extension = fileInfo.Extension;
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
			Process.Start(this.FullPath);
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return null;
			}
		}

		public static bool Exists(string path) {
			return System.IO.File.Exists(path);
		}
	}
}