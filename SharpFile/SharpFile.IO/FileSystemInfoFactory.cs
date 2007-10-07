namespace SharpFile.IO {
	public static class FileSystemInfoFactory {
		public static FileSystemInfo GetFileSystemInfo(string path) {
			if (System.IO.Directory.Exists(path)) {
				return new DirectoryInfo(path);
			} else if (System.IO.File.Exists(path)) {
				return new FileInfo(path);
			} else if (driveExists(path)) {
				return new DriveInfo(new System.IO.DriveInfo(path));
			}

			return null;
		}

		private static bool driveExists(string path) {
			foreach (System.IO.DriveInfo driveInfo in System.IO.DriveInfo.GetDrives()) {
				if (driveInfo.IsReady &&
					driveInfo.RootDirectory.FullName.Equals(path)) {
					return true;
				}
			}

			return false;
		}
	}
}
