using System.Collections.Generic;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : CompressedFileRetriever {
        public IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new ReadOnlyCompressedFileRetriever();
            List<ColumnInfo> clonedColumnInfos = Settings.DeepCopy<List<ColumnInfo>>(ColumnInfos);
            childResourceRetriever.ColumnInfos = clonedColumnInfos;
            childResourceRetriever.Name = name;

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        protected override IEnumerable<SharpFile.Infrastructure.IChildResource> getResources(IResource resource, string filter) {
            // TODO: Finish this.
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            childResourceRetrievers.Add(this);

            byte[] data = new byte[4096];

            using (ZipInputStream s = new ZipInputStream(System.IO.File.OpenRead(resource.FullPath))) {
                ZipEntry zipEntry;

                while ((zipEntry = s.GetNextEntry()) != null) {
                    //resources.Add(ChildResourceFactory.GetChildResource(@"c:\#storage\", Settings.Instance.Resources[0].ChildResourceRetrievers));
                    resources.Add(new FileInfo(zipEntry.Name, childResourceRetrievers));

                    /*
                    Console.WriteLine("Name : {0}", zipEntry.Name);
                    Console.WriteLine("Date : {0}", zipEntry.DateTime);
                    Console.WriteLine("Size : (-1, if the size information is in the footer)");
                    Console.WriteLine("      Uncompressed : {0}", zipEntry.Size);
                    Console.WriteLine("      Compressed   : {0}", zipEntry.CompressedSize);
                    */

                    /*
                    if (zipEntry.IsFile) {

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

                        Console.WriteLine();
                    }
                     */
                }

                // Close can be ommitted as the using statement will do it automatically
                // but leaving it here reminds you that is should be done.
                s.Close();
            }

            return resources;
        }
    }
}