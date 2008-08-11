using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure.IO.Retrievers {
    [Serializable]
    public class DefaultRetriever : ChildResourceRetriever {
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

				try {
					Process.Start(processStartInfo);
				} catch (System.ComponentModel.Win32Exception ex) {
					Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "File, {0}, cannot be opened.",
						resource.FullName);
				}

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
        protected override IList<IResource> getResources(IResource resource, string filter) {
            List<IResource> childResources = new List<IResource>();

			// TODO: Encapsulate the decision to show the root/parent director in a static method somewhere.
			// Show root directory if specified.
			if (Settings.Instance.ShowRootDirectory) {
				if (!resource.Root.FullName.Equals(resource.FullName, StringComparison.OrdinalIgnoreCase)) {
					childResources.Add(new RootDirectoryInfo(resource.Root.FullName));
				}
			}

			// Show parent directory if specified.
			if (Settings.Instance.ShowParentDirectory) {
				if (!Settings.Instance.ShowRootDirectory ||
					(Settings.Instance.ShowRootDirectory &&
					!resource.Path.Equals(resource.Root.FullName, StringComparison.OrdinalIgnoreCase))) {
					childResources.Add(new ParentDirectoryInfo(resource.Path));
				}
			}

			FileSystemEnumerator filesystemEnumerator = filesystemEnumerator = new FileSystemEnumerator(resource.FullName, filter);
            childResources.AddRange(filesystemEnumerator.Matches().ConvertAll(delegate(IChildResource c) {
                return (IResource)c;
            }));

            return childResources;
        }
    }
}