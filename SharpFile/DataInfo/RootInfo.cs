using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFile {
	public class RootInfo : DirectoryInfo {
		public RootInfo(System.IO.DirectoryInfo directoryInfo)
			: base(directoryInfo) {
			this.displayName = ".";
		}
	}
}