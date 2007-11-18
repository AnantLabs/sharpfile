using System;
using Common;
using SharpFile.IO;

namespace SharpFile.ParentResources.IO {
	public class DriveInfo :FileSystemInfo, IParentResource {
		private string label;
		private string format;
		private long availableFreeSpace;
		private DriveType driveType;
		private bool isReady;

		public DriveInfo(System.IO.DriveInfo driveInfo) {
			this.name = driveInfo.Name;
			this.driveType = (DriveType)Enum.Parse(typeof(DriveType), driveInfo.DriveType.ToString());
			this.fullPath = name;
			this.isReady = driveInfo.IsReady;

			if (driveInfo.IsReady) {
				this.label = driveInfo.VolumeLabel;
				this.format = driveInfo.DriveFormat;
				this.size = driveInfo.TotalSize;
				this.availableFreeSpace = driveInfo.AvailableFreeSpace;
			}

			this.displayName = string.Format("{0} <{1}>",
				fullPath,
				Common.General.GetHumanReadableSize(availableFreeSpace.ToString())); //string.Format("{0:0,0}", availableFreeSpace));
		}

		public bool IsReady {
			get {
				return isReady;
			}
		}

		public string Label {
			get {
				return label;
			}
		}

		public DriveType DriveType {
			get {
				return driveType;
			}
		}

		public long AvailableFreeSpace {
			get {
				return availableFreeSpace;
			}
		}

		public string Format {
			get {
				return format;
			}
		}
	}
}