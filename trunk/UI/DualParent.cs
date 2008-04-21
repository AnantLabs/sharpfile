using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using SharpFile.UI;

namespace SharpFile {
    public class DualParent : BaseParent {
        protected ToolStripMenuItem filterPanel1ToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem filterPanel2ToolStripMenuItem = new ToolStripMenuItem();
        private SplitContainer splitContainer = new SplitContainer();
        private ToolStripMenuItem swapPanelItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanelItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanel1Item = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanel2Item = new ToolStripMenuItem();
        private ToolStripMenuItem panelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem verticalPanelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem horizontalPanelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem panel1Menu = new ToolStripMenuItem();
        private ToolStripMenuItem panel2Menu = new ToolStripMenuItem();        
        private int splitterPercentage;
        private bool panel1HasFocus = false;
        private bool panel2HasFocus = false;

        public DualParent() {
            Child child1 = new Child("view1");
            child1.TabControl.Appearance = TabAppearance.FlatButtons;
            child1.TabControl.IsVisible = true;
            child1.Dock = DockStyle.Fill;

            child1.GetImageIndex += delegate(IResource fsi) {
                return IconManager.GetImageIndex(fsi, ImageList);
            };

            child1.UpdatePath += delegate(string path) {
                this.Text = string.Format("{0} - {1}",
                                          formName,
                                          path);
            };

            Child child2 = new Child("view2");
            child2.Dock = DockStyle.Fill;
            child2.TabControl.IsVisible = true;
            child2.TabControl.Appearance = TabAppearance.FlatButtons;

            child2.GetImageIndex += delegate(IResource fsi) {
                return IconManager.GetImageIndex(fsi, ImageList);
            };

            child2.UpdatePath += delegate(string path) {
                this.Text = string.Format("{0} - {1}",
                                          formName,
                                          path);
            };

            this.Shown += delegate {
                // Attach the handler to any children that have the specified event.
                Forms.AddEventHandlerInChildren(this, "UpdateStatus",
                (SharpFile.Infrastructure.View.UpdateStatusDelegate)delegate(string status) {
                    toolStripStatus.Text = status;
                });

                Forms.AddEventHandlerInChildren(this, "UpdateProgress",
                (SharpFile.Infrastructure.View.UpdateProgressDelegate)delegate(int value) {
                    updateProgress(value);
                });
            };

            splitContainer.SplitterWidth = 1;
            splitContainer.Panel1.Controls.Add(child1);
            splitContainer.Panel2.Controls.Add(child2);

            splitContainer.SplitterMoving += delegate(object sender, SplitterCancelEventArgs e) {
                decimal percent = 0;
                int mouseCursorX = 0;
                int mouseCursorY = 0;

                if (this.splitContainer.Orientation == Orientation.Vertical) {
                    percent = Convert.ToDecimal(e.SplitX) / Convert.ToDecimal(this.Width);
                    mouseCursorX = e.MouseCursorX - 10;
                    mouseCursorY = e.MouseCursorY;
                } else {
                    percent = Convert.ToDecimal(e.SplitY) / Convert.ToDecimal(this.Height - 75);
                    mouseCursorX = e.MouseCursorX;
                    mouseCursorY = e.MouseCursorY - 10;
                }

                splitterPercentage = Convert.ToInt32(percent * 100);

                string tip = string.Format("{0}%",
                    splitterPercentage);

                toolTip.Show(tip, this, mouseCursorX, mouseCursorY);
            };

            splitContainer.SplitterMoved += delegate {
                toolTip.RemoveAll();
            };

            splitContainer.Panel1.Enter += delegate {
                panel1HasFocus = true;
                panel2HasFocus = false;
            };

            splitContainer.Panel2.Enter += delegate {
                panel1HasFocus = false;
                panel2HasFocus = true;
            };

            MenuItem twentyFivePercentMenuItem = new MenuItem("25%", splitterContextMenuOnClick);
            MenuItem fiftyPercentMenuItem = new MenuItem("50%", splitterContextMenuOnClick);
            MenuItem seventyFivePercentMenuItem = new MenuItem("75%", splitterContextMenuOnClick);
            ContextMenu splitterContextMenu = new ContextMenu(new MenuItem[] {
				twentyFivePercentMenuItem,
				fiftyPercentMenuItem,
				seventyFivePercentMenuItem
			});

            splitContainer.ContextMenu = splitterContextMenu;
        }

