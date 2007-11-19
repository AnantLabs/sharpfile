using System;
using System.Collections.Generic;
using SharpFile.ChildResources.IO;

/*
namespace SharpFile.IO {
	// TODO: Move all of this functionality to the DirectoryInfo object.
	public static class Directory {
		public static bool Exists(string path) {
			return System.IO.Directory.Exists(path);
		}

		public static void Move(DirectoryInfo source, string destination) {
			System.IO.Directory.Move(source.FullPath, destination);
		}

		public static List<FileInfo> GetFiles(DirectoryInfo source) {
			string[] fileStringArray = System.IO.Directory.GetFiles(source.FullPath);
			FileInfo[] fileInfoArray = Array.ConvertAll<string, FileInfo>(fileStringArray, delegate(string fileName) {
				return new FileInfo(fileName);
			});

			return new List<FileInfo>(fileInfoArray);
		}

		public static List<DirectoryInfo> GetDirectories(DirectoryInfo source) {
			string[] directoryStringArray = System.IO.Directory.GetDirectories(source.FullPath);
			DirectoryInfo[] directoryInfoArray = Array.ConvertAll<string, DirectoryInfo>(directoryStringArray, delegate(string directory) {
				return new DirectoryInfo(directory);
			});

			return new List<DirectoryInfo>(directoryInfoArray);
		}

		public static System.IO.DirectoryInfo Create(string path) {
			return System.IO.Directory.CreateDirectory(path);
		}

		public static void Copy(DirectoryInfo source, string destination) {
			if (!destination.EndsWith(@"\")) {
				destination += @"\";
			}

			if (!Exists(destination)) {
				Create(destination);
			}

			foreach (FileInfo fileInfo in GetFiles(source)) {
				fileInfo.Copy(destination + fileInfo.Name);
				//File.Copy(fileInfo, destination + fileInfo.Name, true);
			}

			foreach (DirectoryInfo directory in GetDirectories(source)) {
				string subFolder = directory.Name;
				Create(destination + @"\" + subFolder);
				Copy(directory, destination + @"\" + subFolder);
			}
		}
	}
}
*/