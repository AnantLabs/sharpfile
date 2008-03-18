using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
    [Serializable]
    public class FileRetriever : IChildResourceRetriever {
        private List<ColumnInfo> columnInfos;
        private string name;
        private IView view;
        private List<string> customMethodArguments;

        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;
        public event ChildResourceRetriever.CustomMethodDelegate CustomMethod;
        public event ChildResourceRetriever.CustomMethodWithArgumentsDelegate CustomMethodWithArguments;

        public FileRetriever() {
        }

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

        public bool OnCustomMethodWithArguments(IResource resource, List<string> arguments) {
            if (CustomMethodWithArguments != null) {
                return CustomMethodWithArguments(resource, arguments);
            }

            return false;
        }

        public void Execute(IView view, IResource resource) {
            Stopwatch sw = new Stopwatch();

            if (resource is FileInfo) {
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.ErrorDialog = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.FileName = resource.FullName;
                System.Diagnostics.Process.Start(processStartInfo);

                return;
            } else if (resource is DirectoryInfo || resource is DriveInfo) {
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
                                sw.Start();

                                // TODO: Update the view here, it would take less memory than storing all of that data.
                                e.Result = getResources(resource, view.Filter);

                                Settings.Instance.Logger.Log(LogLevelType.Verbose, "Finish getting resources for {0} took {1} ms.",
                                    resource.FullName,
                                    sw.ElapsedMilliseconds.ToString());
                                sw.Reset();
                            }
                        } catch (UnauthorizedAccessException ex) {
                            e.Cancel = true;

                            Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Access is unauthorized for {0}.", resource.FullName);
                            Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;
                        } catch (Exception ex) {
                            e.Cancel = true;

                            Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Exception when getting resources for {0}.", resource.FullName);
                            Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;
                        } finally {
                            backgroundWorker.ReportProgress(100);
                        }
                    };

                    // Method that runs when the DoWork method is finished.
                    backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
                        if (e.Error == null &&
                            !e.Cancelled &&
                            e.Result != null &&
                            e.Result is IEnumerable<IResource>) {
                            IEnumerable<IResource> resources = (IEnumerable<IResource>)e.Result;

                            view.BeginUpdate();
                            view.ColumnInfos = ColumnInfos;
                            view.Clear();
                            view.AddItemRange(resources);
                            view.EndUpdate();

                            // Update some information about the current directory.
                            view.OnUpdatePath(resource.FullName);

                            // Set up the watcher.
                            view.FileSystemWatcher.Path = resource.FullName;
                            view.FileSystemWatcher.Filter = view.Filter;
                            view.FileSystemWatcher.EnableRaisingEvents = true;
                        }

                        OnGetComplete();
                    };

                    // Anonymous method that updates the status to the parent form.
                    backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
                        view.OnUpdateProgress(e.ProgressPercentage);
                    };

                    backgroundWorker.RunWorkerAsync();
                }
            }
        }

        public IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new FileRetriever();
            List<ColumnInfo> clonedColumnInfos = Settings.DeepCopy<List<ColumnInfo>>(ColumnInfos);
            childResourceRetriever.ColumnInfos = clonedColumnInfos;
            childResourceRetriever.Name = name;
            childResourceRetriever.View = View;
            childResourceRetriever.CustomMethodArguments = CustomMethodArguments;

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.CustomMethodWithArguments += OnCustomMethodWithArguments;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        public List<ColumnInfo> ColumnInfos {
            get {
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

        public IView View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        public List<string> CustomMethodArguments {
            get {
                return customMethodArguments;
            }
            set {
                customMethodArguments = value;
            }
        }

        private IEnumerable<IResource> getResources(IResource resource, string filter) {
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