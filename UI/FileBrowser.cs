using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;
using SharpFile.UI;
using View = SharpFile.Infrastructure.View;

namespace SharpFile {
    public class FileBrowser : TabPage {
        private const int filterWidth = 50;
        private static readonly object lockObject = new object();

        private FileSystemWatcher fileSystemWatcher;
        private ImageList imageList;
        private DriveDetector driveDetector;
        private ChildResourceRetrievers childResourceRetrievers;
        private bool handleCreated = false;
        private bool isExecuting = false;
        private bool isFiltering = false;
        private FormatTemplate driveFormatTemplate;

        private ToolStrip toolStrip;
        private ToolStripSplitButton tlsDrives;
        private ToolStripSpringTextBox tlsPath;
        private ToolStripTextBox tlsFilter;
        private IView view;

        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;

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
            this.toolStrip.RenderMode = ToolStripRenderMode.System;
            this.toolStrip.ShowItemToolTips = false;
            this.toolStrip.TabIndex = 1;

            this.tlsDrives.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tlsDrives.ImageTransparentColor = Color.Magenta;
            this.tlsDrives.Overflow = ToolStripItemOverflow.Never;

            this.tlsPath.AutoSize = true;
            this.tlsPath.Overflow = ToolStripItemOverflow.Never;
            this.tlsPath.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.tlsPath.AutoCompleteSource = AutoCompleteSource.FileSystem;

            this.tlsFilter.AutoSize = false;
            this.tlsFilter.Size = new Size(filterWidth, 25);
            this.tlsFilter.Overflow = ToolStripItemOverflow.Never;

            this.Controls.Add(this.view.Control);
            this.Controls.Add(this.toolStrip);
            this.ForeColor = SystemColors.ControlText;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

            // Wire up the file system watcher.
            fileSystemWatcher = new FileSystemWatcher(this, 100);
            fileSystemWatcher.Changed += fileSystemWatcher_Changed;

            // Wire up the drive detector.
            DriveDetector.DeviceArrived += driveDetector_DeviceArrived;
            DriveDetector.DeviceRemoved += driveDetector_DeviceRemoved;
        }

