using System;

namespace SharpFile.IO {
	public static class Directory {
		public static bool Exists(string path) {
			return System.IO.Directory.Exists(path);
		}
	}
}
