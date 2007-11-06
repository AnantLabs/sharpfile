namespace SharpFile.IO {
	public static class Directory {
		public static bool Exists(string path) {
			return System.IO.Directory.Exists(path);
		}

		public static void Move(string source, string destination) {
			System.IO.Directory.Move(source, destination);
		}

		public static void Copy(string source, string destination) {
			throw new System.Exception("Copying a directory is not available, yet.");

			/*
			 if (!dest.EndsWith("\\")) dest += "\\";

foreach (string file in Directory.GetFiles( source ))
{
File.Copy(file, dest + Path.GetFileName(file), true);
}

foreach (string folder in Directory.GetDirectories( source ))
{
string subFolder = Path.GetFileName(folder);
Directory.CreateDirectory(dest + "\\" + subFolder);
CopyFolder(folder, dest + "\\" + subFolder);
}
			 */
		}
	}
}
