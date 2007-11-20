using System;
using System.ComponentModel;
using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using SharpFile.IO.Retrievers;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public class FileRetriever :IChildResourceRetriever {
		private BackgroundWorker backgroundWorker;

		public void Get(IView view, IResource resource) {
			// Create another thread to get the file information asynchronously.
			using (backgroundWorker = new BackgroundWorker()) {
				backgroundWorker.WorkerReportsProgress = true;

				// Anonymous method that retrieves the file information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					// Disable the filewatcher.
					view.FileSystemWatcher.EnableRaisingEvents = false;

					// Grab the files and report the progress to the parent.
					backgroundWorker.ReportProgress(50);

					e.Result = getFiles(resource, view.Filter);
					backgroundWorker.ReportProgress(100);
				};

				// Method that runs when the DoWork method is finished.
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					if (e.Error == null &&
						e.Result != null &&
						e.Result is IEnumerable<IChildResource>) {
						IEnumerable<IChildResource> fileSystemInfoList = (IEnumerable<IChildResource>)e.Result;

						view.BeginUpdate();
						view.ClearView();
						view.UpdateView(fileSystemInfoList);
						view.EndUpdate();

						// Update some information about the current directory.
						view.UpdatePath(resource.FullPath);

						// Set up the watcher.
						view.FileSystemWatcher.Path = resource.FullPath;
						view.FileSystemWatcher.Filter = view.Filter;
						view.FileSystemWatcher.EnableRaisingEvents = true;
					}
				};

				// Anonymous method that updates the status to the parent form.
				backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
					view.UpdateProgress(e.ProgressPercentage);
				};

				backgroundWorker.RunWorkerAsync();
			}
		}

		public void Cancel() {
			if (backgroundWorker != null &&
				backgroundWorker.IsBusy &&
				!backgroundWorker.CancellationPending) {
				backgroundWorker.CancelAsync();
			}
		}

		private IEnumerable<IChildResource> getFiles(IResource resource, string filter) {
			IFileContainer container = resource as IFileContainer;
			List<IChildResource> resources = new List<IChildResource>();

			// TODO: Setting that specifies whether to show root directory or not.
			/*
			if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
				resources.Add(new RootDirectoryInfo(directoryInfo.Root));
			}
			*/

			// TODO: Setting that specifies whether to show parent directory or not.
			if (container is DirectoryInfo) {
				DirectoryInfo directoryInfo = container as DirectoryInfo;

				if (directoryInfo.Parent != null) {
					//if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
					resources.Add(new ParentDirectoryInfo(directoryInfo.Parent));
					//}
				}
			}

			resources.AddRange(container.GetDirectories());
			resources.AddRange(container.GetFiles(filter));

			return resources;
		}
	}
}