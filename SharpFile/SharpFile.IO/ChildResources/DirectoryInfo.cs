using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpFile.IO.Retrievers;
using SharpFile.IO;

namespace SharpFile.IO.ChildResources {
	public class DirectoryInfo : FileSystemInfo, IChildResource {
		private System.IO.DirectoryInfo directoryInfo;

		public DirectoryInfo(string path)
			: this(new System.IO.DirectoryInfo(path)) {
		}

		public DirectoryInfo(System.IO.DirectoryInfo directoryInfo) {
			this.size = 0;
			this.directoryInfo = directoryInfo;
			this.name = directoryInfo.Name;
			this.lastWriteTime = directoryInfo.LastWriteTime;
			this.fullPath = directoryInfo.FullName;
		}

		public System.IO.DirectoryInfo Parent {
			get {
				if (directoryInfo.Parent != null) {
					return directoryInfo.Parent;
				} else {
					return null;
				}
			}
		}

		public IEnumerable<IChildResource> GetDirectories() {
			System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

			return Array.ConvertAll<System.IO.DirectoryInfo, DirectoryInfo>(directoryInfos,
				delegate(System.IO.DirectoryInfo di) {
					return new DirectoryInfo(di);
				});
		}

		public IEnumerable<IChildResource> GetFiles() {
			return GetFiles(string.Empty);
		}

		public IEnumerable<IChildResource> GetFiles(string filter) {
			System.IO.FileInfo[] fileInfos = null;

			if (string.IsNullOrEmpty(filter)) {
				fileInfos = directoryInfo.GetFiles();
			} else {
				fileInfos = directoryInfo.GetFiles("*" + filter + "*", 
					System.IO.SearchOption.TopDirectoryOnly);
			}

			return Array.ConvertAll<System.IO.FileInfo, FileInfo>(fileInfos,
				delegate(System.IO.FileInfo fi) {
					return new FileInfo(fi);
				});
		}

		public long GetSize() {
			if (size == 0) {
				size = getSize(directoryInfo);
			}

			return size;
		}

		public void Copy(string destination) {
			if (!destination.EndsWith(@"\")) {
				destination += @"\";
			}

			if (!Exists(destination)) {
				Create(destination);
			}

			foreach (FileInfo fileInfo in GetFiles()) {
				fileInfo.Copy(destination + fileInfo.Name);
			}

			foreach (DirectoryInfo directory in GetDirectories()) {
				string subFolder = directory.Name;
				Create(destination + @"\" + subFolder);
				directory.Copy(destination + @"\" + subFolder);
			}
		}

		public void Move(string destination) {
			System.IO.Directory.Move(this.FullPath, destination);
		}

		public void Execute(IView view) {
			/*
			// Get the directory information.
			this.ChildResourceRetriever.Get(view, this);
			*/

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
		}

		public static DirectoryInfo Create(string path) {
			System.IO.DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(path);
			return new DirectoryInfo(directoryInfo);
		}

		public static bool Exists(string path) {
			return System.IO.Directory.Exists(path);
		}

		private long getSize(System.IO.DirectoryInfo directoryInfo) {
			long totalSize = 0;

			try {
				foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles()) {
					totalSize += fileInfo.Length;
				}

				System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
				foreach (System.IO.DirectoryInfo subDirectoryInfo in directoryInfos) {
					totalSize += getSize(subDirectoryInfo);
				}
			} catch (Exception ex) {
			}

			return totalSize;
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				return new FileRetriever();
			}
		}
	}
}