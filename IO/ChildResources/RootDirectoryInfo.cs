namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DirectoryInfo {
        private const string rootDirectoryName = ".";

        public RootDirectoryInfo(string path) : base(path) {
            name = rootDirectoryName;
            displayName = rootDirectoryName;
		}
    }
}