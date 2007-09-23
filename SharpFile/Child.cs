using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
//using System.IO;
using System.Diagnostics;
using Common;
using System.ComponentModel;
using SharpFile.FileSystem;
using System.Collections;

namespace SharpFile {
	public partial class Child : Form {
		private string _path;
		private string _pattern;
		private UnitDisplay unitDisplay = UnitDisplay.Bytes;
		private IList<string> selectedFiles = new List<string>();

		public delegate void OnUpdateStatusDelegate(int value);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate int OnGetImageIndexDelegate(FileSystemInfo dataInfo);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		public Child() {
			InitializeComponent();
			setup();
		}

		protected void UpdateStatus(int value) {
			if (OnUpdateStatus != null)
				OnUpdateStatus(value);
		}

		protected int GetImageIndex(FileSystemInfo dataInfo) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(dataInfo);
			}

			return -1;
		}

		#region Private methods
		private void setup() {
			this.DoubleBuffered = true;

			// Attach to some events.
			this.ClientSizeChanged += new EventHandler(listView_ClientSizeChanged);
			this.listView.DoubleClick += new EventHandler(listView_DoubleClick);
			this.listView.KeyDown += new KeyEventHandler(listView_KeyDown);
			this.txtPath.KeyDown += new KeyEventHandler(txtPath_KeyDown);
			this.txtPattern.KeyDown += new KeyEventHandler(txtPattern_KeyDown);
			this.ddlDrives.SelectedIndexChanged += new EventHandler(ddlDrives_SelectedIndexChanged);
			this.ddlDrives.DataSourceChanged += new EventHandler(ddlDrives_DataSourceChanged);

			resizeControls();

			// Set some options on the listview.
			listView.View = View.Details;

			List<string> columns = new List<string>();
			columns.Add("Filename");
			columns.Add("Size");
			columns.Add("Date");
			columns.Add("Time");

			foreach (string column in columns) {
				listView.Columns.Add(column);
			}

			txtPattern.Text = "*.*";
		}

		void ddlDrives_DataSourceChanged(object sender, EventArgs e) {
			resizeDropDown(ddlDrives);
		}

		private void resizeControls() {
			txtPath.Left = ddlDrives.Right + 5;
			txtPattern.Left = txtPath.Right + 5;

			if (this.WindowState == FormWindowState.Maximized) {
				listView.Width = this.Width - 13;
			} else {
				listView.Width = base.Width - 15;
			}

			listView.Height = this.Height - 60;
		}
		#endregion

		#region Events
		void txtPath_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
			}
		}

		void listView_DoubleClick(object sender, EventArgs e) {
			if (listView.SelectedItems.Count > 0) {
				string path = listView.SelectedItems[0].Name;
				ExecuteOrUpdate(path);
			}
		}		

		void listView_ClientSizeChanged(object sender, EventArgs e) {
			resizeControls();
		}

		void listView_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Space) {
				if (listView.SelectedItems.Count > 0) {
					ListViewItem item = listView.SelectedItems[0];
					string fileName = item.Name;

					if (!selectedFiles.Contains(fileName)) {
						item.ForeColor = Color.Red;
						selectedFiles.Add(fileName);
					} else {
						item.ForeColor = Color.Black;
						selectedFiles.Remove(fileName);
					}
				}
			}
		}

		void txtPattern_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				ExecuteOrUpdate();
			}
		}

		void ddlDrives_SelectedIndexChanged(object sender, EventArgs e) {
			ExecuteOrUpdate(((DriveInfo)ddlDrives.SelectedItem).FullPath);
		}
		#endregion

		#region Public methods
		public void UpdateDriveListing() {
			// Set up a new background worker to delegate the asynchronous retrieval.
			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				// Anonymous method that grabs the drive information.
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					e.Result = Infrastructure.GetDrives();
				};

				// Anonymous method to be run when after the drives are retrieved.
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					List<DriveInfo> drives = new List<DriveInfo>((IEnumerable<DriveInfo>)e.Result);

					ddlDrives.Items.Clear();
					ddlDrives.DataSource = drives;
					ddlDrives.DisplayMember = "DisplayName";
					ddlDrives.ValueMember = "FullPath";

					DriveInfo localDisk = drives.Find(delegate(DriveInfo di) {
						return di.DriveType == DriveType.LocalDisk;
					});

					if (localDisk != null) {
						ddlDrives.SelectedItem = localDisk;
					}
				};

				backgroundWorker.RunWorkerAsync();
			}
		}

		public void ExecuteOrUpdate() {
			ExecuteOrUpdate(txtPath.Text);
		}

		public void ExecuteOrUpdate(string path) {
			if (System.IO.File.Exists(path)) {
				Process.Start(path);
			} else if (System.IO.Directory.Exists(path)) {
				updateFileListing(path, txtPattern.Text);
			} else {
				MessageBox.Show("The path, " + path + " looks like it is incorrect.");
			}
		}

		private void resizeDropDown(ComboBox cmbBox) {
			if (!cmbBox.IsHandleCreated || !(cmbBox.DataSource is IList)) {
				return;
			}

			using (Graphics g = cmbBox.CreateGraphics()) {
				int maxLength = 0;
				IList list = cmbBox.DataSource as IList;

				int numItems = Math.Min(list.Count, cmbBox.MaxDropDownItems);

				// Find the longest string in the first MaxDropDownItems
				for (int i = 0; i < numItems; i++)
					maxLength = Math.Max(maxLength,
					(int)g.MeasureString(cmbBox.GetItemText(list[i]), cmbBox.Font).Width);

				maxLength += 20; // Add a little buffer for the scroll bar

				// Make sure we are inbounds of the screen
				int left = this.PointToScreen(new Point(0, this.Left)).X;
				if (maxLength > Screen.PrimaryScreen.WorkingArea.Width - left)
					maxLength = Screen.PrimaryScreen.WorkingArea.Width - left;

				cmbBox.DropDownWidth = Math.Max(maxLength, cmbBox.Width);
			}
		}

		#region UpdateFileListing
		private void updateFileListing(string path, string pattern) {
			if (listView.SmallImageList == null) {
				listView.SmallImageList = ((Parent)this.MdiParent).ImageList;
			}

			// Prevents the retrieval of file information if unneccessary.
			if (path.Equals(_path) &&
				pattern.Equals(_pattern)) {
				return;
			}

			_path = path;
			_pattern = pattern;

			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
			string directoryPath = directoryInfo.FullName;
			directoryPath = string.Format("{0}{1}",
				directoryPath,
				directoryPath.EndsWith(@"\") ? string.Empty : @"\");

			listView.Items.Clear();

			using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e) {
					// If this was split out differently, this might make more sense.
					// Maybe get directories first, then files. Then filter them. Or something.
					backgroundWorker.ReportProgress(50);
					e.Result = Infrastructure.GetFiles(directoryInfo, pattern);
					backgroundWorker.ReportProgress(100);
				};

				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
				backgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e) {
					UpdateStatus(e.ProgressPercentage);
				};

				backgroundWorker.RunWorkerAsync();
			}

			txtPath.Text = directoryPath;
			this.Text = directoryPath;
		}

		void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (e.Error == null && 
				e.Result != null &&
				e.Result is IEnumerable<FileSystemInfo>) {
				IEnumerable<FileSystemInfo> dataInfos = (IEnumerable<FileSystemInfo>)e.Result;

				try {
					listView.BeginUpdate();
					foreach (FileSystemInfo dataInfo in dataInfos) {
						double size;
						ListViewItem item = new ListViewItem(dataInfo.DisplayName);
						item.Name = dataInfo.FullPath;
						int imageIndex = OnGetImageIndex(dataInfo);
						item.ImageIndex = imageIndex;

						switch (unitDisplay) {
							case UnitDisplay.KiloBytes:
								size = Math.Round(Convert.ToDouble(dataInfo.Size), 2) / 1024;
								break;
							case UnitDisplay.MegaBytes:
								size = Math.Round(Convert.ToDouble(dataInfo.Size), 2) / 1024 / 1024;
								break;
							default:
								size = dataInfo.Size;
								break;
						}

						if (dataInfo is FileInfo) {
							item.SubItems.Add(General.GetHumanReadableSize(size));
						} else {
							item.SubItems.Add(string.Empty);
						}

						item.SubItems.Add(dataInfo.LastWriteTime.ToShortDateString());
						item.SubItems.Add(dataInfo.LastWriteTime.ToShortTimeString());
						listView.Items.Add(item);
					}
					listView.EndUpdate();

					// Basic stuff that should happen everytime files are shown.
					listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
	}
}