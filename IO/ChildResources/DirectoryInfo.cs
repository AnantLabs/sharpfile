using SharpFile.IO.ChildResources;
using SharpFile.Infrastructure.Win32;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ChildResources {
    public class DirectoryInfo : FileSystemInfo, IChildResource {
        private DirectoryInfo root;
        private DirectoryInfo parent;

        public DirectoryInfo(string path)
            : base(path) {
        }

        public DirectoryInfo(WIN32_FIND_DATA findData)
            : base(findData) {
        }

        public override void getSize() {
            size = 0;
        }

        private void getRoot() {
            WIN32_FIND_DATA rootFindData = new WIN32_FIND_DATA();
            string rootPath = this.fullName.Substring(0, this.fullName.IndexOf('\\') + 1);

            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                rootPath, rootFindData)) {
                root = new DirectoryInfo(rootFindData);
            }
        }

        private void getParent() {
            WIN32_FIND_DATA parentFindData = new WIN32_FIND_DATA();
            string parentPath = this.fullName.Substring(0, this.fullName.LastIndexOf('\\') + 1);

            using (SafeFindHandle handle = NativeMethods.FindFirstFile(
                parentPath, parentFindData)) {
                parent = new DirectoryInfo(parentFindData);
            }
        }

        public DirectoryInfo Parent {
            get {
                if (parent == null) {
                    getParent();
                }

                return parent;
            }
        }

        public DirectoryInfo Root {
            get {
                if (root == null) {
                    getRoot();
                }

                return root;
            }
        }
    }
}