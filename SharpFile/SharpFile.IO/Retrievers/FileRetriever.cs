using System;
using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using SharpFile.IO.Retrievers;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
	public class FileRetriever : IChildResourceRetriever {
		// TODO: Follow the BackgroundThread article's suggestion here to encapsulate the background worker.

		public IEnumerable<IChildResource> Get(IView view, IResource resource) {
			/*
			// Get the directory information.
			DirectoryInfo directoryInfo = new DirectoryInfo(this.FullPath);
			string directoryPath = directoryInfo.FullPath;
			//directoryPath = string.Format("{0}{1}",
			//    directoryPath,
			//    directoryPath.EndsWith(@"\") ? string.Empty : @"\");

			// Create another thread to get the file information asynchronously.
			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				backgroundWorker.WorkerReportsProgress = true;

				// Anonymous method that retrieves the file information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					// Disable the filewatcher.
					//FileSystemWatcher.EnableRaisingEvents = false;

					// Grab the files and report the progress to the parent.
					backgroundWorker.ReportProgress(50);

					e.Result = this.ChildResourceRetriever.Get(view, directoryInfo);
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
						view.UpdatePath(directoryPath);

						// Set up the watcher.
						//FileSystemWatcher.Path = directoryPath;
						//FileSystemWatcher.Filter = filter;
						//FileSystemWatcher.EnableRaisingEvents = true;
					}
				};

				// Anonymous method that updates the status to the parent form.
				backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
					view.UpdateProgress(e.ProgressPercentage);
				};

				backgroundWorker.RunWorkerAsync();
			}
			*/

			return getFiles(resource, view.Filter);
		}

		private IEnumerable<IChildResource> getFiles(IResource resource, string filter) {
			DirectoryInfo directoryInfo = resource as DirectoryInfo;
			List<IChildResource> dataInfos = new List<IChildResource>();

			// TODO: Setting that specifies whether to show root directory or not.
			/*
			if (!directoryInfo.Root.Name.Equals(directoryInfo.FullName)) {
				dataInfos.Add(new RootDirectoryInfo(directoryInfo.Root));
			}
			*/

			// TODO: Setting that specifies whether to show parent directory or not.
			if (directoryInfo.Parent != null) {
				//if (!directoryInfo.Parent.Name.Equals(directoryInfo.Root.Name)) {
				dataInfos.Add(new ParentDirectoryInfo(directoryInfo.Parent));
				//}
			}

			// TODO: Specify an IFileContainer (or something) that can return GetDirectories and GetFiles.
			// Specify that driectoryinfo and driveinfo implement the above-mentioned interface.
			dataInfos.AddRange(directoryInfo.GetDirectories());
			dataInfos.AddRange(directoryInfo.GetFiles(filter));

			return dataInfos;
		}
	}
}