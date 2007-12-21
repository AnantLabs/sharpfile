using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ProgressDisk;
using SharpFile.Infrastructure;
using SharpFile.UI;

namespace SharpFile {
	public class BaseParent : Form {
		protected const string formName = "SharpFile";

		private static readonly object lockObject = new object();
        private DriveDetector driveDetector;

		private int progressCount = 0;
		protected ToolTip toolTip = new ToolTip();
		protected Timer timer = new Timer();
		protected ProgressDisk.ProgressDisk progressDisk = new ProgressDisk.ProgressDisk();
		protected StatusStrip statusStrip = new StatusStrip();
		protected ToolStripStatusLabel toolStripStatus = new ToolStripStatusLabel();
		protected MenuStrip menuStrip = new MenuStrip();

		protected ToolStripMenuItem fileMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem reloadSettingsStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem editMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem undoToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem redoToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem cutToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem copyToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem pasteToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem selectAllToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripSeparator toolStripSeparator6 = new ToolStripSeparator();
		protected ToolStripSeparator toolStripSeparator7 = new ToolStripSeparator();

		protected ToolStripMenuItem viewMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem statusBarToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem toolsMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem optionsToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem helpMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem aboutToolStripMenuItem = new ToolStripMenuItem();

		public BaseParent() {
			initializeComponents();
			this.DoubleBuffered = true;

            driveDetector = new DriveDetector();

			SetStyle(ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.OptimizedDoubleBuffer, true);

			this.FormClosing += BaseParent_FormClosing;
			this.Load += BaseParent_Load;

			this.Resize += delegate {
				this.progressDisk.Location = new Point(base.ClientSize.Width - 35,
													   base.ClientSize.Height - 18);
			};

			timer.Enabled = true;
			timer.Tick += delegate {
				progressDisk.Value = (progressDisk.Value + 1) % 12;
			};
		}

        protected void updateProgress(int value) {
            lock (lockObject) {
                if (value < 100) {
                    progressCount++;
                } else if (value == 100) {
                    if (progressCount > 0) {
                        progressCount--;
                    }
                }

                if (progressCount > 0) {
                    if (!timer.Enabled) {
                        progressDisk.Value = 4;
                        progressDisk.Visible = true;
                        timer.Enabled = true;
                    }
                } else {
                    if (timer.Enabled) {
                        progressDisk.Visible = false;
                        timer.Enabled = false;
                    }
                }
            }
        }

		private void BaseParent_Load(object sender, EventArgs e) {
			this.Width = Settings.Instance.Width;
			this.Height = Settings.Instance.Height;

			onFormLoad();
		}

		private void BaseParent_FormClosing(object sender, FormClosingEventArgs e) {
			Settings.Instance.Width = this.Width;
			Settings.Instance.Height = this.Height;

			onFormClosing();
		}

		private void initializeComponents() {
			ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseParent));

			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();

			addMenuStripItems();
			this.menuStrip.BackColor = SystemColors.Control;
            this.menuStrip.RenderMode = ToolStripRenderMode.System;
            this.menuStrip.TabIndex = 0;

