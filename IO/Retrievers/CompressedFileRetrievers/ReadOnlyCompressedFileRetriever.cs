using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : ChildResourceRetriever {
        protected override IList<IResource> getResources(IResource resource, string filter) {
            List<IResource> resources = new List<IResource>();

            if (Settings.Instance.ShowRootDirectory) {
                resources.Add(new RootDirectoryInfo(resource.Root.Name));
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.Equals(resource.Root.Name))) {
                    resources.Add(new ParentDirectoryInfo(resource.Path));
                }
            }
            
            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;

                        resources.Add(new CompressedFileInfo(fileName, fileName, zipEntry.Size,
                            zipEntry.CompressedSize, zipEntry.DateTime));
                    } else if (zipEntry.IsDirectory) {
                        resources.Add(new CompressedDirectoryInfo(zipEntry.Name, zipEntry.Name,
                            zipEntry.DateTime));
                    }
                }
            }

            return resources;
        }
    }
}