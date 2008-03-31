using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using System;

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

                    if (zipEntryName.EndsWith(@"\")) {
                        zipEntryName = zipEntryName.Remove(zipEntryName.Length - 1, 1);
                    }

                    string[] directoryLevels = zipEntryName.Split('\\');

                    // Only show the current directories filesystem objects.
                    if (directoryLevels.Length == 1) {
                        string fullName = string.Format(@"{0}\{1}",
                                resource.FullName,
                                zipEntryName);

                        if (zipEntry.IsFile) {
                            childResources.Add(new CompressedFileInfo(fullName, zipEntryName, zipEntry.Size,
                                zipEntry.CompressedSize, zipEntry.DateTime));
                        } else if (zipEntry.IsDirectory) {
                            childResources.Add(new CompressedDirectoryInfo(fullName, zipEntryName,
                                    zipEntry.DateTime));
                        }
                    } else if (directoryLevels.Length > 1) {
                        // Derive the folder if a file is nested deep.
                        string directoryName = directoryLevels[0];
                        string fullName = string.Format(@"{0}\{1}",
                               resource.FullName,
                               directoryName);                        

                        if (childResources.Find(delegate(IChildResource c) {
                            return c.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase);
                        }) == null) {
                            childResources.Add(new CompressedDirectoryInfo(fullName, directoryName,
                                    DateTime.MinValue));
                        }
                    }
                }
            }

            return childResources;
        }
    }
}