        private void splitterContextMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            splitterPercentage = Convert.ToInt32(menuItem.Text.Replace("%", string.Empty));
            double percent = Convert.ToDouble(splitterPercentage) * 0.01;

            if (splitContainer.Orientation == Orientation.Vertical) {
                splitContainer.SplitterDistance = Convert.ToInt32(this.Width * percent);
            } else if (splitContainer.Orientation == Orientation.Horizontal) {
                splitContainer.SplitterDistance = Convert.ToInt32((this.Height - 75) * percent);
            }
        }

        private void setSplitterDistance() {
            decimal percent = Convert.ToDecimal(splitterPercentage * 0.01);
            int splitterDistance = 0;

            switch (this.splitContainer.Orientation) {
                case Orientation.Horizontal:
                    splitterDistance = Convert.ToInt32(percent * (this.Height - 75));
                    break;
                case Orientation.Vertical:
                    splitterDistance = Convert.ToInt32(percent * this.Width);
                    break;
            }

            splitContainer.SplitterDistance = splitterDistance;
        }

        protected override void addMenuStripItems() {
            this.menuStrip.Items.AddRange(new ToolStripItem[]
			                              	{
			                              		this.fileMenu,
                                                this.panel1Menu,
			                              		this.editMenu,
			                              		this.viewMenu,
			                              		this.toolsMenu,
                                                this.panel2Menu,
			                              		this.helpMenu
			                              	});

            this.panel1Menu.Text = "Panel &1";
            this.panel1Menu.DropDownItems.AddRange(new ToolStripItem[] {
                this.filterPanel1ToolStripMenuItem
            });

            this.panel2Menu.Text = "Panel &2";
            this.panel2Menu.DropDownItems.AddRange(new ToolStripItem[] {
                this.filterPanel2ToolStripMenuItem
            });

            this.filterPanel1ToolStripMenuItem.Checked = true;
            this.filterPanel1ToolStripMenuItem.CheckOnClick = true;
            this.filterPanel1ToolStripMenuItem.CheckState = CheckState.Checked;
            this.filterPanel1ToolStripMenuItem.Size = new Size(135, 22);
            this.filterPanel1ToolStripMenuItem.Text = "&Show Filter";
            this.filterPanel1ToolStripMenuItem.Click += delegate {
                Common.Forms.SetPropertyInChild<bool>(this.splitContainer.Panel1, "ShowFilter",
                    this.filterPanel1ToolStripMenuItem.Checked);
            };

            this.filterPanel2ToolStripMenuItem.Checked = true;
            this.filterPanel2ToolStripMenuItem.CheckOnClick = true;
            this.filterPanel2ToolStripMenuItem.CheckState = CheckState.Checked;
            this.filterPanel2ToolStripMenuItem.Size = new Size(135, 22);
            this.filterPanel2ToolStripMenuItem.Text = "&Show Filter";
            this.filterPanel2ToolStripMenuItem.Click += delegate {
                Common.Forms.SetPropertyInChild<bool>(this.splitContainer.Panel2, "ShowFilter",
                    this.filterPanel2ToolStripMenuItem.Checked);
            };

            this.viewMenu.DropDownItems.AddRange(new ToolStripItem[] {
                this.displayPanelItem,
                this.panelOrientationItem,
                this.swapPanelItem
            });

            this.displayPanelItem.Text = "Display Panel";
            this.displayPanelItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.displayPanel1Item,
				this.displayPanel2Item
			});

            this.displayPanel1Item.Text = "Panel 1";
            this.displayPanel1Item.Checked = true;
            this.displayPanel1Item.CheckOnClick = true;
            this.displayPanel1Item.CheckStateChanged += delegate(object sender, EventArgs e) {
                if (!this.splitContainer.Panel2Collapsed) {
                    this.splitContainer.Panel1Collapsed = !this.displayPanel1Item.Checked;
                } else {
                    this.displayPanel1Item.Checked = true;
                }
            };

            this.displayPanel2Item.Text = "Panel 2";
            this.displayPanel2Item.Checked = true;
            this.displayPanel2Item.CheckOnClick = true;
            this.displayPanel2Item.CheckStateChanged += delegate(object sender, EventArgs e) {
                if (!this.splitContainer.Panel1Collapsed) {
                    this.splitContainer.Panel2Collapsed = !this.displayPanel2Item.Checked;
                } else {
                    this.displayPanel2Item.Checked = true;
                }
            };

            this.panelOrientationItem.Text = "Orientation";
            this.panelOrientationItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.horizontalPanelOrientationItem,
				this.verticalPanelOrientationItem
			});

            this.horizontalPanelOrientationItem.Text = "Horizontal";
            this.horizontalPanelOrientationItem.Click += delegate(object sender, EventArgs e) {
                if (this.splitContainer.Orientation != Orientation.Horizontal) {
                    this.splitContainer.Orientation = Orientation.Horizontal;
                    this.horizontalPanelOrientationItem.Checked = true;
                    this.verticalPanelOrientationItem.Checked = false;
                } else {
                    this.horizontalPanelOrientationItem.Checked = true;
                    this.verticalPanelOrientationItem.Checked = false;
                }

                setSplitterDistance();
            };

            this.verticalPanelOrientationItem.Text = "Vertical";
            this.verticalPanelOrientationItem.Click += delegate(object sender, EventArgs e) {
                if (this.splitContainer.Orientation != Orientation.Vertical) {
                    this.splitContainer.Orientation = Orientation.Vertical;
                    this.verticalPanelOrientationItem.Checked = true;
                    this.horizontalPanelOrientationItem.Checked = false;
                } else {
                    this.verticalPanelOrientationItem.Checked = true;
                    this.horizontalPanelOrientationItem.Checked = false;
                }

                setSplitterDistance();
            };

            this.swapPanelItem.Text = "Swap Panels";
            this.swapPanelItem.MouseUp += delegate(object sender, MouseEventArgs e) {
                Control panel1Control = this.splitContainer.Panel1.Controls[0];
                Control panel2Control = this.splitContainer.Panel2.Controls[0];

                this.splitContainer.Panel2.Controls.Clear();
                this.splitContainer.Panel2.Controls.Add(panel1Control);

                this.splitContainer.Panel1.Controls.Clear();
                this.splitContainer.Panel1.Controls.Add(panel2Control);
            };
        }

        protected override void addControls() {
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Size = new Size(641, 364);
            this.splitContainer.SplitterDistance = 318;

            ((SplitContainer)this.Controls["baseSplitContainer"]).Panel1.Controls.Add(this.splitContainer);
            base.addControls();
        }

        protected override void onFormClosing() {
            base.onFormClosing();

            // Save the paths for each panel.
            Settings.Instance.DualParent.Panel1.Paths = Forms.GetPropertyInChild<List<string>>(
                this.splitContainer.Panel1, "Paths");
            Settings.Instance.DualParent.Panel2.Paths = Forms.GetPropertyInChild<List<string>>(
                this.splitContainer.Panel2, "Paths");

            // Save whether each panel is collapsed or not.
            Settings.Instance.DualParent.Panel1.Collapsed = this.splitContainer.Panel1Collapsed;
            Settings.Instance.DualParent.Panel2.Collapsed = this.splitContainer.Panel2Collapsed;

            // Save some non-panel specific information.
            Settings.Instance.DualParent.SplitterPercentage = splitterPercentage;
            Settings.Instance.DualParent.Orientation = this.splitContainer.Orientation;

            // Save whether each panel has a visible filter.
            Settings.Instance.DualParent.Panel1.ShowFilter = Forms.GetPropertyInChild<bool>(
                this.splitContainer.Panel1, "ShowFilter");
            Settings.Instance.DualParent.Panel2.ShowFilter = Forms.GetPropertyInChild<bool>(
                this.splitContainer.Panel2, "ShowFilter");
        }

        protected override void onFormLoad() {
            base.onFormLoad();

            // Get the paths from the panels.
            Forms.SetPropertyInChild<List<string>>(this.splitContainer.Panel1, "Paths", 
                Settings.Instance.DualParent.Panel1.Paths);
            Forms.SetPropertyInChild<List<string>>(this.splitContainer.Panel2, "Paths", 
                Settings.Instance.DualParent.Panel2.Paths);

            // Get the split container orientation.
            Orientation orientation = Settings.Instance.DualParent.Orientation;
            this.splitContainer.Orientation = orientation;

            // Calculate the splitter distance based on the percentage stored and the width of the form.
            splitterPercentage = Settings.Instance.DualParent.SplitterPercentage;
            setSplitterDistance();

            // Check the appropriate orientation in the menus.
            switch (orientation) {
                case Orientation.Horizontal:
                    this.horizontalPanelOrientationItem.Checked = true;
                    break;
                case Orientation.Vertical:
                    this.verticalPanelOrientationItem.Checked = true;
                    break;
            }

            // Collapse/check menu panel 1 if appropriate.
            bool panel1Collapsed = Settings.Instance.DualParent.Panel1.Collapsed;
            this.splitContainer.Panel1Collapsed = panel1Collapsed;
            this.displayPanel1Item.Checked = !panel1Collapsed;

            // Collapse/check menu panel 2 if appropriate.
            bool panel2Collapsed = Settings.Instance.DualParent.Panel2.Collapsed;
            this.splitContainer.Panel2Collapsed = panel2Collapsed;
            this.displayPanel2Item.Checked = !panel2Collapsed;

            // Show filter/check menu panel 1 if appropriate.
            bool panel1ShowFilter = Settings.Instance.DualParent.Panel1.ShowFilter;
            Forms.SetPropertyInChild<bool>(this.splitContainer.Panel1, "ShowFilter",
                panel1ShowFilter);
            this.filterPanel1ToolStripMenuItem.Checked = panel1ShowFilter;

            // Show filter/check menu panel 2 if appropriate.
            bool panel2ShowFilter = Settings.Instance.DualParent.Panel2.ShowFilter;
            Forms.SetPropertyInChild<bool>(this.splitContainer.Panel2, "ShowFilter",
                panel2ShowFilter);
            this.filterPanel2ToolStripMenuItem.Checked = panel2ShowFilter;

            // Set the drive name format.
            Forms.SetPropertyInChild<FormatTemplate>(this.splitContainer.Panel1, "DriveFormatTemplate",
                Settings.Instance.DualParent.Panel1.DriveFormatTemplate);
            Forms.SetPropertyInChild<FormatTemplate>(this.splitContainer.Panel2, "DriveFormatTemplate",
                Settings.Instance.DualParent.Panel2.DriveFormatTemplate);
        }

        public string SelectedPath1 {
            get {
                return Forms.GetPropertyInChild<string>(this.splitContainer.Panel1, "SelectedPath");
            }
        }

        public string SelectedPath2 {
            get {
                return Forms.GetPropertyInChild<string>(this.splitContainer.Panel2, "SelectedPath");
            }
        }

        public string SelectedPath {
            get {
                if (panel1HasFocus) {
                    return SelectedPath1;
                } else {
                    return SelectedPath2;
                }
            }
        }

        public string SelectedFile1 {
            get {
                return Forms.GetPropertyInChild<string>(this.splitContainer.Panel1, "SelectedFile");
            }
        }

        public string SelectedFile2 {
            get {
                return Forms.GetPropertyInChild<string>(this.splitContainer.Panel2, "SelectedFile");
            }
        }

        public string SelectedFile {
            get {
                if (panel1HasFocus) {
                    return SelectedFile1;
                } else {
                    return SelectedFile2;
                }
            }
        }
    }
}