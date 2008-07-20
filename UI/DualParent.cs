using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;
using Common.Logger;

namespace SharpFile.UI {
    public class DualParent : BaseParent {
        private ToolStripMenuItem swapPaneItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPaneItem = new ToolStripMenuItem();
        private ToolStripMenuItem displayPane1Item = new ToolStripMenuItem();
        private ToolStripMenuItem displayPane2Item = new ToolStripMenuItem();
        private ToolStripMenuItem paneOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem verticalPaneOrientationItem = new ToolStripMenuItem();
        private ToolStripMenuItem horizontalPaneOrientationItem = new ToolStripMenuItem();
        private int splitterPercentage = 50;
        private Orientation orientation = Orientation.Vertical;
        private ContextMenu splitterContextMenu = new ContextMenu();
        DockPane pane1;
        DockPane pane2;

        public DualParent() {
            setContextMenu();
        }

        private void setContextMenu() {
            this.splitterContextMenu.Popup += delegate {
                splitterContextMenu.MenuItems.Clear();
                int splitterX = PointToScreen(pane2.NestedDockingStatus.SplitterBounds.Location).X;

                if (MousePosition.X > splitterX - 3
                 && MousePosition.X < (splitterX + pane2.NestedDockingStatus.SplitterBounds.Width)) {
                    MenuItem twentyFiveMenuItem = new MenuItem("25%", splitterMenuOnClick);
                    MenuItem fiftyMenuItem = new MenuItem("50%", splitterMenuOnClick);
                    MenuItem seventyFiveMenuItem = new MenuItem("75%", splitterMenuOnClick);

                    splitterContextMenu.MenuItems.AddRange(new MenuItem[] {
                        twentyFiveMenuItem,
                        fiftyMenuItem,
                        seventyFiveMenuItem
                    });
                }
            };

            dockPanel.ContextMenu = splitterContextMenu;
        }

        private void splitterMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;

            switch (menuItem.Text) {
                case "25%":
                case "50%":
                case "75%":
                    splitterPercentage = Convert.ToInt32(menuItem.Text.Replace("%", string.Empty));
                    double percent = Convert.ToDouble(100 - splitterPercentage) * 0.01;
                    pane2.DockTo(pane1.NestedPanesContainer, pane1, pane2.NestedDockingStatus.Alignment,
                        percent);
                    break;
            }
        }

        protected override void dockPanelContextMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;

