using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.SettingsSection;

namespace SharpFile.IO.Retrievers {
    public abstract class ChildResourceRetriever : IChildResourceRetriever {
        protected bool useFileAttributes = false;

        private List<ColumnInfo> columnInfos;
        private string name;
        private IView view;
        private List<string> filterMethodArguments = new List<string>();

        /// <summary>
        /// Fired when the resources are received.
        /// </summary>
        public event Infrastructure.ChildResourceRetriever.GetCompleteDelegate GetComplete;

        /// <summary>
        /// The Custom method that determines if this retriever should be used for a particular resource.
        /// </summary>
        public event Infrastructure.ChildResourceRetriever.FilterMethodDelegate FilterMethod;

        /// <summary>
        /// The Custom method with arguments that determines if this retriever should be used for a particular resource.
        /// </summary>
        public event Infrastructure.ChildResourceRetriever.FilterMethodWithArgumentsDelegate FilterMethodWithArguments;

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
        public bool OnFilterMethod(IResource resource) {
            if (FilterMethod != null) {
                return FilterMethod(resource);
            }

            return false;
        }

        /// <summary>
        /// Fires the Custom method with arguments that determines if this retriever should be used for a particular resource.
        /// </summary>
        /// <param name="resource">Resource passed to the custom method.</param>
        /// <param name="arguments">Arguments passed to the custom method.</param>
        /// <returns>Whether or not this retriever should be executed for a particular resource.</returns>
        public bool OnFilterMethodWithArguments(IResource resource, List<string> arguments) {
            if (FilterMethodWithArguments != null) {
                return FilterMethodWithArguments(resource, arguments);
            }

            return false;
        }

        /// <summary>
        /// Gets output for a particular resource and outputs the results to a view.
        /// </summary>
        /// <param name="view">View to show output.</param>
        /// <param name="resource">Resource to get output for.</param>
        public virtual void Execute(IView view, IResource resource) {
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
                    sw.Start();

                    try {
                        if (backgroundWorker.CancellationPending) {
                            e.Cancel = true;
                        } else {
                            Common.Forms.SetPropertyInParent<bool>(view.Control, "Executing", true);
                            IList<IResource> childResources = getResources(resource, view.Filter);

                            view.BeginUpdate();
                            view.Clear();
                            view.ColumnInfos = ColumnInfos;
                            view.AddItemRange(childResources);
                            view.UpdateImageIndexes(useFileAttributes);
                            view.EndUpdate();
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
                        Common.Forms.SetPropertyInParent<bool>(view.Control, "Executing", false);

                        Settings.Instance.Logger.Log(LogLevelType.Verbose, "Finish getting resources for {0} took {1} ms.",
                                    resource.FullName,
                                    sw.ElapsedMilliseconds.ToString());
                        sw.Reset();
                    }
                };

                // Method that runs when the DoWork method is finished.
                backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
                    if (e.Error == null &&
                        !e.Cancelled) {
                        // Update some information about the current directory.
                        view.OnUpdatePath(resource.FullName);
                        view.OnUpdatePanels(view);

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
            // Clone this object to a new retriever.
            IChildResourceRetriever childResourceRetriever = 
                Reflection.DuplicateObject<IChildResourceRetriever>(this);

            // Attach events to the new retriever.
            childResourceRetriever.FilterMethod += OnFilterMethod;
            childResourceRetriever.FilterMethodWithArguments += OnFilterMethodWithArguments;
            childResourceRetriever.GetComplete += OnGetComplete;

            return childResourceRetriever;
        }

		protected abstract IList<IResource> getResources(IResource resource, string filter);        

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
        public List<string> FilterMethodArguments {
            get {
                return filterMethodArguments;
            }
            set {
                filterMethodArguments = value;
            }
        }
    }
}