using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.UI;
using SharpFile.IO;

namespace SharpFile {
	public enum ParentType {
		Mdi,
		[Obsolete("Tab ParentType not explicitedly supported.")]
		Tab,
		Dual
	}

	public partial class MdiParent : Form {
		private static object lockObject = new object();
		private Timer timer = new Timer();
		private ImageList imageList = new ImageList();
		private ParentType parentType = ParentType.Mdi;

		public MdiParent() {
			InitializeComponent();
			this.DoubleBuffered = true;

			SetStyle(ControlStyles.AllPaintingInWmPaint | 
				ControlStyles.OptimizedDoubleBuffer, true);

			ShowNewForm(null, EventArgs.Empty);

			if (this.MdiChildren.Length == 1) {
				this.MdiChildren[0].WindowState = FormWindowState.Maximized;
			}

			timer.Enabled = true;
			timer.Tick += delegate {
				progressDisk.Value = (progressDisk.Value + 1) % 12;
			};

			imageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		/*
		void Parent_MdiChildActivate(object sender, EventArgs e) {
			// If child form is new and no has tabPage, create new tabPage
			if (this.ActiveMdiChild.Tag == null) {
				// Add a tabPage to tabControl with child form caption
				TabPage tabPage = new TabPage(this.ActiveMdiChild.Text);
				tabPage.Controls.Add(this.ActiveMdiChild);
				tabPage.Tag = this.ActiveMdiChild;
				tabPage.Parent = tabControl;
				tabControl.SelectedTab = tabPage;

				this.ActiveMdiChild.WindowState = FormWindowState.Maximized;

				this.ActiveMdiChild.Tag = tabPage;
				//this.ActiveMdiChild.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed);
			}

			if (!tabControl.Visible)
				tabControl.Visible = true;
		}
		*/

		private void ShowNewForm(object sender, EventArgs e) {
			// Create a new instance of the child form.
			MdiChild childForm = new MdiChild();
			// Make it a child of this MDI form before showing it.
			childForm.MdiParent = this;
			childForm.Show();

			childForm.Child.OnUpdateStatus += delegate(string status) {
				toolStripStatus.Text = status;
			};

			childForm.Child.OnUpdateProgress += delegate(int value) {
				if (value < 100) {
					if (!timer.Enabled) {
						progressDisk.Value = 4;
						progressDisk.Visible = true;
						timer.Enabled = true;
					}
				} else if (value == 100) {
					if (timer.Enabled) {
						progressDisk.Visible = false;
						timer.Enabled = false;
					}
				}
			};

			childForm.Child.OnGetImageIndex += delegate(FileSystemInfo fsi, DriveType driveType) {
				return IconManager.GetImageIndex(fsi, ImageList, driveType);
			};
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);

			this.progressDisk.Location = new Point(base.ClientSize.Width - 35,
				base.ClientSize.Height - 18);
		}

		public ImageList ImageList {
			get {
				lock (lockObject) {
					return imageList;
				}
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