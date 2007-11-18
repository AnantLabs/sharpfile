using System;
using System.IO;
using Common;

namespace SharpFile.IO {
	public class ServerInfo : FileSystemInfo, IParentResource {
		private string label;
		private string format;
		private long availableFreeSpace;
		private DriveType driveType;
		private bool isReady;

		public ServerInfo(string server) {
			this.name = server;
			this.displayName = server;
			this.fullPath = server;
			this.label = server;

			//this.driveType = (DriveType)Enum.Parse(typeof(DriveType), driveInfo.DriveType.ToString());
		}

		public bool IsReady {
			get {
				return true;
			}
		}

		public string Label {
			get {
				return label;
			}
		}

		public DriveType DriveType {
			get {
				throw new Exception("Drive type not defined.");
			}
		}

		public long AvailableFreeSpace {
			get {
				return 0;
			}
		}

		public string Format {
			get {
				throw new Exception("Format not defined.");
			}
		}
	}
}