using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using SharpFile.IO;
using SharpFile.Infrastructure;
using SharpFile.UI;
using System.Threading;

namespace SharpFile {
	public partial class FileBrowser : System.Windows.Forms.TabPage {
		private string _path;
		//private string _filter;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private ImageList imageList;

		private static readonly object lockObject = new object();

		private ToolStrip toolStrip;
		private ToolStripSplitButton tlsDrives;
		private ToolStripSpringTextBox tlsPath;
		private ToolStripTextBox tlsFilter;
		private ListView listView;

		public delegate int OnGetImageIndexDelegate(IResource fsi);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		public delegate void OnUpdatePathDelegate(string path);
		public event OnUpdatePathDelegate OnUpdatePath;

		/// <summary>
		/// Filebrowser ctor.
		/// </summary>
		public FileBrowser() {
			this.toolStrip = new ToolStrip();
			this.tlsDrives = new ToolStripSplitButton();
			this.tlsPath = new ToolStripSpringTextBox();
			this.tlsFilter = new ToolStripTextBox();
			this.listView = new SharpFile.ListView();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.tlsDrives,
				this.tlsPath,
				this.tlsFilter});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.RenderMode = ToolStripRenderMode.System;
			this.toolStrip.ShowItemToolTips = false;
			this.toolStrip.Size = new System.Drawing.Size(454, 25);
			this.toolStrip.TabIndex = 1;
			// 
			// tlsDrives
			// 
			this.tlsDrives.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsDrives.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsDrives.Size = new System.Drawing.Size(32, 22);
			// 
			// tlsPath
			// 
			this.tlsPath.Size = new System.Drawing.Size(100, 25);
			// 
			// tlsFilter
			// 
			this.tlsFilter.Size = new System.Drawing.Size(50, 25);
			// 
			// listView
			// 
			this.listView.AllowColumnReorder = true;
			this.listView.AllowDrop = true;
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.FullRowSelect = true;
			this.listView.LabelEdit = true;
			this.listView.Location = new System.Drawing.Point(0, 25);
			this.listView.Size = new System.Drawing.Size(454, 229);
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// FileBrowser
			// 
			this.Controls.Add(this.listView);
			this.Controls.Add(this.toolStrip);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			initializeComponent();

			List<IParentResourceRetriever> resourceRetrievers = new List<IParentResourceRetriever>();
			resourceRetrievers.Add(new DriveRetriever());
			resourceRetrievers.Add(new ServerRetriever());

			UpdateParentListing(resourceRetrievers);
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

		/// <summary>
		/// Passes the path to any listening events.
		/// </summary>
		/// <param name="value">Percentage value for status.</param>
		protected void UpdatePath(string path) {
			if (OnUpdatePath != null) {
				OnUpdatePath(path);
			}
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
			this.tlsPath.KeyDown += tlsPath_KeyDown;
			this.tlsFilter.KeyUp += tlsFilter_KeyUp;
			this.tlsDrives.DropDownItemClicked += tlsDrives_DropDownItemClicked;
			this.tlsDrives.ButtonClick += tlsDrives_ButtonClick;
			this.listView.OnUpdatePath += listView_OnUpdatePath;

			fileSystemWatcher = new System.IO.FileSystemWatcher();
			fileSystemWatcher.Changed += fileSystemWatcher_Changed;
			fileSystemWatcher.Renamed += fileSystemWatcher_Changed;
			fileSystemWatcher.Created += fileSystemWatcher_Changed;
			fileSystemWatcher.Deleted += fileSystemWatcher_Changed;
		}
		#endregion

		#region Events
		/// <summary>
		/// Displays the current path in the tab text and textbox.
		/// </summary>
		private void listView_OnUpdatePath(string path) {
			this.Text = path;
			this.tlsPath.Text = path;
			_path = path;

			UpdatePath(path);
		}

		/// <summary>
		/// Refreshes the listview when Enter is pressed in the path textbox.
		/// </summary>
		private void tlsPath_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
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
			DriveInfo driveInfo = (DriveInfo)e.ClickedItem.Tag;
			tlsDrives.Image = e.ClickedItem.Image;
			tlsDrives.Tag = driveInfo;

