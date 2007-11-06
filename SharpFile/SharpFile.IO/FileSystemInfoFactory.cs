namespace SharpFile.IO {
	public static class FileSystemInfoFactory {
		/// <summary>
		/// Retrieve the correct FileSystemInfo-derived object from the path passed in.
		/// </summary>
		/// <param name="path">Path to retrieve the object for.</param>
		/// <returns>A FileSystemInfo-derived object, or null if it is not valid.</returns>
		public static FileSystemInfo GetFileSystemInfo(string path) {
			if (Directory.Exists(path)) {
				return new DirectoryInfo(path);
			} else if (File.Exists(path)) {
				return new FileInfo(path);
			} else if (driveExists(path)) {
				return new DriveInfo(new System.IO.DriveInfo(path));
			}

			return null;
		}

		/// <summary>
		/// Checks to see if the FileSystemInfo exists.
		/// </summary>
		/// <param name="fsi">FileSystemInfo object.</param>
		/// <returns>Whether or not the path exists.</returns>
		public static bool Exists(FileSystemInfo fsi) {
			if (fsi is DirectoryInfo) {
				return Directory.Exists(fsi.FullPath);
			} else if (fsi is FileInfo) {
				return File.Exists(fsi.FullPath);
			} else {
				throw new System.Exception("Type is not valid: " + fsi.GetType());
			}
		}

		/// <summary>
		/// Copies the FileSystemInfo object to the destination.
		/// </summary>
		/// <param name="fsi">FileSystemInfo object.</param>
		/// <param name="destination">The full destination path for the file or directory.</param>
		public static void Copy(FileSystemInfo fsi, string destination) {
			if (fsi is DirectoryInfo) {
				Directory.Copy(fsi.FullPath, destination);
			} else if (fsi is FileInfo) {
				File.Copy(fsi.FullPath, destination, false);
			} else {
				throw new System.Exception("Type is not valid: " + fsi.GetType());
			}
		}

		/// <summary>
		/// Moves the FileSystemInfo object to the destination.
		/// </summary>
		/// <param name="fsi">FileSystemInfo object.</param>
		/// <param name="destination">The full destination path for the file or directory.</param>
		public static void Move(FileSystemInfo fsi, string destination) {
			if (fsi is DirectoryInfo) {
				Directory.Move(fsi.FullPath, destination);
			} else if (fsi is FileInfo) {
				File.Move(fsi.FullPath, destination);
			} else {
				throw new System.Exception("Type is not valid: " + fsi.GetType());
			}
		}

		/// <summary>
		/// Determines if the drive exists.
		/// </summary>
		/// <param name="path">Path to check.</param>
		/// <returns>True/False.</returns>
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