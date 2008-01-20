using System.Collections.Generic;
using SharpFile.Infrastructure;
using ICSharpCode.SharpZipLib.Zip;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadWriteCompressedFileRetriever : CompressedFileRetriever {
        protected override IEnumerable<SharpFile.Infrastructure.IChildResource> getResources(IResource resource, string filter) {
            // TODO: Finish this.
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            childResourceRetrievers.Add(this);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(resource.FullPath, "tmp", string.Empty);

            return resources;
        }
    }
}