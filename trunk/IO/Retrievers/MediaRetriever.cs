using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using System.IO;

namespace SharpFile.IO.Retrievers {
    [Serializable]
    public class MediaFileRetriever : IChildResourceRetriever {
        private List<ColumnInfo> columnInfos;
        private string name;
        private IView view;
        private List<string> customMethodArguments;

        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;
        public event ChildResourceRetriever.CustomMethodDelegate CustomMethod;
        public event ChildResourceRetriever.CustomMethodWithArgumentsDelegate CustomMethodWithArguments;

        public MediaFileRetriever() {
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
                "Starting to Execute in the MediaListView.");

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

                            e.Result = resource;
                        }
                    } catch (UnauthorizedAccessException ex) {
                        e.Cancel = true;

                        Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                            "Access is unauthorized for {0}.", resource.FullPath);
                        Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;
                    } catch (Exception ex) {
                        e.Cancel = true;

                        Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                            "Exception when getting resources for {0}.", resource.FullPath);
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
                        e.Result is IChildResource) {
                        Settings.Instance.Logger.Log(LogLevelType.Verbose,
                            "Get resources complete.");

                        //IChildResource resource = (IChildResource)e.Result;

                        view.BeginUpdate();
                        view.ColumnInfos = ColumnInfos;
                        view.Clear();

                        List<IChildResource> resources = new List<IChildResource>();

                        ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
                        childResourceRetrievers.AddRange(Settings.Instance.Resources[0].ChildResourceRetrievers);

                        if (Settings.Instance.ShowRootDirectory) {
                            resources.Add(new RootDirectoryInfo(new System.IO.DirectoryInfo(resource.Root.FullPath),
                                        childResourceRetrievers));
                        }

                        if (Settings.Instance.ShowParentDirectory) {
                            if (!Settings.Instance.ShowRootDirectory ||
                                (Settings.Instance.ShowRootDirectory &&
                                !resource.Path.ToLower().Equals(resource.Root.Name.ToLower()))) {
                                resources.Add(new ParentDirectoryInfo(new System.IO.DirectoryInfo(resource.Path),
                                    childResourceRetrievers));
                            }
                        }

                        view.AddItemRange(resources);
                        view.InsertItem((IChildResource)resource);
                        view.EndUpdate();

                        // Update some information about the current directory.
                        view.OnUpdatePath(resource.FullPath);

                        // Set up the watcher.
                        view.FileSystemWatcher.Path = resource.FullPath;
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

        public IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new MediaFileRetriever();
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
    }
}