			ExecuteOrUpdate(driveInfo.FullPath);
			highlightDrive(driveInfo);
		}

		/// <summary>
		/// Refreshes the listview with the current root drive.
		/// </summary>
		private void tlsDrives_ButtonClick(object sender, EventArgs e) {
			ExecuteOrUpdate(((DriveInfo)tlsDrives.Tag).FullPath);
		}

		/// <summary>
		/// Fires when the filesystem watcher sees a filesystem event.
		/// </summary>
		private void fileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e) {
			string path = e.FullPath;
			IChildResource resource = ChildResourceFactory.GetChildResource(path);

			// Required to ensure the listview update occurs on the calling thread.
			MethodInvoker updater = new MethodInvoker(delegate() {
				listView.BeginUpdate();

				switch (e.ChangeType) {
					case System.IO.WatcherChangeTypes.Changed:
						listView.Items.RemoveByKey(path);
						listView.UpdateListView(resource);
						break;
					case System.IO.WatcherChangeTypes.Created:
						listView.UpdateListView(resource);
						break;
					case System.IO.WatcherChangeTypes.Deleted:
						listView.Items.RemoveByKey(path);
						break;
					case System.IO.WatcherChangeTypes.Renamed:
						string oldFullPath = ((System.IO.RenamedEventArgs)e).OldFullPath;
						listView.Items.RemoveByKey(oldFullPath);
						listView.UpdateListView(resource);
						break;
				}

				listView.EndUpdate();
			});

			listView.Invoke(updater);
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Update the drive information contained in the drive dropdown asynchronously.
		/// </summary>
		public void UpdateParentListing(IEnumerable<IParentResourceRetriever> resourceRetrievers) {
			// Clear the dropdown.
			tlsDrives.DropDownItems.Clear();

			// Queue all of the resource retrievers onto the threadpool and set up the callback method.
			foreach (IParentResourceRetriever resourceRetriever in resourceRetrievers) {
				ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateParentListing_Callback), resourceRetriever);
			}
		}

		/// <summary>
		/// The callback that inserts the information retrieved fromt he resource retriever into the parent dropdown.
		/// </summary>
		/// <param name="stateInfo">The resource retriever.</param>
		private void UpdateParentListing_Callback(object stateInfo) {
			lock (lockObject) {
				if (stateInfo is IParentResourceRetriever) {
					IParentResourceRetriever resourceRetriever = (IParentResourceRetriever)stateInfo;
					List<IParentResource> resources = new List<IParentResource>(resourceRetriever.Get());

					MethodInvoker updater = new MethodInvoker(delegate() {
						bool isLocalDiskFound = false;

						// Create a new menu item in the dropdown for each drive.
						foreach (IParentResource resource in resources) {
							ToolStripMenuItem item = new ToolStripMenuItem();
							item.Text = resource.DisplayName;
							item.Name = resource.FullPath;
							item.Tag = resource;

							int imageIndex = GetImageIndex(resource);
							if (imageIndex > -1) {
								item.Image = ImageList.Images[imageIndex];
							}

							tlsDrives.DropDownItems.Add(item);

							// Grab some information for the first fixed disk we find that is ready.
							if (resource is DriveInfo && !isLocalDiskFound) {
								DriveInfo driveInfo = (DriveInfo)resource;

								if (driveInfo.DriveType == DriveType.Fixed &&
									driveInfo.IsReady) {
									isLocalDiskFound = true;
									tlsDrives.Image = item.Image;
									tlsDrives.Tag = driveInfo;
									highlightDrive(driveInfo);
									ExecuteOrUpdate(driveInfo.FullPath);
								}
							}
						}
					});

					this.Invoke(updater);
				}
			}
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
				// Required to ensure the listview update occurs on the calling thread.
				MethodInvoker updater = new MethodInvoker(delegate() {
					listView.UpdateListView(path, tlsFilter.Text);
				});

				listView.Invoke(updater);
			} else {
				MessageBox.Show("The path, " + path + ", looks like it is incorrect.");
			}
		}
		#endregion
		#endregion

		#region Private methods.
		/// <summary>
		/// Highlights the passed-in drive.
		/// </summary>
		private void highlightDrive(IParentResource driveInfo) {
			foreach (ToolStripItem item in tlsDrives.DropDownItems) {
				if (item.Tag == driveInfo) {
					item.BackColor = SystemColors.HighlightText;
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
					imageList = IconManager.FindImageList(this.Parent);
				}

				return imageList;
			}
		}

		/// <summary>
		/// The current path.
		/// </summary>
		public string Path {
			get {
				return _path;
			}
		}

		/// <summary>
		/// The current filter.
		/// </summary>
		//public string Filter {
		//    get {
		//        return _filter;
		//    }
		//}

		/// <summary>
		/// The current FileSystemWatcher.
		/// </summary>
		public System.IO.FileSystemWatcher FileSystemWatcher {
			get {
				return fileSystemWatcher;
			}
		}

		/// <summary>
		/// The currently selected drive.
		/// </summary>
		public DriveInfo DriveInfo {
			get {
				return ((DriveInfo)tlsDrives.Tag);
			}
		}

		/// <summary>
		/// The child listview.
		/// </summary>
		public ListView ListView {
			get {
				return listView;
			}
		}
		#endregion
	}
}