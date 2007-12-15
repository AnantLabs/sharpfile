using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpFile.IO;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;
using SharpFile.IO.Retrievers;
using DriveInfo=SharpFile.IO.ParentResources.DriveInfo;
using DriveType=SharpFile.IO.DriveType;
using View=SharpFile.Infrastructure.View;
using SharpFile.Infrastructure;
using Common;
using SharpFile.UI;

namespace SharpFile {
    public class FileBrowser : TabPage {
        private static readonly object lockObject = new object();

        private FileSystemWatcher fileSystemWatcher;
        private ImageList imageList;
        private DriveDetector driveDetector;
        private bool handleCreated = false;

        private ToolStrip toolStrip;
        private ToolStripSplitButton tlsDrives;
        private ToolStripSpringTextBox tlsPath;
        private ToolStripTextBox tlsFilter;
        private IView view;

        public event View.OnGetImageIndexDelegate OnGetImageIndex;
        public event View.OnUpdatePathDelegate OnUpdatePath;

        /// <summary>
        /// Filebrowser ctor.
        /// </summary>
        public FileBrowser() {
            this.toolStrip = new ToolStrip();
            this.tlsDrives = new ToolStripSplitButton();
            this.tlsPath = new ToolStripSpringTextBox();
            this.tlsFilter = new ToolStripTextBox();
            this.view = new ListView();
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
            this.tlsDrives.Size = new Size(32, 22);

            this.tlsPath.Size = new Size(100, 25);

            this.tlsFilter.Size = new Size(50, 25);

            this.Controls.Add(this.view.Control);
            this.Controls.Add(this.toolStrip);
            this.ForeColor = SystemColors.ControlText;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            DriveDetector.DeviceArrived += driveDetector_DeviceChanged;
            DriveDetector.DeviceRemoved += driveDetector_DeviceChanged;

            initializeComponent();
            UpdateParentListing();
        }

        public void CancelOperations() {
            //foreach (IResourceRetriever resourceRetriever in resourceRetrievers) {
            //    resourceRetriever.ChildResourceRetriever.Cancel();
            //}

            view.CancelChildRetrieverOperations();
        }

        #region Delegate methods
        /// <summary>
        /// Passes the filesystem info to any listening events.
        /// </summary>
        /// <returns>Image index.</returns>
        protected int GetImageIndex(IResource fsi) {
            if (OnGetImageIndex != null) {
                return OnGetImageIndex(fsi);
            }

            return -1;
        }

        protected void UpdatePath(string path) {
            if (OnUpdatePath != null) {
                OnUpdatePath(path);
            }

            this.Text = path;
            Path = path;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Sets the filebrowser up.
        /// </summary>
        private void initializeComponent() {
            this.DoubleBuffered = true;
            tlsFilter.Text = string.Empty;

            // Attach to some events.
            this.HandleCreated += fileBrowser_HandleCreated;
            this.tlsPath.KeyDown += tlsPath_KeyDown;
            this.tlsFilter.KeyUp += tlsFilter_KeyUp;
            this.tlsDrives.DropDownItemClicked += tlsDrives_DropDownItemClicked;
            this.tlsDrives.ButtonClick += tlsDrives_ButtonClick;
            this.view.OnUpdatePath += UpdatePath;
            this.view.OnCancelOperations += CancelOperations;

            fileSystemWatcher = new FileSystemWatcher(this, 100);
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;
            //fileSystemWatcher.Renamed += fileSystemWatcher_Changed;
            //fileSystemWatcher.Created += fileSystemWatcher_Changed;
            //fileSystemWatcher.Deleted += fileSystemWatcher_Changed;
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
                    MessageBox.Show("The path, " + Path + ", looks like it is incorrect.");
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
        /// Refreshes the view when a different drive is selected.
        /// </summary>
        private void tlsDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            IResource resource = (IResource)e.ClickedItem.Tag;
            resource.Execute(view);
            highlightParentResource(resource, e.ClickedItem.Image);
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

            // Required to ensure the view update occurs on the calling thread.
            MethodInvoker updater = delegate {
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
            };

            view.Control.BeginInvoke(updater);
        }

        /// <summary>
        /// Fires when a drive changes.
        /// </summary>
        private void driveDetector_DeviceChanged(object sender, DriveDetectorEventArgs e) {
            MethodInvoker updater = delegate {
                Settings.ClearResources();
                UpdateParentListing();
            };

            this.BeginInvoke(updater);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Update the drive information contained in the drive dropdown asynchronously.
        /// </summary>
        //public void UpdateParentListing(IEnumerable<IResourceRetriever> resourceRetrievers) {
        public void UpdateParentListing() {
            // Clear the dropdown.
            tlsDrives.DropDownItems.Clear();

            // Queue all of the resource retrievers onto the threadpool and set up the callback method.
            ThreadPool.QueueUserWorkItem(updateParentListing_Callback, Settings.Instance.Resources);
        }

        /// <summary>
        /// The callback that inserts the information retrieved fromt he resource retriever into the parent dropdown.
        /// </summary>
        /// <param name="stateInfo">The resource retriever.</param>
        private void updateParentListing_Callback(object stateInfo) {
            lock (lockObject) {
                if (stateInfo is List<IResource>) {
                    List<IResource> resources = (List<IResource>)stateInfo;

                    MethodInvoker updater = delegate {
                        bool isLocalDiskFound = false;

                        // Create a new menu item in the dropdown for each drive.
                        foreach (IResource resource in resources) {
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
                                    IChildResource pathResource = ChildResourceFactory.GetChildResource(Path);

                                    if (pathResource != null &&
                                        pathResource.Root.FullPath.Equals(resource.FullPath)) {
                                        isLocalDiskFound = true;

                                        highlightParentResource(resource.Root, item.Image);
                                        pathResource.Execute(view);
                                    }
                                }

                                // If the view hasn't been updated and the resource is a drive, then grab some information about it.
                                if (!isLocalDiskFound && resource is DriveInfo) {
                                    DriveInfo driveInfo = (DriveInfo)resource;

                                    if (driveInfo.DriveType == DriveType.Fixed &&
                                        driveInfo.IsReady) {
                                        isLocalDiskFound = true;

                                        highlightParentResource(resource, item.Image);
                                        resource.Execute(view);
                                    }
                                }
                            }
                        }
                    };

                    // Make sure that the handle is created before invoking the updater.
                    while (!handleCreated) {
                        System.Threading.Thread.Sleep(100);
                    }

                    this.BeginInvoke(updater);
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
                        // TODO: This shouldn't be hard-coded.
                        this.tlsPath.Text = @"c:\";
                    } else {
                        this.tlsPath.Text = ParentResource.FullPath;
                    }
                }

                return this.tlsPath.Text;
            }
            set {
                if (value != null) {
                    string path = value;

                    if (!path.EndsWith(@"\")) {
                        path += @"\";
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
        /// The child view.
        /// </summary>
        public IView View {
            get {
                return view;
            }
        }
        #endregion
    }
}