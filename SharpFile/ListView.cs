using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using SharpFile.IO;
using SharpFile.IO.ChildResources;
using SharpFile.UI;
using DirectoryInfo = SharpFile.IO.ChildResources.DirectoryInfo;
using FileInfo = SharpFile.IO.ChildResources.FileInfo;
using IOException = SharpFile.IO.IOException;
using View = SharpFile.Infrastructure.View;
using System.Reflection;

namespace SharpFile {
    public class ListView : System.Windows.Forms.ListView, IView {
        private const int ALT = 32;
        private const int CTRL = 8;
        private const int SHIFT = 4;

        private UnitDisplay unitDisplay = UnitDisplay.Bytes;
        private IList<IChildResource> selectedFileSystemInfos = new List<IChildResource>();
        private long totalSelectedSize = 0;
        private Dictionary<string, ListViewItem> itemDictionary = new Dictionary<string, ListViewItem>();
        private IViewComparer comparer;
        private IEnumerable<ColumnInfo> columnInfos;

        public event View.OnUpdateStatusDelegate OnUpdateStatus;
        public event View.OnUpdateProgressDelegate OnUpdateProgress;
        public event View.OnGetImageIndexDelegate OnGetImageIndex;
        public event View.OnUpdatePathDelegate OnUpdatePath;
        public event View.OnCancelOperationsDelegate OnCancelOperations;

        public ListView() {
            // This prevents flicker in the listview. 
            // It is a protected property, so it is only available if we derive from ListView.
            this.DoubleBuffered = true;

            initializeComponent();
        }

        #region Private methods.
        private void initializeComponent() {
            this.DoubleClick += listView_DoubleClick;
            this.KeyDown += listView_KeyDown;
            this.MouseUp += listView_MouseUp;
            this.ItemDrag += listView_ItemDrag;
            this.DragOver += listView_DragOver;
            this.DragDrop += listView_DragDrop;
            this.KeyUp += listView_KeyUp;
            this.BeforeLabelEdit += listView_BeforeLabelEdit;
            this.AfterLabelEdit += listView_AfterLabelEdit;
            this.GotFocus += listView_GotFocus;
            this.ColumnClick += listView_ColumnClick;

            // Set some options on the listview.
            // TODO: This should be able to be set via dropdown/settings.
            this.View = System.Windows.Forms.View.Details;
            this.Dock = DockStyle.Fill;
            this.AllowColumnReorder = true;
            this.AllowDrop = true;
            this.FullRowSelect = true;
            this.LabelEdit = true;
            this.UseCompatibleStateImageBehavior = false;
            this.Sorting = SortOrder.Ascending;

            this.ListViewItemSorter = Comparer;
        }

        /// <summary>
        /// Calculates the size of the currently selected item.
        /// </summary>
        private void calculateSize() {
            if (this.SelectedItems != null &&
                        this.SelectedItems.Count > 0) {
                int maxIndex = 0;

                foreach (ListViewItem item in this.SelectedItems) {
                    IChildResource fileSystemInfo = (IChildResource)item.Tag;

                    if (item.Index > maxIndex) {
                        maxIndex = item.Index;
                    }

                    if (!selectedFileSystemInfos.Contains(fileSystemInfo)) {
                        item.ForeColor = Color.Red;
                        selectedFileSystemInfos.Add(fileSystemInfo);

                        int sizeIndex = 0;
                        foreach (ColumnHeader columnHeader in item.ListView.Columns) {
                            if (columnHeader.Text.Equals("Size")) {
                                sizeIndex = columnHeader.Index;
                            }
                        }

                        if (sizeIndex > 0) {
                            long size = 0;

                            if (string.IsNullOrEmpty(item.SubItems[sizeIndex].Text) ||
                                fileSystemInfo is DirectoryInfo) {
                                using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
                                    backgroundWorker.WorkerReportsProgress = true;

                                    backgroundWorker.DoWork += delegate(object anonymousSender, DoWorkEventArgs eventArgs) {
                                        backgroundWorker.ReportProgress(50);
                                        item.SubItems[sizeIndex].Text = "...";
                                        eventArgs.Result = ((DirectoryInfo)eventArgs.Argument).GetSize();
                                        this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.HeaderSize);
                                        backgroundWorker.ReportProgress(100);
                                    };

                                    backgroundWorker.ProgressChanged += delegate(object anonymousSender, ProgressChangedEventArgs eventArgs) {
                                        UpdateProgress(eventArgs.ProgressPercentage);
                                    };

                                    backgroundWorker.RunWorkerCompleted +=
                                        delegate(object anonymousSender, RunWorkerCompletedEventArgs eventArgs) {
                                            if (eventArgs.Error == null &&
                                                eventArgs.Result != null) {
                                                size = (long)eventArgs.Result;

                                                item.SubItems[sizeIndex].Text = General.GetHumanReadableSize(size.ToString());
                                                updateSelectedTotalSize(size);
                                                this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.HeaderSize);
                                            }
                                        };

                                    backgroundWorker.RunWorkerAsync(fileSystemInfo);
                                }
                            } else {
                                updateSelectedTotalSize(fileSystemInfo.Size);
                            }
                        }
                    } else {
                        item.ForeColor = Color.Black;
                        selectedFileSystemInfos.Remove(fileSystemInfo);
                        updateSelectedTotalSize(-fileSystemInfo.Size);
                    }

