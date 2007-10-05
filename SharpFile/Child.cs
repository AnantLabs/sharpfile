using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using Common;
using SharpFile.IO;
using SharpFile.Infrastructure;
using DirectoryInfo = SharpFile.IO.DirectoryInfo;
using DriveInfo = SharpFile.IO.DriveInfo;
using FileInfo = SharpFile.IO.FileInfo;
using FileSystemInfo = SharpFile.IO.FileSystemInfo;
using System.Runtime.InteropServices;

namespace SharpFile {
	public partial class Child : Form {
		private const int ALT = 32;
		private const int CTRL = 8;
		private const int SHIFT = 4;

		private string _path;
		private string _filter;
		private UnitDisplay unitDisplay = UnitDisplay.Bytes;
		private SharpFile.Infrastructure.FileSystemWatcher fileSystemWatcher;
		private IList<FileSystemInfo> selectedFileSystemInfos = new List<FileSystemInfo>();
		private long totalSelectedSize = 0;

		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo dataInfo);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public Child() {
			InitializeComponent();
			initializeComponent();
		}

		#region Delegate methods
		/// <summary>
		/// Passes the status to any listening events.
		/// </summary>
		/// <param name="status">Status to show.</param>
		protected void UpdateStatus(string status) {
			if (OnUpdateStatus != null)
				OnUpdateStatus(status);
		}

		/// <summary>
		/// Passes the value to any listening events.
		/// </summary>
		/// <param name="value">Percentage value for status.</param>
		protected void UpdateProgress(int value) {
			if (OnUpdateProgress != null)
				OnUpdateProgress(value);
		}

