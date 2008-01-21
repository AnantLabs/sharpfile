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

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        protected override IEnumerable<IChildResource> getResources(IResource resource, string filter) {
            List<IChildResource> resources = new List<IChildResource>();

            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            childResourceRetrievers.Add(this);

            byte[] data = new byte[4096];

            using (ZipInputStream s = new ZipInputStream(System.IO.File.OpenRead(resource.FullPath))) {
                resources.Add(new ParentDirectoryInfo(new System.IO.DirectoryInfo(resource.Path),
                    Settings.Instance.Resources[0].ChildResourceRetrievers));

                ZipEntry zipEntry;

                while ((zipEntry = s.GetNextEntry()) != null) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;
                        string extension = Common.General.GetExtension(fileName);

                        resources.Add(new CompressedFileInfo(fileName, zipEntry.Size, zipEntry.CompressedSize,
                            zipEntry.DateTime, fileName, extension, childResourceRetrievers));

                        /*
                        // Assuming the contents are text may be ok depending on what you are doing
                        // here its fine as its shows how data can be read from a Zip archive.
                        Console.Write("Show entry text (y/n) ?");

                        if (Console.ReadLine() == "y") {
                            int size = s.Read(data, 0, data.Length);
                            while (size > 0) {
                                Console.Write(System.Text.Encoding.ASCII.GetString(data, 0, size));
                                size = s.Read(data, 0, data.Length);
                            }
                        }
                        */
                    } else if (zipEntry.IsDirectory) {
                        resources.Add(new CompressedDirectoryInfo(zipEntry.Name, zipEntry.Size, zipEntry.CompressedSize,
                            zipEntry.DateTime, zipEntry.Name, childResourceRetrievers));
                    }
                }

                // Close can be ommitted as the using statement will do it automatically
                // but leaving it here reminds you that is should be done.
                s.Close();
            }

            return resources;
        }
    }
}