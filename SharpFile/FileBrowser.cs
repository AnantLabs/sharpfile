using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using SharpFile.IO.Retrievers;
using SharpFile.IO.ParentResources;
using SharpFile.IO.ChildResources;
using SharpFile.IO;
using SharpFile.UI;
using Common;

namespace SharpFile {
	public partial class FileBrowser : System.Windows.Forms.TabPage {
		private static readonly object lockObject = new object();

		private System.IO.FileSystemWatcher fileSystemWatcher;
		private ImageList imageList;
		private bool handleCreated = false;

		private ToolStrip toolStrip;
		private ToolStripSplitButton tlsDrives;
		private ToolStripSpringTextBox tlsPath;
		private ToolStripTextBox tlsFilter;
		private IView view;

		public event SharpFile.IO.View.OnGetImageIndexDelegate OnGetImageIndex;
		public event SharpFile.IO.View.OnUpdatePathDelegate OnUpdatePath;

		/// <summary>
		/// Filebrowser ctor.
		/// </summary>
		public FileBrowser() {
			this.toolStrip = new ToolStrip();
			this.tlsDrives = new ToolStripSplitButton();
			this.tlsPath = new ToolStripSpringTextBox();
			this.tlsFilter = new ToolStripTextBox();
			this.view = new SharpFile.ListView();
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
			// FileBrowser
			// 
			this.Controls.Add(this.view.Control);
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
		/// <param name="path">Path.</param>
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
			this.view.OnUpdatePath += view_OnUpdatePath;
			this.HandleCreated += FileBrowser_HandleCreated;

			fileSystemWatcher = new System.IO.FileSystemWatcher();
			fileSystemWatcher.Changed += fileSystemWatcher_Changed;
			fileSystemWatcher.Renamed += fileSystemWatcher_Changed;
			fileSystemWatcher.Created += fileSystemWatcher_Changed;
			fileSystemWatcher.Deleted += fileSystemWatcher_Changed;
		}
		#endregion

		#region Events
		/// <summary>
		/// Fires when the FileBrowser handle has been created.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FileBrowser_HandleCreated(object sender, EventArgs e) {
			handleCreated = true;
		}

		/// <summary>
		/// Displays the current path in the tab text and textbox.
		/// </summary>
		private void view_OnUpdatePath(string path) {
			this.Text = path;
			this.tlsPath.Text = path;

			UpdatePath(path);
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
			IParentResource resource = (IParentResource)e.ClickedItem.Tag;
			resource.Execute(view);
			highlightParentResource(resource, e.ClickedItem.Image);
		}

		/// <summary>
		/// Refreshes the view with the current root drive.
		/// </summary>
		private void tlsDrives_ButtonClick(object sender, EventArgs e) {
			IParentResource resource = (IParentResource)tlsDrives.Tag;
			resource.Execute(view);
		}

		/// <summary>
		/// Fires when the filesystem watcher sees a filesystem event.
		/// </summary>
		private void fileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e) {
			string path = e.FullPath;
			IChildResource resource = ChildResourceFactory.GetChildResource(path);

			// Required to ensure the view update occurs on the calling thread.
			MethodInvoker updater = new MethodInvoker(delegate() {
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
			});

			view.Control.BeginInvoke(updater);
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

					MethodInvoker updater = delegate() {
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

							// Gets information about the path already specified or the drive currently being added.
							if (!isLocalDiskFound) {
								// If the path has been defined and it is valid, then grab information about it.
								if (!string.IsNullOrEmpty(Path)) {
									IChildResource childResource = ChildResourceFactory.GetChildResource(Path);

									if (childResource != null &&
										childResource.Root.FullPath == resource.FullPath) {
										isLocalDiskFound = true;

										highlightParentResource(childResource.Root, item.Image);
										childResource.Execute(view);
									}
								}

								// If the view hasn't been updated and the resource is a drive, then grab some information about it.
								if (!isLocalDiskFound && resource is DriveInfo) {
									DriveInfo driveInfo = (DriveInfo)resource;

									if (driveInfo.DriveType == DriveType.Fixed &&
										driveInfo.IsReady) {
										isLocalDiskFound = true;

										highlightParentResource(driveInfo, item.Image);
										driveInfo.Execute(view);
									}
								}
							}
						}
					};

					if (handleCreated) {
						this.BeginInvoke(updater);
					}
				}
			}
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Highlights the passed-in drive.
		/// </summary>
		private void highlightParentResource(IParentResource resource, Image image) {
			foreach (ToolStripItem item in tlsDrives.DropDownItems) {
				IParentResource parentResource = ((IParentResource)item.Tag);

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
		/// The current path.
		/// </summary>
		public string Path {
			get {
				if (string.IsNullOrEmpty(this.tlsPath.Text)) {
					if (string.IsNullOrEmpty(ParentResource.FullPath)) {
						// TODO: This shouldn't be hard-coded.
						this.tlsPath.Text = @"c:\";
					} else {
						this.tlsPath.Text = ParentResource.FullPath;
					}
				}

				return this.tlsPath.Text;
			} set {
				this.tlsPath.Text = value;
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
		public System.IO.FileSystemWatcher FileSystemWatcher {
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