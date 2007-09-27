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

namespace SharpFile {
	public partial class Child : Form {
		private string _path;
		private string _pattern;
		private UnitDisplay unitDisplay = UnitDisplay.Bytes;

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
			setup();
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
		private void setup() {
			this.DoubleBuffered = true;

			// Attach to some events.
			this.ClientSizeChanged += new EventHandler(listView_ClientSizeChanged);
			this.listView.DoubleClick += new EventHandler(listView_DoubleClick);
			this.listView.KeyDown += new KeyEventHandler(listView_KeyDown);
			this.tlsPath.KeyDown += new KeyEventHandler(tlsPath_KeyDown);
			this.tlsPattern.KeyDown += new KeyEventHandler(tlsPattern_KeyDown);
			this.tlsDrives.DropDownItemClicked += new ToolStripItemClickedEventHandler(tlsDrives_DropDownItemClicked);

			resizeControls();

			// Set some options on the listview.
			listView.View = View.Details;
			tlsPattern.Text = "*.*";

			List<string> columns = new List<string>();
			columns.Add("Filename");
			columns.Add("Size");
			columns.Add("Date");
			columns.Add("Time");

			foreach (string column in columns) {
				listView.Columns.Add(column);
			}
		}

		#region Resize methods
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

			tlsPath.Size = new Size(base.Width - 15 - (tlsPattern.Width + tlsDrives.Width), tlsPath.Height);// .Width = 100; //base.Width - (tlsPattern.Width + tlsDrives.Width) + 100;
		}
		#endregion
		#endregion

		#region Events
		/// <summary>
		/// Refreshes the listview when Enter is pressed in the path textbox.
		/// </summary>
		void tlsPath_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
			}
		}

		/// <summary>
		/// Refreshes the listview when a file/directory is double-clicked in the listview.
		/// </summary>
		void listView_DoubleClick(object sender, EventArgs e) {
			if (listView.SelectedItems.Count > 0) {
				string path = listView.SelectedItems[0].Name;
				ExecuteOrUpdate(path);
			}
		}

		/// <summary>
		/// Resizes the controls when the listview changes size.
		/// </summary>
		void listView_ClientSizeChanged(object sender, EventArgs e) {
			resizeControls();
		}

		/// <summary>
		/// Selects an item in the listview when the Space bar is hit.
		/// </summary>
		void listView_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space) {
				if (listView.SelectedItems.Count > 0) {
					ListViewItem item = listView.SelectedItems[0];
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

		/// <summary>
		/// Refreshes the listview when Enter is pressed in the pattern textbox.
		/// </summary>
		void tlsPattern_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
			}
		}

		/// <summary>
		/// Refreshes the listview when a different drive is selected.
		/// </summary>
		void tlsDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			e.ClickedItem.Select();
			ExecuteOrUpdate(e.ClickedItem.Name);
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

						int imageIndex = IconManager.GetImageIndex(driveInfo, ImageList);
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
				updateFileListing(path, tlsPattern.Text);
			} else {
				MessageBox.Show("The path, " + path + ", looks like it is incorrect.");
			}
		}

		private void updateSelectedTotalSize(long size) {
			totalSelectedSize += size;

			UpdateStatus(string.Format("Selected items: {0}",
									General.GetHumanReadableSize(totalSelectedSize.ToString())));
		}

		#region UpdateFileListing
		/// <summary>
		/// Updates the listview with the specified path and pattern.
		/// </summary>
		/// <param name="path">Path to get information about.</param>
		/// <param name="pattern">Pattern to filter the information.</param>
		private void updateFileListing(string path, string pattern) {
			if (listView.SmallImageList == null) {
				listView.SmallImageList = ImageList;
			}

			// Prevents the retrieval of file information if unneccessary.
			if (path.Equals(_path) &&
				pattern.Equals(_pattern)) {
				return;
			} else {
				_path = path;
				_pattern = pattern;
			}

			// Get the directory information.
			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
			string directoryPath = directoryInfo.FullName;
			directoryPath = string.Format("{0}{1}",
				directoryPath,
				directoryPath.EndsWith(@"\") ? string.Empty : @"\");

			// Clear the listview.
			listView.Items.Clear();

			// Create another thread to get the file information asynchronously.
			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				backgroundWorker.WorkerReportsProgress = true;

				// Anonymous method that retrieves the file information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					// If this was split out differently, this might make more sense.
					// Maybe get directories first, then files. Then filter them. Or something.
					backgroundWorker.ReportProgress(50);
					e.Result = FileSystem.GetFiles(directoryInfo, pattern);
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
		}

		/// <summary>
		/// Event that gets fired when all of the file/directory information has been retrieved.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			// Make sure we got back good information.
			if (e.Error == null && 
				e.Result != null &&
				e.Result is IEnumerable<FileSystemInfo>) {
				IEnumerable<FileSystemInfo> dataInfos = (IEnumerable<FileSystemInfo>)e.Result;
				int fileCount = 0;
				int folderCount = 0;

				try {
					listView.BeginUpdate();
					foreach (FileSystemInfo dataInfo in dataInfos) {
						// Create a new listview item with the display name.
						ListViewItem item = new ListViewItem(dataInfo.DisplayName);
						item.Name = dataInfo.FullPath;

						// Get the image index for this filesystem object from the parent's ImageList.
						int imageIndex = OnGetImageIndex(dataInfo);
						item.ImageIndex = imageIndex;

						if (dataInfo is FileInfo) {
							item.SubItems.Add(General.GetHumanReadableSize(dataInfo.Size.ToString()));
							fileCount++;
						} else {
							item.SubItems.Add(string.Empty);
							folderCount++;
						}

						item.SubItems.Add(dataInfo.LastWriteTime.ToShortDateString());
						item.SubItems.Add(dataInfo.LastWriteTime.ToShortTimeString());
						listView.Items.Add(item);
					}
					listView.EndUpdate();

					// Basic stuff that should happen everytime files are shown.
					listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
		#endregion

		public ImageList ImageList {
			get {
				return ((Parent)this.MdiParent).ImageList;
			}
		}
	}
}