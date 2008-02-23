using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.UI;
using View = SharpFile.Infrastructure.View;
using SharpFile.IO.ChildResources;

namespace SharpFile {
    public class FileBrowser : TabPage {
        private const int filterWidth = 50;
        private static readonly object lockObject = new object();

        private FileSystemWatcher fileSystemWatcher;
        private ImageList imageList;
        private DriveDetector driveDetector;
        private ChildResourceRetrievers childResourceRetrievers;
        private bool handleCreated = false;

        private ToolStrip toolStrip;
        private ToolStripSplitButton tlsDrives;
        private ToolStripSpringTextBox tlsPath;
        private ToolStripTextBox tlsFilter;
        private IView view;

        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;
        public event View.UpdateProgressDelegate UpdateProgress;
        public event View.UpdateStatusDelegate UpdateStatus;

        /// <summary>
        /// Filebrowser ctor.
        /// </summary>
        public FileBrowser(string name) {
            this.Name = name;
            this.toolStrip = new ToolStrip();
            this.tlsDrives = new ToolStripSplitButton();
            this.tlsPath = new ToolStripSpringTextBox();
            this.tlsFilter = new ToolStripTextBox();
            this.view = new ListView(this.Name);
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();

            this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new ToolStripItem[] {
			                                                  	this.tlsDrives,
			                                                  	this.tlsPath,
			                                                  	this.tlsFilter
			                                                  });
            this.toolStrip.Location = new Point(0, 0);
            this.toolStrip.RenderMode = ToolStripRenderMode.System;
            this.toolStrip.ShowItemToolTips = false;
            this.toolStrip.Size = new Size(454, 25);
            this.toolStrip.TabIndex = 1;

            this.tlsDrives.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tlsDrives.ImageTransparentColor = Color.Magenta;

            this.tlsFilter.Size = new Size(filterWidth, 25);

            this.Controls.Add(this.view.Control);
            this.Controls.Add(this.toolStrip);
            this.ForeColor = SystemColors.ControlText;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            DriveDetector.DeviceArrived += driveDetector_DeviceArrived;
            DriveDetector.DeviceRemoved += driveDetector_DeviceRemoved;

            initializeComponent();
            UpdateParentListing();
        }

        #region Delegate methods
        /// <summary>
        /// Passes the filesystem info to any listening events.
        /// </summary>
        /// <returns>Image index.</returns>
        protected int OnGetImageIndex(IResource fsi) {
            if (GetImageIndex != null) {
                return GetImageIndex(fsi);
            }

            return -1;
        }

        protected void OnUpdatePath(string path) {
            if (UpdatePath != null) {
                UpdatePath(path);
            }

            this.Text = path;
            Path = path;
        }

        protected void OnUpdateProgress(int value) {
            if (UpdateProgress != null) {
                UpdateProgress(value);
            }
        }

