using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using SharpFile.IO.ChildResources;
using SharpFile.IO.ParentResources;
using SharpFile.UI;
using SharpFile.IO;
using Common;

namespace SharpFile {
	public class ListView : System.Windows.Forms.ListView, IView {
		private const int ALT = 32;
		private const int CTRL = 8;
		private const int SHIFT = 4;

		private UnitDisplay unitDisplay = UnitDisplay.Bytes;
		private IList<IChildResource> selectedFileSystemInfos = new List<IChildResource>();
		private long totalSelectedSize = 0;
		private Dictionary<string, ListViewItem> itemDictionary = new Dictionary<string, ListViewItem>();

		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(IResource fsi);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		public delegate void OnUpdatePathDelegate(string path);
		public event OnUpdatePathDelegate OnUpdatePath;

		public ListView() {
			// This prevents flicker in the listview. 
			// It is a protected property, so it is only available if we derive from ListView.
			this.DoubleBuffered = true;

			initializeComponent();
		}

		private void initializeComponent() {
			this.Dock = DockStyle.Fill;
			this.DoubleClick += listView_DoubleClick;
			this.KeyDown += listView_KeyDown;
			this.MouseUp += listView_MouseUp;
			this.ItemDrag += listView_ItemDrag;
			this.DragOver += listView_DragOver;
			this.DragDrop += listView_DragDrop;
			this.KeyUp += listView_KeyUp;
			this.AfterLabelEdit += listView_AfterLabelEdit;

			// Set some options on the listview.
			// TODO: This should be able to be set via dropdown/settings.
			this.View = View.Details;

			// TODO: Columns should be set by the IChildResource properties that are actually populated.
			// Maybe there should be a list associated with IParenttResources where the columns available are specified.
			List<string> columns = new List<string>();
			columns.Add("Filename");
			columns.Add("Size");
			columns.Add("Date");
			columns.Add("Time");

			foreach (string column in columns) {
				this.Columns.Add(column);
			}
		}

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
		#endregion

		#region Events.
		/// <summary>
		/// Refreshes the listview when a file/directory is double-clicked in the listview.
		/// </summary>
		private void listView_DoubleClick(object sender, EventArgs e) {
			if (this.SelectedItems.Count > 0) {
				string path = this.SelectedItems[0].Name;
				IChildResource resource = ChildResourceFactory.GetChildResource(path);

				if (resource != null) {
					resource.Execute(this);
				}
			}
		}

		/// <summary>
		/// Selects an item in the listview when the Space bar is hit.
		/// </summary>
		private void listView_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space) {
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
											backgroundWorker.ReportProgress(100);
										};

										backgroundWorker.ProgressChanged += delegate(object anonymousSender, ProgressChangedEventArgs eventArgs) {
											UpdateProgress(eventArgs.ProgressPercentage);
										};

