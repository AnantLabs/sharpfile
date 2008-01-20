using System.Collections.Generic;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrievers : List<IChildResourceRetriever> {
        public ChildResourceRetrievers()
            : base() {
        }

        public ChildResourceRetrievers(int capacity)
            : base(capacity) {
        }

        public IEnumerable<IChildResourceRetriever> Filter(IResource resource) {
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                if (childResourceRetriever.OnCustomMethod(resource)) {
                    yield return childResourceRetriever;
                }
            }
        }

        public ChildResourceRetrievers Clone() {
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers(this.Count);

            foreach (IChildResourceRetriever childResourceRetriever in this) {
                childResourceRetrievers.Add(childResourceRetriever.Clone());
            }

            return childResourceRetrievers;
        }

        public static bool DefaultCustomMethod(IResource resource) {
            return true;
        }

        public static bool IsProgrammingDirectory(IResource resource) {
            if (resource.Name.Equals("Programming")) {
                return true;
            }

            return false;
        }

        public static bool IsCompressedFile(IResource resource) {
            if (resource is IChildResource) {
                try {
                    ICSharpCode.SharpZipLib.Zip.ZipFile f = new ICSharpCode.SharpZipLib.Zip.ZipFile(resource.FullPath);

                    return true;
                } catch { 
                    // The resource is not a compressed file.
                }

                /*
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(resource.FullPath);

                if (fileInfo.Extension.ToLower().Equals(".zip")) {
                    ICSharpCode.SharpZipLib.Zip.ZipFile f = new ICSharpCode.SharpZipLib.Zip.ZipFile(resource.FullPath);
                    if (f.Count > 0) {
                        return true;
                    }
                }
                */
            }

            return false;
        }
    }
}