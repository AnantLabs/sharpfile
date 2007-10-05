using System;

namespace SharpFile.IO {
	public abstract class FileSystemInfo {
		protected string displayName;
		protected string name;
		protected string fullPath;
		protected long size;
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

		public string Path {
			get {
				return fullPath.Replace(name, string.Empty);
			}
		}

		public override bool Equals(object obj) {
			FileSystemInfo fileSystemInfo = obj as FileSystemInfo;

			if (fileSystemInfo != null) {
				if (this.fullPath == fileSystemInfo.fullPath) {
					return true;
				} else {
					return false;
				}
			}

			return base.Equals(obj);
		}
	}
}