        protected void OnUpdateStatus(string status) {
            if (UpdateStatus != null) {
                UpdateStatus(status);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Sets the filebrowser up.
        /// </summary>
        private void initializeComponent() {
            this.DoubleBuffered = true;
            clearFilter();

            // Attach to some events.
            this.HandleCreated += fileBrowser_HandleCreated;
            this.tlsPath.KeyDown += tlsPath_KeyDown;
            this.tlsFilter.KeyUp += tlsFilter_KeyUp;
            this.tlsFilter.LostFocus += tlsFilter_LostFocus;
            this.tlsDrives.DropDownItemClicked += tlsDrives_DropDownItemClicked;
            this.tlsDrives.ButtonClick += tlsDrives_ButtonClick;

            // Wire up the view delegates.
            this.view.UpdatePath += OnUpdatePath;
            this.view.GetImageIndex += OnGetImageIndex;
            this.view.UpdateProgress += OnUpdateProgress;
            this.view.UpdateStatus += OnUpdateStatus;

            // Wire up the file system watcher.
            fileSystemWatcher = new FileSystemWatcher(this, 100);
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
        }

        /// <summary>
        /// Clears the filter.
        /// </summary>
        private void clearFilter() {
            tlsFilter.Text = "*.*";
        }
        #endregion

        #region Events
        /// <summary>
        /// Fires when the FileBrowser handle has been created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileBrowser_HandleCreated(object sender, EventArgs e) {
            handleCreated = true;
        }

        /// <summary>
        /// Refreshes the view when Enter is pressed in the path textbox.
        /// </summary>
        private void tlsPath_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                IChildResource resource = ChildResourceFactory.GetChildResource(Path);

                if (resource != null) {
                    resource.Execute(view);
                } else {
                    Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, 
                        "The path, {0}, looks like it is incorrect.", Path);
                    Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;

                    Path = this.Text;
                }
            }
        }

        /// <summary>
        /// Refreshes the view when Enter is pressed in the filter textbox.
        /// </summary>
        private void tlsFilter_KeyUp(object sender, KeyEventArgs e) {
            IChildResource resource = ChildResourceFactory.GetChildResource(Path);
            resource.Execute(view);
        }

        /// <summary>
        /// Clears the filter when the focus is lost.
        /// </summary>
        private void tlsFilter_LostFocus(object sender, EventArgs e) {
            if (Filter.Equals(string.Empty) ||
                Filter.Equals("*") ||
                Filter.Equals("*.") ||
                Filter.Equals(".*")) {
                clearFilter();
            }
        }

        /// <summary>
        /// Refreshes the view when a different drive is selected.
        /// </summary>
        private void tlsDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            IResource resource = (IResource)e.ClickedItem.Tag;
            resource.Execute(view);

            tlsDrives.Enabled = false;
            tlsPath.Enabled = false;
            tlsFilter.Enabled = false;
            view.Enabled = false;

            foreach (IChildResourceRetriever childResourceRetriever in resource.ChildResourceRetrievers) {
                childResourceRetriever.GetComplete += delegate {
                    highlightParentResource(resource, e.ClickedItem.Image);
                    tlsDrives.Enabled = true;
                    tlsPath.Enabled = true;
                    tlsFilter.Enabled = true;
                    view.Enabled = true;
                };
            }
        }

        /// <summary>
        /// Refreshes the view with the current root drive.
        /// </summary>
        private void tlsDrives_ButtonClick(object sender, EventArgs e) {
            IResource resource = (IResource)tlsDrives.Tag;
            resource.Execute(view);
        }

        /// <summary>
        /// Fires when the filesystem watcher sees a filesystem event.
        /// </summary>
        private void fileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e) {
            string path = e.FullPath;
            IChildResource resource = ChildResourceFactory.GetChildResource(path);

            if (view.Control.IsHandleCreated) {
                // Required to ensure the view update occurs on the calling thread.
                view.Control.BeginInvoke((MethodInvoker)delegate {
                    view.BeginUpdate();

                    switch (e.ChangeType) {
                        case System.IO.WatcherChangeTypes.Changed:
                            view.RemoveItem(path);
                            view.InsertItem(resource);
                            break;
                        case System.IO.WatcherChangeTypes.Created:
                            view.InsertItem(resource);
                            break;
                        case System.IO.WatcherChangeTypes.Deleted:
                            view.RemoveItem(path);
                            break;
                        case System.IO.WatcherChangeTypes.Renamed:
                            string oldFullPath = ((System.IO.RenamedEventArgs)e).OldFullPath;
                            view.RemoveItem(oldFullPath);
                            view.InsertItem(resource);
                            break;
                    }

                    view.EndUpdate();
                });
            }
        }

        /// <summary>
        /// Fires when a drive is added.
        /// </summary>
        private void driveDetector_DeviceArrived(object sender, DriveDetectorEventArgs e) {
            Settings.ClearResources();

            this.BeginInvoke((MethodInvoker)delegate {
                UpdateParentListing();
            });
        }

        /// <summary>
        /// Fires when a drive is removed.
        /// </summary>
        private void driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e) {
            Settings.ClearResources();

            this.BeginInvoke((MethodInvoker)delegate {
                int index = 0;

                foreach (ToolStripItem item in tlsDrives.DropDownItems) {
                    if (item.Name.ToLower().Equals(e.Drive.ToLower())) {
                        string pathRoot = Path.Substring(0, Path.IndexOf(':')).ToLower();
                        string drive = e.Drive.Substring(0, e.Drive.IndexOf(':')).ToLower();

                        if (pathRoot.Equals(drive)) {
                            // Update the path to the local drive.
                        }

                        tlsDrives.DropDownItems.RemoveAt(index);
                        break;
                    }

                    index++;
                }
            });
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Update the drive information contained in the drive dropdown asynchronously.
        /// </summary>
        public void UpdateParentListing() {
            // Queue all of the resource retrievers onto the threadpool and set up the callback method.
            ThreadPool.QueueUserWorkItem(updateParentListing_Callback, Settings.Instance.Resources);
        }

        /// <summary>
        /// The callback that inserts the information retrieved from the resource retriever into the parent dropdown.
        /// </summary>
        /// <param name="stateInfo">The resource retriever.</param>
        private void updateParentListing_Callback(object stateInfo) {
            lock (lockObject) {
                if (stateInfo is List<IResource>) {
                    List<IResource> resources = (List<IResource>)stateInfo;

                    // Make sure that the handle is created before invoking the updater.
                    while (!handleCreated) {
                        System.Threading.Thread.Sleep(100);
                    }

                    this.BeginInvoke((MethodInvoker)delegate {
                        bool isLocalDiskFound = false;

                        // Create a new menu item in the dropdown for each drive.
                        foreach (IResource resource in resources) {
                            if (!tlsDrives.DropDownItems.ContainsKey(resource.FullPath)) {
                                ToolStripMenuItem item = new ToolStripMenuItem();
                                item.Text = resource.DisplayName;
                                item.Name = resource.FullPath;
                                item.Tag = resource;

                                int imageIndex = GetImageIndex(resource);
                                if (imageIndex > -1) {
                                    item.Image = ImageList.Images[imageIndex];
                                }

                                tlsDrives.DropDownItems.Add(item);

                                // Gets information about the path already specified or the drive currently being added.
                                if (!isLocalDiskFound) {
                                    // If the path has been defined and it is valid, then grab information about it.
                                    if (!string.IsNullOrEmpty(Path)) {
                                        childResourceRetrievers = resource.ChildResourceRetrievers.Clone();

                                        foreach (IChildResourceRetriever childResourceRetriever in childResourceRetrievers) {
                                            IChildResource pathResource = ChildResourceFactory.GetChildResource(Path);

                                            if (pathResource != null &&
                                                pathResource.Root.FullPath.ToLower().Equals(resource.FullPath.ToLower())) {
                                                isLocalDiskFound = true;

                                                pathResource.Execute(view);
                                                highlightParentResource(resource.Root, item.Image);
                                            }

                                            break;
                                        }
                                    }

                                    // If there is no defined path to retrieve, then attempt to get 
                                    // information about the the first drive found tha tis local and ready.
                                    if (!isLocalDiskFound && resource is SharpFile.IO.ParentResources.DriveInfo) {
                                        SharpFile.IO.ParentResources.DriveInfo driveInfo =
                                            (SharpFile.IO.ParentResources.DriveInfo)resource;

                                        if (driveInfo.DriveType == DriveType.Fixed &&
                                            driveInfo.IsReady) {
                                            isLocalDiskFound = true;

                                            resource.Execute(view);
                                            highlightParentResource(resource, item.Image);
                                        }
                                    }
                                }
                            }
                        }

                        if (childResourceRetrievers == null &&
                            resources != null &&
                            resources.Count > 0) {
                            childResourceRetrievers = resources[0].ChildResourceRetrievers.Clone();
                        }
                    });
                }
            }
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Highlights the passed-in drive.
        /// </summary>
        private void highlightParentResource(IResource resource, Image image) {
            foreach (ToolStripItem item in tlsDrives.DropDownItems) {
                IResource parentResource = ((IResource)item.Tag);

                if (parentResource.FullPath == resource.FullPath) {
                    item.BackColor = SystemColors.HighlightText;

                    tlsDrives.Image = image;
                    tlsDrives.Tag = resource;
                } else {
                    item.BackColor = SystemColors.Control;
                }
            }
        }
        #endregion

        #region Properties.
        /// <summary>
        /// Shared ImageList from the parent form.
        /// </summary>
        public ImageList ImageList {
            get {
                if (imageList == null) {
                    imageList = Forms.GetPropertyInParent<ImageList>(this.Parent, "ImageList");
                }

                return imageList;
            }
        }

        /// <summary>
        /// Drive detector.
        /// </summary>
        public DriveDetector DriveDetector {
            get {
                // TODO: This drive detector should really call the parent's (BaseParent) DriveDetector.
                driveDetector = new DriveDetector();

                if (driveDetector == null) {
                    driveDetector = Forms.GetPropertyInParent<DriveDetector>(this.Parent, "DriveDetector");
                }

                return driveDetector;
            }
        }

        /// <summary>
        /// The current path.
        /// </summary>
        public string Path {
            get {
                if (string.IsNullOrEmpty(this.tlsPath.Text)) {
                    if (ParentResource == null ||
                        string.IsNullOrEmpty(ParentResource.FullPath)) {
                        this.tlsPath.Text = @"C:\";

                        Settings.Instance.Logger.Log(LogLevelType.Verbose,
                            @"Path is null for {0}; assume C:\ is valid.", Name);
                    } else {
                        this.tlsPath.Text = ParentResource.FullPath;
                    }
                }

                return this.tlsPath.Text;
            }
            set {
                if (value != null) {
                    string path = value;

                    IChildResource resource = ChildResourceFactory.GetChildResource(path);

                    if (resource is DirectoryInfo && 
                        !path.EndsWith(@"\")) {
                        path = string.Format(@"{0}\",
                            path);
                    }

                    this.tlsPath.Text = path;
                }
            }
        }

        public string Filter {
            get {
                return tlsFilter.Text;
            }
        }

        public bool ShowFilter {
            get {
                return tlsFilter.Visible;
            }
            set {
                bool isVisible = value;

                if (!isVisible) {
                    tlsFilter.Visible = false;
                    tlsFilter.Width = 0;
                } else {
                    tlsFilter.Width = filterWidth;
                    tlsFilter.Visible = true;
                }
            }
        }

        /// <summary>
        /// The current FileSystemWatcher.
        /// </summary>
        public FileSystemWatcher FileSystemWatcher {
            get {
                return fileSystemWatcher;
            }
        }

        /// <summary>
        /// The currently selected drive.
        /// </summary>
        public IResource ParentResource {
            get {
                return (IResource)tlsDrives.Tag;
            }
        }

        /// <summary>
        /// The view.
        /// </summary>
        public IView View {
            get {
                return view;
            }
            set {
                IView newView = value;
                newView.GetImageIndex += view.OnGetImageIndex;
                newView.UpdatePath += view.OnUpdatePath;
                newView.UpdateProgress += view.OnUpdateProgress;
                newView.UpdateStatus += view.OnUpdateStatus;

                int index = this.Controls.IndexOf(view.Control);
                this.Controls.Remove(view.Control);

                view = newView;
                view.Control.Dock = DockStyle.Fill;

                this.Controls.Add(view.Control);
                this.Controls.SetChildIndex(view.Control, index);
            }
        }

        public ChildResourceRetrievers ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
        }
        #endregion
    }
}