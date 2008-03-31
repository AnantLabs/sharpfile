using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.Retrievers.CompressedRetrievers {
    public class ReadOnlyCompressedRetriever : ChildResourceRetriever {
        protected override IList<IChildResource> getResources(IResource resource, string filter) {
            List<IChildResource> childResources = new List<IChildResource>();

            if (Settings.Instance.ShowRootDirectory) {
                childResources.Add(new RootDirectoryInfo(resource.Root.Name));
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.Equals(resource.Root.Name))) {
                    childResources.Add(new ParentDirectoryInfo(resource.Path));
                }
            }

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    string zipEntryName = zipEntry.Name.Replace("/", @"\");

                    if (zipEntry.IsFile) {
                        //if (zipEntryName.LastIndexOf(@"\") < 1) {
                        string name = zipEntry.Name.Remove(0, zipEntry.Name.LastIndexOf(@"\") + 1);
                        string fullName = string.Format(@"{0}\{1}",
                            resource.FullName,
                            zipEntryName);

                        //if (string.IsNullOrEmpty(resource.FullName)) {
                        childResources.Add(new CompressedFileInfo(zipEntryName, zipEntryName, zipEntry.Size,
                            zipEntry.CompressedSize, zipEntry.DateTime));
                        //}
                    } else if (zipEntry.IsDirectory) {
                        string directoryName = zipEntryName.Remove(0, zipEntryName.LastIndexOf(@"\"));

                        //if (zipEntryName.LastIndexOf(@"\") < 1) {
                        childResources.Add(new CompressedDirectoryInfo(zipEntryName, zipEntryName,
                            zipEntry.DateTime));
                    }
                }
            }

            return childResources;
        }
    }
}