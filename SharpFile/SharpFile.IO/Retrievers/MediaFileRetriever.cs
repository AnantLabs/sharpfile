﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Logger;
using SharpFile.Infrastructure;

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

                            e.Result = Image.FromFile(resource.FullPath);
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
                        e.Result is Image) {
                        Settings.Instance.Logger.Log(LogLevelType.Verbose,
                            "Get resources complete.");

                        Image image = (Image)e.Result;

                        view.BeginUpdate();
                        view.ColumnInfos = ColumnInfos;
                        view.Clear();
                        view.InsertItem((IChildResource)resource);
                        view.EndUpdate();

                        // Update some information about the current directory.
                        view.OnUpdatePath(resource.FullPath);

                        // Set up the watcher.
                        //view.FileSystemWatcher.Path = resource.FullPath;
                        //view.FileSystemWatcher.Filter = view.Filter;
                        //view.FileSystemWatcher.EnableRaisingEvents = true;
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

        /*
        private IEnumerable<IChildResource> getResources(IResource resource, string filter) {
            IFileContainer container = resource as IFileContainer;
            List<IChildResource> resources = new List<IChildResource>();

            resources.AddRange(container.GetDirectories());
            resources.AddRange(container.GetFiles(filter));

            return resources;
        }
        */
    }
}