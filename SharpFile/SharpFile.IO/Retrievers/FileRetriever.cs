using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
    public class FileRetriever : IChildResourceRetriever {
        private BackgroundWorker backgroundWorker;
        private List<ColumnInfo> columnInfos;

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
                    view.ColumnInfos = ColumnInfos;
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

            if (!backgroundWorker.CancellationPending &&
                !backgroundWorker.IsBusy) {
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

        public List<ColumnInfo> ColumnInfos {
            get {
                /*
                if (columnInfos == null) {
                    columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(
                        new ColumnInfo("Filename", "DisplayName", new StringLogicalComparer(), true));

                    columnInfos.Add(
                        new ColumnInfo("Size", "Size",
                            new ColumnInfo.CustomMethod(Common.General.GetHumanReadableSize),
                            new StringLogicalComparer()));

                    columnInfos.Add(
                        new ColumnInfo("Date", "LastWriteTime",
                            new ColumnInfo.CustomMethod(Settings.GetDateTimeShortDateString),
                            new StringLogicalComparer()));

                    columnInfos.Add(
                        new ColumnInfo("Time", "LastWriteTime",
                            new ColumnInfo.CustomMethod(Settings.GetDateTimeShortTimeString),
                            new StringLogicalComparer()));
                }
                */

                if (columnInfos == null) {
                    throw new Exception("No column information has been set.");

                    //columnInfos = new List<ColumnInfo>();
                    //columnInfos.Add(
                    //    new ColumnInfo("Filename", "DisplayName", new StringLogicalComparer(), true));
                }

                return columnInfos;
            }
            set {
                columnInfos = value;
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