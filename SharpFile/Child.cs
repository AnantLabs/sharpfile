using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Common;

namespace SharpFile {
	public partial class Child : Form {
		private UnitDisplay unitDisplay = UnitDisplay.Bytes;
		private string _path;
		private string _pattern;

		public Child() {
			InitializeComponent();
			setup();
		}

		#region Private methods
		private void setup() {
			// Attach to some events.
			this.ClientSizeChanged += new EventHandler(listView_ClientSizeChanged);
			this.Shown += new EventHandler(Child_Shown);
			this.listView.DoubleClick += new EventHandler(listView_DoubleClick);
			this.txtPath.KeyDown += new KeyEventHandler(txtPath_KeyDown);
			this.txtPath.Leave += new EventHandler(txtPath_Leave);
			this.txtPattern.KeyDown += new KeyEventHandler(txtPattern_KeyDown);
			this.txtPattern.Leave += new EventHandler(txtPattern_Leave);

			resizeControls();

			// Set some options ont he listview.
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

		private void resizeControls() {
			ddlDrives.Size = new Size(50, ddlDrives.Height);
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
		void txtPath_Leave(object sender, EventArgs e) {
			UpdateFileListing();
		}

		void txtPath_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				UpdateFileListing();
			}
		}

		void listView_DoubleClick(object sender, EventArgs e) {
			ListView view = (ListView)sender;

			string path = txtPath.Text + view.SelectedItems[0].Text;
			UpdateFileListing(path);
		}

		void Child_Shown(object sender, EventArgs e) {
			resizeControls();
		}

		void listView_ClientSizeChanged(object sender, EventArgs e) {
			resizeControls();
		}

		void txtPattern_Leave(object sender, EventArgs e) {
			UpdateFileListing();
		}

		void txtPattern_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				UpdateFileListing();
			}
		}
		#endregion

		#region Public methods
		public void UpdateDriveListing() {
			ddlDrives.Items.Clear();
			IEnumerable<DriveInfo> drives = Infrastructure.GetDrives();

			foreach (DriveInfo driveInfo in drives) {
				ddlDrives.Items.Add(driveInfo.Name + @"\");
			}

			ddlDrives.SelectedIndex = 0;
		}

		public void UpdateFileListing() {
			UpdateFileListing(txtPath.Text, txtPattern.Text);
		}

		public void UpdateFileListing(string path) {
			UpdateFileListing(path, txtPattern.Text);
		}

		public void UpdateFileListing(string path, string pattern) {
			if (path.Equals(_path) &&
				pattern.Equals(_pattern)) {
				return;
			}

			_path = path;
			_pattern = pattern;

			if (File.Exists(path)) {
				Process.Start(path);
			} else if (Directory.Exists(path)) {
				System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);

				string directoryPath = directoryInfo.FullName;
				txtPath.Text = string.Format("{0}{1}",
					directoryPath,
					directoryPath.EndsWith(@"\") ? string.Empty : @"\");

				listView.Items.Clear();

				IEnumerable<DataInfo> dataInfos = Infrastructure.GetFiles(directoryInfo, pattern);

				try {
					foreach (DataInfo dataInfo in dataInfos) {
						double size;
						ListViewItem item = new ListViewItem(dataInfo.DisplayName);
						//item.ImageIndex = Data.GetImageIndex(theFile, imageList, imageHash);

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

						listView.BeginUpdate();
						listView.Items.Add(item);
						listView.EndUpdate();
					}

					// Basic stuff that should happen everytime files are shown.
					listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
					this.Text = directoryPath;
				} catch (System.UnauthorizedAccessException) {
					listView.BeginUpdate();
					listView.Items.Add("Unauthorized Access");
					listView.EndUpdate();
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + ex.StackTrace);
				}
			} else {
				MessageBox.Show("The path, " + path + " looks like it is incorrect.");
			}
		}
		#endregion
	}
}