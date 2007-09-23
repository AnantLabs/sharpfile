namespace SharpFile.FileSystem {
	public static class FileSystemInfoFactory {
		public static FileSystemInfo GetFileSystemInfo(string path) {
			if (System.IO.Directory.Exists(path)) {
				return new DirectoryInfo(path);
			} else if (System.IO.File.Exists(path)) {
				return new FileInfo(path);
			}

			return null;
		}
	}
}
