using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : ChildResourceRetriever {
        protected override IEnumerable<IResource> getResources(IResource resource, string filter) {
            if (Settings.Instance.ShowRootDirectory) {
                yield return new RootDirectoryInfo(resource.Root.Name);
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.Equals(resource.Root.Name))) {
                    yield return new ParentDirectoryInfo(resource.Path);
                }
            }

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;
                        string extension = Common.General.GetExtension(fileName);

                        yield return new CompressedFileInfo(fileName, fileName, zipEntry.Size,
                            zipEntry.CompressedSize, zipEntry.DateTime);
                    } else if (zipEntry.IsDirectory) {
                        yield return new CompressedDirectoryInfo(zipEntry.Name, zipEntry.Name,
                            zipEntry.DateTime);
                    }
                }
            }
        }
    }
}