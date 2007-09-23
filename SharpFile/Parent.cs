using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpFile.FileSystem;

namespace SharpFile {
	public partial class Parent : Form {
		private static object lockObject = new object();

		public Parent() {
			InitializeComponent();
			this.DoubleBuffered = true;

			ShowNewForm(null, EventArgs.Empty);

			if (this.MdiChildren.Length == 1) {
				this.MdiChildren[0].WindowState = FormWindowState.Maximized;
			}
		}

		private void ShowNewForm(object sender, EventArgs e) {
			// Create a new instance of the child form.
			Child childForm = new Child();

			// Make it a child of this MDI form before showing it.
			childForm.MdiParent = this;
			childForm.Show();
			childForm.OnUpdateStatus += delegate(int value) {
				toolStripProgressBar.Value = value;
			};

			childForm.OnGetImageIndex += delegate(FileSystemInfo dataInfo) {
				return Infrastructure.GetImageIndex(dataInfo, imageList);
			};

			// Update the list view.
			childForm.UpdateDriveListing();
		}

		public ImageList ImageList {
			get {
				return imageList;
			}
		}

		private void OpenFile(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
				string FileName = openFileDialog.FileName;
				// TODO: Add code here to open the file.
			}
		}

		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
				string FileName = saveFileDialog.FileName;
				// TODO: Add code here to save the current contents of the form to a file.
			}
		}

		private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void CutToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
		}

		private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
		}

		private void PasteToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
		}

		private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
			toolStrip.Visible = toolBarToolStripMenuItem.Checked;
		}

		private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
			statusStrip.Visible = statusBarToolStripMenuItem.Checked;
		}

		private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) {
			LayoutMdi(MdiLayout.Cascade);
		}

		private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e) {
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e) {
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
			foreach (Form childForm in MdiChildren) {
				childForm.Close();
			}
		}
	}
}