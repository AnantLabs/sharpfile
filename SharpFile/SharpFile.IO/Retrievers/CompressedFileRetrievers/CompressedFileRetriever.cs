using System;
using System.Collections.Generic;
using System.ComponentModel;
using Common.Logger;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    [Serializable]
    public abstract class CompressedFileRetriever : IChildResourceRetriever {
        private List<ColumnInfo> columnInfos;
        private string name;

        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;
        public event ChildResourceRetriever.CustomMethodDelegate CustomMethod;

        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
        }

        public bool OnCustomMethod(IResource resource) {
            if (CustomMethod != null) {
                return CustomMethod(resource);
            }

            return false;
        }

        public void Execute(IView view, IResource resource) {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.WorkerReportsProgress = true;

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
                        string message = string.Format("Access is unauthorized for {0}.",
                            resource.FullPath);

                        view.ShowMessageBox(message);
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message);
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
                        view.Clear();
                        view.AddItemRange(resources);
                        view.EndUpdate();

                        // Update some information about the current directory.
                        view.UpdatePath(resource.FullPath);

                        // Set up the watcher.
                        view.FileSystemWatcher.Path = resource.FullPath;
                        view.FileSystemWatcher.Filter = view.Filter;
                        view.FileSystemWatcher.EnableRaisingEvents = true;
                    }

                    OnGetComplete();
                };

                // Anonymous method that updates the status to the parent form.
                backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
                    view.UpdateProgress(e.ProgressPercentage);
                };

                backgroundWorker.RunWorkerAsync();
            }
        }

        public List<ColumnInfo> ColumnInfos {
            get {
                if (columnInfos == null) {
                    throw new Exception("No column information has been set.");
                }

                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public abstract IChildResourceRetriever Clone();

        protected abstract IEnumerable<IChildResource> getResources(IResource resource, string filter);
    }
}