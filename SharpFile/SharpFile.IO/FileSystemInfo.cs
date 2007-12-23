using System;
using SharpFile.IO.ParentResources;
using SharpFile.IO.ChildResources;
using SharpFile.Infrastructure;

namespace SharpFile.IO {
    [Serializable]
	public abstract class FileSystemInfo {
		protected string displayName;
		protected string name;
		protected string fullPath;
		protected long size;
		protected IResource root;
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

		public IResource Root {
			get {
				if (root == null) {
                    string rootString = string.Empty;
                    IResource resource = null;

                    if (FullPath.Length >= 3) {
                        rootString = FullPath.Substring(0, 3);

                        resource = Settings.Instance.Resources.Find(delegate(IResource r) {
                            return r.FullPath.ToLower().Equals(rootString.ToLower());
                        });
                    }

                    if (resource != null) {
                        root = new DriveInfo(new System.IO.DriveInfo(rootString.Substring(0, 1)), resource.ChildResourceRetriever);
                    } else {
                        throw new Exception("Root cannot be found.");
                    }
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