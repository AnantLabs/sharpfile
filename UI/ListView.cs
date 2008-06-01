using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.Logger;
using Common.WindowsApi;
using Common.WindowsApi.Messages;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.SettingsSection;
using SharpFile.IO;
using SharpFile.IO.ChildResources;
using View = SharpFile.Infrastructure.View;

namespace SharpFile.UI {
    public class ListView : System.Windows.Forms.ListView, IView {
        private static readonly object lockObject = new object();
        private const int ALT = 32;
        private const int CTRL = 8;
        private const int SHIFT = 4;

        private UnitDisplay unitDisplay = UnitDisplay.Bytes;
        private IList<IChildResource> selectedFileSystemInfos = new List<IChildResource>();
        private long totalSelectedSize = 0;
        private Dictionary<string, ListViewItem> itemDictionary = new Dictionary<string, ListViewItem>();
        private IViewComparer comparer = new ListViewItemComparer();
        private List<ColumnInfo> columnInfos;
        private long fileCount = 0;
        private long folderCount = 0;
        private int lastSelectedItemIndex = 0;
        private Dictionary<string, int> previousTopIndexes = new Dictionary<string, int>();

        public event View.UpdateStatusDelegate UpdateStatus;
        public event View.UpdateProgressDelegate UpdateProgress;
        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;
        public event View.UpdatePreviewPanelDelegate UpdatePreviewPanel;

        // TODO: This empty ListView ctor shouldn't be necessary, but the view can't be instantiated without it.
        public ListView()
            : this("Instantiated") {
        }

		public ListView(string name) {
			// This prevents flicker in the listview. 
			// It is a protected property, so it is only available if we derive from ListView.
			this.DoubleBuffered = true;
			this.Name = name;

			initializeComponent();
		}

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
            this.ItemSelectionChanged += listView_ItemSelectionChanged;

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
            this.ListViewItemSorter = comparer;

			// Enables the OnDrawItem override.
			this.OwnerDraw = true;
        }

        #region Protected methods.
        /// <summary>
        /// Executes the currently selected item.
        /// </summary>
        protected virtual void execute() {
            if (this.SelectedItems.Count > 0) {
                IChildResource resource = this.SelectedItems[0].Tag as IChildResource;

                if (resource != null) {
                    if (!(resource is FileInfo) && !(resource is RootDirectoryInfo) && !(resource is ParentDirectoryInfo)) {
                        string path = ((IChildResource)this.TopItem.Tag).Path.ToLower();

                        /*
                        int lastVisibleIndex = this.TopItem.Index;

                        for (int i = TopItem.Index + 1; i < Items.Count; i++) {
                            if (ClientRectangle.Contains(Items[i].Bounds)) {
                                lastVisibleIndex = i;
                            } else {
                                break;
                            }
                        }
                        */

                        //get the top of the bottom scrollbar
                        int bottomBound = this.ClientRectangle.Bottom;
                        //get the client height and divide by the top item's rectangle height
                        //this gives the number of visible items (plus one for the headerbar)
                        int itemsVisible =
                            (this.ClientRectangle.Height / this.TopItem.Bounds.Height) - 2;

                        // Subtract one item if the horizontal scrollbar is visible.
                        if (false) {
                            itemsVisible++;
                        }

                        int lastVisibleIndex = this.TopItem.Index + itemsVisible;

                        if (previousTopIndexes.ContainsKey(path)) {
                            previousTopIndexes[path] = lastVisibleIndex;
                        } else {
                            previousTopIndexes.Add(path, lastVisibleIndex);
                        }
                    }

                    resource.Execute(this);
                }
            }
        }

