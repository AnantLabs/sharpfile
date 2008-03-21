using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
    public abstract class ChildResourceRetriever : IChildResourceRetriever {
        private List<ColumnInfo> columnInfos;
        private string name;
        private IView view;
        private List<string> customMethodArguments = new List<string>();

        /// <summary>
        /// Fired when the resources are received.
        /// </summary>
        public event SharpFile.Infrastructure.ChildResourceRetriever.GetCompleteDelegate GetComplete;

        /// <summary>
        /// The Custom method that determines if this retriever should be used for a particular resource.
        /// </summary>
        public event SharpFile.Infrastructure.ChildResourceRetriever.CustomMethodDelegate CustomMethod;

        /// <summary>
        /// The Custom method with arguments that determines if this retriever should be used for a particular resource.
        /// </summary>
        public event SharpFile.Infrastructure.ChildResourceRetriever.CustomMethodWithArgumentsDelegate CustomMethodWithArguments;

        /// <summary>
        /// Fires the GetComplete event.
        /// </summary>
        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
        }

        /// <summary>
        /// Fires the Custom method that determines if this retriever should be used for a particular resource.
        /// </summary>
        /// <param name="resource">Resource passed to the custom method.</param>
        /// <returns>Whether or not this retriever should be executed for a particular resource.</returns>
        public bool OnCustomMethod(IResource resource) {
            if (CustomMethod != null) {
                return CustomMethod(resource);
            }

            return false;
        }

        /// <summary>
        /// Fires the Custom method with arguments that determines if this retriever should be used for a particular resource.
        /// </summary>
        /// <param name="resource">Resource passed to the custom method.</param>
        /// <param name="arguments">Arguments passed to the custom method.</param>
        /// <returns>Whether or not this retriever should be executed for a particular resource.</returns>
        public bool OnCustomMethodWithArguments(IResource resource, List<string> arguments) {
            if (CustomMethodWithArguments != null) {
                return CustomMethodWithArguments(resource, arguments);
            }

            return false;
        }

        /// <summary>
        /// Gets output for a particular resource and outputs the results to a view.
        /// </summary>
        /// <param name="view">View to show output.</param>
        /// <param name="resource">Resource to get output for.</param>
        public void Execute(IView view, IResource resource) {
            Stopwatch sw = new Stopwatch();

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
                            "Error when getting resources for {0}.", resource.FullName);
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

        /// <summary>
        /// Clones the ChildResourceRetriever.
        /// </summary>
        /// <returns></returns>
        public IChildResourceRetriever Clone() {
            // Instantiate an object for whatever type this currently is (so that derived classes can call this method).
            IChildResourceRetriever childResourceRetriever = Reflection.InstantiateObject<IChildResourceRetriever>(
                GetType().Assembly.FullName, GetType().FullName);

            // Deep copy the column infos.
            List<ColumnInfo> clonedColumnInfos = Settings.DeepCopy<List<ColumnInfo>>(ColumnInfos);
            childResourceRetriever.ColumnInfos = clonedColumnInfos;
            childResourceRetriever.Name = Name;
            childResourceRetriever.View = View;
            childResourceRetriever.CustomMethodArguments = CustomMethodArguments;

            childResourceRetriever.CustomMethod += OnCustomMethod;
            childResourceRetriever.CustomMethodWithArguments += OnCustomMethodWithArguments;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

        protected abstract IEnumerable<IResource> getResources(IResource resource, string filter);        

        /// <summary>
        /// Column information.
        /// </summary>
        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// View.
        /// </summary>
        public IView View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        /// <summary>
        /// Arguments to be used with the custom method with arguments.
        /// </summary>
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