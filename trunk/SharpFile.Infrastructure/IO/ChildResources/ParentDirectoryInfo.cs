namespace SharpFile.Infrastructure.IO.ChildResources {
    public class ParentDirectoryInfo : DirectoryInfo {
        private const string parentDirectoryName = "..";

        public ParentDirectoryInfo(string fullName)
            : base(fullName) {
            displayName = parentDirectoryName;
        }
    }
}