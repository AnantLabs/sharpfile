using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadWriteCompressedFileRetriever : CompressedFileRetriever {
        public override IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new ReadOnlyCompressedFileRetriever();
            List<ColumnInfo> clonedColumnInfos = Settings.DeepCopy<List<ColumnInfo>>(ColumnInfos);
            childResourceRetriever.ColumnInfos = clonedColumnInfos;
            childResourceRetriever.Name = Name;
            childResourceRetriever.View = View;

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        protected override IEnumerable<IChildResource> getResources(IResource resource, string filter) {
            List<IChildResource> resources = new List<IChildResource>();

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