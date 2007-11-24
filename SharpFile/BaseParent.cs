using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
	public class BaseParent : Form {
		protected Settings settings;
		protected Timer timer = new Timer();
		protected ProgressDisk.ProgressDisk progressDisk = new ProgressDisk.ProgressDisk();
		protected StatusStrip statusStrip = new StatusStrip();
		protected ToolStripStatusLabel toolStripStatus = new ToolStripStatusLabel();

		public BaseParent(Settings settings) {
			this.settings = settings;
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
			this.Width = settings.Width;
			this.Height = settings.Height;

			onFormLoad();
		}

		void BaseParent_FormClosing(object sender, FormClosingEventArgs e) {
			settings.Width = this.Width;
			settings.Height = this.Height;

			onFormClosing();
		}

		private void initializeComponents() {
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();

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

			this.Controls.Add(this.progressDisk);
			this.Controls.Add(this.statusStrip);

			this.statusStrip.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		protected virtual void addControls() {
		}

		protected virtual void onFormClosing() {
		}

		protected virtual void onFormLoad() {
		}

		public ImageList ImageList {
			get {
				return settings.ImageList;
			}
		}
	}
}