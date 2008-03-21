using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using Common.Logger;
using System.ComponentModel;
using System;
//using System.IO;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : ChildResourceRetriever {
        protected override IEnumerable<IResource> getResources(IResource resource, string filter) {
            List<IResource> resources = new List<IResource>();

            //ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            //childResourceRetrievers.AddRange(Settings.Instance.ParentResources[0].GetChildResourceRetrievers());

			// TODO: Want to include support for this. Will probably have to add Path to IResource.
            /*
            if (Settings.Instance.ShowRootDirectory) {
                resources.Add(resource.Root);
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
                        !resource.Path.Equals(resource.Root.Name))) {
                    resources.Add(resource);
                }
            }
             */

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;
                        string extension = Common.General.GetExtension(fileName);

                        resources.Add(new CompressedFileInfo(fileName, fileName, zipEntry.Size, zipEntry.CompressedSize,
                            zipEntry.DateTime));
                    } else if (zipEntry.IsDirectory) {
                        resources.Add(new CompressedDirectoryInfo(zipEntry.Name, zipEntry.Name, zipEntry.DateTime));
                    }
                }
            }

            return resources;
        }
    }
}