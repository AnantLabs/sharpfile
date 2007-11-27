using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using System.ComponentModel;

namespace SharpFile {
	public class BaseParent : Form {
		protected const string formName = "SharpFile";

		protected Timer timer = new Timer();
		protected ProgressDisk.ProgressDisk progressDisk = new ProgressDisk.ProgressDisk();
		protected StatusStrip statusStrip = new StatusStrip();
		protected ToolStripStatusLabel toolStripStatus = new ToolStripStatusLabel();
		protected MenuStrip menuStrip = new MenuStrip();

		protected ToolStripMenuItem fileMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem newToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem editMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem undoToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem redoToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem cutToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem copyToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem pasteToolStripMenuItem = new ToolStripMenuItem();
		protected ToolStripMenuItem selectAllToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripSeparator toolStripSeparator5 = new ToolStripSeparator();
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

			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer, true);

			this.FormClosing += BaseParent_FormClosing;
			this.Load += BaseParent_Load;

			this.Resize += delegate(object sender, EventArgs e) {
				this.progressDisk.Location = new Point(base.ClientSize.Width - 35,
					base.ClientSize.Height - 18);
			};

			timer.Enabled = true;
			timer.Tick += delegate {
				progressDisk.Value = (progressDisk.Value + 1) % 12;
			};
		}

		void BaseParent_Load(object sender, EventArgs e) {
			this.Width = Settings.Instance.Width;
			this.Height = Settings.Instance.Height;

			onFormLoad();
		}

		void BaseParent_FormClosing(object sender, FormClosingEventArgs e) {
			Settings.Instance.Width = this.Width;
			Settings.Instance.Height = this.Height;

			onFormClosing();
		}

		private void initializeComponents() {
			ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseParent));

			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();

			this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.fileMenu,
				this.editMenu,
				this.viewMenu,
				this.toolsMenu,
				//this.windowsMenu,
				this.helpMenu});
			//this.menuStrip.MdiWindowListItem = this.windowsMenu;
			this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip.TabIndex = 0;

			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.newToolStripMenuItem,
				this.toolStripSeparator5,
				this.exitToolStripMenuItem});
			this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
			this.fileMenu.Size = new System.Drawing.Size(35, 20);
			this.fileMenu.Text = "&File";

			this.toolStripSeparator5.Size = new System.Drawing.Size(142, 6);

			this.exitToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);

			this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.undoToolStripMenuItem,
				this.redoToolStripMenuItem,
				this.toolStripSeparator6,
				this.cutToolStripMenuItem,
				this.copyToolStripMenuItem,
				this.pasteToolStripMenuItem,
				this.toolStripSeparator7,
				this.selectAllToolStripMenuItem});
			this.editMenu.Size = new System.Drawing.Size(37, 20);
			this.editMenu.Text = "&Edit";

			this.undoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripMenuItem.Image")));
			this.undoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.undoToolStripMenuItem.Text = "&Undo";

			this.redoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripMenuItem.Image")));
			this.redoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.redoToolStripMenuItem.Text = "&Redo";

			this.toolStripSeparator6.Size = new System.Drawing.Size(164, 6);

			this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
			this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);

			this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
			this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);

			this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
			this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);

			this.toolStripSeparator7.Size = new System.Drawing.Size(164, 6);

			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";

			this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.statusBarToolStripMenuItem});
			this.viewMenu.Size = new System.Drawing.Size(41, 20);
			this.viewMenu.Text = "&View";

			this.statusBarToolStripMenuItem.Checked = true;
			this.statusBarToolStripMenuItem.CheckOnClick = true;
			this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.statusBarToolStripMenuItem.Text = "&Status Bar";
			this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);

			this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.optionsToolStripMenuItem});
			this.toolsMenu.Size = new System.Drawing.Size(44, 20);
			this.toolsMenu.Text = "&Tools";

			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
			this.optionsToolStripMenuItem.Text = "&Options";

			this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.aboutToolStripMenuItem});
			this.helpMenu.Size = new System.Drawing.Size(40, 20);
			this.helpMenu.Text = "&Help";

			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.aboutToolStripMenuItem.Text = "&About ...";

			this.progressDisk.ActiveForeColor1 = System.Drawing.Color.LightGray;
			this.progressDisk.ActiveForeColor2 = System.Drawing.Color.White;
			this.progressDisk.BackGroundColor = System.Drawing.Color.Transparent;
			this.progressDisk.BlockSize = ProgressDisk.BlockSize.Medium;
			this.progressDisk.InactiveForeColor1 = System.Drawing.Color.DimGray;
			this.progressDisk.InactiveForeColor2 = System.Drawing.Color.DarkGray;
			this.progressDisk.Location = new System.Drawing.Point(597, 435);
			this.progressDisk.Size = new System.Drawing.Size(16, 16);
			this.progressDisk.SquareSize = 16;
			this.progressDisk.TabIndex = 4;

			this.statusStrip.Items.AddRange(new ToolStripItem[] {
			    this.toolStripStatus});
			this.statusStrip.Location = new System.Drawing.Point(0, 431);
			this.statusStrip.Size = new System.Drawing.Size(632, 22);
			this.statusStrip.Dock = DockStyle.Bottom;
			this.statusStrip.Visible = true;

			this.toolStripStatus.Size = new System.Drawing.Size(0, 10);
			this.toolStripStatus.Dock = DockStyle.Bottom;

			addControls();

			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.progressDisk);
			this.Controls.Add(this.statusStrip);

			this.menuStrip.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.ResumeLayout(false);
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

		protected virtual void addControls() {
		}

		protected virtual void onFormClosing() {
		}

		protected virtual void onFormLoad() {
		}

		public ImageList ImageList {
			get {
				return Settings.Instance.ImageList;
			}
		}
	}
}