		/// <summary>
		/// Passes the filesystem info to any listening events.
		/// </summary>
		/// <param name="dataInfo">Filesystem information.</param>
		/// <returns>The index of the image in the master ImageList.</returns>
		protected int GetImageIndex(FileSystemInfo dataInfo) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(dataInfo);
			}

			return -1;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Sets the child up.
		/// </summary>
		private void initializeComponent() {
			this.DoubleBuffered = true;

			// Attach to some events.
			this.ClientSizeChanged += new EventHandler(this_ClientSizeChanged);
			this.listView.DoubleClick += new EventHandler(listView_DoubleClick);
			this.listView.KeyDown += new KeyEventHandler(listView_KeyDown);
			this.tlsPath.KeyDown += new KeyEventHandler(tlsPath_KeyDown);
			this.tlsFilter.KeyUp += new KeyEventHandler(tlsFilter_KeyUp);
			this.tlsDrives.DropDownItemClicked += new ToolStripItemClickedEventHandler(tlsDrives_DropDownItemClicked);
			this.listView.MouseUp += new MouseEventHandler(listView_MouseUp);
			this.listView.ItemDrag += new ItemDragEventHandler(listView_ItemDrag);
			this.listView.DragOver += new DragEventHandler(listView_DragOver);
			this.listView.DragDrop += new DragEventHandler(listView_DragDrop);
			this.listView.KeyUp += new KeyEventHandler(listView_KeyUp);
			this.listView.AfterLabelEdit += new LabelEditEventHandler(listView_AfterLabelEdit);

			resizeControls();

			// Set some options on the listview.
			listView.View = View.Details;
			tlsFilter.Text = string.Empty;

			List<string> columns = new List<string>();
			columns.Add("Filename");
			columns.Add("Size");
			columns.Add("Date");
			columns.Add("Time");

			foreach (string column in columns) {
				listView.Columns.Add(column);
			}

			fileSystemWatcher = new SharpFile.Infrastructure.FileSystemWatcher(this, 5000);
			fileSystemWatcher.Changed += delegate {
				updateFileListing(true);
			};
		}

		private void updateSelectedTotalSize(long size) {
			totalSelectedSize += size;

			UpdateStatus(string.Format("Selected items: {0}",
									General.GetHumanReadableSize(totalSelectedSize.ToString())));
		}

		/// <summary>
		/// Gets the selected paths.
		/// </summary>
		private List<string> getSelectedPaths() {
			if (listView.SelectedItems.Count == 0) {
				return new List<string>(0);
			}

			// Get an array of the listview items.
			ListViewItem[] itemArray = new ListViewItem[listView.SelectedItems.Count];
			listView.SelectedItems.CopyTo(itemArray, 0);

			// Convert the listviewitem array into a string array of the paths.
			string[] nameArray = Array.ConvertAll<ListViewItem, string>(itemArray, delegate(ListViewItem item) {
				return item.Name;
			});

			return new List<string>(nameArray);
		}

		/// <summary>
		/// Resizes the controls correctly.
		/// </summary>
		private void resizeControls() {
			if (this.WindowState == FormWindowState.Maximized) {
				listView.Width = this.Width - 13;
			} else {
				listView.Width = base.Width - 15;
			}

			listView.Height = this.Height - 60;

			tlsPath.Size = new Size(base.Width - 20 - (tlsFilter.Width + tlsDrives.Width), tlsPath.Height);
		}
		#endregion

		#region Events
		/// <summary>
		/// Refreshes the listview when Enter is pressed in the path textbox.
		/// </summary>
		private void tlsPath_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
			}
		}

		/// <summary>
		/// Refreshes the listview when a file/directory is double-clicked in the listview.
		/// </summary>
		private void listView_DoubleClick(object sender, EventArgs e) {
			if (listView.SelectedItems.Count > 0) {
				string path = listView.SelectedItems[0].Name;
				ExecuteOrUpdate(path);
			}
		}

		/// <summary>
		/// Resizes the controls when the listview changes size.
		/// </summary>
		private void this_ClientSizeChanged(object sender, EventArgs e) {
			resizeControls();
		}

		/// <summary>
		/// Selects an item in the listview when the Space bar is hit.
		/// </summary>
		private void listView_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space) {
				if (listView.SelectedItems != null &&
					listView.SelectedItems.Count > 0) {
					foreach (ListViewItem item in listView.SelectedItems) {
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
		/// Refreshes the listview when Enter is pressed in the filter textbox.
		/// </summary>
		void tlsFilter_KeyUp(object sender, KeyEventArgs e) {
			ExecuteOrUpdate();
		}

		/// <summary>
		/// Refreshes the listview when a different drive is selected.
		/// </summary>
		private void tlsDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			e.ClickedItem.Select();
			ExecuteOrUpdate(e.ClickedItem.Name);
		}

		private void listView_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				ShellContextMenu m = new ShellContextMenu();
				ShellContextMenu.ContextMenuResult contextMenuResult;

				if (listView.SelectedItems.Count > 1) {
					List<string> paths = getSelectedPaths();

					contextMenuResult = m.PopupMenu(paths, this.Handle);
				} else if (listView.SelectedItems.Count == 1) {
					contextMenuResult = m.PopupMenu(listView.SelectedItems[0].Name, this.Handle);
				} else {
					contextMenuResult = m.PopupMenu(Path, this.Handle);
				}

				// Update the file listing.
				if (contextMenuResult != ShellContextMenu.ContextMenuResult.NoUserFeedback &&
					contextMenuResult != ShellContextMenu.ContextMenuResult.ContextMenuError) {
					updateFileListing(true);
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

			updateFileListing(true);
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
				updateFileListing(true);
			}
		}

		void listView_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F2) {
				if (listView.SelectedItems.Count > 0) {
					ListViewItem item = listView.SelectedItems[0];
					item.BeginEdit();
				}
			}
		}

		void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
			if (!string.IsNullOrEmpty(e.Label)) {
				ListViewItem item = listView.Items[e.Item];
				FileSystemInfo fileSystemInfo = FileSystemInfoFactory.GetFileSystemInfo(item.Name);
				string path = fileSystemInfo.Path;

				try {
					File.Move(path + item.Text, path + e.Label);
				} catch (Exception ex) {
					e.CancelEdit = true;
					MessageBox.Show(ex.Message);
				}
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Update the drive information contained in the drive dropdown asynchronously.
		/// </summary>
		public void UpdateDriveListing() {
			// Set up a new background worker to delegate the asynchronous retrieval.
			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				// Anonymous method that grabs the drive information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					e.Result = FileSystem.GetDrives();
				};

				// Anonymous method to run after the drives are retrieved.
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					List<DriveInfo> drives = new List<DriveInfo>((IEnumerable<DriveInfo>)e.Result);

					tlsDrives.DropDownItems.Clear();
					bool isLocalDiskFound = false;

					foreach (DriveInfo driveInfo in drives) {
						ToolStripMenuItem item = new ToolStripMenuItem();
						item.Text = driveInfo.DisplayName;
						item.Name = driveInfo.FullPath;

						int imageIndex = OnGetImageIndex(driveInfo);
						item.Image = ImageList.Images[imageIndex];
						
						tlsDrives.DropDownItems.Add(item);

						if (!isLocalDiskFound) {
							if (driveInfo.DriveType == DriveType.Fixed &&
								driveInfo.IsReady) {
								isLocalDiskFound = true;
								item.Select();
								ExecuteOrUpdate(driveInfo.FullPath);
							}
						}
					}
				};

				backgroundWorker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Executes the file, or refreshes the listview for the selected directory in the path textbox.
		/// </summary>
		public void ExecuteOrUpdate() {
			ExecuteOrUpdate(tlsPath.Text);
		}

		/// <summary>
		/// Executes the provided file, or refreshes the listview for the provided directory.
		/// </summary>
		/// <param name="path"></param>
		public void ExecuteOrUpdate(string path) {
			if (System.IO.File.Exists(path)) {
				Process.Start(path);
			} else if (System.IO.Directory.Exists(path)) {
				updateFileListing(path, tlsFilter.Text);
			} else {
				MessageBox.Show("The path, " + path + ", looks like it is incorrect.");
			}
		}

		#region UpdateFileListing
		private void updateFileListing(bool forceUpdate) {
			updateFileListing(tlsPath.Text, tlsFilter.Text, forceUpdate);
		}

		private void updateFileListing(string path, string filter) {
			updateFileListing(path, filter, false);
		}

		/// <summary>
		/// Updates the listview with the specified path and filter.
		/// </summary>
		/// <param name="path">Path to get information about.</param>
		/// <param name="filter">Pattern to filter the information.</param>
		private void updateFileListing(string path, string filter, bool forceUpdate) {
			if (listView.SmallImageList == null) {
				listView.SmallImageList = ImageList;
			}

			// Prevents the retrieval of file information if unneccessary.
			if (!forceUpdate &&
				path.Equals(_path) &&
				filter.Equals(_filter)) {
				return;
			} else {
				_path = path;
				_filter = filter;
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
					// Grab the files and report the progress to the parent.
					backgroundWorker.ReportProgress(50);
					e.Result = FileSystem.GetFiles(directoryInfo, filter);
					backgroundWorker.ReportProgress(100);
				};
				
				// Method that runs when the DoWork method is finished.
				backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

				// Anonymous method that updates the status to the parent form.
				backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
					UpdateProgress(e.ProgressPercentage);
				};

				backgroundWorker.RunWorkerAsync();
			}

			// Update some information about the current directory.
			tlsPath.Text = directoryPath;
			this.Text = directoryPath;

			// Set up the watcher.
			fileSystemWatcher.Path = path;
			fileSystemWatcher.Filter = filter;
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// Event that gets fired when all of the file/directory information has been retrieved.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// Make sure we got back good information.
			if (e.Error == null && 
				e.Result != null &&
				e.Result is IEnumerable<FileSystemInfo>) {
				IEnumerable<FileSystemInfo> fileSystemInfoList = (IEnumerable<FileSystemInfo>)e.Result;
				int fileCount = 0;
				int folderCount = 0;

				try {
					listView.BeginUpdate();

					// TODO: Prevent listview scrolling here.

					// Clear the listview.
					listView.Items.Clear();

					// Create a new listview item with the display name.
					foreach (FileSystemInfo fileSystemInfo in fileSystemInfoList) {
						ListViewItem item = createListViewItem(fileSystemInfo, ref fileCount, ref folderCount);
						listView.Items.Add(item);
					}

					// Basic stuff that should happen everytime files are shown.
					listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

					listView.EndUpdate();

					UpdateStatus(string.Format("Folders: {0}; Files: {1}",
						folderCount-2,
						fileCount));
				} catch (System.UnauthorizedAccessException) {
					listView.BeginUpdate();
					listView.Items.Add("Unauthorized Access");
					listView.EndUpdate();
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + ex.StackTrace);
				}
			}
		}
		#endregion

		private ListViewItem createListViewItem(FileSystemInfo fileSystemInfo, ref int fileCount, ref int folderCount) {
			ListViewItem item = new ListViewItem(fileSystemInfo.DisplayName);
			item.Name = fileSystemInfo.FullPath;
			item.Tag = fileSystemInfo;

			// Get the image index for this filesystem object from the parent's ImageList.
			int imageIndex = OnGetImageIndex(fileSystemInfo);
			item.ImageIndex = imageIndex;

			if (fileSystemInfo is FileInfo) {
				item.SubItems.Add(General.GetHumanReadableSize(fileSystemInfo.Size.ToString()));
				fileCount++;
			} else {
				item.SubItems.Add(string.Empty);
				folderCount++;
			}

			item.SubItems.Add(fileSystemInfo.LastWriteTime.ToShortDateString());
			item.SubItems.Add(fileSystemInfo.LastWriteTime.ToShortTimeString());
			return item;
		}
		#endregion

		public ImageList ImageList {
			get {
				return ((Parent)this.MdiParent).ImageList;
			}
		}

		public string Path {
			get {
				return _path;
			}
		}

		public string Filter {
			get {
				return _filter;
			}
		}
	}
}