        #region Overrides.
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m) {
            if (m.Msg == (int)WM.COMMAND) {
                uint hWordWParam = ((uint)m.WParam.ToInt32() & 0xFFFF0000) >> 16;

                if (hWordWParam == (int)WM.CUT) {
                    try {
                        if (LabelEditHandle != null && LabelEditHandle != IntPtr.Zero) {
                            validateEditLabel(LabelEditHandle);
                        }
                    } catch (Exception ex) {
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Error while validating label.");
                    }
				}
			}

            base.WndProc(ref m);
        }

        /// <summary>
        /// Override the OnDrawSubItem method because OwnerDrawn is true.
        /// </summary>
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e) {
            e.DrawDefault = true;
            base.OnDrawSubItem(e);
        }

        /// <summary>
        /// Override the OnDrawColumnHeader method because OwnerDrawn is true.
        /// </summary>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e) {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

		/// <summary>
		/// Override the OnDrawItem method to get the item's image when it is drawn.
		/// </summary>
		protected override void OnDrawItem(DrawListViewItemEventArgs e) {
			e.DrawDefault = true;
			base.OnDrawItem(e);
		}
        #endregion
        #endregion

        #region Delegate methods
        /// <summary>
        /// Passes the status to any listening events.
        /// </summary>
        /// <param name="status">Status to show.</param>
        public void OnUpdateStatus(string status) {
            if (UpdateStatus != null) {
                UpdateStatus(status);
            }
        }

        /// <summary>
        /// Passes the value to any listening events.
        /// </summary>
        /// <param name="value">Percentage value for status.</param>
        public void OnUpdateProgress(int value) {
            if (UpdateProgress != null) {
                UpdateProgress(value);
            }
        }

        /// <summary>
        /// Passes the path to any listening events.
        /// </summary>
        /// <param name="path">Path to update.</param>
        public void OnUpdatePath(string path) {
            if (UpdatePath != null) {
                UpdatePath(path);
            }

            Settings.Instance.DualParent.SelectedPath = path;
        }

        /// <summary>
        /// Passes the filesystem info to any listening events.
        /// </summary>
        /// <param name="fsi"></param>
        /// <returns></returns>
        public int OnGetImageIndex(IResource fsi, bool useFileAttributes) {
            if (GetImageIndex != null) {
                return GetImageIndex(fsi, useFileAttributes);
            }

            return -1;
        }

        public void OnUpdatePreviewPanel(IResource resource) {
            if (UpdatePreviewPanel != null) {
                UpdatePreviewPanel(resource);
            }

            Settings.Instance.DualParent.SelectedFile = resource.Name;
        }
        #endregion

        #region Events.
        void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			if (SelectedItems.Count > 0) {
				lastSelectedItemIndex = e.ItemIndex;
				IChildResource resource = ((IChildResource)e.Item.Tag);
				OnUpdatePreviewPanel(resource);
			} else {
				// Make sure that the item's icon doesn't disappear.
				this.BeginInvoke((MethodInvoker)delegate {
					if (lastSelectedItemIndex > 0 && lastSelectedItemIndex < Items.Count) {
						ListViewItem item = Items[lastSelectedItemIndex];
						item.Selected = true;
						item.ImageIndex = OnGetImageIndex((IChildResource)item.Tag, false);
					}
				});
			}
        }

        void listView_ColumnClick(object sender, ColumnClickEventArgs e) {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == comparer.ColumnIndex) {
                // Reverse the current sort direction for this column.
                if (comparer.Order == Order.Ascending) {
                    comparer.Order = Order.Descending;
                } else {
                    comparer.Order = Order.Ascending;
                }
            } else {
                // Set the column number that is to be sorted; default to ascending.
                comparer.ColumnIndex = e.Column;
                comparer.Order = Order.Ascending;
            }

            // Perform the sort with these new sort options.
            this.Sort();
        }

        /// <summary>
        /// Fires when the list view gets focus.
        /// </summary>
        private void listView_GotFocus(object sender, EventArgs e) {
            OnUpdatePath(Path);
            OnUpdateStatus(Status);

            IResource resource = FileSystemInfoFactory.GetFileSystemInfo(Path);
            OnUpdatePreviewPanel(resource);
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
                    OnUpdateProgress(100);
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
                IResource resource = FileSystemInfoFactory.GetFileSystemInfo(fileDrop);

                if (resource != null && resource is IChildResource) {
                    string destination = string.Format(@"{0}{1}",
                                                       Path,
                                                       resource.Name);

                    if (!System.IO.File.Exists(destination)) {
                        try {
                            switch (e.Effect) {
                                case DragDropEffects.Copy:
                                    ((IChildResource)resource).Copy(destination, false);
                                    break;
                                case DragDropEffects.Move:
                                    ((IChildResource)resource).Move(destination);
                                    break;
                                case DragDropEffects.Link:
                                    // TODO: Need to handle links.
                                    break;
                            }
                        } catch (System.IO.IOException ex) {
                            Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Failed to perform the specified operation for {0}", destination);
                            Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
                        } catch (Exception ex) {
                            Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "The selected operation could not be completed for {0}.", destination);
                            Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
                        }
                    } else {
                        Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.MildlyVerbose,
                            "The file, {0}, already exists.", destination);
                        Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
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

                // If the disk is different, then default to a COPY operation.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0) {
                    FileInfo fileInfo = new FileInfo(files[0]);
                    string originalPath = fileInfo.FullName.Substring(0, fileInfo.FullName.IndexOf(':'));
                    string currentPath = Path.Substring(0, Path.IndexOf(':'));

                    if (!originalPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase)) {
                        if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    } else if (fileInfo.DirectoryName.Equals(Path, StringComparison.OrdinalIgnoreCase)) {
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
            IChildResource resource = (IChildResource)item.Tag;

            if (item.Tag is ParentDirectoryInfo ||
                item.Tag is RootDirectoryInfo) {
                e.CancelEdit = true;
            }

            int index = e.Item;

            // BeginInvoke will ensure that this message will get executed after the Framework's messages to select all of the text.
            this.BeginInvoke((MethodInvoker)delegate {
                // Only select the name in the textbox, not the extension.
                int selectedLength = Items[index].Text.Length;

                if (Items[index].Text.LastIndexOf('.') > -1) {
                    selectedLength = Items[index].Text.LastIndexOf('.');
                }

                User32.SendMessage(LabelEditHandle, (int)EM.SETSEL, 0, selectedLength);
            });
        }

        /// <summary>
        /// Renames the file/directory that was being edited.
        /// </summary>
        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else {
                if (!validateEditLabel(LabelEditHandle)) {
                    Items[e.Item].BeginEdit();
                    e.CancelEdit = true;
                }
            }

            if (!e.CancelEdit) {
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

                        Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                            "Renaming to {0} failed.", destination);
                        Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
                    }
                }
            }
        }
        #endregion

        #region Public methods.
        public new void Invoke(Delegate method) {
            if (this.InvokeRequired) {
                base.Invoke(method);
            } else {
                method.DynamicInvoke(null);
            }
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public void ShowMessageBox(string text) {
            Invoke((MethodInvoker)delegate {
                MessageBox.Show(this, text);
            });
        }

        /// <summary>
        /// Clears the listview.
        /// </summary>
        public new void Clear() {
            lock (lockObject) {
                this.Invoke((MethodInvoker)delegate {
                    this.Items.Clear();
                    this.itemDictionary.Clear();
                });

                lastSelectedItemIndex = 0;
                fileCount = 0;
                folderCount = 0;
            }
        }

        public new void BeginUpdate() {
            this.Invoke((MethodInvoker)delegate {
                base.BeginUpdate();
            });
        }

        public new void EndUpdate() {
            this.Invoke((MethodInvoker)delegate {
                base.EndUpdate();
            });
        }

        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveItem(string path) {
            lock (lockObject) {
                this.Items.RemoveByKey(path);
                this.itemDictionary.Remove(path);

				IResource resource = FileSystemInfoFactory.GetFileSystemInfo(path);

				if (resource != null) {
					if (resource is FileInfo) {
						fileCount--;
					} else if (resource is DirectoryInfo) {
						folderCount--;
					}
				}
            }
        }

        /// <summary>
        /// Parses the file/directory information and updates the listview.
        /// </summary>
        public void AddItemRange(IList<IChildResource> resources) {
            StringBuilder sb = new StringBuilder();
            Stopwatch sw = new Stopwatch();

            this.Invoke((MethodInvoker)delegate {
                if (this.SmallImageList == null) {
                    this.SmallImageList = Forms.GetPropertyInParent<ImageList>(this.Parent, "ImageList");
                }
            });

            sw.Start();

            // Create new listview items from the resources.
            ListViewItemsCreator listViewItemsCreator = new ListViewItemsCreator(this, resources);
            List<ListViewItem> listViewItems = listViewItemsCreator.Get(sb);

            fileCount = listViewItemsCreator.FileCount;
            folderCount = listViewItemsCreator.FolderCount;

            this.Invoke((MethodInvoker)delegate {
                this.Items.AddRange(listViewItems.ToArray());
            });

            Settings.Instance.Logger.Log(LogLevelType.Verbose, "Add resources took {0} ms.",
                sw.ElapsedMilliseconds.ToString());
            sw.Reset();

            if (sb.Length > 0) {
                Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, sb.ToString());
                Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
            }

            comparer.ColumnIndex = 0;
            comparer.Order = Order.Ascending;

            this.Invoke((MethodInvoker)delegate {               
                this.Sort();

                // Resize the columns based on the column content.
                this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            });

            OnUpdateStatus(Status);

            string path = string.Empty;

            foreach (IChildResource resource in resources) {
                if (!(resource is FileInfo) && !(resource is ParentDirectoryInfo) && !(resource is RootDirectoryInfo)) {
                    path = resource.Path.ToLower();
                    break;
                }
            }

            if (previousTopIndexes.ContainsKey(path)) {
                int previousTopIndex = previousTopIndexes[path];
                
                if (previousTopIndex > 0 && previousTopIndex < Items.Count) {
                    this.Invoke((MethodInvoker)delegate {
                        if (previousTopIndex < Items.Count) {
                            //Items[previousTopIndex].EnsureVisible();
                            this.EnsureVisible(previousTopIndex);
                        } else {
                            //Items[Items.Count-1].EnsureVisible();
                            this.EnsureVisible(Items.Count - 1);
                        }

                        previousTopIndexes.Remove(path);
                    });
                }
            }
        }

        public void UpdateImageIndexes(bool useFileAttributes) {
            ListViewItemsImageIndexer imageIndexer = new ListViewItemsImageIndexer(this);
            imageIndexer.Update(useFileAttributes);
        }

        public void ClearPreviousTopIndexes() {
            previousTopIndexes.Clear();
        }

        /// <summary>
        /// Parses the file/directory information and inserts the file info into the listview.
        /// </summary>
        public void AddItem(IChildResource resource) {
            if (resource != null) {
                // Create a new listview item.
                StringBuilder sb = new StringBuilder();
                ListViewItemsCreator listViewItemsCreator = new ListViewItemsCreator(this, resource);
                List<ListViewItem> items = listViewItemsCreator.Get(sb);

                if (items.Count > 0) {
                    ListViewItem item = items[0];
                    fileCount += listViewItemsCreator.FileCount;
                    folderCount += listViewItemsCreator.FolderCount;

                    if (sb.Length > 0) {
                        Settings.Instance.Logger.ProcessContent += ShowMessageBox;
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, sb.ToString());
                        Settings.Instance.Logger.ProcessContent -= ShowMessageBox;
                    }

                    this.Items.Add(item);

                    // Basic stuff that should happen everytime files are shown.
                    this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    OnUpdateStatus(Status);
                    this.Sort();

                    int imageIndex = OnGetImageIndex(resource, false);

                    if (imageIndex > -1) {
                        item.ImageIndex = imageIndex;
                    }
                }
            }
        }
		#endregion

        #region Private methods.
        /// <summary>
        /// Validates the edit label and displays a blloon tip if it is not.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        private bool validateEditLabel(IntPtr handle) {
            StringBuilder sb = new StringBuilder(256);
            User32.GetWindowText(handle.ToInt32(), sb, 255);
            string label = sb.ToString();

            foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) {
                if (label.IndexOf(ch) > -1) {
                    const string text = "Filenames cannot have any illegal characters including: \", |, <, >, *, ?, :, /, \\";

                    EditBalloon editBalloon = new EditBalloon(handle);
                    editBalloon.Title = "Illegal Characters";
                    editBalloon.TitleIcon = TooltipIcon.Warning;
                    editBalloon.Text = text;
                    editBalloon.Show();

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the size of the currently selected item.
        /// </summary>
        private void calculateSize() {
            lock (lockObject) {
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
                            ColumnInfo columnInfo = null;
                            foreach (ColumnInfo ci in ColumnInfos) {
                                if (ci.Property.Equals("Size")) {
                                    foreach (ColumnHeader columnHeader in Columns) {
                                        if (columnHeader.Text.Equals(ci.Text)) {
                                            sizeIndex = columnHeader.Index;
                                            columnInfo = ci;
                                            break;
                                        }
                                    }
                                }

                                if (sizeIndex > 0) {
                                    break;
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

                                            // Update the list view through BeginInvoke so that the correct thread is used.
                                            this.BeginInvoke((MethodInvoker)delegate {
                                                item.SubItems[sizeIndex].Text = "...";
                                                this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.ColumnContent);
                                            });

                                            eventArgs.Result = ((DirectoryInfo)eventArgs.Argument).Size;
                                            backgroundWorker.ReportProgress(100);
                                        };

                                        backgroundWorker.ProgressChanged += delegate(object anonymousSender, ProgressChangedEventArgs eventArgs) {
                                            OnUpdateProgress(eventArgs.ProgressPercentage);
                                        };

                                        backgroundWorker.RunWorkerCompleted +=
                                            delegate(object anonymousSender, RunWorkerCompletedEventArgs eventArgs) {
                                                if (eventArgs.Error == null &&
                                                    eventArgs.Result != null &&
                                                    eventArgs.Result is long) {
                                                    size = (long)eventArgs.Result;

                                                    // Update the list view through BeginInvoke so that the correct thread is used.
                                                    this.BeginInvoke((MethodInvoker)delegate {
                                                        if (columnInfo.MethodDelegate != null) {
                                                            item.SubItems[sizeIndex].Text = columnInfo.MethodDelegate.Invoke(size.ToString());
                                                        } else {
                                                            item.SubItems[sizeIndex].Text = size.ToString();
                                                        }

                                                        updateSelectedTotalSize(size);
                                                        this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.ColumnContent);
                                                    });
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

            OnUpdateStatus(string.Format("Selected items: {0}",
                                       General.GetHumanReadableSize(totalSelectedSize.ToString())));
        }
        #endregion

        #region Properties.
        /// <summary>
        /// Current status.
        /// </summary>
        public string Status {
            get {
                return string.Format("Folders: {0}; Files: {1}",
                                           folderCount,
                                           fileCount);
            }
        }

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

        public Control Control {
            get {
                return this;
            }
        }

        public IViewComparer Comparer {
            get {
                return comparer;
            }
            set {
                comparer = value;
            }
        }

        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;

                this.Invoke((MethodInvoker)delegate {
                    this.Columns.Clear();

                    foreach (ColumnInfo columnInfo in columnInfos) {
                        ColumnHeader columnHeader = new ColumnHeader();
                        columnHeader.Text = columnInfo.Text;
                        columnHeader.Tag = columnInfo;
                        this.Columns.Add(columnHeader);
                    }
                });
            }
        }

        public IntPtr LabelEditHandle {
            get {
                return User32.SendMessage(Handle,
                    (int)LVM.GETEDITCONTROL, 0, IntPtr.Zero);
            }
        }

        public string SelectedFile {
            get {
                if (SelectedItems.Count == 0) {
                    return Path;
                } else {
                    return ((IChildResource)SelectedItems[0].Tag).FullName;
                }
            }
        }

        public Dictionary<string, ListViewItem> ItemDictionary {
            get {
                return itemDictionary;
            }
        }
        #endregion
    }
}