using System;

namespace SharpFile.IO {
	public static class File {
		public static bool Exists(string path) {
			return System.IO.File.Exists(path);
		}

		public static void Move(string source, string destination) {
			System.IO.File.Move(source, destination);
		}

		public static void Copy(string source, string destination) {
			Copy(source, destination, false);
		}

		public static void Copy(string source, string destination, bool overwrite) {
			System.IO.File.Copy(source, destination, overwrite);
		}
	}
}