        /// <summary>
        /// Clears the filter.
        /// </summary>
        private void clearFilter() {
            tlsFilter.Text = "*.*";
            isFiltering = false;
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
                IResource resource = FileSystemInfoFactory.GetFileSystemInfo(Path);

                if (resource != null) {
                    execute(resource);
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
            if (e.KeyCode != Keys.Left &&
                e.KeyCode != Keys.Right &&
                e.KeyCode != Keys.Shift) {
                isFiltering = true;

                if (e.KeyCode == Keys.Enter) {
                    isFiltering = false;
                }

                IResource resource = FileSystemInfoFactory.GetFileSystemInfo(Path);
                execute(resource);
            }
        }

        /// <summary>
        /// Clears the filter when the focus is lost.
        /// </summary>
        private void tlsFilter_LostFocus(object sender, EventArgs e) {
            if (!isFiltering) {
                if (Filter.Equals(string.Empty) ||
                    Filter.Equals("*") ||
                    Filter.Equals("*.*")) {
                    clearFilter();
                }
            }
        }

        /// <summary>
        /// Refreshes the view when a different drive is selected.
        /// </summary>
        private void tlsDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            DriveInfo driveInfo = (DriveInfo)e.ClickedItem.Tag;
            execute(driveInfo);
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
            IResource resource = FileSystemInfoFactory.GetFileSystemInfo(path);

            if (view.Control.IsHandleCreated) {
                // Required to ensure the view update occurs on the calling thread.
                view.Control.BeginInvoke((MethodInvoker)delegate {
                    view.BeginUpdate();

                    switch (e.ChangeType) {
                        case System.IO.WatcherChangeTypes.Changed:
                            if (resource is IChildResource) {
                                view.RemoveItem(path);
                                view.AddItem((IChildResource)resource);
                            }
                            break;
                        case System.IO.WatcherChangeTypes.Created:
                            if (resource is IChildResource) {
                                view.AddItem((IChildResource)resource);
                            }
                            break;
                        case System.IO.WatcherChangeTypes.Deleted:
                            view.RemoveItem(path);
                            break;
                        case System.IO.WatcherChangeTypes.Renamed:
                            if (resource is IChildResource) {
                                string oldFullPath = ((System.IO.RenamedEventArgs)e).OldFullPath;
                                view.RemoveItem(oldFullPath);
                                view.AddItem((IChildResource)resource);
                            }
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
            UpdateParentListing();
        }

        /// <summary>
        /// Fires when a drive is removed.
        /// </summary>
        private void driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e) {
            this.BeginInvoke((MethodInvoker)delegate {
                int index = 0;

                foreach (ToolStripItem item in tlsDrives.DropDownItems) {
                    if (item.Name.Equals(e.Drive, StringComparison.OrdinalIgnoreCase)) {
                        string pathRoot = Path.Substring(0, Path.IndexOf(':'));
                        string drive = e.Drive.Substring(0, e.Drive.IndexOf(':'));

                        if (pathRoot.Equals(drive, StringComparison.OrdinalIgnoreCase)) {
                            // Update the path to the local drive.
                            foreach (ToolStripItem i in tlsDrives.DropDownItems) {
                                DriveInfo driveInfo = (DriveInfo)i.Tag;

                                if (driveInfo.IsReady &&
                                    driveInfo.DriveType == System.IO.DriveType.Fixed) {
                                    execute(driveInfo);
                                    break;
                                }
                            }
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
            ThreadPool.QueueUserWorkItem(updateParentListing_Callback);
        }

        /// <summary>
        /// The callback that inserts the information retrieved from the resource retriever into the parent dropdown.
        /// </summary>
        /// <param name="stateInfo">The resource retriever.</param>
        private void updateParentListing_Callback(object stateInfo) {
            lock (lockObject) {
                // Make sure that the handle is created before invoking the updater.
                while (!handleCreated) {
                    System.Threading.Thread.Sleep(100);
                }

                this.BeginInvoke((MethodInvoker)delegate {
                    bool isLocalDiskFound = false;

                    // Create a new menu item in the dropdown for each drive.
                    foreach (IParentResource resource in Settings.Instance.ParentResources) {
                        if (!tlsDrives.DropDownItems.ContainsKey(resource.Name)) {
                            ToolStripMenuItem item = new ToolStripMenuItem();
                            string text = resource.Name;

                            if (resource.IsReady && !string.IsNullOrEmpty(driveFormatTemplate.Template)) {
                                Templater templater = new Templater(resource);
                                text = templater.Generate(driveFormatTemplate.Template);
                            }

                            if (driveFormatTemplate.MethodDelegate != null) {
                                text = driveFormatTemplate.MethodDelegate(text);
                            }

                            item.Text = text;

                            item.Name = resource.Name;
                            item.Tag = resource;

                            int imageIndex = OnGetImageIndex(resource);
                            if (imageIndex > -1) {
                                item.Image = ImageList.Images[imageIndex];
                            }

                            tlsDrives.DropDownItems.Add(item);

                            // Gets information about the path already specified or the drive currently being added.
                            if (!isLocalDiskFound) {
                                // If the path has been defined and it is valid, then grab information about it.
                                if (!string.IsNullOrEmpty(Path)) {
                                    childResourceRetrievers = resource.GetChildResourceRetrievers().Clone();

                                    foreach (IChildResourceRetriever childResourceRetriever in childResourceRetrievers) {
                                        IResource pathResource = FileSystemInfoFactory.GetFileSystemInfo(Path);

                                        if (pathResource is IChildResource) {
                                            IChildResource childResource = (IChildResource)pathResource;

                                            if (childResource.Root.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase)) {
                                                isLocalDiskFound = true;
                                            }
                                        } else if (pathResource is IParentResource) {
                                            IParentResource parentResource = (IParentResource)resource;

                                            if (parentResource != null &&
												parentResource.Name.Equals(resource.Name, StringComparison.OrdinalIgnoreCase) &&
												parentResource.DriveType == System.IO.DriveType.Fixed &&
												parentResource.IsReady) {
                                                isLocalDiskFound = true;
                                            }
                                        }

                                        execute(resource);
                                        break;
                                    }
                                }

                                // If there is no defined path to retrieve, then attempt to get 
                                // information about the the first drive found that is local and ready.
                                if (!isLocalDiskFound) {
                                    if (resource.DriveType == System.IO.DriveType.Fixed &&
                                        resource.IsReady) {
                                        isLocalDiskFound = true;
										execute(resource);
                                    }
                                }
                            }
                        } else {
                            // The drive dropdown already contained the drive trying to be added, so assume that a local drive has already been found.
                            isLocalDiskFound = true;
                        }
                    }

                    // If nothing has been found yet, but there are parent resources, then just execute the first known one.
                    if (childResourceRetrievers == null &&
                        Settings.Instance.ParentResources != null &&
                        Settings.Instance.ParentResources.Count > 0) {
                        childResourceRetrievers = Settings.Instance.ParentResources[0].GetChildResourceRetrievers().Clone();
                    }
                });
            }
        }
        #endregion

        #region Private methods.
        private void execute(IResource resource) {
            Image image = null;
            IParentResource parentResource = null;

            // Determine the correct parent resource to be highlighted.
            if (resource is IParentResource) {
                parentResource = (IParentResource)resource;
            } else if (resource is IChildResource) {
                parentResource = ((IChildResource)resource).Root;
            }

            if (parentResource.IsReady) {
                resource.Execute(view);

                // Determine the correct image to be highlighted.
                foreach (ToolStripItem item in this.tlsDrives.DropDownItems) {
                    if (resource.FullName.ToLower().Contains(((IParentResource)item.Tag).FullName.ToLower())) {
                        image = item.Image;
                        break;
                    }
                }

                // Once the resources have been retrieved enable some controls and highlight the correct parent resource.
                foreach (IChildResourceRetriever childResourceRetriever in resource.GetChildResourceRetrievers()) {
                    childResourceRetriever.GetComplete += delegate {
                        highlightParentResource(parentResource, image);
                    };
                }
            } else {
                tlsDrives.HideDropDown();

                Settings.Instance.Logger.ProcessContent += view.ShowMessageBox;
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, 
                    "It appears the drive, {0}, is not ready.", parentResource.FullName);
                Settings.Instance.Logger.ProcessContent -= view.ShowMessageBox;
            }
        }

        /// <summary>
        /// Highlights the passed-in drive.
        /// </summary>
        private void highlightParentResource(IParentResource resource, Image image) {
            foreach (ToolStripItem item in tlsDrives.DropDownItems) {
                IParentResource parentResource = (IParentResource)item.Tag;

                if (parentResource.FullName.Equals(resource.FullName, StringComparison.OrdinalIgnoreCase)) {
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
                        string.IsNullOrEmpty(ParentResource.Name)) {
                        this.tlsPath.Text = @"C:\";

                        Settings.Instance.Logger.Log(LogLevelType.Verbose,
                            @"Path is null for {0}; assume C:\ is valid.", Name);
                    } else {
                        this.tlsPath.Text = ParentResource.Name;
                    }
                }

                if (this.tlsPath.Text.Contains(":") &&
                    !this.tlsPath.Text.Contains(FileSystemInfo.DirectorySeparator)) {
                    this.tlsPath.Text += FileSystemInfo.DirectorySeparator;
                } else if (!this.tlsPath.Text.Contains(@":\")) {
                    this.tlsPath.Text += @":\";
                } else if (this.tlsPath.Text.EndsWith("/")) {
                    this.tlsPath.Text = this.tlsPath.Text.Replace("/", FileSystemInfo.DirectorySeparator);
                }

                return this.tlsPath.Text;
            }
            set {
                if (value != null) {
                    string path = value;

                    IResource resource = FileSystemInfoFactory.GetFileSystemInfo(path);

                    if (resource is DirectoryInfo &&
                        !path.EndsWith(FileSystemInfo.DirectorySeparator)) {
                        path = string.Format(@"{0}{1}",
                            path,
                            FileSystemInfo.DirectorySeparator);
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
                    // Need to set the width to 0 so that the path resizes correctly.
                    tlsFilter.Width = 0;
                    tlsFilter.Visible = false;          
                } else {
                    // Need to set the width so that the path resizes correctly.
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
        public IParentResource ParentResource {
            get {
                return (IParentResource)tlsDrives.Tag;
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

        public bool Executing {
            get {
                return isExecuting;
            }
            set {
                isExecuting = value;

                if (isExecuting) {
                    tlsDrives.Enabled = false;
                    tlsPath.Enabled = false;
                    tlsFilter.Enabled = false;
                } else {
                    tlsDrives.Enabled = true;
                    tlsPath.Enabled = true;
                    tlsFilter.Enabled = true;
                }

                // If we were filtering the selection, focus the textbox correctly again. 
                // Otherwise the disabling/enabling the textbox kills its focus.
                if (isFiltering) {
                    tlsFilter.Focus();
                }
            }
        }

        public FormatTemplate DriveFormatTemplate {
            get {
                return driveFormatTemplate;
            }
            set {
                driveFormatTemplate = value;
            }
        }
        #endregion
    }
}