            switch (menuItem.Text) {
                case "Add tab":
                    if (MousePosition.X < PointToScreen(dockPanel.Panes[1].Location).X) {
                        addPane1Browser(Infrastructure.SettingsSection.DualParent.DefaultDrive);
                    } else {
                        addPane2Browser(Infrastructure.SettingsSection.DualParent.DefaultDrive);
                    }
                    break;
                case "Close tab":
                    dockPanel.ActiveDocument.DockHandler.Close();
                    break;
            }
        }

        protected new Browser getBrowser(string path) {
            Browser browser = base.getBrowser(path);

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

        private void addPane1Browser(string path) {
            Browser browser = getBrowser(path);

            browser.UpdatePath += delegate(string updatePath) {
                Settings.Instance.DualParent.SelectedPath1 = updatePath;
            };

            browser.UpdatePanels += delegate(IView view) {
                if (view.SelectedResource != null) {
                    Settings.Instance.DualParent.SelectedFile1 = view.SelectedResource.FullName;
                }
            };

            browser.Show(dockPanel);

            if (dockPanel.Panes[0].Contents.Count == 1) {
                pane1 = dockPanel.Panes[0];
                pane1.ContextMenu = dockPanelContextMenu;
                pane1.Contents[0].DockHandler.CloseButton = false;
            } else {
                foreach (IDockContent content in pane1.Contents) {
                    content.DockHandler.CloseButton = true;
                }
            }
        }

        private void addPane2Browser(string path) {
            Browser browser = getBrowser(path);

            browser.UpdatePath += delegate(string updatePath) {
                Settings.Instance.DualParent.SelectedPath2 = updatePath;
            };

            browser.UpdatePanels += delegate(IView view) {
                if (view.SelectedResource != null) {
                    Settings.Instance.DualParent.SelectedFile2 = view.SelectedResource.FullName;
                }
            };

            // Create the second pane if necessary, otherwise just add a tab to the second pane.
            if (dockPanel.Panes.Count == 1) {
                if (orientation == Orientation.Horizontal) {
                    browser.Show(pane1, DockAlignment.Bottom, (splitterPercentage * 0.01));
                } else {
                    browser.Show(pane1, DockAlignment.Right, (splitterPercentage * 0.01));
                }
            } else {
                browser.Show(dockPanel.Panes[1], null);
            }

            if (dockPanel.Panes[1].Contents.Count == 1) {
                pane2 = dockPanel.Panes[1];
                pane2.Contents[0].DockHandler.CloseButton = false;
                pane2.ContextMenu = dockPanelContextMenu;

                pane2.SizeChanged += delegate {
                    splitterPercentage = Convert.ToInt32(pane2.NestedDockingStatus.Proportion * 100);
                };
            } else {
                foreach (IDockContent content in pane2.Contents) {
                    content.DockHandler.CloseButton = true;
                }
            }
        }

        protected override void addMenuStripItems() {
            this.menuStrip.Items.AddRange(new ToolStripItem[]
			                              	{
			                              		this.fileMenu,
			                              		this.editMenu,
			                              		this.viewMenu,
			                              		this.toolsMenu,
			                              		this.helpMenu
			                              	});

            this.viewMenu.DropDownItems.AddRange(new ToolStripItem[] {
                this.displayPaneItem,
                this.paneOrientationItem,
                this.swapPaneItem
            });

            this.displayPaneItem.Text = "Display Pane";
            this.displayPaneItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.displayPane1Item,
				this.displayPane2Item
			});

            this.displayPane1Item.Text = "Pane 1";
            this.displayPane1Item.Checked = true;
            this.displayPane1Item.CheckOnClick = true;
            this.displayPane1Item.CheckStateChanged += delegate(object sender, EventArgs e) {
                if (ArePanesSwapped) {
                    togglePaneDisplay(pane2);
                } else {
                    togglePaneDisplay(pane1);
                }
            };

            this.displayPane2Item.Text = "Pane 2";
            this.displayPane2Item.Checked = true;
            this.displayPane2Item.CheckOnClick = true;
            this.displayPane2Item.CheckStateChanged += delegate(object sender, EventArgs e) {
                if (ArePanesSwapped) {
                    togglePaneDisplay(pane1);
                } else {
                    togglePaneDisplay(pane2);
                }
            };

            this.paneOrientationItem.Text = "Orientation";
            this.paneOrientationItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.verticalPaneOrientationItem,
                this.horizontalPaneOrientationItem				
			});

            this.horizontalPaneOrientationItem.Text = "Horizontal";
            this.horizontalPaneOrientationItem.Click += delegate(object sender, EventArgs e) {
                if (!this.horizontalPaneOrientationItem.Checked) {
                    if (pane2.NestedDockingStatus.Alignment == DockAlignment.Right) {
                        pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Bottom, .5);
                        this.horizontalPaneOrientationItem.Checked = true;
                        this.verticalPaneOrientationItem.Checked = false;
                    } else if (pane2.NestedDockingStatus.Alignment == DockAlignment.Left) {
                        pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Top, .5);
                        this.horizontalPaneOrientationItem.Checked = true;
                        this.verticalPaneOrientationItem.Checked = false;
                    }

                    orientation = Orientation.Horizontal;
                }
            };

            this.verticalPaneOrientationItem.Text = "Vertical";
            this.verticalPaneOrientationItem.Click += delegate(object sender, EventArgs e) {
                if (!this.verticalPaneOrientationItem.Checked) {
                    if (pane2.NestedDockingStatus.Alignment == DockAlignment.Bottom) {
                        pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Right, .5);
                        this.horizontalPaneOrientationItem.Checked = false;
                        this.verticalPaneOrientationItem.Checked = true;
                    } else if (pane2.NestedDockingStatus.Alignment == DockAlignment.Top) {
                        pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Left, .5);
                        this.horizontalPaneOrientationItem.Checked = false;
                        this.verticalPaneOrientationItem.Checked = true;
                    }

                    orientation = Orientation.Vertical;
                }
            };

            this.swapPaneItem.Text = "Swap Panes";
            this.swapPaneItem.MouseUp += delegate(object sender, MouseEventArgs e) {
                double swappedPercentage = ((100 - splitterPercentage) * 0.01);

                if (pane2.NestedDockingStatus.Alignment == DockAlignment.Right) {
                    pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Left, swappedPercentage);
                } else if (pane2.NestedDockingStatus.Alignment == DockAlignment.Bottom) {
                    pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Top, swappedPercentage);
                } else if (pane2.NestedDockingStatus.Alignment == DockAlignment.Left) {
                    pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Right, swappedPercentage);
                } else if (pane2.NestedDockingStatus.Alignment == DockAlignment.Top) {
                    pane2.DockTo(pane1.NestedPanesContainer, pane1, DockAlignment.Bottom, swappedPercentage);
                }
            };
        }

        private void togglePaneDisplay(DockPane pane) {
            if (pane.Visible) {
                pane.Visible = false;

                foreach (IDockContent content in pane.Contents) {
                    content.DockHandler.Hide();
                }
            } else {
                pane.Visible = true;

                foreach (IDockContent content in pane.Contents) {
                    content.DockHandler.Activate();
                }
            }
        }

        protected override void addControls() {
            base.addControls();
        }

        private List<string> getPanePaths(DockPane pane) {
            List<string> paths = new List<string>();

            foreach (IDockContent content in pane.Contents) {
                paths.Add(((Browser)content).Path);
            }

            return paths;
        }

        protected override void onFormClosing() {
            base.onFormClosing();

            // Save the paths for each pane and the splitter percentage.
            if (ArePanesSwapped) {
                Settings.Instance.DualParent.Pane1.Paths = getPanePaths(pane2);
                Settings.Instance.DualParent.Pane2.Paths = getPanePaths(pane1);
                Settings.Instance.DualParent.SplitterPercentage = (100 - splitterPercentage);
            } else {
                Settings.Instance.DualParent.Pane1.Paths = getPanePaths(pane1);
                Settings.Instance.DualParent.Pane2.Paths = getPanePaths(pane2);
                Settings.Instance.DualParent.SplitterPercentage = splitterPercentage;
            }

            Settings.Instance.DualParent.Orientation = orientation;

            // Save whether each pane is collapsed or not.
            Settings.Instance.DualParent.Pane1.Collapsed = !pane1.Visible;
            Settings.Instance.DualParent.Pane2.Collapsed = !pane2.Visible;
        }

        protected override void onFormLoad() {
            base.onFormLoad();

            // Get the split container orientation.
            orientation = Settings.Instance.DualParent.Orientation;

            // Check the appropriate orientation in the menus.
            switch (orientation) {
                case Orientation.Horizontal:
                    this.horizontalPaneOrientationItem.Checked = true;
                    break;
                case Orientation.Vertical:
                    this.verticalPaneOrientationItem.Checked = true;
                    break;
            }

            // Calculate the splitter distance based on the percentage stored and the width of the form.
            splitterPercentage = Settings.Instance.DualParent.SplitterPercentage;

            // Set the paths for the panes.
            foreach (string path in Settings.Instance.DualParent.Pane1.Paths) {
                addPane1Browser(path);
            }

            foreach (string path in Settings.Instance.DualParent.Pane2.Paths) {
                addPane2Browser(path);
            }

            addPluginPanes();

            // Collapse/check menu pane 1 if appropriate.
            bool pane1Collapsed = Settings.Instance.DualParent.Pane1.Collapsed;
            this.displayPane1Item.Checked = !pane1Collapsed;

            if (pane1Collapsed) {
                togglePaneDisplay(pane1);
            }            

            // Collapse/check menu pane 2 if appropriate.
            bool pane2Collapsed = Settings.Instance.DualParent.Pane2.Collapsed;
            this.displayPane2Item.Checked = !pane2Collapsed;

            if (pane2Collapsed) {
                togglePaneDisplay(pane2);
            }
        }

        public bool ArePanesSwapped {
            get {
                return (DockAlignment.Left == pane2.NestedDockingStatus.Alignment
                    || DockAlignment.Top == pane2.NestedDockingStatus.Alignment);
            }
        }
    }
}