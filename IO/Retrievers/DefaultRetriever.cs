using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;
using Common.Logger;

namespace SharpFile.IO.Retrievers {
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
        protected override IEnumerable<IChildResource> getResources(IResource resource, string filter) {
			// TODO: Encapsulate the decision to show the root/parent director in a static method somewhere.
			// Show root directory if specified.
			if (Settings.Instance.ShowRootDirectory) {
				if (!resource.Root.FullName.Equals(resource.FullName, StringComparison.OrdinalIgnoreCase)) {
					yield return new RootDirectoryInfo(resource.Root.FullName);
				}
			}

			// Show parent directory if specified.
			if (Settings.Instance.ShowParentDirectory) {
				if (!resource.Path.Equals(resource.FullName, StringComparison.OrdinalIgnoreCase)) {
					if (!Settings.Instance.ShowRootDirectory ||
						(Settings.Instance.ShowRootDirectory &&
						!resource.Path.Equals(resource.Root.FullName, StringComparison.OrdinalIgnoreCase))) {
						yield return new ParentDirectoryInfo(resource.Path);
					}
				}
			}

			FileSystemEnumerator filesystemEnumerator = new FileSystemEnumerator(resource.FullName);
			foreach (IChildResource childResource in filesystemEnumerator.Matches()) {
				yield return childResource;
			}
        }
    }
}