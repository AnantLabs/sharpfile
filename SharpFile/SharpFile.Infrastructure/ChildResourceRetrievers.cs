using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

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

        public static bool IsCompressedFile(IResource resource) {
            if (resource is IChildResource) {
                if (Common.General.GetExtension(resource.FullPath).ToLower().Equals(".zip")) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCompressedFileThorough(IResource resource) {
            if (resource is IChildResource) {
                try {
                    ZipFile zipFile = new ZipFile(resource.FullPath);
                    return true;
                } catch {
                    // The resource is not a compressed file.
                }
            }

            return false;
        }

        public static bool IsMediaFile(IResource resource) {
            if (resource is IChildResource) {

                if (Common.General.GetExtension(resource.FullPath).ToLower().Equals(".bmp")) {
                    return true;
                }
            }

            return false;
        }
    }
}