using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DirectoryInfo {
        private const string name = ".";
        private DirectoryInfo directoryInfo;

        public RootDirectoryInfo(string path) {
            directoryInfo = new DirectoryInfo(path);
		}

        /*
        public override void Delete() {
            throw new System.NotImplementedException();
        }

        public override bool Exists {
            get { throw new System.NotImplementedException(); }
        }

        public override string Name {
            get { return name; }
        }

        public override string FullName {
            get {
                return directoryInfo.FullName;
            }
        }

        public DirectoryInfo DirectoryInfo {
            get {
                return directoryInfo;
            }
        }
         */
    }
}