namespace SharpFile.IO {
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
	}
}