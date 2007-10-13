using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using SharpFile.IO;
using SharpFile.UI;
using SharpFile.Infrastructure;
using Common;

namespace SharpFile {
	public partial class FileBrowser : UserControl {
		private string _path;
		private string _filter;
		private System.IO.FileSystemWatcher fileSystemWatcher;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo fsi, DriveType driveType);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		public FileBrowser() {
			InitializeComponent();
			initializeComponent();
		}

		#region Delegate methods
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

		#region Private methods
		/// <summary>
		/// Sets the filebrowser up.
		/// </summary>
		private void initializeComponent() {
			this.Dock = DockStyle.Fill;
			this.DoubleBuffered = true;

			// Attach to some events.
			this.ClientSizeChanged += this_ClientSizeChanged;
			this.tlsPath.KeyDown += tlsPath_KeyDown;
			this.tlsFilter.KeyUp += tlsFilter_KeyUp;
			this.tlsDrives.DropDownItemClicked += tlsDrives_DropDownItemClicked;
			this.tlsDrives.ButtonClick += tlsDrives_ButtonClick;

			resizeControls();

			tlsFilter.Text = string.Empty;

			fileSystemWatcher = new System.IO.FileSystemWatcher(); //this, 5000);
			fileSystemWatcher.Changed += fileSystemWatcher_Changed;
			fileSystemWatcher.Renamed += fileSystemWatcher_Changed;
			fileSystemWatcher.Created += fileSystemWatcher_Changed;
			fileSystemWatcher.Deleted += fileSystemWatcher_Changed;
		}

		/// <summary>
		/// Resizes the controls correctly.
		/// </summary>
		private void resizeControls() {
			/*
			if (this.WindowState == FormWindowState.Maximized) {
				listView.Width = this.Width - 13;
			} else {
				listView.Width = base.Width - 15;
			}

			listView.Height = this.Height - 60;
			*/

			this.listView.Height = this.Height - 25;
			tlsPath.Size = new Size(base.Width - 5 - (tlsFilter.Width + tlsDrives.Width), tlsPath.Height);
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
		/// Resizes the controls when the listview changes size.
		/// </summary>
		private void this_ClientSizeChanged(object sender, EventArgs e) {
			resizeControls();
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
			tlsDrives.Image = e.ClickedItem.Image;
			tlsDrives.Tag = e.ClickedItem.Tag;
			ExecuteOrUpdate(((DriveInfo)e.ClickedItem.Tag).FullPath);
		}

		private void tlsDrives_ButtonClick(object sender, EventArgs e) {
			if (tlsDrives.Tag != null) {
				ExecuteOrUpdate(((DriveInfo)tlsDrives.Tag).FullPath);
			}
		}

		private void fileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e) {
			string path = e.FullPath;
			FileSystemInfo fsi = FileSystemInfoFactory.GetFileSystemInfo(path);

			MethodInvoker invoker = new MethodInvoker(delegate() {
				listView.BeginUpdate();

				switch (e.ChangeType) {
					case System.IO.WatcherChangeTypes.Changed:
						listView.Items.RemoveByKey(path);
						listView.UpdateListView(fsi);
						break;
					case System.IO.WatcherChangeTypes.Created:
						listView.UpdateListView(fsi);
						break;
					case System.IO.WatcherChangeTypes.Deleted:
						listView.Items.RemoveByKey(path);
						break;
					case System.IO.WatcherChangeTypes.Renamed:
						string oldFullPath = ((System.IO.RenamedEventArgs)e).OldFullPath;
						listView.Items.RemoveByKey(oldFullPath);
						listView.UpdateListView(fsi);
						break;
				}

				listView.EndUpdate();
			});

			listView.Invoke(invoker);
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
						item.Tag = driveInfo;

						int imageIndex = GetImageIndex(driveInfo, driveInfo.DriveType);
						if (imageIndex > -1) {
							item.Image = ImageList.Images[imageIndex];
						}

						tlsDrives.DropDownItems.Add(item);

						if (!isLocalDiskFound) {
							if (driveInfo.DriveType == DriveType.Fixed &&
								driveInfo.IsReady) {
								isLocalDiskFound = true;
								item.Select();
								tlsDrives.Image = item.Image;
								tlsDrives.Tag = driveInfo;
								updateListView(driveInfo.FullPath);
							}
						}
					}
				};

				backgroundWorker.RunWorkerAsync();
			}
		}
		#endregion

		private void updateListView(string path) {
			listView.UpdateListView(path, tlsFilter.Text, false, false);
		}

		private void updateListView(string path, bool forceUpdate) {
			listView.UpdateListView(path, tlsFilter.Text, forceUpdate, false);
		}

		#region ExecuteOrUpdate methods.
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
				listView.UpdateListView(path, tlsFilter.Text, true, true);
			} else {
				MessageBox.Show("The path, " + path + ", looks like it is incorrect.");
			}
		}
		#endregion

		public ImageList ImageList {
			get {
				return ((Child)this.Parent.Parent.Parent).ImageList;
			}
		}

		public string Path {
			get {
				return _path;
			}
			set {
				_path = value;
				tlsPath.Text = _path;
			}
		}

		public string Filter {
			get {
				return _filter;
			}
			set {
				_filter = value;
				tlsFilter.Text = _filter;
			}
		}

		public System.IO.FileSystemWatcher FileSystemWatcher {
			get {
				return fileSystemWatcher;
			}
		}

		public DriveInfo DriveInfo {
			get {
				return ((DriveInfo)tlsDrives.Tag);
			}
		}

		public ListView ListView {
			get {
				return listView;
			}
		}
	}
}