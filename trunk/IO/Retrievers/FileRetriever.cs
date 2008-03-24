using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
    [Serializable]
    public class FileRetriever : ChildResourceRetriever {
        /// <summary>
        /// Override the default Execute for ChildResourceRetrievers for files. Use the default for everything else.
        /// </summary>
        /// <param name="view">View to output the results to.</param>
        /// <param name="resource">Resource to grab output from.</param>
        public override void Execute(IView view, IResource resource) {
            if (resource is FileInfo) {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.ErrorDialog = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.FileName = resource.FullName;
                Process.Start(processStartInfo);

                return;
            } else if (resource is DirectoryInfo || resource is DriveInfo) {
                base.Execute(view, resource);
            }
        }

        /// <summary>
        /// Get directory/file resources.
        /// </summary>
        /// <param name="resource">Resource to grab directories/files from.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>List of directories/files.</returns>
        protected override IEnumerable<IResource> getResources(IResource resource, string filter) {
            if (resource is IResourceGetter) {
                IResourceGetter resourceGetter = (IResourceGetter)resource;

                foreach (IChildResource childResource in resourceGetter.GetDirectories()) {
                    yield return childResource;
                }

                foreach (IChildResource childResource in resourceGetter.GetFiles(filter)) {
                    yield return childResource;
                }
            }
        }
    }
}