using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    public class DualParent : BaseParent {
        private ToolStripMenuItem swapPanelItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanelItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanel1Item = new ToolStripMenuItem();
        private ToolStripMenuItem displayPanel2Item = new ToolStripMenuItem();
        private ToolStripMenuItem panelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem verticalPanelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem horizontalPanelOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem panel1Menu = new ToolStripMenuItem();
        private ToolStripMenuItem panel2Menu = new ToolStripMenuItem();        
        private int splitterPercentage = 50;

        protected override void dockPanelContextMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;

            switch (menuItem.Text) {
                case "Add tab":
                    if (MousePosition.X < PointToScreen(dockPanel.Panes[1].Location).X) {
                        addPanel1Browser();
                    } else {
                        addPanel2Browser();
                    }
                    break;
                case "Close tab":
                    dockPanel.ActiveDocument.DockHandler.Close();
                    break;
                case "25%":
                case "50%":
                case "75%":
                    // TODO: Setting widths doesn't work correctly.
                    splitterPercentage = Convert.ToInt32(menuItem.Text.Replace("%", string.Empty));
                    double percent = Convert.ToDouble(splitterPercentage) * 0.01;
                    dockPanel.Panes[1].Width = Convert.ToInt32(dockPanel.Panes[1].Width * percent);
                    break;
            }
        }

        protected new Browser getBrowser() {
            Browser browser = base.getBrowser();
            browser.FormClosing += delegate {
                // Prevent the last browser from being closed if there is only one browser left in the active pane.
                if (browser.Pane.Contents.Count == 2) {
                    foreach (IDockContent content in browser.Pane.Contents) {
                        content.DockHandler.CloseButton = false;
                    }
                }
            };

            return browser;
        }

        protected new void dockPanelContextMenu_Popup(object sender, EventArgs e) {
            base.dockPanelContextMenu_Popup(sender, e);

            if (dockPanel.Panes.Count == 2
                && (MousePosition.X > (PointToScreen(dockPanel.Panes[1].Location).X - 5)
                && MousePosition.X < (PointToScreen(dockPanel.Panes[1].Location).X + 5))) {
                ContextMenu dockPanelContextMenu = (ContextMenu)sender;
                dockPanelContextMenu.MenuItems.Clear();

                MenuItem twentyFiveMenuItem = new MenuItem("25%", dockPanelContextMenuOnClick);
                MenuItem fiftyMenuItem = new MenuItem("50%", dockPanelContextMenuOnClick);
                MenuItem seventyFiveMenuItem = new MenuItem("75%", dockPanelContextMenuOnClick);

                dockPanelContextMenu.MenuItems.AddRange(new MenuItem[] {
                    twentyFiveMenuItem,
                    fiftyMenuItem,
                    seventyFiveMenuItem
                });
            }
        }

        private void addPanel1Browser() {
            Browser browser = getBrowser();

            browser.UpdatePath += delegate(string path) {
                Settings.Instance.DualParent.SelectedPath1 = path;
            };

            browser.UpdatePanels += delegate(IView view) {
                if (view.SelectedResource != null) {
                    Settings.Instance.DualParent.SelectedFile1 = view.SelectedResource.FullName;
                }
            };

            browser.Show(dockPanel);

            if (dockPanel.Panes[0].Contents.Count == 1) {
                dockPanel.Panes[0].Contents[0].DockHandler.CloseButton = false;
            } else {
                foreach (IDockContent content in dockPanel.Panes[0].Contents) {
                    content.DockHandler.CloseButton = true;
                }
            }
        }

        private void addPanel2Browser() {
            Browser browser = getBrowser();

            browser.UpdatePath += delegate(string path) {
                Settings.Instance.DualParent.SelectedPath2 = path;
            };

            browser.UpdatePanels += delegate(IView view) {
                if (view.SelectedResource != null) {
                    Settings.Instance.DualParent.SelectedFile2 = view.SelectedResource.FullName;
                }
            };

            // Create the second pane if necessary, otherwise just add a tab to the second pane.
            if (dockPanel.Panes.Count == 1) {
                browser.Show(dockPanel.Panes[0], DockAlignment.Right, (splitterPercentage * 0.01));
            } else {
                browser.Show(dockPanel.Panes[1], null);
            }

            if (dockPanel.Panes[1].Contents.Count == 1) {
                dockPanel.Panes[1].Contents[0].DockHandler.CloseButton = false;
            } else {
                foreach (IDockContent content in dockPanel.Panes[1].Contents) {
                    content.DockHandler.CloseButton = true;
                }
            }

            dockPanel.Panes[1].SizeChanged += delegate(object sender, EventArgs e) {
                splitterPercentage = Convert.ToInt32(
                    Convert.ToDouble(dockPanel.Panes[1].Width) / (Convert.ToDouble(this.Width)) * 100);

                string tip = string.Format("{0}%",
                    splitterPercentage);

                int mouseCursorX = 0;
                int mouseCursorY = 0;

                mouseCursorX = MousePosition.X - 10;
                mouseCursorY = MousePosition.Y;

                //if (this.splitContainer.Orientation == Orientation.Vertical) {
                //    //percent = Convert.ToDecimal(e.SplitX) / Convert.ToDecimal(this.Width);
                //    mouseCursorX = e.MouseCursorX - 10;
                //    mouseCursorY = e.MouseCursorY;
                //} else {
                //    //percent = Convert.ToDecimal(e.SplitY) / Convert.ToDecimal(this.Height - 75);
                //    mouseCursorX = e.MouseCursorX;
                //    mouseCursorY = e.MouseCursorY - 10;
                //}

                toolTip.Show(tip, this, mouseCursorX, mouseCursorY);
                toolTip.RemoveAll();

                //splitContainer.SplitterMoved += delegate {
                //    toolTip.RemoveAll();
                //};
            };
        }

        //private void setSplitterDistance() {
        //    decimal percent = Convert.ToDecimal(splitterPercentage * 0.01);
        //    int splitterDistance = 0;

        //    switch (this.splitContainer.Orientation) {
        //        case Orientation.Horizontal:
        //            splitterDistance = Convert.ToInt32(percent * (this.Height - 75));
        //            break;
        //        case Orientation.Vertical:
        //            splitterDistance = Convert.ToInt32(percent * this.Width);
        //            break;
        //    }

        //    splitContainer.SplitterDistance = splitterDistance;
        //}

        protected override void addMenuStripItems() {
            this.menuStrip.Items.AddRange(new ToolStripItem[]
			                              	{
			                              		this.fileMenu,
                                                //this.panel1Menu,
			                              		this.editMenu,
			                              		this.viewMenu,
			                              		this.toolsMenu,
                                                //this.panel2Menu,
			                              		this.helpMenu
			                              	});

            //this.panel1Menu.Text = "Panel &1";
            //this.panel1Menu.DropDownItems.AddRange(new ToolStripItem[] {
            //});

            //this.panel2Menu.Text = "Panel &2";
            //this.panel2Menu.DropDownItems.AddRange(new ToolStripItem[] {
            //});

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
                if (dockPanel.Panes[0].Visible) {
                    ///dockPanel.Panes[0].AutoSizeMode = AutoSizeMode.
                    //dockPanel.Panes[1].Width = 
                } else {
                    dockPanel.Panes[0].Visible = true;
                }

                //if (!this.splitContainer.Panel2Collapsed) {
                //    this.splitContainer.Panel1Collapsed = !this.displayPanel1Item.Checked;
                //} else {
                //    this.displayPanel1Item.Checked = true;
                //}
            };

            this.displayPanel2Item.Text = "Panel 2";
            this.displayPanel2Item.Checked = true;
            this.displayPanel2Item.CheckOnClick = true;
            //this.displayPanel2Item.CheckStateChanged += delegate(object sender, EventArgs e) {
            //    if (!this.splitContainer.Panel1Collapsed) {
            //        this.splitContainer.Panel2Collapsed = !this.displayPanel2Item.Checked;
            //    } else {
            //        this.displayPanel2Item.Checked = true;
            //    }
            //};

            this.panelOrientationItem.Text = "Orientation";
            this.panelOrientationItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.horizontalPanelOrientationItem,
				this.verticalPanelOrientationItem
			});

            this.horizontalPanelOrientationItem.Text = "Horizontal";
            //this.horizontalPanelOrientationItem.Click += delegate(object sender, EventArgs e) {
            //    if (this.splitContainer.Orientation != Orientation.Horizontal) {
            //        this.splitContainer.Orientation = Orientation.Horizontal;
            //        this.horizontalPanelOrientationItem.Checked = true;
            //        this.verticalPanelOrientationItem.Checked = false;
            //    } else {
            //        this.horizontalPanelOrientationItem.Checked = true;
            //        this.verticalPanelOrientationItem.Checked = false;
            //    }

            //    setSplitterDistance();
            //};

            this.verticalPanelOrientationItem.Text = "Vertical";
            //this.verticalPanelOrientationItem.Click += delegate(object sender, EventArgs e) {
            //    if (this.splitContainer.Orientation != Orientation.Vertical) {
            //        this.splitContainer.Orientation = Orientation.Vertical;
            //        this.verticalPanelOrientationItem.Checked = true;
            //        this.horizontalPanelOrientationItem.Checked = false;
            //    } else {
            //        this.verticalPanelOrientationItem.Checked = true;
            //        this.horizontalPanelOrientationItem.Checked = false;
            //    }

            //    setSplitterDistance();
            //};

            this.swapPanelItem.Text = "Swap Panels";
            //this.swapPanelItem.MouseUp += delegate(object sender, MouseEventArgs e) {
            //    Control panel1Control = this.splitContainer.Panel1.Controls[0];
            //    Control panel2Control = this.splitContainer.Panel2.Controls[0];

            //    this.splitContainer.Panel2.Controls.Clear();
            //    this.splitContainer.Panel2.Controls.Add(panel1Control);

            //    this.splitContainer.Panel1.Controls.Clear();
            //    this.splitContainer.Panel1.Controls.Add(panel2Control);
            //};
        }

        protected override void addControls() {
            addPanel1Browser();
            addPanel2Browser();

            //this.splitContainer.Dock = DockStyle.Fill;
            //this.splitContainer.Size = new Size(641, 364);
            //this.splitContainer.SplitterDistance = 318;

            //((SplitContainer)this.Controls["previewPanelSplitContainer"]).Panel1.Controls.Add(this.splitContainer);
            //((SplitContainer)this.Controls["commandLinePanelSplitContainer"]).Panel1.Controls.Add(this.splitContainer);
            base.addControls();
        }

        protected override void onFormClosing() {
            base.onFormClosing();

            /*
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
             */

            Settings.Instance.DualParent.SplitterPercentage = splitterPercentage;
        }

        protected override void onFormLoad() {
            base.onFormLoad();

            splitterPercentage = Settings.Instance.DualParent.SplitterPercentage;

            /*
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

            // Set the drive format template.
            Forms.SetPropertyInChild<FormatTemplate>(this.splitContainer.Panel1, "DriveFormatTemplate",
                Settings.Instance.DualParent.Panel1.DriveFormatTemplate);

            Forms.SetPropertyInChild<FormatTemplate>(this.splitContainer.Panel2, "DriveFormatTemplate",
                Settings.Instance.DualParent.Panel2.DriveFormatTemplate);
             */
        }
    }
}