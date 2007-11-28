using System;
using SharpFile.IO.ParentResources;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO {
	public abstract class FileSystemInfo {
		protected string displayName;
		protected string name;
		protected string fullPath;
		protected long size;
		protected DriveInfo root;
		protected DateTime lastWriteTime;

		public string DisplayName {
			get {
				if (string.IsNullOrEmpty(displayName)) {
					return name;
				} else {
					return displayName;
				}
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public long Size {
			get {
				return size;
			}
		}

		public DateTime LastWriteTime {
			get {
				return lastWriteTime;
			}
		}

		public string FullPath {
			get {
				return fullPath;
			}
		}

		public DriveInfo Root {
			get {
				if (root == null) {
					string rootString = fullPath.Substring(0, FullPath.IndexOf(":"));
					root = new DriveInfo(new System.IO.DriveInfo(rootString));					
				}

				return root;
			}
		}

		public string Path {
			get {
				if (!fullPath.Equals(name)) {
					return fullPath.Replace(name, string.Empty);
				} else {
					return fullPath;
				}
			}
		}

		/*
		public override int GetHashCode() {
			// TODO: Convert the path to an int.
			return this.fullPath;
		}
		*/

		public override bool Equals(object obj) {
			IChildResource fileSystemInfo = obj as IChildResource;

			if (fileSystemInfo != null) {
				if (this.fullPath == fileSystemInfo.FullPath) {
					return true;
				} else {
					return false;
				}
			}

			return base.Equals(obj);
		}
	}
}