namespace SharpFile.IO {
	public static class Directory {
		public static bool Exists(string path) {
			return System.IO.Directory.Exists(path);
		}

		public static void Move(string source, string destination) {
			System.IO.Directory.Move(source, destination);
		}
	}
}
