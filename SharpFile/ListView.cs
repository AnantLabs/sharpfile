using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using SharpFile.UI;
using SharpFile.IO;
using SharpFile.Infrastructure;
using Common;

namespace SharpFile {
	public class ListView : System.Windows.Forms.ListView {
		private const int ALT = 32;
		private const int CTRL = 8;
		private const int SHIFT = 4;

		private UnitDisplay unitDisplay = UnitDisplay.Bytes;
		private IList<FileSystemInfo> selectedFileSystemInfos = new List<FileSystemInfo>();
		private long totalSelectedSize = 0;

		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo fsi, DriveType driveType);
		public event OnGetImageIndexDelegate OnGetImageIndex;

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
			this.View = View.Details;

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
		protected void UpdateProgress(int value) {
			if (OnUpdateProgress != null) {
				OnUpdateProgress(value);
			}
		}

		/// <summary>
		/// Passes the filesystem info to any listening events.
		/// </summary>
		/// <param name="fsi"></param>
		/// <returns></returns>
		protected int GetImageIndex(FileSystemInfo fsi, DriveType driveType) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(fsi, driveType);
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
				ExecuteOrUpdate(path, Filter);
			}
		}

		/// <summary>
		/// Selects an item in the listview when the Space bar is hit.
		/// </summary>
		private void listView_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space) {
				if (this.SelectedItems != null &&
					this.SelectedItems.Count > 0) {
					foreach (ListViewItem item in this.SelectedItems) {
						string path = item.Name;
						FileSystemInfo fileSystemInfo = FileSystemInfoFactory.GetFileSystemInfo(path);

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

								if (item.SubItems[sizeIndex].Text.Equals(string.Empty) ||
									fileSystemInfo is DirectoryInfo) {
									using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
										backgroundWorker.WorkerReportsProgress = true;

										backgroundWorker.DoWork += delegate(object anonymousSender, DoWorkEventArgs eventArgs) {
											backgroundWorker.ReportProgress(50);
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
						}
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

		private void listView_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				ShellContextMenu m = new ShellContextMenu();
				ShellContextMenu.ContextMenuResult contextMenuResult;

				if (this.SelectedItems.Count > 1) {
					List<string> paths = getSelectedPaths();

					contextMenuResult = m.PopupMenu(paths, this.Handle);
				} else if (this.SelectedItems.Count == 1) {
					contextMenuResult = m.PopupMenu(this.SelectedItems[0].Name, this.Handle);
				} else {
					contextMenuResult = m.PopupMenu(Path, this.Handle);
				}

				// Update the file listing.
				if (contextMenuResult != ShellContextMenu.ContextMenuResult.NoUserFeedback &&
					contextMenuResult != ShellContextMenu.ContextMenuResult.ContextMenuError) {
					UpdateListView(true);
				}
			}
		}

		private void listView_DragDrop(object sender, DragEventArgs e) {
			// Can only drop files, so check
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}

			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string file in files) {
				string dest = Path + "\\" + System.IO.Path.GetFileName(file);

				bool isFolder = Directory.Exists(file);
				bool isFile = File.Exists(file);

				// Ignore if it doesn't exist
				if (!isFolder && !isFile)
					continue;

				try {
					switch (e.Effect) {
						case DragDropEffects.Copy:
							// TODO: Need to handle folders.
							if (isFile)
								File.Copy(file, dest, false);
							break;
						case DragDropEffects.Move:
							// TODO: Need to handle folders.
							if (isFile)
								File.Move(file, dest);
							break;
						case DragDropEffects.Link:
							// TODO: Need to handle links.
							break;
					}
				} catch (IOException ex) {
					MessageBox.Show(this, "Failed to perform the specified operation:\n\n" + ex.Message, "File operation failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}

			UpdateListView(true);
		}

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
				(e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
					e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
			List<string> paths = getSelectedPaths();

			if (paths.Count > 0) {
				DoDragDrop(new DataObject(DataFormats.FileDrop, paths.ToArray()), DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);

				UpdateListView(true);
			}
		}

		void listView_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F2) {
				if (this.SelectedItems.Count > 0) {
					ListViewItem item = this.SelectedItems[0];
					item.BeginEdit();
				}
			}
		}

		void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
			if (!string.IsNullOrEmpty(e.Label)) {
				ListViewItem item = this.Items[e.Item];
				FileSystemInfo fileSystemInfo = FileSystemInfoFactory.GetFileSystemInfo(item.Name);

				string source = string.Format("{0}{1}",
					fileSystemInfo.Path,
					item.Text);

				string destination = string.Format("{0}{1}",
					fileSystemInfo.Path,
					e.Label);

				try {
					if (fileSystemInfo is FileInfo) {
						File.Move(source, destination);
					} else if (fileSystemInfo is DirectoryInfo) {
						Directory.Move(source, destination);
					}
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
		public void ExecuteOrUpdate() {
			ExecuteOrUpdate(Path, Filter);
		}

		/// <summary>
		/// Executes the provided file, or refreshes the listview for the provided directory.
		/// </summary>
		/// <param name="path"></param>
		public void ExecuteOrUpdate(string path, string filter) {
			if (System.IO.File.Exists(path)) {
				Process.Start(path);
			} else if (System.IO.Directory.Exists(path)) {
				UpdateListView(path, filter, true, true);
			} else {
				MessageBox.Show("The path, " + path + ", looks like it is incorrect.");
			}
		}
		#endregion

		#region UpdateListView methods.
		public void UpdateListView(bool forceUpdate) {
			UpdateListView(Path, Filter, forceUpdate, false);
		}

		public void UpdateListView(string path, string filter) {
			UpdateListView(path, filter, false, false);
		}

		/// <summary>
		/// Updates the listview with the specified path and filter.
		/// </summary>
		/// <param name="path">Path to get information about.</param>
		/// <param name="filter">Pattern to filter the information.</param>
		public void UpdateListView(string path, string filter, bool forceUpdate, bool clearListView) {
			if (this.SmallImageList == null) {
				this.SmallImageList = ImageList;
			}

			// Prevents the retrieval of file information if unneccessary.
			if (!forceUpdate &&
				path.Equals(Path) &&
				filter.Equals(Filter)) {
				return;
			} else {
				Path = path;
				Filter = filter;
			}

			// Get the directory information.
			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
			string directoryPath = directoryInfo.FullName;
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
					e.Result = FileSystem.GetFiles(directoryInfo, filter);
					backgroundWorker.ReportProgress(100);
				};

				// Method that runs when the DoWork method is finished.
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					if (e.Error == null &&
						e.Result != null &&
						e.Result is IEnumerable<FileSystemInfo>) {
						IEnumerable<FileSystemInfo> fileSystemInfoList = (IEnumerable<FileSystemInfo>)e.Result;

						this.BeginUpdate();
						if (clearListView) {
							this.Items.Clear();
						}						

						UpdateListView(fileSystemInfoList);
						this.EndUpdate();

						// Update some information about the current directory.
						//tlsPath.Text = directoryPath;
						this.Path = directoryPath;
						this.Text = directoryPath;
						this.Parent.Text = directoryPath;

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

		/// <summary>
		/// Parses the file/directory information and updates the listview.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UpdateListView(IEnumerable<FileSystemInfo> fileSystemInfoList) {
			int fileCount = 0;
			int folderCount = 0;

			try {
				// Create a new listview item with the display name.
				foreach (FileSystemInfo fileSystemInfo in fileSystemInfoList) {
					if (!this.Items.ContainsKey(fileSystemInfo.FullPath)) {
						ListViewItem item = createListViewItem(fileSystemInfo, ref fileCount, ref folderCount);
						this.Items.Add(item);

						/*
						// TODO: This doesn't need to be done except when inserting a new listitem.
						int listViewIndex = 0;
						listViewIndex = getListViewIndex(fileSystemInfo);

						if (listViewIndex == -1) {
							this.Items.Insert(0, item);
						} else {
							this.Items.Insert(listViewIndex, item);
						}
						*/
					}
				}

				// Basic stuff that should happen everytime files are shown.
				this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

				UpdateStatus(string.Format("Folders: {0}; Files: {1}",
					folderCount,
					fileCount));
			//} catch (UnauthorizedAccessException) {
			//    this.BeginUpdate();
			//    this.Items.Add("Unauthorized Access");
			//    this.EndUpdate();
			} catch (Exception ex) {
				MessageBox.Show(ex.Message + ex.StackTrace);
			}
		}

		/// <summary>
		/// Parses the file/directory information and inserts the file info into the listview.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void UpdateListView(FileSystemInfo fileSystemInfo) {
			int fileCount = 0;
			int folderCount = 0;

			try {
				// Create a new listview item with the display name.
				if (!this.Items.ContainsKey(fileSystemInfo.FullPath)) {
					ListViewItem item = createListViewItem(fileSystemInfo, ref fileCount, ref folderCount);
					int listViewIndex = 0;
					listViewIndex = getListViewIndex(fileSystemInfo);

					if (listViewIndex == -1) {
						this.Items.Insert(0, item);
					} else {
						this.Items.Insert(listViewIndex, item);
					}
				}

				// Basic stuff that should happen everytime files are shown.
				this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

				UpdateStatus(string.Format("Folders: {0}; Files: {1}",
					folderCount,
					fileCount));
			//} catch (UnauthorizedAccessException) {
			//    this.BeginUpdate();
			//    this.Items.Add("Unauthorized Access");
			//    this.EndUpdate();
			} catch (Exception ex) {
				MessageBox.Show(ex.Message + ex.StackTrace);
			}
		}

		/// <summary>
		/// Retrieves the index that the list view item should be inserted into.
		/// </summary>
		/// <param name="fsi"></param>
		/// <returns></returns>
		private int getListViewIndex(FileSystemInfo fsi) {
			// Copy the items to an array for further processing.
			ListViewItem[] items = new ListViewItem[this.Items.Count + 1];
			this.Items.CopyTo(items, 0);

			// Add the new filesysteminfo item to the array of items.
			ListViewItem item = new ListViewItem(fsi.FullPath);
			item.Name = fsi.FullPath;
			item.Tag = fsi;
			item.Text = fsi.DisplayName;
			items[items.Length - 1] = item;

			// Determine if the file should be pushed further down based on the number of directories above it.
			int indexOffset = 0;
			if (fsi is FileInfo) {
				indexOffset = Array.FindAll<ListViewItem>(items, delegate(ListViewItem i) {
					return ((FileSystemInfo)i.Tag) is DirectoryInfo;
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

		private ListViewItem createListViewItem(FileSystemInfo fileSystemInfo, ref int fileCount, ref int folderCount) {
			ListViewItem item = new ListViewItem(fileSystemInfo.DisplayName);
			item.Name = fileSystemInfo.FullPath;
			item.Tag = fileSystemInfo;
			DriveType driveType = DriveInfo.DriveType;

			int imageIndex = 0;
			if (fileSystemInfo is FileInfo) {
				imageIndex = GetImageIndex(fileSystemInfo, driveType);
				item.SubItems.Add(General.GetHumanReadableSize(fileSystemInfo.Size.ToString()));
				fileCount++;
			} else {
				imageIndex = GetImageIndex(fileSystemInfo, driveType);
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
		#endregion

		public ImageList ImageList {
			get {
				return ((FileBrowser)this.Parent).ImageList;
			}
		}

		public string Path {
			get {
				return ((FileBrowser)this.Parent).Path;
			}
			set {
				((FileBrowser)this.Parent).Path = value;
			}
		}

		public string Filter {
			get {
				return ((FileBrowser)this.Parent).Filter;
			}
			set {
				((FileBrowser)this.Parent).Filter = value;
			}
		}

		public System.IO.FileSystemWatcher FileSystemWatcher {
			get {
				return ((FileBrowser)this.Parent).FileSystemWatcher;
			}
		}

		public DriveInfo DriveInfo {
			get {
				return ((FileBrowser)this.Parent).DriveInfo;
			}
		}
	}
}