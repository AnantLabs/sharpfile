using System;
using System.Collections.Generic;

namespace SharpFile.IO {
	public class DirectoryInfo : FileSystemInfo, IChildResource {
		private System.IO.DirectoryInfo directoryInfo;

		public DirectoryInfo(string path)
			: this(new System.IO.DirectoryInfo(path)) {
		}

		public DirectoryInfo(System.IO.DirectoryInfo directoryInfo) {
			this.size = 0;
			this.directoryInfo = directoryInfo;
			this.name = directoryInfo.Name;
			this.lastWriteTime = directoryInfo.LastWriteTime;
			this.fullPath = directoryInfo.FullName;
		}

		public System.IO.DirectoryInfo Parent {
			get {
				if (directoryInfo.Parent != null) {
					return directoryInfo.Parent;
				} else {
					return null;
				}
			}
		}

		public IEnumerable<IChildResource> GetDirectories() {
			System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

			return Array.ConvertAll<System.IO.DirectoryInfo, DirectoryInfo>(directoryInfos,
				delegate(System.IO.DirectoryInfo di) {
					return new DirectoryInfo(di);
				});
		}

		public IEnumerable<IChildResource> GetFiles(string filter) {
			System.IO.FileInfo[] fileInfos = null;

			if (string.IsNullOrEmpty(filter)) {
				fileInfos = directoryInfo.GetFiles();
			} else {
				fileInfos = directoryInfo.GetFiles("*" + filter + "*", 
					System.IO.SearchOption.TopDirectoryOnly);
			}

			return Array.ConvertAll<System.IO.FileInfo, FileInfo>(fileInfos,
				delegate(System.IO.FileInfo fi) {
					return new FileInfo(fi);
				});
		}

		public long GetSize() {
			if (size == 0) {
				size = getSize(directoryInfo);
			}

			return size;
		}

		public void Copy(string destination) {
			Directory.Copy(this, destination);
		}

		public void Move(string destination) {
			Directory.Move(this, destination);
		}

		private long getSize(System.IO.DirectoryInfo directoryInfo) {
			long totalSize = 0;

			try {
				foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles()) {
					totalSize += fileInfo.Length;
				}

				System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
				foreach (System.IO.DirectoryInfo subDirectoryInfo in directoryInfos) {
					totalSize += getSize(subDirectoryInfo);
				}
			} catch (Exception ex) {
			}

			return totalSize;
		}
	}
}