namespace SharpFile.IO.ChildResources {
    public class ParentDirectoryInfo : DirectoryInfo {
        private const string parentDirectoryName = "..";

        public ParentDirectoryInfo(string path) : base(path) {
            displayName = parentDirectoryName;
        }
    }
}