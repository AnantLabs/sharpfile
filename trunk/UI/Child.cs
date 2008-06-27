using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using View = SharpFile.Infrastructure.View;

namespace SharpFile.UI {
    public class Child : UserControl {
        public event View.UpdateStatusDelegate UpdateStatus;
        public event View.UpdateProgressDelegate UpdateProgress;
        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;
        public event View.UpdatePanelsDelegate UpdatePanels;

        private TabControl tabControl;
        private FormatTemplate driveFormatTemplate;

        public Child(string name) {
            this.Name = name;
            this.tabControl = new TabControl();
            this.SuspendLayout();

            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.IsVisible = true;

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.ResumeLayout(false);
        }

        #region Events.
        private void OnUpdateStatus(string status) {
            if (UpdateStatus != null) {
                UpdateStatus(status);
            }
        }

        private void OnUpdateProgress(int value) {
            if (UpdateProgress != null) {
                UpdateProgress(value);
            }
        }

        private int OnGetImageIndex(IResource fsi, bool useFileAttributes) {
            if (GetImageIndex != null) {
                return GetImageIndex(fsi, useFileAttributes);
            }

            return -1;
        }

        private void OnUpdatePath(string path) {
            if (UpdatePath != null) {
                UpdatePath(path);
            }

            Settings.Instance.DualParent.SelectedPath = SelectedPath;
        }

        private void OnUpdatePanels(IView view) {
            if (UpdatePanels != null) {
                UpdatePanels(view);
            }
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e) {
            this.Text = SelectedPath;
            OnUpdatePath(SelectedPath);

            // Update status for the currently selected tab.
            string status = Forms.GetPropertyInChild<string>(e.TabPage, "Status");
            OnUpdateStatus(status);
        }
        #endregion

        public void AddTab() {
            // TODO: Make this more generic for cross-platform compatibility.
            AddTab(@"C:\", true);
        }

        public void AddTab(string path, bool selectNewTab) {
            FileBrowser fileBrowser = new FileBrowser(this.Name);
            fileBrowser.GetImageIndex += OnGetImageIndex;
            fileBrowser.UpdatePath += OnUpdatePath;
            fileBrowser.UpdateProgress += OnUpdateProgress;
            fileBrowser.UpdateStatus += OnUpdateStatus;
            fileBrowser.UpdatePanels += OnUpdatePanels;

            fileBrowser.Path = path;

            this.TabControl.Controls.Add(fileBrowser);
            this.TabControl.Selected += tabControl_Selected;

            if (selectNewTab) {
                this.TabControl.SelectedTab = fileBrowser;
            }
        }

        public TabControl TabControl {
            get {
                return tabControl;
            }
        }

        public List<string> Paths {
            get {
                List<string> paths = new List<string>();

                foreach (Control control in this.TabControl.Controls) {
                    if (control is FileBrowser) {
                        string path = ((FileBrowser)control).Path;

                        paths.Add(path);
                    }
                }

                return paths;
            }
            set {
                foreach (string path in value) {
                    AddTab(path, false);
                }
            }
        }

        public string SelectedPath {
            get {
                return Forms.GetPropertyInChild<string>(this.TabControl.SelectedTab, "Path");
            }
        }

        public FormatTemplate DriveFormatTemplate {
            get {
                return driveFormatTemplate;
            }
            set {
                driveFormatTemplate = value;
            }
        }
    }
}