										backgroundWorker.RunWorkerCompleted += delegate(object anonymousSender, RunWorkerCompletedEventArgs eventArgs) {
											if (eventArgs.Error == null &&
												eventArgs.Result != null) {
												size = (long)eventArgs.Result;

												item.SubItems[sizeIndex].Text = General.GetHumanReadableSize(size.ToString());
												updateSelectedTotalSize(size);
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
		/// Update the selected file sytem objects' total size.
		/// </summary>
		/// <param name="size"></param>
		private void updateSelectedTotalSize(long size) {
			totalSelectedSize += size;

			UpdateStatus(string.Format("Selected items: {0}",
				General.GetHumanReadableSize(totalSelectedSize.ToString())));
		}

		/// <summary>
		/// Displays the right-click context menu.
		/// </summary>
		private void listView_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				ShellContextMenu menu = new ShellContextMenu();
				ShellContextMenu.ContextMenuResult contextMenuResult;

				if (this.SelectedItems.Count > 1) {
					List<string> paths = getSelectedPaths();

					contextMenuResult = menu.PopupMenu(paths, this.Handle);
				} else if (this.SelectedItems.Count == 1) {
					contextMenuResult = menu.PopupMenu(this.SelectedItems[0].Name, this.Handle);
				} else {
					contextMenuResult = menu.PopupMenu(Path, this.Handle);
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
						MessageBox.Show(this, "Failed to perform the specified operation:\n\n" + ex.Message, "File operation failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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

				if (files.Length > 0 && !files[0].ToUpper().StartsWith(Path) &&
				(e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
					e.Effect = DragDropEffects.Copy;
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
		void listView_KeyUp(object sender, KeyEventArgs e) {
			// TODO: Should be specified by config.
			if (e.KeyCode == Keys.F2) {
				if (this.SelectedItems.Count > 0) {
					ListViewItem item = this.SelectedItems[0];
					item.BeginEdit();
				}
			}
		}

		/// <summary>
		/// Renames the file/directory that was being edited.
		/// </summary>
		void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
			if (!string.IsNullOrEmpty(e.Label)) {
				ListViewItem item = this.Items[e.Item];
				IChildResource resource = (IChildResource)item.Tag;

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
		#endregion

		#region ExecuteOrUpdate methods.
		/// <summary>
		/// Executes the file, or refreshes the listview for the selected directory in the path textbox.
		/// </summary>
		//public void ExecuteOrUpdate() {
		//    ExecuteOrUpdate(Path);
		//}

		/// <summary>
		/// Executes the provided file, or refreshes the listview for the provided directory.
		/// </summary>
		/// <param name="path"></param>
		//public void ExecuteOrUpdate(string path) {
		//    IChildResource resource = ChildResourceFactory.GetChildResource(path);

		//    if (resource != null) {
		//        if (resource is FileInfo) {
		//            Process.Start(path);
		//        } else if (resource is DirectoryInfo) {
		//            UpdateListView(path, string.Empty);
		//        }
		//    } else {
		//        MessageBox.Show(string.Format("The path, {0}, looks like it is incorrect.",
		//            path));
		//    }
		//}
		#endregion

		#region UpdateListView methods.
		/*
		/// <summary>
		/// Updates the listview with the specified path and filter.
		/// </summary>
		/// <param name="path">Path to get information about.</param>
		/// <param name="filter">Pattern to filter the information.</param>
		public void UpdateListView(string path, string filter) {
			if (this.SmallImageList == null) {
			    this.SmallImageList = IconManager.FindImageList(this.Parent);
			}

			// Prevents the retrieval of file information if unneccessary.
			if (path.Equals(Path)) {
			    return;
			}

			// Get the directory information.
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			string directoryPath = directoryInfo.FullPath;
			directoryPath = string.Format("{0}{1}",
				directoryPath,
				directoryPath.EndsWith(@"\") ? string.Empty : @"\");

			// Create another thread to get the file information asynchronously.
			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				backgroundWorker.WorkerReportsProgress = true;

				// Anonymous method that retrieves the file information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					// Disable the filewatcher.
					FileSystemWatcher.EnableRaisingEvents = false;

					// Grab the files and report the progress to the parent.
					backgroundWorker.ReportProgress(50);

					// TODO: The ChildRetriever should be specified in the ParentResource.
					IChildResourceRetriever fileRetriever = new FileRetriever();

					e.Result = fileRetriever.Get(directoryInfo, filter);
					backgroundWorker.ReportProgress(100);
				};

				// Method that runs when the DoWork method is finished.
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					if (e.Error == null &&
						e.Result != null &&
						e.Result is IEnumerable<IChildResource>) {
						IEnumerable<IChildResource> fileSystemInfoList = (IEnumerable<IChildResource>)e.Result;

						this.BeginUpdate();
						this.Items.Clear();
						UpdateView(fileSystemInfoList);
						this.EndUpdate();

						// Update some information about the current directory.
						UpdatePath(directoryPath);

						// Set up the watcher.
						FileSystemWatcher.Path = directoryPath;
						FileSystemWatcher.Filter = filter;
						FileSystemWatcher.EnableRaisingEvents = true;
					}
				};

				// Anonymous method that updates the status to the parent form.
				backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
					UpdateProgress(e.ProgressPercentage);
				};

				backgroundWorker.RunWorkerAsync();
			}
		}
		*/

		private void addItem(IChildResource resource, ref int fileCount, ref int folderCount) {
			if (!itemDictionary.ContainsKey(resource.FullPath)) {
				ListViewItem item = createListViewItem(resource, ref fileCount, ref folderCount);
				itemDictionary.Add(resource.FullPath, item);
				this.Items.Add(item);
			}
		}

		private void insertItem(IChildResource resource, ref int fileCount, ref int folderCount)
		{
			//if (!itemDictionary.ContainsKey(resource.FullPath))
			//{
			//  ListViewItem item = createListViewItem(resource, ref fileCount, ref folderCount);
			//  this.Items.Add(item);
			//  itemDictionary.Add(resource.FullPath, item);
			//}

			if (!itemDictionary.ContainsKey(resource.FullPath))
			{
				ListViewItem item = createListViewItem(resource, ref fileCount, ref folderCount);
				int listViewIndex = getListViewIndex(item);

				if (listViewIndex == -1)
				{
					this.Items.Insert(0, item);
				}
				else
				{
					this.Items.Insert(listViewIndex, item);
				}

				itemDictionary.Add(resource.FullPath, item);
			}
		}

		/// <summary>
		/// Parses the file/directory information and updates the listview.
		/// </summary>
		public void UpdateView(IEnumerable<IChildResource> fileSystemInfoList) {
			if (this.SmallImageList == null) {
				this.SmallImageList = IconManager.FindImageList(this.Parent);
			}

			int fileCount = 0;
			int folderCount = 0;

			try {
				// Create a new listview item with the display name.
				foreach (IChildResource resource in fileSystemInfoList) {
					addItem(resource, ref fileCount, ref folderCount);
				}

				// Resize the columns based on the column content.
				this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

				UpdateStatus(string.Format("Folders: {0}; Files: {1}",
                   folderCount,
                   fileCount));
			} catch (Exception ex) {
				MessageBox.Show(ex.Message + ex.StackTrace);
			}
		}

		/// <summary>
		/// Parses the file/directory information and inserts the file info into the listview.
		/// </summary>
		public void UpdateListView(IChildResource fileSystemInfo) {
			int fileCount = 0;
			int folderCount = 0;

			if (fileSystemInfo != null) {
				try {
					// Create a new listview item with the display name.
					insertItem(fileSystemInfo, ref fileCount, ref folderCount);

					// Basic stuff that should happen everytime files are shown.
					this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

					UpdateStatus(string.Format("Folders: {0}; Files: {1}",
						folderCount,
						fileCount));
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + ex.StackTrace);
				}
			}
		}

		/// <summary>
		/// Retrieves the index that the list view item should be inserted into.
		/// </summary>
		private int getListViewIndex(ListViewItem item) {
			// Get the filesysteminfo object from the item's tag.
			IChildResource fsi = (IChildResource)item.Tag;

			// Copy the items to an array for further processing.
			ListViewItem[] items = new ListViewItem[this.Items.Count + 1];
			this.Items.CopyTo(items, 0);

			// Add the new listview item to the array of items.
			items[items.Length - 1] = item;

			// Determine if the file should be pushed further down based on the number of directories above it.
			int indexOffset = 0;
			if (fsi is FileInfo) {
				indexOffset = Array.FindAll<ListViewItem>(items, delegate(ListViewItem i) {
					return (i.Tag is DirectoryInfo);
				}).Length;
			} else if (fsi is DirectoryInfo) {
				indexOffset = Array.FindAll<ListViewItem>(items, delegate(ListViewItem i) {
					return (i.Tag is ParentDirectoryInfo ||
						i.Tag is RootDirectoryInfo);
				}).Length;
			}

			// Filter out any items that are not the same type.
			items = Array.FindAll<ListViewItem>(items, delegate(ListViewItem i) {
				// TODO: Fix this -- it prevents the parent directory from getting sorted.
				if (i.Tag.GetType() == fsi.GetType()) {
					return true;
				}

				return false;
			});

			// Sort the array ascending.
			Array.Sort<ListViewItem>(items, delegate(ListViewItem i1, ListViewItem i2) {
				// Sort root directories so they appear first.
				if (i2.Text == RootDirectoryInfo.DisplayName) {
					return 1;
				}

				// Sort parent directories so they appear second.
				if (i2.Text == ParentDirectoryInfo.DisplayName) {
					return 1;
				}

				// Sort everything else according to their name.
				return i1.Name.CompareTo(i2.Name);
			});

			// Add the index of the item to the correct offset.
			int index = Array.IndexOf<ListViewItem>(items, item) + indexOffset;
			return index;
		}

		/// <summary>
		/// Create listview from the filesystem information.
		/// </summary>
		/// <param name="fileSystemInfo">Filesystem information.</param>
		/// <returns>Listview item that references the filesystem object.</returns>
		private ListViewItem createListViewItem(IChildResource fileSystemInfo, ref int fileCount, ref int folderCount) {
			ListViewItem item = new ListViewItem(fileSystemInfo.DisplayName);
			item.Name = fileSystemInfo.FullPath;
			item.Tag = fileSystemInfo;
			DriveType driveType = DriveInfo.DriveType;

			int imageIndex = 0;
			if (fileSystemInfo is FileInfo) {
				imageIndex = GetImageIndex(fileSystemInfo);
				item.SubItems.Add(General.GetHumanReadableSize(fileSystemInfo.Size.ToString()));
				fileCount++;
			} else {
				imageIndex = GetImageIndex(fileSystemInfo);
				item.SubItems.Add(string.Empty);

				if (!(fileSystemInfo is ParentDirectoryInfo) &&
					!(fileSystemInfo is RootDirectoryInfo)) {
					folderCount++;
				}
			}

			item.ImageIndex = imageIndex;
			item.SubItems.Add(fileSystemInfo.LastWriteTime.ToShortDateString());
			item.SubItems.Add(fileSystemInfo.LastWriteTime.ToShortTimeString());
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
		#endregion

		public void ClearView() {
			this.Items.Clear();
			this.itemDictionary.Clear();
		}

		/// <summary>
		/// Current path.
		/// </summary>
		public string Path {
			get {
				return ((FileBrowser)this.Parent).Path;
			}
		}

		/// <summary>
		/// Current filter.
		/// </summary>
		public string Filter {
			get {
				return ((FileBrowser)this.Parent).Filter;
			}
		}

		/// <summary>
		/// Current FileSystemWatcher.
		/// </summary>
		public System.IO.FileSystemWatcher FileSystemWatcher {
			get {
				return ((FileBrowser)this.Parent).FileSystemWatcher;
			}
		}

		/// <summary>
		/// Current drive.
		/// </summary>
		public DriveInfo DriveInfo {
			get {
				return ((FileBrowser)this.Parent).DriveInfo;
			}
		}
	}
}