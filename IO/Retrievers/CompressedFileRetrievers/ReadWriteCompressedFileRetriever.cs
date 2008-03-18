using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using System.IO;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadWriteCompressedFileRetriever : CompressedFileRetriever {
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

        protected override IEnumerable<FileSystemInfo> getResources(FileSystemInfo resource, string filter) {
            List<FileSystemInfo> resources = new List<FileSystemInfo>();

            // TODO: Finish this.
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            childResourceRetrievers.Add(this);

            string unzippedPath = string.Format(@"tmp\{0}",
                resource.Name);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(resource.FullPath, unzippedPath, string.Empty);

            return resources;
        }
    }
}