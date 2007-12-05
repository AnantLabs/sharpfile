using System;
using System.ComponentModel;
using System.Collections.Generic;
using SharpFile.IO.ChildResources;
using SharpFile.IO.Retrievers;
using SharpFile.IO;

namespace SharpFile.IO.Retrievers {
    public class FileRetriever : IChildResourceRetriever {
        private BackgroundWorker backgroundWorker;

        public FileRetriever() {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
        }

        public void Get(IView view, IResource resource) {
            // Anonymous method that retrieves the file information.
            backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
                // Disable the filewatcher.
                view.FileSystemWatcher.EnableRaisingEvents = false;

                // Grab the files and report the progress to the parent.
                backgroundWorker.ReportProgress(50);

                try {
                    if (backgroundWorker.CancellationPending) {
                        e.Cancel = true;
                    } else {
                        e.Result = getResources(resource, view.Filter);
                    }
                } catch (UnauthorizedAccessException ex) {
                    e.Cancel = true;
                    view.ShowMessageBox(ex.Message);
                } finally {
                    backgroundWorker.ReportProgress(100);
                }
            };

            // Method that runs when the DoWork method is finished.
            backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
                if (e.Error == null &&
                    !e.Cancelled &&
                    e.Result != null &&
                    e.Result is IEnumerable<IChildResource>) {
                    IEnumerable<IChildResource> resources = (IEnumerable<IChildResource>)e.Result;

                    view.BeginUpdate();
                    view.ClearView();
                    view.AddItemRange(resources);
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

            if (!backgroundWorker.CancellationPending) {
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

        private IEnumerable<IChildResource> getResources(IResource resource, string filter) {
            IFileContainer container = resource as IFileContainer;
            List<IChildResource> resources = new List<IChildResource>();

            resources.AddRange(container.GetDirectories());
            resources.AddRange(container.GetFiles(filter));

            return resources;
        }
    }
}