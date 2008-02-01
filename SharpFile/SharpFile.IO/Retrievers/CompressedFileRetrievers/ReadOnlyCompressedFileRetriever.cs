using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : CompressedFileRetriever {
        public override IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new ReadOnlyCompressedFileRetriever();
            List<ColumnInfo> clonedColumnInfos = Settings.DeepCopy<List<ColumnInfo>>(ColumnInfos);
            childResourceRetriever.ColumnInfos = clonedColumnInfos;
            childResourceRetriever.Name = Name;
            childResourceRetriever.View = View;
            childResourceRetriever.CustomMethodArguments = CustomMethodArguments;

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.CustomMethodWithArguments += OnCustomMethodWithArguments;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        protected override IEnumerable<IChildResource> getResources(IResource resource, string filter) {
            List<IChildResource> resources = new List<IChildResource>();

            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            childResourceRetrievers.AddRange(Settings.Instance.Resources[0].ChildResourceRetrievers);

            if (Settings.Instance.ShowRootDirectory) {
                resources.Add(new RootDirectoryInfo(new System.IO.DirectoryInfo(resource.Root.FullPath),
                            childResourceRetrievers));
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.ToLower().Equals(resource.Root.Name.ToLower()))) {
                    resources.Add(new ParentDirectoryInfo(new System.IO.DirectoryInfo(resource.Path),
                        childResourceRetrievers));
                }
            }

            using (ZipFile zipFile = new ZipFile(resource.FullPath)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;
                        string extension = Common.General.GetExtension(fileName);

                        resources.Add(new CompressedFileInfo(fileName, zipEntry.Size, zipEntry.CompressedSize,
                            zipEntry.DateTime, fileName, extension, childResourceRetrievers));
                    } else if (zipEntry.IsDirectory) {
                        resources.Add(new CompressedDirectoryInfo(zipEntry.Name, zipEntry.DateTime,
                            zipEntry.Name, childResourceRetrievers));
                    }
                }
            }

            return resources;
        }
    }
}