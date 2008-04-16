namespace SharpFile.IO.ChildResources {
	public class RootDirectoryInfo : DirectoryInfo {
        private const string rootDirectoryName = ".";

        public RootDirectoryInfo(string fullName)
            : base(fullName) {
            name = Path;
            displayName = rootDirectoryName;
		}
    }
}