            this.fileMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
                                                        this.reloadSettingsStripMenuItem,
														this.exitToolStripMenuItem
			                                     	});
            this.fileMenu.ImageTransparentColor = SystemColors.ActiveBorder;
            this.fileMenu.Size = new Size(35, 20);
            this.fileMenu.Text = "&File";

            this.exitToolStripMenuItem.Size = new Size(145, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += this.ExitToolsStripMenuItem_Click;

            this.reloadSettingsStripMenuItem.Text = "&Reload Settings";
            this.reloadSettingsStripMenuItem.Click += this.reloadSettingsStripMenuItem_Click;

			this.editMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.undoToolStripMenuItem,
			                                     		this.redoToolStripMenuItem,
			                                     		this.toolStripSeparator6,
			                                     		this.cutToolStripMenuItem,
			                                     		this.copyToolStripMenuItem,
			                                     		this.pasteToolStripMenuItem,
			                                     		this.toolStripSeparator7,
			                                     		this.selectAllToolStripMenuItem
			                                     	});
			this.editMenu.Size = new Size(37, 20);
			this.editMenu.Text = "&Edit";

			this.undoToolStripMenuItem.Image = ((Image)(resources.GetObject("undoToolStripMenuItem.Image")));
			this.undoToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.undoToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.Z);
			this.undoToolStripMenuItem.Size = new Size(167, 22);
			this.undoToolStripMenuItem.Text = "&Undo";

			this.redoToolStripMenuItem.Image = (Image)(resources.GetObject("redoToolStripMenuItem.Image"));
			this.redoToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.redoToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.Y);
			this.redoToolStripMenuItem.Size = new Size(167, 22);
			this.redoToolStripMenuItem.Text = "&Redo";

			this.toolStripSeparator6.Size = new Size(164, 6);

			this.cutToolStripMenuItem.Image = (Image)(resources.GetObject("cutToolStripMenuItem.Image"));
			this.cutToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.cutToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.X);
			this.cutToolStripMenuItem.Size = new Size(167, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			this.cutToolStripMenuItem.Click += this.CutToolStripMenuItem_Click;

			this.copyToolStripMenuItem.Image = (Image)(resources.GetObject("copyToolStripMenuItem.Image"));
			this.copyToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.copyToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.C);
			this.copyToolStripMenuItem.Size = new Size(167, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += this.CopyToolStripMenuItem_Click;

			this.pasteToolStripMenuItem.Image = (Image)(resources.GetObject("pasteToolStripMenuItem.Image"));
			this.pasteToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.pasteToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.V);
			this.pasteToolStripMenuItem.Size = new Size(167, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += this.PasteToolStripMenuItem_Click;

			this.toolStripSeparator7.Size = new Size(164, 6);

			this.selectAllToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.A);
			this.selectAllToolStripMenuItem.Size = new Size(167, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";

			this.viewMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.statusBarToolStripMenuItem
			                                     	});
			this.viewMenu.Size = new Size(41, 20);
			this.viewMenu.Text = "&View";

			this.statusBarToolStripMenuItem.Checked = true;
			this.statusBarToolStripMenuItem.CheckOnClick = true;
			this.statusBarToolStripMenuItem.CheckState = CheckState.Checked;
			this.statusBarToolStripMenuItem.Size = new Size(135, 22);
			this.statusBarToolStripMenuItem.Text = "&Status Bar";
			this.statusBarToolStripMenuItem.Click += this.StatusBarToolStripMenuItem_Click;

			this.toolsMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                      	{
			                                      		this.optionsToolStripMenuItem
			                                      	});
			this.toolsMenu.Size = new Size(44, 20);
			this.toolsMenu.Text = "&Tools";

			this.optionsToolStripMenuItem.Size = new Size(122, 22);
			this.optionsToolStripMenuItem.Text = "&Options";

			this.helpMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.aboutToolStripMenuItem
			                                     	});
			this.helpMenu.Size = new Size(40, 20);
			this.helpMenu.Text = "&Help";

			this.aboutToolStripMenuItem.Size = new Size(173, 22);
			this.aboutToolStripMenuItem.Text = "&About ...";

			this.progressDisk.ActiveForeColor1 = Color.LightGray;
			this.progressDisk.ActiveForeColor2 = Color.White;
			this.progressDisk.BackGroundColor = Color.Transparent;
			this.progressDisk.BlockSize = BlockSize.Medium;
			this.progressDisk.InactiveForeColor1 = Color.DimGray;
			this.progressDisk.InactiveForeColor2 = Color.DarkGray;
			this.progressDisk.Location = new Point(597, 435);
			this.progressDisk.Size = new Size(16, 16);
			this.progressDisk.SquareSize = 16;
			this.progressDisk.TabIndex = 4;

			this.statusStrip.Items.AddRange(new ToolStripItem[]
			                                	{
			                                		this.toolStripStatus
			                                	});
			this.statusStrip.Location = new Point(0, 431);
			this.statusStrip.Size = new Size(632, 22);
			this.statusStrip.Dock = DockStyle.Bottom;
			this.statusStrip.Visible = true;

			this.toolStripStatus.Size = new Size(0, 10);
			this.toolStripStatus.Dock = DockStyle.Bottom;

			addControls();

			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.progressDisk);
			this.Controls.Add(this.statusStrip);

			this.MainMenuStrip = this.menuStrip;

			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

        private void reloadSettingsStripMenuItem_Click(object sender, EventArgs e) {
            Settings.Load();

            // Refresh the screen.
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

		protected virtual void addControls() {
		}

		protected virtual void onFormClosing() {
		}

		protected virtual void onFormLoad() {
		}

		protected virtual void addMenuStripItems() {
		}

        public DriveDetector DriveDetector {
            get {
                return driveDetector;
            }
        }

		public ImageList ImageList {
			get {
				return Settings.Instance.ImageList;
			}
		}
	}
}