                    item.Focused = false;
                    item.Selected = false;
                }

                int nextIndex = maxIndex + 1;
                if (this.Items.Count > nextIndex) {
                    this.Items[nextIndex].Focused = true;
                    this.Items[nextIndex].Selected = true;
                }
            }
        }

        /// <summary>
        /// Executes the currently selected item.
        /// </summary>
        private void execute() {
            if (this.SelectedItems.Count > 0) {
                IChildResource resource = this.SelectedItems[0].Tag as IChildResource;

                if (resource != null) {
                    CancelOperations();

                    resource.Execute(this);
                }
            }
        }
        #endregion

        #region Delegate methods
        /// <summary>
        /// Passes the status to any listening events.
        /// </summary>
        /// <param name="status">Status to show.</param>
        protected void UpdateStatus(string status) {
            if (OnUpdateStatus != null) {
                OnUpdateStatus(status);
            }
        }

        /// <summary>
        /// Passes the value to any listening events.
        /// </summary>
        /// <param name="value">Percentage value for status.</param>
        public void UpdateProgress(int value) {
            if (OnUpdateProgress != null) {
                OnUpdateProgress(value);
            }
        }

        /// <summary>
        /// Passes the path to any listening events.
        /// </summary>
        /// <param name="path">Path to update.</param>
        public void UpdatePath(string path) {
            if (OnUpdatePath != null) {
                OnUpdatePath(path);
            }
        }

        /// <summary>
        /// Passes the filesystem info to any listening events.
        /// </summary>
        /// <param name="fsi"></param>
        /// <returns></returns>
        protected int GetImageIndex(IResource fsi) {
            if (OnGetImageIndex != null) {
                return OnGetImageIndex(fsi);
            }

            return -1;
        }

        /// <summary>
        /// Passes the cancel operation action to any listening events.
        /// </summary>
        protected void CancelOperations() {
            if (OnCancelOperations != null) {
                OnCancelOperations();
            }
        }
        #endregion

        #region Events.
        void listView_ColumnClick(object sender, ColumnClickEventArgs e) {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == Comparer.ColumnIndex) {
                // Reverse the current sort direction for this column.
                if (Comparer.Order == Order.Ascending) {
                    Comparer.Order = Order.Descending;
                } else {
                    Comparer.Order = Order.Ascending;
                }
            } else {
                // Set the column number that is to be sorted; default to ascending.
                Comparer.ColumnIndex = e.Column;
                Comparer.Order = Order.Ascending;
            }

            // Perform the sort with these new sort options.
            this.Sort();
        }

        /// <summary>
        /// Fires when the list view gets focus.
        /// </summary>
        private void listView_GotFocus(object sender, EventArgs e) {
            UpdatePath(Path);
        }

        /// <summary>
        /// Refreshes the listview when a file/directory is double-clicked in the listview.
        /// </summary>
        private void listView_DoubleClick(object sender, EventArgs e) {
            execute();
        }

        /// <summary>
        /// Selects an item in the listview when the Space bar is hit.
        /// </summary>
        private void listView_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Space:
                    calculateSize();
                    break;
                case Keys.Escape:
                    CancelOperations();
                    UpdateProgress(100);
                    break;
                case Keys.Enter:
                    execute();
                    break;
            }
        }

        /// <summary>
        /// Displays the right-click context menu.
        /// </summary>
        private void listView_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ShellContextMenu menu = new ShellContextMenu();

                if (this.SelectedItems.Count > 1) {
                    List<string> paths = getSelectedPaths();
                    menu.PopupMenu(paths, this.Handle);
                } else if (this.SelectedItems.Count == 1) {
                    menu.PopupMenu(this.SelectedItems[0].Name, this.Handle);
                } else {
                    menu.PopupMenu(Path, this.Handle);
                }
            }
        }

        /// <summary>
        /// Performs the necessary action when a file is dropped on the form.
        /// </summary>
        private void listView_DragDrop(object sender, DragEventArgs e) {
            // Can only drop files.
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                return;
            }

            string[] fileDrops = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileDrop in fileDrops) {
                IChildResource resource = ChildResourceFactory.GetChildResource(fileDrop);

                if (resource != null) {
                    string destination = string.Format(@"{0}{1}",
                                                       Path,
                                                       resource.Name);

                    try {
                        switch (e.Effect) {
                            case DragDropEffects.Copy:
                                resource.Copy(destination);
                                break;
                            case DragDropEffects.Move:
                                resource.Move(destination);
                                break;
                            case DragDropEffects.Link:
                                // TODO: Need to handle links.
                                break;
                        }
                    } catch (IOException ex) {
                        MessageBox.Show(this, "Failed to perform the specified operation:\n\n" + ex.Message, "File operation failed",
                                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    } catch (Exception ex) {
                        MessageBox.Show(this, "Shit went down:\n\n" + ex.Message, "Oh no.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        /// <summary>
        /// Performs action neccessary to allow drags from listview.
        /// </summary>
        private void listView_DragOver(object sender, DragEventArgs e) {
            // Determine whether file data exists in the drop data. If not, then
            // the drop effect reflects that the drop cannot occur.
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.None;
                return;
            }

            // Set the effect based upon the KeyState.
            // Can't get links to work - Use of Ole1 services requiring DDE windows is disabled.
            /*
            if ((e.KeyState & (CTRL | ALT)) == (CTRL | ALT) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) {
                e.Effect = DragDropEffects.Link;
            } else if ((e.KeyState & ALT) == ALT &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) {
                e.Effect = DragDropEffects.Link;
            } else*/
            if ((e.KeyState & SHIFT) == SHIFT &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                e.Effect = DragDropEffects.Move;
            } else if ((e.KeyState & CTRL) == CTRL &&
                     (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                e.Effect = DragDropEffects.Copy;
            } else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;

                // Implement the rather strange behaviour of explorer that if the disk
                // is different, then default to a COPY operation
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0) {
                    IChildResource fileResource = ChildResourceFactory.GetChildResource(files[0]);
                    IChildResource pathResource = ChildResourceFactory.GetChildResource(Path);

                    if (!fileResource.Root.FullPath.Equals(pathResource.Root.FullPath)) {
                        if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    } else if (fileResource.Path.Equals(pathResource.FullPath)) {
                        e.Effect = DragDropEffects.None;
                    }
                }
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Performs a drag operation.
        /// </summary>
        private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
            List<string> paths = getSelectedPaths();

            if (paths.Count > 0) {
                DoDragDrop(new DataObject(DataFormats.FileDrop, paths.ToArray()),
                           DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }
        }

        /// <summary>
        /// Performs actions based on the key pressed.
        /// </summary>
        private void listView_KeyUp(object sender, KeyEventArgs e) {
            // TODO: Should be specified by config.
            if (e.KeyCode == Keys.F2) {
                if (this.SelectedItems.Count > 0) {
                    ListViewItem item = this.SelectedItems[0];

                    if (!(item.Tag is ParentDirectoryInfo) && !(item.Tag is RootDirectoryInfo)) {
                        item.BeginEdit();
                    }
                }
            }
        }

        /// <summary>
        /// Make sure that the item being edited is valid.
        /// </summary>
        private void listView_BeforeLabelEdit(object sender, LabelEditEventArgs e) {
            ListViewItem item = this.Items[e.Item];
            IResource resource = (IResource)item.Tag;

            if (item.Tag is ParentDirectoryInfo ||
                item.Tag is RootDirectoryInfo) {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// Renames the file/directory that was being edited.
        /// </summary>
        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (!string.IsNullOrEmpty(e.Label)) {
                ListViewItem item = this.Items[e.Item];
                IChildResource resource = (IChildResource)item.Tag;

                if (!(item.Tag is ParentDirectoryInfo) && !(item.Tag is RootDirectoryInfo)) {
                    string destination = string.Format("{0}{1}",
                                                       Path,
                                                       e.Label);

                    try {
                        resource.Move(destination);
                    } catch (Exception ex) {
                        e.CancelEdit = true;
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public void ShowMessageBox(string text) {
            MessageBox.Show(text);
        }

        /// <summary>
        /// Cancels the child retriever operations.
        /// </summary>
        public void CancelChildRetrieverOperations() {
            foreach (ListViewItem item in itemDictionary.Values) {
                IResource resource = (IResource)item.Tag;

                if (resource.ChildResourceRetriever != null) {
                    resource.ChildResourceRetriever.Cancel();
                }
            }
        }

        /// <summary>
        /// Clears the listview.
        /// </summary>
        public void ClearView() {
            this.Items.Clear();
            this.itemDictionary.Clear();
        }

        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveItem(string path) {
            this.Items.RemoveByKey(path);
            this.itemDictionary.Remove(path);
        }

        /// <summary>
        /// Parses the file/directory information and updates the listview.
        /// </summary>
        public void AddItemRange(IEnumerable<IChildResource> resources) {
            if (this.SmallImageList == null) {
                this.SmallImageList = Forms.GetPropertyInParent<ImageList>(this.Parent, "ImageList");
            }

            int fileCount = 0;
            int folderCount = 0;
            StringBuilder sb = new StringBuilder();

            // Create a new listview item with the display name.
            foreach (IChildResource resource in resources) {
                try {
                    addItem(resource, ref fileCount, ref folderCount);
                } catch (Exception ex) {
                    sb.AppendFormat("{0}: {1}",
                        resource.FullPath,
                        ex.Message);
                }
            }

            if (sb.Length > 0) {
                MessageBox.Show(sb.ToString());
            }

            Comparer.ColumnIndex = 0;
            Comparer.Order = Order.Ascending;
            this.Sort();

            // Resize the columns based on the column content.
            this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            UpdateStatus(string.Format("Folders: {0}; Files: {1}",
                                       folderCount,
                                       fileCount));
        }

        /// <summary>
        /// Parses the file/directory information and inserts the file info into the listview.
        /// </summary>
        public void InsertItem(IChildResource resource) {
            int fileCount = 0;
            int folderCount = 0;

            if (resource != null) {
                try {
                    // Create a new listview item with the display name.
                    addItem(resource, ref fileCount, ref folderCount);

                    // Basic stuff that should happen everytime files are shown.
                    this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                    UpdateStatus(string.Format("Folders: {0}; Files: {1}",
                                               folderCount,
                                               fileCount));

                    this.Sort();
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Adds the item to the view.
        /// </summary>
        /// <param name="resource">Resource to add.</param>
        /// <param name="fileCount">Count of files.</param>
        /// <param name="folderCount">Count of folders.</param>
        private void addItem(IChildResource resource, ref int fileCount, ref int folderCount) {
            if (!itemDictionary.ContainsKey(resource.FullPath)) {
                ListViewItem item = createListViewItem(resource, ref fileCount, ref folderCount);
                itemDictionary.Add(resource.FullPath, item);
                this.Items.Add(item);
            }
        }

        /// <summary>
        /// Create listview from the filesystem information.
        /// </summary>
        /// <param name="fileSystemInfo">Filesystem information.</param>
        /// <returns>Listview item that references the filesystem object.</returns>
        private ListViewItem createListViewItem(IChildResource resource, ref int fileCount, ref int folderCount) {
            ListViewItem item = new ListViewItem();
            item.Tag = resource;
            item.Name = resource.FullPath;

            foreach (ColumnInfo columnInfo in ColumnInfos) {
                PropertyInfo propertyInfo = resource.GetType().GetProperty(columnInfo.Property);

                if (propertyInfo != null) {
                    string text = propertyInfo.GetValue(resource, null).ToString();

                    // The original value will be set on the tag for sort-ability.
                    string tag = text;

                    if (columnInfo.MethodDelegate != null) {
                        text = columnInfo.MethodDelegate.Invoke(text);
                    }

                    if (columnInfo.PrimaryColumn) {
                        item.Text = text;
                        item.SubItems[0].Tag = tag;
                    } else {
                        System.Windows.Forms.ListViewItem.ListViewSubItem listViewSubItem = 
                            new ListViewItem.ListViewSubItem();
                        listViewSubItem.Text = text;
                        listViewSubItem.Tag = tag;

                        item.SubItems.Add(listViewSubItem);
                    }
                }
            }

            int imageIndex = GetImageIndex(resource);
            item.ImageIndex = imageIndex;

            return item;
        }

        /// <summary>
        /// Gets the selected paths.
        /// </summary>
        private List<string> getSelectedPaths() {
            if (this.SelectedItems.Count == 0) {
                return new List<string>(0);
            }

            // Get an array of the listview items.
            ListViewItem[] itemArray = new ListViewItem[this.SelectedItems.Count];
            this.SelectedItems.CopyTo(itemArray, 0);

            // Convert the listviewitem array into a string array of the paths.
            string[] nameArray = Array.ConvertAll<ListViewItem, string>(itemArray, delegate(ListViewItem item) {
                return item.Name;
            });

            return new List<string>(nameArray);
        }

        /// <summary>
        /// Update the selected file sytem objects' total size.
        /// </summary>
        /// <param name="size"></param>
        private void updateSelectedTotalSize(long size) {
            totalSelectedSize += size;

            UpdateStatus(string.Format("Selected items: {0}",
                                       General.GetHumanReadableSize(totalSelectedSize.ToString())));
        }

        #endregion

        /// <summary>
        /// Current path.
        /// </summary>
        public string Path {
            get {
                return Forms.GetPropertyInParent<string>(this.Parent, "Path");
            }
        }

        /// <summary>
        /// Current filter.
        /// </summary>
        public string Filter {
            get {
                return Forms.GetPropertyInParent<string>(this.Parent, "Filter");
            }
        }

        /// <summary>
        /// Current FileSystemWatcher.
        /// </summary>
        public FileSystemWatcher FileSystemWatcher {
            get {
                return Forms.GetPropertyInParent<FileSystemWatcher>(this.Parent, "FileSystemWatcher");
            }
        }

        /// <summary>
        /// Current drive.
        /// </summary>
        public IResource DriveInfo {
            get {
                return Forms.GetPropertyInParent<IResource>(this.Parent, "ParentResource");
            }
        }

        public Control Control {
            get {
                return this;
            }
        }

        public IViewComparer Comparer {
            get {
                if (comparer == null) {
                    comparer = new ListViewItemComparer();
                }

                return comparer;
            }
        }

        public IEnumerable<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                if (columnInfos == null) {
                    columnInfos = value;

                    foreach (ColumnInfo columnInfo in columnInfos) {
                        ColumnHeader columnHeader = new ColumnHeader();
                        columnHeader.Text = columnInfo.Text;
                        columnHeader.Tag = columnInfo;
                        this.Columns.Add(columnHeader);
                    }
                }
            }
        }
    }
}