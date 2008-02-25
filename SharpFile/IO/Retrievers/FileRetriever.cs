using System;
using System.Collections.Generic;
using System.ComponentModel;
using Common.Logger;
using SharpFile.Infrastructure;
using System.IO;
using SharpFile.IO.ChildResources;
using SharpFile.ExtensionMethods;

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

        public bool OnCustomMethod(FileSystemInfo resource) {
            if (CustomMethod != null) {
                return CustomMethod(resource);
            }

            return false;
        }

        public bool OnCustomMethodWithArguments(FileSystemInfo resource, List<string> arguments) {
            if (CustomMethodWithArguments != null) {
                return CustomMethodWithArguments(resource, arguments);
            }

            return false;
        }

        public void Execute(IView view, FileSystemInfo resource) {
            Settings.Instance.Logger.Log(LogLevelType.Verbose,
                "Starting to Execute.");

            if (resource is FileInfo) {
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.ErrorDialog = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.FileName = resource.FullName;
                System.Diagnostics.Process.Start(processStartInfo);

                return;
            } else if (resource is FileSystemInfo) {
                DirectoryInfo directoryInfo = null;

                if (resource is DirectoryInfo) {
                    directoryInfo = (DirectoryInfo)resource;
                } else if (resource is ParentDirectoryInfo) {
                    directoryInfo = ((ParentDirectoryInfo)resource).DirectoryInfo;
                } else if (resource is RootDirectoryInfo) {
                    directoryInfo = ((RootDirectoryInfo)resource).DirectoryInfo;
                }

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
                                Settings.Instance.Logger.Log(LogLevelType.Verbose,
                                    "Start to get resources.");

                                e.Result = getResources(directoryInfo, view.Filter);

                                Settings.Instance.Logger.Log(LogLevelType.Verbose,
                                    "Finish getting resources.");
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
                            e.Result is IEnumerable<FileSystemInfo>) {
                            Settings.Instance.Logger.Log(LogLevelType.Verbose,
                                "Get resources complete.");

                            IEnumerable<System.IO.FileSystemInfo> resources = (IEnumerable<System.IO.FileSystemInfo>)e.Result;

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

        private IEnumerable<FileSystemInfo> getResources(DirectoryInfo directoryInfo, string filter) {
            List<FileSystemInfo> resources = new List<FileSystemInfo>();

            resources.AddRange(directoryInfo.ExtGetDirectories());
            resources.AddRange(directoryInfo.ExtGetFiles(filter));

            return resources;
        }
    }
}