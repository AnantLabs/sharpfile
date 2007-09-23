using System;
using SharpFile.FileSystem;

namespace SharpFile {
	public class DriveInfo : FileSystemInfo {
		private string description;
		private string providerName;
		private long freeSpace;
		private DriveType driveType;

		public DriveInfo(string name, string providerName, DriveType driveType, string description,
			long size, long freeSpace) {
			this.name = name;
			this.providerName = providerName;
			this.driveType = driveType;
			this.description = description;
			this.size = size;
			this.freeSpace = freeSpace;
			this.fullPath = name + @"\";
			this.displayName = fullPath + " <" + driveType.ToString() + ">";
		}

		public string ProviderName {
			get {
				return providerName;
			}
		}

		public DriveType DriveType {
			get {
				return driveType;
			}
		}

		public long FreeSpace {
			get {
				return freeSpace;
			}
		}
	}
}