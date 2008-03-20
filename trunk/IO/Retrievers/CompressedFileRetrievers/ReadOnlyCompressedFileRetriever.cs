using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;
using Common.Logger;
using System.ComponentModel;
using System;
//using System.IO;

namespace SharpFile.IO.Retrievers.CompressedFileRetrievers {
    public class ReadOnlyCompressedFileRetriever : ChildResourceRetriever {
        public override IChildResourceRetriever Clone() {
            IChildResourceRetriever childResourceRetriever = new ReadOnlyCompressedFileRetriever();
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

        // TODO: Add Execute to the ChildResourceRetriever abstract class and have hooks for different getResources calls.
        public override void Execute(IView view, IResource resource) {
            Settings.Instance.Logger.Log(LogLevelType.Verbose,
                "Starting to Execute.");

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

                        Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                            "Access is unauthorized for {0}.", resource.FullName);
                        Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;
                    } catch (Exception ex) {
                        e.Cancel = true;

                        Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                            "Error when getting compressed file contents for {0}.", resource.FullName);
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

        private IEnumerable<IResource> getResources(IResource resource, string filter) {
            List<IResource> resources = new List<IResource>();

            //ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers();
            //childResourceRetrievers.AddRange(Settings.Instance.ParentResources[0].GetChildResourceRetrievers());

            /*
            if (Settings.Instance.ShowRootDirectory) {
                resources.Add(resource.Root);
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                        (Settings.Instance.ShowRootDirectory &&
                        !resource.Path.Equals(resource.Root.Name))) {
                    resources.Add(resource);
                }
            }
             */

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    if (zipEntry.IsFile) {
                        string fileName = zipEntry.Name;
                        string extension = Common.General.GetExtension(fileName);

                        resources.Add(new CompressedFileInfo(fileName, fileName, zipEntry.Size, zipEntry.CompressedSize,
                            zipEntry.DateTime));
                    } else if (zipEntry.IsDirectory) {
                        resources.Add(new CompressedDirectoryInfo(zipEntry.Name, zipEntry.Name, zipEntry.DateTime));
                    }
                }
            }

            return resources;
        }
    }
}