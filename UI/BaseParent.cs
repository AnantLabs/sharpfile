using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.SettingsSection;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
	public class BaseParent : Form {
		protected const string formName = "SharpFile";

		private static readonly object lockObject = new object();
        private DriveDetector driveDetector;
        private int splitterPercentage;
		private int progressCount = 0;

		protected ToolTip toolTip = new ToolTip();
		protected Timer timer = new Timer();
		protected ProgressDisk progressDisk = new ProgressDisk();
		protected StatusStrip statusStrip = new StatusStrip();
		protected ToolStripStatusLabel toolStripStatus = new ToolStripStatusLabel();
		protected MenuStrip menuStrip = new MenuStrip();
        protected PreviewPanel previewPanel = new PreviewPanel();
        protected CommandLinePanel commandLinePanel = new CommandLinePanel();
        protected NotifyIcon notifyIcon = new NotifyIcon();
        protected DockPanel dockPanel = new DockPanel();

        //protected SplitContainer previewPanelSplitContainer = new SplitContainer();
        //protected SplitContainer commandLinePanelSplitContainer = new SplitContainer();

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
        protected ToolStripMenuItem previewPanelToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem toolsMenu = new ToolStripMenuItem();

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

                if (Settings.Instance.MinimizeToSystray) {
                    if (FormWindowState.Minimized == WindowState) {
                        Hide();
                        this.notifyIcon.Visible = true;
                    }
                }
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

            try {
                onFormLoad();
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Error while trying to generate settings when the form was opening.");

                throw;
            }
        }

		private void BaseParent_FormClosing(object sender, FormClosingEventArgs e) {
			Settings.Instance.Width = this.Width;
			Settings.Instance.Height = this.Height;

            try {
                onFormClosing();
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Error while trying to save settings when the form was closing.");
            }
		}

		private void initializeComponents() {
			ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseParent));

            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
			this.exitToolStripMenuItem.Click += this.exitToolsStripMenuItem_Click;

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
			this.cutToolStripMenuItem.Click += this.cutToolStripMenuItem_Click;

			this.copyToolStripMenuItem.Image = (Image)(resources.GetObject("copyToolStripMenuItem.Image"));
			this.copyToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.copyToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.C);
			this.copyToolStripMenuItem.Size = new Size(167, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += this.copyToolStripMenuItem_Click;

			this.pasteToolStripMenuItem.Image = (Image)(resources.GetObject("pasteToolStripMenuItem.Image"));
			this.pasteToolStripMenuItem.ImageTransparentColor = Color.Black;
			this.pasteToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.V);
			this.pasteToolStripMenuItem.Size = new Size(167, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += this.pasteToolStripMenuItem_Click;

			this.toolStripSeparator7.Size = new Size(164, 6);

			this.selectAllToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.A);
			this.selectAllToolStripMenuItem.Size = new Size(167, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";

			this.viewMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.statusBarToolStripMenuItem,
                                                        this.previewPanelToolStripMenuItem
			                                     	});
			this.viewMenu.Size = new Size(41, 20);
			this.viewMenu.Text = "&View";

			this.statusBarToolStripMenuItem.Checked = true;
			this.statusBarToolStripMenuItem.CheckOnClick = true;
			this.statusBarToolStripMenuItem.CheckState = CheckState.Checked;
			this.statusBarToolStripMenuItem.Size = new Size(135, 22);
			this.statusBarToolStripMenuItem.Text = "&Status Bar";
			this.statusBarToolStripMenuItem.Click += statusBarToolStripMenuItem_Click;

            this.previewPanelToolStripMenuItem.Size = new Size(135, 22);
            this.previewPanelToolStripMenuItem.Text = "&Preview Panel";
            this.previewPanelToolStripMenuItem.Click += previewPanelToolStripMenuItem_Click;

			this.toolsMenu.Text = "&Tools";

			this.helpMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.aboutToolStripMenuItem
			                                     	});
			this.helpMenu.Size = new Size(40, 20);
			this.helpMenu.Text = "&Help";

			this.aboutToolStripMenuItem.Size = new Size(173, 22);
			this.aboutToolStripMenuItem.Text = "&About ...";
            this.aboutToolStripMenuItem.Click += delegate {
                AboutBox aboutBox = new AboutBox();
                aboutBox.Show();
            };

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

            MenuItem addTabMenuItem = new MenuItem("Add tab", dockPanelContextMenuOnClick);
            MenuItem closeTabMenuItem = new MenuItem("Close tab", dockPanelContextMenuOnClick);

            ContextMenu dockPanelContextMenu = new ContextMenu(new MenuItem[] {
                addTabMenuItem,
                closeTabMenuItem
            });

            dockPanelContextMenu.Popup += delegate(object sender, EventArgs e) {
                Control control = GetChildAtPoint(new Point(MousePosition.X, MousePosition.Y));

                if (control is DockPaneStripBase) {
                    dockPanelContextMenu.MenuItems[1].Visible = true;
                } else {
                    dockPanelContextMenu.MenuItems[1].Visible = false;
                }
            };

            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.TabIndex = 0;
            this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            this.dockPanel.ContextMenu = dockPanelContextMenu;

            this.Controls.Add(this.dockPanel);

            /*
            this.previewPanelSplitContainer.Dock = DockStyle.Fill;
            this.previewPanelSplitContainer.Name = "previewPanelSplitContainer";
            this.previewPanelSplitContainer.SplitterWidth = 1;
            this.previewPanelSplitContainer.Orientation = Orientation.Horizontal;
            this.previewPanelSplitContainer.Panel2.Controls.Add(previewPanel);

            this.previewPanelSplitContainer.SplitterMoving += delegate(object sender, SplitterCancelEventArgs e) {
                decimal percent = 0;
                int mouseCursorX = 0;
                int mouseCursorY = 0;

                if (this.previewPanelSplitContainer.Orientation == Orientation.Vertical) {
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

            this.previewPanelSplitContainer.SplitterMoved += delegate {
                toolTip.RemoveAll();
            };

            this.Controls.Add(this.previewPanelSplitContainer);

            this.commandLinePanelSplitContainer.Dock = DockStyle.Fill;
            this.commandLinePanelSplitContainer.Name = "commandLinePanelSplitContainer";
            this.commandLinePanelSplitContainer.SplitterWidth = 1;
            this.commandLinePanelSplitContainer.Orientation = Orientation.Horizontal;
            this.commandLinePanelSplitContainer.Panel2.Controls.Add(commandLinePanel);

            this.commandLinePanelSplitContainer.SplitterMoving += delegate(object sender, SplitterCancelEventArgs e) {
                decimal percent = 0;
                int mouseCursorX = 0;
                int mouseCursorY = 0;

                if (this.commandLinePanelSplitContainer.Orientation == Orientation.Vertical) {
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

            this.commandLinePanelSplitContainer.SplitterMoved += delegate {
                toolTip.RemoveAll();
            };

            //this.Controls.Add(this.commandLinePanelSplitContainer);
            */

			addControls();

			this.MainMenuStrip = this.menuStrip;

			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();

            this.notifyIcon.Text = "SharpFile";
            this.notifyIcon.Visible = false;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            
            this.notifyIcon.DoubleClick += delegate(object sender, EventArgs e) {
                Show();
                this.WindowState = FormWindowState.Normal;
                this.notifyIcon.Visible = false;
            };

			this.ResumeLayout(false);
			this.PerformLayout();
		}

        private void addBrowser() {
            Browser browser = getBrowser();
            browser.Show(dockPanel);
        }

        protected Browser getBrowser() {
            Browser browser = new Browser("view1");
            browser.AllowEndUserDocking = false;
            browser.DockHandler.DockAreas = DockAreas.Document;

            browser.UpdateStatus += delegate(string status) {
                toolStripStatus.Text = status;
            };

            browser.UpdateProgress += delegate(int value) {
                updateProgress(value);
            };

            browser.GetImageIndex += delegate(IResource fsi, bool useFileAttributes) {
                return IconManager.GetImageIndex(fsi, useFileAttributes, ImageList);
            };

            browser.UpdatePath += delegate(string path) {
                this.Text = string.Format("{0} - {1}",
                                          formName,
                                          path);

                Settings.Instance.DualParent.SelectedPath1 = path;
            };

            browser.UpdatePanels += delegate(IView view) {
                this.previewPanel.Update(view);
                this.commandLinePanel.Update(view);
                Settings.Instance.DualParent.SelectedFile1 = view.SelectedResource.Name;
            };

            return browser;
        }

        protected virtual void dockPanelContextMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Text) {
                case "Add tab":
                    addBrowser();
                    break;
                case "Close tab":
                    MessageBox.Show("close tab");
                    // TODO: Calculate if we are over top of a current tab.
                    break;
            }
        }

        private void reloadSettingsStripMenuItem_Click(object sender, EventArgs e) {
            Settings.Load();

            // Call a method called reload on every child control to reload data.
            // Refresh the screen.
        }

		private void exitToolsStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
			// TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
		}

		private void statusBarToolStripMenuItem_Click(object sender, EventArgs e) {
			statusStrip.Visible = statusBarToolStripMenuItem.Checked;
		}

        private void previewPanelToolStripMenuItem_Click(object sender, EventArgs e) {
            //previewPanelSplitContainer.Panel2Collapsed = previewPanelToolStripMenuItem.Checked;
            previewPanelToolStripMenuItem.Checked = !previewPanelToolStripMenuItem.Checked;
        }

        private void setSplitterDistance() {
            decimal percent = Convert.ToDecimal(splitterPercentage * 0.01);
            int splitterDistance = 0;

            //switch (this.previewPanelSplitContainer.Orientation) {
            //    case Orientation.Horizontal:
            //        splitterDistance = Convert.ToInt32(percent * (this.Height - 75));
            //        break;
            //    case Orientation.Vertical:
            //        splitterDistance = Convert.ToInt32(percent * this.Width);
            //        break;
            //}

            //previewPanelSplitContainer.SplitterDistance = splitterDistance;
        }

		protected virtual void addControls() {
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.progressDisk);
            this.Controls.Add(this.statusStrip);
		}

		protected virtual void onFormClosing() {
            //Settings.Instance.PreviewPanel.Collapsed = this.previewPanelSplitContainer.Panel2Collapsed;
            Settings.Instance.PreviewPanel.SplitterPercentage = 100 - splitterPercentage;
		}

        protected virtual void onFormLoad() {
            splitterPercentage = (100 - Settings.Instance.PreviewPanel.SplitterPercentage);
            setSplitterDistance();

            //this.previewPanelSplitContainer.Panel2Collapsed = Settings.Instance.PreviewPanel.Collapsed;
            previewPanelToolStripMenuItem.Checked = !Settings.Instance.PreviewPanel.Collapsed;

            foreach (Tool tool in Settings.Instance.DualParent.Tools) {
                string menuItemName = tool.Name;

                if (tool.Key.HasValue && tool.Key.Value.PrimaryKey != Keys.None) {
                    string keys = tool.Key.Value.PrimaryKey.ToString();

                    if (tool.Key.Value.ModifierKeys != null && tool.Key.Value.ModifierKeys.Count > 0) {
                        keys = string.Format("{0}+{1}",
                            string.Join("+",
                                tool.Key.Value.ModifierKeys.ConvertAll<string>(delegate(Keys k) {
                                    return k.ToString();
                                }).ToArray()),
                                keys);
                    }

                    menuItemName = string.Format("{0} ({1})",
                        menuItemName,
                        keys);
                }

                ToolStripMenuItem menuItem = new ToolStripMenuItem(menuItemName);
                menuItem.Tag = tool;

                menuItem.Click += (EventHandler)delegate {
                    Tool t = (Tool)menuItem.Tag;

                    if (!t.Name.Equals(Tool.SeperatorName, StringComparison.InvariantCultureIgnoreCase)) {
                        t.Execute();
                    }
                };

                if (tool.Name.Equals(Tool.SeperatorName, StringComparison.InvariantCultureIgnoreCase)) {
                    this.toolsMenu.DropDownItems.Insert(this.toolsMenu.DropDownItems.Count, new ToolStripSeparator());
                } else {
                    this.toolsMenu.DropDownItems.Insert(this.toolsMenu.DropDownItems.Count, menuItem);
                }
            }
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