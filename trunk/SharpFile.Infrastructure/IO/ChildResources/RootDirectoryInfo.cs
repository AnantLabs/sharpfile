using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure.IO.ChildResources {
    public class RootDirectoryInfo : DriveInfo {
        private const string rootDirectoryName = ".";

        public RootDirectoryInfo(string fullName)
            : base(fullName) {
            name = Path;
            displayName = rootDirectoryName;
        }
    }
}