using System;
using System.Windows.Forms;
using SharpFile.IO;
using SharpFile.UI;
using SharpFile.Infrastructure;

namespace SharpFile {
    public class MdiParent : BaseParent {
        protected ToolStripMenuItem windowsMenu = new ToolStripMenuItem();
        protected ToolStripMenuItem newWindowToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem cascadeToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem tileVerticalToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem tileHorizontalToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem closeAllToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem arrangeIconsToolStripMenuItem = new ToolStripMenuItem();

        public MdiParent() {
            this.IsMdiContainer = true;

            ShowNewForm(null, EventArgs.Empty);

            if (this.MdiChildren.Length == 1) {
                this.MdiChildren[0].WindowState = FormWindowState.Maximized;
            }

            this.MdiChildActivate += delegate {
                ((MdiChild)this.ActiveMdiChild).Child.OnUpdatePath += delegate(string path) {
                    this.Text = string.Format("{0} - {1}",
                            formName,
                            path);
                };
            };
        }

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
                updateProgress(value);
            };

            childForm.Child.OnGetImageIndex += delegate(IResource fsi) {
                return IconManager.GetImageIndex(fsi, ImageList);
            };

            childForm.Child.OnUpdatePath += delegate(string path) {
                this.Text = string.Format("{0} - {1}",
                    formName,
                    path);
            };
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

        protected override void addMenuStripItems() {
            base.menuStrip.Items.AddRange(new ToolStripItem[]
			                              	{
			                              		base.fileMenu,
			                              		base.editMenu,
			                              		base.viewMenu,
			                              		base.toolsMenu,
			                              		this.windowsMenu,
			                              		base.helpMenu
			                              	});

            base.menuStrip.MdiWindowListItem = this.windowsMenu;

            this.windowsMenu.DropDownItems.AddRange(new ToolStripItem[] {
				this.newWindowToolStripMenuItem,
				this.cascadeToolStripMenuItem,
				this.tileVerticalToolStripMenuItem,
				this.tileHorizontalToolStripMenuItem,
				this.closeAllToolStripMenuItem,
				this.arrangeIconsToolStripMenuItem});
            this.windowsMenu.Size = new System.Drawing.Size(62, 20);
            this.windowsMenu.Text = "&Windows";

            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.newWindowToolStripMenuItem.Text = "&New Window";
            this.newWindowToolStripMenuItem.Click += this.ShowNewForm;

            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            this.cascadeToolStripMenuItem.Click += this.CascadeToolStripMenuItem_Click;

            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tileVerticalToolStripMenuItem.Text = "Tile &Vertical";
            this.tileVerticalToolStripMenuItem.Click += this.TileVerticleToolStripMenuItem_Click;

            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tileHorizontalToolStripMenuItem.Text = "Tile &Horizontal";
            this.tileHorizontalToolStripMenuItem.Click += this.TileHorizontalToolStripMenuItem_Click;

            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.closeAllToolStripMenuItem.Text = "C&lose All";
            this.closeAllToolStripMenuItem.Click += this.CloseAllToolStripMenuItem_Click;

            this.arrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.arrangeIconsToolStripMenuItem.Text = "&Arrange Icons";
            this.arrangeIconsToolStripMenuItem.Click += this.ArrangeIconsToolStripMenuItem_Click;
        }
    }
}