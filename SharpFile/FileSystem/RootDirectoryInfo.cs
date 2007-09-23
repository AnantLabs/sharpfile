using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFile {
	public class RootDirectoryInfo : DirectoryInfo {
		public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo)
			: base(directoryInfo) {
			this.displayName = ".";
		}
	}
}